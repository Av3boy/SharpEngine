#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

uniform float width;
uniform float height;

uniform vec2 position;
uniform float rotation;
uniform mat4 orthoMatrix;

uniform mat4 uModel;

out vec3 Normal;
out vec3 FragPos;
out vec2 TexCoords;

void main()
{
    // Create the rotation matrix for the z-axis
    mat4 rotationMatrix = mat4(
        cos(rotation), -sin(rotation), 0.0, 0.0,
        sin(rotation),  cos(rotation), 0.0, 0.0,
        0.0,            0.0,           1.0, 0.0,
        0.0,            0.0,           0.0, 1.0
    );

    // Scale the vertex position using pixel-space width and height.
    vec4 scaledPos = vec4(
        aPos.x * width * 0.5,
        aPos.y * height * 0.5,
        aPos.z,
        1.0
    );

    // Apply the model scale and rotation before translating in pixel space.
    vec4 transformedPos = rotationMatrix * uModel * scaledPos;

    // Project directly using the window-sized orthographic matrix.
    gl_Position = orthoMatrix * vec4(
        transformedPos.x + position.x,
        transformedPos.y + position.y,
        transformedPos.z,
        1.0
    );

    // Pass through other attributes
    Normal = mat3(rotationMatrix * uModel) * aNormal; // Transform the normal
    FragPos = vec3(transformedPos.x + position.x, transformedPos.y + position.y, transformedPos.z);
    TexCoords = aTexCoords;
}