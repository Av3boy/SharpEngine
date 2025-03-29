#version 330 core

uniform sampler2D texture1;
out vec4 FragColor;
in vec2 TexCoords;

void main()
{
    // FragColor = vec4(1.0, 0.0, 0.0, 1.0); // set all 4 vector values to 1.0
    FragColor = texture(texture1, TexCoords);
}