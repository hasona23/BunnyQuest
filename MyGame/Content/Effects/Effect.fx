#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
float2 xTextureSize : VPOS;
float4 xOutlineColour ;
float xTime;
float4 xFontColour;

sampler2D InputSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 UV : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    
    float4 currentPixel = tex2D(InputSampler, input.UV) * input.Color;
    float4 output = currentPixel;

    if (currentPixel.a == 0.0f)
    {
        
        float2 uvPix = float2(1 / xTextureSize.x, 1 / xTextureSize.y);

        if ( tex2D(InputSampler, float2((1 * uvPix.x) + input.UV.x, (0 * uvPix.y) + input.UV.y)).a > 0
			|| tex2D(InputSampler, float2((0 * uvPix.x) + input.UV.x, (1 * uvPix.y) + input.UV.y)).a > 0
			|| tex2D(InputSampler, float2((-1 * uvPix.x) + input.UV.x, (0 * uvPix.y) + input.UV.y)).a > 0
			|| tex2D(InputSampler, float2((0 * uvPix.x) + input.UV.x, (-1 * uvPix.y) + input.UV.y)).a > 0
		)
        {
            output = xOutlineColour * sin(xTime);
        }
    }
    if(currentPixel.a == 1)
        return xFontColour;
    return output;
}

technique SpriteOutline
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};