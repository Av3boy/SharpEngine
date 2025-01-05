#version 330 core

layout (location = 0) in vec3 aPos;

uniform mat4 model;

out vec3 FragPos;

void main()
{
    vec4 pos = vec4(aPos, 1.0);

    gl_Position = model * pos;
    FragPos = vec3(model * pos);
}