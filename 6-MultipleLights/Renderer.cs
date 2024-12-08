using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace Core
{
    public class Renderer
    {
        private int _vertexBufferObject;
        private int _vaoModel;
        private int _vaoLamp;

        private Shader _lampShader;
        private Shader _lightingShader;
        private Texture _diffuseMap;
        private Texture _specularMap;

        private IGame _game;
        private Scene _scene;

        // TODO: Read only once, load into OpenGL buffer once.
        // If already loaded, add mesh indetifier to a dictionary. If dict contains mesh, skip it.
        public float[] Vertices => GetVertices();

        private float[] GetVertices()
        {
            var mesh = Primitives.Cube.Mesh; // Mesh is identical for all cubes
            var vertices = new List<float>();

            for (int i = 0; i < mesh.Vertices.Length / 3; i++)
            {
                vertices.Add(mesh.Vertices[i * 3]);
                vertices.Add(mesh.Vertices[i * 3 + 1]);
                vertices.Add(mesh.Vertices[i * 3 + 2]);

                vertices.Add(mesh.Normals[i * 3]);
                vertices.Add(mesh.Normals[i * 3 + 1]);
                vertices.Add(mesh.Normals[i * 3 + 2]);

                vertices.Add(mesh.TextureCoordinates[i * 2]);
                vertices.Add(mesh.TextureCoordinates[i * 2 + 1]);
            }

            return vertices.ToArray();
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
            LoadTextures();
        }

        private void InitializeBuffers()
        {
            _vertexBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);
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
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

            var normalLocation = _lightingShader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

            var texCoordLocation = _lightingShader.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));

            _vaoLamp = GL.GenVertexArray();
            GL.BindVertexArray(_vaoLamp);

            positionLocation = _lampShader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
        }

        private void LoadTextures()
        {
            _diffuseMap = TextureService.Instance.LoadTexture("Resources/container2.png");
            _specularMap = TextureService.Instance.LoadTexture("Resources/container2_specular.png");
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

                    _lightingShader.SetInt("material.diffuse", 0);
                    _lightingShader.SetInt("material.specular", 1);
                    _lightingShader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
                    _lightingShader.SetFloat("material.shininess", 32.0f);

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
            for (int i = 0; i < _game.PointLights.Length; i++)
            {
                Matrix4 lampMatrix = Matrix4.CreateScale(0.2f);
                lampMatrix = lampMatrix * Matrix4.CreateTranslation(_game.PointLights[i].Position);

                _lampShader.SetMatrix4("model", lampMatrix);

                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            }
        }
    }
}
