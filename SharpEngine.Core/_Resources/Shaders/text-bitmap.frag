#version 410 core
in vec2 vUv;
out vec4 FragColor;
uniform sampler2D uTexture;
uniform vec4 uColor;

void main()
{
    vec4 tex = texture(uTexture, vUv);
    FragColor = vec4(uColor.rgb * tex.rgb, uColor.a * tex.a);
}