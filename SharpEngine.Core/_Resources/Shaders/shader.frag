#version 330 core
in vec2 fUv;


uniform sampler2D uTexture0;

out vec4 FragColor;

void main()
{
    FragColor = vec4(1.0); // set all 4 vector values to 1.0
    // FragColor = texture(uTexture0, fUv);

}