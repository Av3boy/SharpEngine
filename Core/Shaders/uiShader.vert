#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

uniform vec2 position;
uniform float rotation;
uniform vec2 scale;
uniform vec2 screenSize;
uniform mat4 clipSpace;
uniform mat4 orthoMatrix;

uniform mat4 view;
uniform mat4 projection;
uniform mat4 model;
uniform vec3 viewPos;

out vec3 Normal;
out vec3 FragPos;
out vec2 TexCoords;

void main()
{
    float reciprScaleOnscreen = 0.001f;

    // Calculate the model-view-projection matrix using the orthographic projection matrix
    //mat4 mvp = projection * view * model;
    vec4 mvp = model * vec4(1.0) * orthoMatrix;

    // Apply the MVP matrix to the vertex position with scaling
    gl_Position = mvp * vec4(aPos.x + (position.x * reciprScaleOnscreen), aPos.y + (position.y * reciprScaleOnscreen), aPos.z, 1);

    // Pass through other attributes
    Normal = mat3(model) * aNormal; // Transform the normal
    FragPos = vec3(model * vec4(aPos, 1.0)); // Transform the fragment position
    TexCoords = aTexCoords;
}