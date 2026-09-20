#version 410 core
in vec2 vUv;
out vec4 FragColor;
uniform sampler2D uTexture;
uniform vec4 uColor;

void main()
{
    float sdf = texture(uTexture, vUv).r; // 0..1 with 0.5 as edge
    float width = fwidth(sdf);
    float alpha = smoothstep(0.5 - width, 0.5 + width, sdf);
    FragColor = vec4(uColor.rgb, uColor.a * alpha);
}