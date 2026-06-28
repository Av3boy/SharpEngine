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

    /// <summary>
    ///     Gets or sets the scale factor applied to the input coordinates.
    ///     Higher values make the noise appear more zoomed out.
    /// </summary>
    public float Scale
    {
        get => _scale;
        set => _scale = value <= 0f ? 0.0001f : value;
    }

    /// <summary>
    ///     Gets or sets the number of octaves to combine.
    /// </summary>
    public int Octaves
    {
        get => _octaves;
        set => _octaves = Math.Max(1, value);
    }

    /// <summary>
    ///     Gets or sets the amplitude multiplier between octaves.
    /// </summary>
    public float Persistence
    {
        get => _persistence;
        set => _persistence = Math.Max(0f, value);
    }

    /// <summary>
    ///     Gets or sets the frequency multiplier between octaves.
    /// </summary>
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
        int xi = FastFloor(x) & 255;
        int yi = FastFloor(y) & 255;
        int zi = FastFloor(z) & 255;

        float xf = x - FastFloor(x);
        float yf = y - FastFloor(y);
        float zf = z - FastFloor(z);

        float u = Fade(xf);
        float v = Fade(yf);
        float w = Fade(zf);

        int aaa = _permutation[_permutation[_permutation[xi] + yi] + zi];
        int aba = _permutation[_permutation[_permutation[xi] + yi + 1] + zi];
        int aab = _permutation[_permutation[_permutation[xi] + yi] + zi + 1];
        int abb = _permutation[_permutation[_permutation[xi] + yi + 1] + zi + 1];

        int baa = _permutation[_permutation[_permutation[xi + 1] + yi] + zi];
        int bba = _permutation[_permutation[_permutation[xi + 1] + yi + 1] + zi];
        int bab = _permutation[_permutation[_permutation[xi + 1] + yi] + zi + 1];
        int bbb = _permutation[_permutation[_permutation[xi + 1] + yi + 1] + zi + 1];

        float x1 = Lerp(
            Grad(aaa, xf, yf, zf),
            Grad(baa, xf - 1f, yf, zf),
            u);

        float x2 = Lerp(
            Grad(aba, xf, yf - 1f, zf),
            Grad(bba, xf - 1f, yf - 1f, zf),
            u);

        float y1 = Lerp(x1, x2, v);

        x1 = Lerp(
            Grad(aab, xf, yf, zf - 1f),
            Grad(bab, xf - 1f, yf, zf - 1f),
            u);

        x2 = Lerp(
            Grad(abb, xf, yf - 1f, zf - 1f),
            Grad(bbb, xf - 1f, yf - 1f, zf - 1f),
            u);

        float y2 = Lerp(x1, x2, v);

        return Lerp(y1, y2, w);
    }

    private static int FastFloor(float value)
    {
        int integer = (int)value;

        return value < integer ? integer - 1 : integer;
    }

    private static float Fade(float t)
    {
        return t * t * t * (t * (t * 6f - 15f) + 10f);
    }

    private static float Lerp(float a, float b, float t)
    {
        return a + t * (b - a);
    }

    private static float Grad(int hash, float x, float y, float z)
    {
        int h = hash & 15;

        float u = h < 8 ? x : y;
        float v = h < 4
            ? y
            : h is 12 or 14
                ? x
                : z;

        return ((h & 1) == 0 ? u : -u) +
               ((h & 2) == 0 ? v : -v);
    }

    private static float ToUnitRange(float value)
    {
        // Perlin is roughly in [-1, 1], so remap to [0, 1].
        float result = value * 0.5f + 0.5f;

        return Math.Clamp(result, 0f, 1f);
    }
}