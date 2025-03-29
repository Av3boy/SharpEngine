#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

uniform vec2 position;
uniform float rotation;
uniform vec2 scale;

out vec3 Normal;
out vec3 FragPos;
out vec2 TexCoords;

void main()
{
    // Create a 2D rotation matrix
    mat2 rot = mat2(cos(rotation), -sin(rotation),
                    sin(rotation),  cos(rotation));

    // Apply the rotation, scaling, and translation
    vec2 transformedPos = (rot * aPos.xy) * scale + (position * 0.01);

    // Set the final position in clip space
    gl_Position = vec4(transformedPos, aPos.z, 1.0);

    FragPos = vec3(transformedPos, aPos.z);
    Normal = aNormal; // No need to transform normals for 2D UI elements
    TexCoords = aTexCoords;
}