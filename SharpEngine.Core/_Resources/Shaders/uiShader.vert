#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

uniform float width;
uniform float height;

uniform vec2 position;
uniform float rotation;
uniform vec2 screenSize;
uniform mat4 orthoMatrix;

uniform mat4 uModel;

out vec3 Normal;
out vec3 FragPos;
out vec2 TexCoords;

void main()
{
    float reciprScaleOnscreen = 0.005f;

    // Calculate the aspect ratio
    float aspectRatio = screenSize.x / screenSize.y;

    // Create the rotation matrix for the z-axis
    mat4 rotationMatrix = mat4(
        cos(rotation), -sin(rotation), 0.0, 0.0,
        sin(rotation),  cos(rotation), 0.0, 0.0,
        0.0,            0.0,           1.0, 0.0,
        0.0,            0.0,           0.0, 1.0
    );

    // Scale the vertex position using width and height
    vec3 scaledPos = vec3(
        aPos.x * width * reciprScaleOnscreen,
        aPos.y * height * reciprScaleOnscreen,
        aPos.z
    );

    // Calculate the model-view-projection matrix using the orthographic projection matrix
    mat4 mvp = orthoMatrix * rotationMatrix * uModel;

    // Apply the MVP matrix to the scaled vertex position with aspect ratio
    gl_Position = mvp * vec4(
        (scaledPos.x + (position.x * reciprScaleOnscreen)) * aspectRatio,
        scaledPos.y + (position.y * reciprScaleOnscreen),
        scaledPos.z,
        1
    );

    // Pass through other attributes
    Normal = mat3(uModel) * aNormal; // Transform the normal
    FragPos = vec3(uModel * vec4(aPos, 1.0)); // Transform the fragment position
    TexCoords = aTexCoords;
}