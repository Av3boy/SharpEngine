#version 410 core
layout(location = 0) in vec3 aPos;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec2 aUv;

out vec2 vUv;

uniform mat4 uModel;
uniform mat4 uViewProj;

void main()
{
    vUv = aUv;
    gl_Position = uViewProj * uModel * vec4(aPos, 1.0);
}