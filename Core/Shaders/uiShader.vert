#version 330 core

layout (location = 0) in vec3 aPos;
// layout (location = 1) in vec3 aNormal;
// layout (location = 2) in vec2 aTexCoord;

uniform mat4 model;
// uniform mat4 view;
// uniform mat4 projection;

out vec3 FragPos;

void main()
{
    vec4 pos = vec4(aPos, 1.0);
    gl_Position = model * pos;

    // gl_Position = vec4(aPos, 1.0) * model * view * projection;
    // FragPos = vec3(vec4(aPos, 1.0) * model);
    // TexCoord = aTexCoord;
    // Normal = aNormal * mat3(transpose(inverse(model)));
    // TexCoords = aTexCoords;
}