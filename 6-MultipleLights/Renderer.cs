using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace Core
{
    public class Renderer
    {
        private int _vaoModel;
        private int _vaoLamp;

        private Shader _lampShader;
        private Shader _lightingShader;

        private readonly IGame _game;
        private readonly Scene _scene;

        // Read only once, load into OpenGL buffer once.
        //
        // TODO: Multiple meshes
        // If already loaded, add mesh indetifier to a dictionary. If dict contains mesh, skip it.
        private static readonly float[] _vertices = GetVertices();

        private static float[] GetVertices()
        {
            var mesh = Primitives.Cube.Mesh; // Mesh is identical for all cubes
            var vertices = new List<float>();

            for (int i = 0; i < mesh.Vertices.Length / 3; i++)
            {
                var vertexIndex = i * 3;

                vertices.Add(mesh.Vertices[vertexIndex]);
                vertices.Add(mesh.Vertices[vertexIndex + 1]);
                vertices.Add(mesh.Vertices[vertexIndex + 2]);

                var normalIndex = i * 3;
                vertices.Add(mesh.Normals[normalIndex]);
                vertices.Add(mesh.Normals[normalIndex + 1]);
                vertices.Add(mesh.Normals[normalIndex + 2]);

                var texCoordIndex = i * 2;
                vertices.Add(mesh.TextureCoordinates[texCoordIndex]);
                vertices.Add(mesh.TextureCoordinates[texCoordIndex + 1]);
            }

            return [.. vertices];
        }

        public Renderer(IGame game, Scene scene)
        {
            _game = game;
            _scene = scene;
        }

        public void Initialize()
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            InitializeBuffers();
            InitializeShaders();
            InitializeVertexArrays();
        }

        private static void InitializeBuffers()
        {
            var vertexBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
        }

        private void InitializeShaders()
        {
            _lightingShader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");
            _lampShader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
        }

        private void InitializeVertexArrays()
        {
            _vaoModel = GL.GenVertexArray();
            GL.BindVertexArray(_vaoModel);

            var positionLocation = _lightingShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, VertexData.VerticesSize, VertexAttribPointerType.Float, false, VertexData.Stride, 0);

            var normalLocation = _lightingShader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, VertexData.NormalsSize, VertexAttribPointerType.Float, false, VertexData.Stride, VertexData.NormalsOffset);

            var texCoordLocation = _lightingShader.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, VertexData.TexCoordsSize, VertexAttribPointerType.Float, false, VertexData.Stride, VertexData.TexCoordsOffset);

            _vaoLamp = GL.GenVertexArray();
            GL.BindVertexArray(_vaoLamp);

            positionLocation = _lampShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, VertexData.Stride, 0);
        }

        public void Render(Camera camera)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindVertexArray(_vaoModel);

            _lightingShader.Use();

            _lightingShader.SetMatrix4("view", camera.GetViewMatrix());
            _lightingShader.SetMatrix4("projection", camera.GetProjectionMatrix());

            _lightingShader.SetVector3("viewPos", camera.Position);

            // Directional light
            _lightingShader.SetVector3("dirLight.direction", _game.DirectionalLight.Direction);
            _lightingShader.SetVector3("dirLight.ambient", _game.DirectionalLight.Ambient);
            _lightingShader.SetVector3("dirLight.diffuse", _game.DirectionalLight.Diffuse);
            _lightingShader.SetVector3("dirLight.specular", _game.DirectionalLight.Specular);

            // Point lights
            for (int i = 0; i < _game.PointLights.Length; i++)
            {
                _lightingShader.SetVector3($"pointLights[{i}].position", _game.PointLights[i].Position);
                _lightingShader.SetVector3($"pointLights[{i}].ambient", _game.PointLights[i].Ambient);
                _lightingShader.SetVector3($"pointLights[{i}].diffuse", _game.PointLights[i].Diffuse);
                _lightingShader.SetVector3($"pointLights[{i}].specular", _game.PointLights[i].Specular);
                _lightingShader.SetFloat($"pointLights[{i}].constant", _game.PointLights[i].Constant);
                _lightingShader.SetFloat($"pointLights[{i}].linear", _game.PointLights[i].Linear);
                _lightingShader.SetFloat($"pointLights[{i}].quadratic", _game.PointLights[i].Quadratic);
            }

            // Spot light
            _game.SpotLight.Position = camera.Position;
            _game.SpotLight.Direction = camera.Front;

            _lightingShader.SetVector3("spotLight.position", _game.SpotLight.Position);
            _lightingShader.SetVector3("spotLight.direction", _game.SpotLight.Direction);
            _lightingShader.SetVector3("spotLight.ambient", _game.SpotLight.Ambient);
            _lightingShader.SetVector3("spotLight.diffuse", _game.SpotLight.Diffuse);
            _lightingShader.SetVector3("spotLight.specular", _game.SpotLight.Specular);
            _lightingShader.SetFloat("spotLight.constant", _game.SpotLight.Constant);
            _lightingShader.SetFloat("spotLight.linear", _game.SpotLight.Linear);
            _lightingShader.SetFloat("spotLight.quadratic", _game.SpotLight.Quadratic);
            _lightingShader.SetFloat("spotLight.cutOff", _game.SpotLight.CutOff);
            _lightingShader.SetFloat("spotLight.outerCutOff", _game.SpotLight.OuterCutOff);

            foreach (var node in _scene.Nodes)
            {
                if (node is GameObject gameObject)
                {
                    gameObject.DiffuseMap.Use(TextureUnit.Texture0);
                    gameObject.SpecularMap.Use(TextureUnit.Texture1);

                    _lightingShader.SetInt("material.diffuse", gameObject.Material.diffuseUnit);
                    _lightingShader.SetInt("material.specular", gameObject.Material.specularUnit);
                    _lightingShader.SetVector3("material.specular", gameObject.Material.Specular);
                    _lightingShader.SetFloat("material.shininess", gameObject.Material.Shininess);

                    Matrix4 model = Matrix4.CreateTranslation(gameObject.Position);
                    model *= Matrix4.CreateFromAxisAngle(gameObject.Quaternion.Axis, MathHelper.DegreesToRadians(gameObject.Quaternion.Angle));
                    _lightingShader.SetMatrix4("model", model);

                    GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
                }
            }

            GL.BindVertexArray(_vaoLamp);

            _lampShader.Use();

            _lampShader.SetMatrix4("view", camera.GetViewMatrix());
            _lampShader.SetMatrix4("projection", camera.GetProjectionMatrix());

            // We use a loop to draw all the lights at the proper position
            foreach (var pointLight in _game.PointLights)
            {
                Matrix4 lampMatrix = Matrix4.CreateScale(pointLight.Scale);
                lampMatrix *= Matrix4.CreateTranslation(pointLight.Position);

                _lampShader.SetMatrix4("model", lampMatrix);

                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            }
        }
    }
}
