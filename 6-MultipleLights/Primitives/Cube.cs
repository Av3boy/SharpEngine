using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnOpenTK.Primitives;
public static class Cube
{
    public static GameObject Create(Vector3 position)
        => new()
        {
            Position = position,
            Mesh = new()
            {
                Vertices =
                [
                    -0.5f, -0.5f, -0.5f,
                     0.5f, -0.5f, -0.5f,
                     0.5f,  0.5f, -0.5f,
                     0.5f,  0.5f, -0.5f,
                    -0.5f,  0.5f, -0.5f,
                    -0.5f, -0.5f, -0.5f,

                    -0.5f, -0.5f,  0.5f,
                     0.5f, -0.5f,  0.5f,
                     0.5f,  0.5f,  0.5f,
                     0.5f,  0.5f,  0.5f,
                    -0.5f,  0.5f,  0.5f,
                    -0.5f, -0.5f,  0.5f,

                    -0.5f,  0.5f,  0.5f,
                    -0.5f,  0.5f, -0.5f,
                    -0.5f, -0.5f, -0.5f,
                    -0.5f, -0.5f, -0.5f,
                    -0.5f, -0.5f,  0.5f,
                    -0.5f,  0.5f,  0.5f,

                     0.5f,  0.5f,  0.5f,
                     0.5f,  0.5f, -0.5f,
                     0.5f, -0.5f, -0.5f,
                     0.5f, -0.5f, -0.5f,
                     0.5f, -0.5f,  0.5f,
                     0.5f,  0.5f,  0.5f,

                    -0.5f, -0.5f, -0.5f,
                     0.5f, -0.5f, -0.5f,
                     0.5f, -0.5f,  0.5f,
                     0.5f, -0.5f,  0.5f,
                    -0.5f, -0.5f,  0.5f,
                    -0.5f, -0.5f, -0.5f,

                    -0.5f,  0.5f, -0.5f,
                     0.5f,  0.5f, -0.5f,
                     0.5f,  0.5f,  0.5f,
                     0.5f,  0.5f,  0.5f,
                    -0.5f,  0.5f,  0.5f,
                    -0.5f,  0.5f, -0.5f,
                ],
                Normals =
                [
                      0.0f,  0.0f, -1.0f,
                      0.0f,  0.0f, -1.0f,
                      0.0f,  0.0f, -1.0f,
                      0.0f,  0.0f, -1.0f,
                      0.0f,  0.0f, -1.0f,
                      0.0f,  0.0f, -1.0f,

                      0.0f,  0.0f,  1.0f,
                      0.0f,  0.0f,  1.0f,
                      0.0f,  0.0f,  1.0f,
                      0.0f,  0.0f,  1.0f,
                      0.0f,  0.0f,  1.0f,
                      0.0f,  0.0f,  1.0f,

                     -1.0f,  0.0f,  0.0f,
                     -1.0f,  0.0f,  0.0f,
                     -1.0f,  0.0f,  0.0f,
                     -1.0f,  0.0f,  0.0f,
                     -1.0f,  0.0f,  0.0f,
                     -1.0f,  0.0f,  0.0f,

                      1.0f,  0.0f,  0.0f,
                      1.0f,  0.0f,  0.0f,
                      1.0f,  0.0f,  0.0f,
                      1.0f,  0.0f,  0.0f,
                      1.0f,  0.0f,  0.0f,
                      1.0f,  0.0f,  0.0f,

                      0.0f, -1.0f,  0.0f,
                      0.0f, -1.0f,  0.0f,
                      0.0f, -1.0f,  0.0f,
                      0.0f, -1.0f,  0.0f,
                      0.0f, -1.0f,  0.0f,
                      0.0f, -1.0f,  0.0f,

                      0.0f,  1.0f,  0.0f,
                      0.0f,  1.0f,  0.0f,
                      0.0f,  1.0f,  0.0f,
                      0.0f,  1.0f,  0.0f,
                      0.0f,  1.0f,  0.0f,
                      0.0f,  1.0f,  0.0f,
                ],
                TextureCoordinates =
                [
                      0.0f, 0.0f,
                      1.0f, 0.0f,
                      1.0f, 1.0f,
                      1.0f, 1.0f,
                      0.0f, 1.0f,
                      0.0f, 0.0f,

                      0.0f, 0.0f,
                      1.0f, 0.0f,
                      1.0f, 1.0f,
                      1.0f, 1.0f,
                      0.0f, 1.0f,
                      0.0f, 0.0f,

                      1.0f, 0.0f,
                      1.0f, 1.0f,
                      0.0f, 1.0f,
                      0.0f, 1.0f,
                      0.0f, 0.0f,
                      1.0f, 0.0f,

                      1.0f, 0.0f,
                      1.0f, 1.0f,
                      0.0f, 1.0f,
                      0.0f, 1.0f,
                      0.0f, 0.0f,
                      1.0f, 0.0f,

                      0.0f, 1.0f,
                      1.0f, 1.0f,
                      1.0f, 0.0f,
                      1.0f, 0.0f,
                      0.0f, 0.0f,
                      0.0f, 1.0f,

                      0.0f, 1.0f,
                      1.0f, 1.0f,
                      1.0f, 0.0f,
                      1.0f, 0.0f,
                      0.0f, 0.0f,
                      0.0f, 1.0f
                ]
            }
        };
}
