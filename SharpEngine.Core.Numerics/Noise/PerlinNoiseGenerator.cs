using MathEx = SharpEngine.Core.Numerics.MathExtensions;

namespace SharpEngine.Core.Numerics.Noise;

/// <summary>
///     Deterministic Perlin noise generator with fractal octave support.
/// </summary>
public sealed class PerlinNoiseGenerator : INoiseGenerator
{
    private readonly int[] _permutation = new int[512];

    private float _scale = 32f;
    private int _octaves = 4;
    private float _persistence = 0.5f;
    private float _lacunarity = 2f;

    /// <summary>
    ///     Creates a new Perlin noise generator.
    /// </summary>
    /// <param name="seed">The seed used to generate the permutation table.</param>
    public PerlinNoiseGenerator(int seed = 1337)
    {
        InitializePermutation(seed);
    }

    /// <inheritdoc />
    public float Scale
    {
        get => _scale;
        set => _scale = value <= 0f ? 0.0001f : value;
    }

    /// <inheritdoc />
    public int Octaves
    {
        get => _octaves;
        set => _octaves = Math.Max(1, value);
    }

    /// <inheritdoc />
    public float Persistence
    {
        get => _persistence;
        set => _persistence = Math.Max(0f, value);
    }

    /// <inheritdoc />
    public float Lacunarity
    {
        get => _lacunarity;
        set => _lacunarity = Math.Max(0.0001f, value);
    }

    /// <inheritdoc />
    public float Sample2D(float x, float z)
    {
        float total = 0f;
        float amplitude = 1f;
        float frequency = 1f;
        float maxAmplitude = 0f;

        for (int i = 0; i < Octaves; i++)
        {
            float sampleX = x / Scale * frequency;
            float sampleZ = z / Scale * frequency;

            float noise = Perlin(sampleX, 0f, sampleZ);

            total += noise * amplitude;
            maxAmplitude += amplitude;

            amplitude *= Persistence;
            frequency *= Lacunarity;
        }

        float normalized = total / maxAmplitude;

        return ToUnitRange(normalized);
    }

    /// <inheritdoc />
    public float Sample3D(float x, float y, float z)
    {
        float total = 0f;
        float amplitude = 1f;
        float frequency = 1f;
        float maxAmplitude = 0f;

        for (int i = 0; i < Octaves; i++)
        {
            float sampleX = x / Scale * frequency;
            float sampleY = y / Scale * frequency;
            float sampleZ = z / Scale * frequency;

            float noise = Perlin(sampleX, sampleY, sampleZ);

            total += noise * amplitude;
            maxAmplitude += amplitude;

            amplitude *= Persistence;
            frequency *= Lacunarity;
        }

        float normalized = total / maxAmplitude;

        return ToUnitRange(normalized);
    }

    private void InitializePermutation(int seed)
    {
        int[] source = new int[256];

        for (int i = 0; i < source.Length; i++)
            source[i] = i;

        Random random = new(seed);

        for (int i = source.Length - 1; i > 0; i--)
        {
            int swapIndex = random.Next(i + 1);

            (source[i], source[swapIndex]) = (source[swapIndex], source[i]);
        }

        for (int i = 0; i < 512; i++)
            _permutation[i] = source[i & 255];
    }

    private float Perlin(float x, float y, float z)
    {
        int xi = MathEx.FastFloor(x) & 255;
        int yi = MathEx.FastFloor(y) & 255;
        int zi = MathEx.FastFloor(z) & 255;

        float xf = x - MathEx.FastFloor(x);
        float yf = y - MathEx.FastFloor(y);
        float zf = z - MathEx.FastFloor(z);

        float u = MathEx.Fade(xf);
        float v = MathEx.Fade(yf);
        float w = MathEx.Fade(zf);

        var yiOffset = _permutation[xi] + yi;

        int aaa = _permutation[_permutation[yiOffset] + zi];
        int aba = _permutation[_permutation[yiOffset + 1] + zi];
        int aab = _permutation[_permutation[yiOffset] + zi + 1];
        int abb = _permutation[_permutation[yiOffset + 1] + zi + 1];

        var yiOffsetNext = _permutation[xi + 1] + yi;

        int baa = _permutation[_permutation[yiOffsetNext] + zi];
        int bba = _permutation[_permutation[yiOffsetNext + 1] + zi];
        int bab = _permutation[_permutation[yiOffsetNext] + zi + 1];
        int bbb = _permutation[_permutation[yiOffsetNext + 1] + zi + 1];

        float x1 = MathEx.Lerp(
            MathEx.Grad(aaa, xf, yf, zf),
            MathEx.Grad(baa, xf - 1f, yf, zf),
            u);

        float x2 = MathEx.Lerp(
            MathEx.Grad(aba, xf, yf - 1f, zf),
            MathEx.Grad(bba, xf - 1f, yf - 1f, zf),
            u);

        float y1 = MathEx.Lerp(x1, x2, v);

        x1 = MathEx.Lerp(
            MathEx.Grad(aab, xf, yf, zf - 1f),
            MathEx.Grad(bab, xf - 1f, yf, zf - 1f),
            u);

        x2 = MathEx.Lerp(
            MathEx.Grad(abb, xf, yf - 1f, zf - 1f),
            MathEx.Grad(bbb, xf - 1f, yf - 1f, zf - 1f),
            u);

        float y2 = MathEx.Lerp(x1, x2, v);

        return MathEx.Lerp(y1, y2, w);
    }

    private static float ToUnitRange(float value)
    {
        // Perlin is roughly in [-1, 1], so remap to [0, 1].
        float result = value * 0.5f + 0.5f;

        return Math.Clamp(result, 0f, 1f);
    }
}