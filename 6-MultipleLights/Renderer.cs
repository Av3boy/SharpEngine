using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;

namespace LearnOpenTK
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
        private Game _game;

        private DirectionalLight _directionalLight;
        private PointLight[] _pointLights;
        private SpotLight _spotLight;

        public Renderer(Game game)
        {
            _game = game;
        }

        public void Initialize()
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            InitializeBuffers();
            InitializeShaders();
            InitializeVertexArrays();
            LoadTextures();

            // Initialize lights
            _directionalLight = new DirectionalLight
            {
                Direction = new Vector3(-0.2f, -1.0f, -0.3f),
                Ambient = new Vector3(0.05f, 0.05f, 0.05f),
                Diffuse = new Vector3(0.4f, 0.4f, 0.4f),
                Specular = new Vector3(0.5f, 0.5f, 0.5f)
            };

            _pointLights = new PointLight[_game.PointLightPositions.Length];
            for (int i = 0; i < _game.PointLightPositions.Length; i++)
            {
                _pointLights[i] = new PointLight
                {
                    Position = _game.PointLightPositions[i],
                    Ambient = new Vector3(0.05f, 0.05f, 0.05f),
                    Diffuse = new Vector3(0.8f, 0.8f, 0.8f),
                    Specular = new Vector3(1.0f, 1.0f, 1.0f),
                    Constant = 1.0f,
                    Linear = 0.09f,
                    Quadratic = 0.032f
                };
            }

            _spotLight = new SpotLight
            {
                Ambient = new Vector3(0.0f, 0.0f, 0.0f),
                Diffuse = new Vector3(1.0f, 1.0f, 1.0f),
                Specular = new Vector3(1.0f, 1.0f, 1.0f),
                Constant = 1.0f,
                Linear = 0.09f,
                Quadratic = 0.032f,
                CutOff = MathF.Cos(MathHelper.DegreesToRadians(12.5f)),
                OuterCutOff = MathF.Cos(MathHelper.DegreesToRadians(17.5f))
            };
        }

        private void InitializeBuffers()
        {
            _vertexBufferObject = GL.GenBuffer();

            var vertices = _game.Vertices;

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
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
            _diffuseMap = Texture.LoadFromFile("Resources/container2.png");
            _specularMap = Texture.LoadFromFile("Resources/container2_specular.png");
        }

        public void Render(Camera camera)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindVertexArray(_vaoModel);

            _diffuseMap.Use(TextureUnit.Texture0);
            _specularMap.Use(TextureUnit.Texture1);
            _lightingShader.Use();

            _lightingShader.SetMatrix4("view", camera.GetViewMatrix());
            _lightingShader.SetMatrix4("projection", camera.GetProjectionMatrix());

            _lightingShader.SetVector3("viewPos", camera.Position);

            _lightingShader.SetInt("material.diffuse", 0);
            _lightingShader.SetInt("material.specular", 1);
            _lightingShader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
            _lightingShader.SetFloat("material.shininess", 32.0f);

            // Directional light
            _lightingShader.SetVector3("dirLight.direction", _directionalLight.Direction);
            _lightingShader.SetVector3("dirLight.ambient", _directionalLight.Ambient);
            _lightingShader.SetVector3("dirLight.diffuse", _directionalLight.Diffuse);
            _lightingShader.SetVector3("dirLight.specular", _directionalLight.Specular);

            // Point lights
            for (int i = 0; i < _pointLights.Length; i++)
            {
                _lightingShader.SetVector3($"pointLights[{i}].position", _pointLights[i].Position);
                _lightingShader.SetVector3($"pointLights[{i}].ambient", _pointLights[i].Ambient);
                _lightingShader.SetVector3($"pointLights[{i}].diffuse", _pointLights[i].Diffuse);
                _lightingShader.SetVector3($"pointLights[{i}].specular", _pointLights[i].Specular);
                _lightingShader.SetFloat($"pointLights[{i}].constant", _pointLights[i].Constant);
                _lightingShader.SetFloat($"pointLights[{i}].linear", _pointLights[i].Linear);
                _lightingShader.SetFloat($"pointLights[{i}].quadratic", _pointLights[i].Quadratic);
            }

            // Spot light
            _spotLight.Position = camera.Position;
            _spotLight.Direction = camera.Front;

            _lightingShader.SetVector3("spotLight.position", _spotLight.Position);
            _lightingShader.SetVector3("spotLight.direction", _spotLight.Direction);
            _lightingShader.SetVector3("spotLight.ambient", _spotLight.Ambient);
            _lightingShader.SetVector3("spotLight.diffuse", _spotLight.Diffuse);
            _lightingShader.SetVector3("spotLight.specular", _spotLight.Specular);
            _lightingShader.SetFloat("spotLight.constant", _spotLight.Constant);
            _lightingShader.SetFloat("spotLight.linear", _spotLight.Linear);
            _lightingShader.SetFloat("spotLight.quadratic", _spotLight.Quadratic);
            _lightingShader.SetFloat("spotLight.cutOff", _spotLight.CutOff);
            _lightingShader.SetFloat("spotLight.outerCutOff", _spotLight.OuterCutOff);

            for (int i = 0; i < _game.Cubes.Length; i++)
            {
                Matrix4 model = Matrix4.CreateTranslation(_game.Cubes[i].Position);
                float angle = 20.0f * i;
                model = model * Matrix4.CreateFromAxisAngle(new Vector3(1.0f, 0.3f, 0.5f), MathHelper.DegreesToRadians(angle));
                _lightingShader.SetMatrix4("model", model);

                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            }

            GL.BindVertexArray(_vaoLamp);

            _lampShader.Use();

            _lampShader.SetMatrix4("view", camera.GetViewMatrix());
            _lampShader.SetMatrix4("projection", camera.GetProjectionMatrix());
            // We use a loop to draw all the lights at the proper position
            for (int i = 0; i < _pointLights.Length; i++)
            {
                Matrix4 lampMatrix = Matrix4.CreateScale(0.2f);
                lampMatrix = lampMatrix * Matrix4.CreateTranslation(_pointLights[i].Position);

                _lampShader.SetMatrix4("model", lampMatrix);

                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            }
        }
    }
}
