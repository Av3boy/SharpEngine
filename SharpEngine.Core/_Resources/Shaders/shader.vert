#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

out vec3 Normal;
out vec3 FragPos;
out vec2 TexCoords;

void main()
{
    gl_Position = vec4(aPos, 1.0) * uModel * uView * uProjection;
    FragPos = vec3(vec4(aPos, 1.0) * uModel);
    Normal = aNormal * mat3(transpose(inverse(uModel)));
    TexCoords = aTexCoords;
}