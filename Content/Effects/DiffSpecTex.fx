
// Declare matrices and light direction
float4x4 World;
float4x4 View;
float4x4 Projection;
float4 LightDir; // need this later
float4 lightPos;

// texture voodoo 
texture ModelTexture;
sampler2D textureSampler = sampler_state {
    Texture = (ModelTexture);
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

// each input vertex contains a position, normal and texture
struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Normal : NORMAL0;
	float2 TextureCoordinate : TEXCOORD0; 

};

// the values to be interpolated across triangle
struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0; 
    float4 Normal : TEXCOORD1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World); // object to world xform
    float4 viewPosition = mul(worldPosition, View);  // world to camera xform

    output.Position = mul(viewPosition, Projection); // perspective xform

	output.TextureCoordinate = input.TextureCoordinate;
	output.Normal = input.Normal;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{

	float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);  // get texture color
	float4 lightDirection = dot(input.Normal, LightDir);
	textureColor *= lightDirection;
	textureColor.a = 1.0;
    return saturate(textureColor);
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
