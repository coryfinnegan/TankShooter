
// Declare matrices and light direction
float4x4 World;
float4x4 View;
float4x4 Projection;
float4  LightDir; // need this later
texture ModelTexture; //It works without the Ambient :(
float4 Ambient;
float4 CameraPos;
float4 vertexPos;
float4 WorldPosition_Global;
float4 SurfaceNormal;
float4 LightColor;



// texture voodoo 

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
	float4 WorldPosition  : TEXCOORD2;

};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World); // object to world xform
    float4 viewPosition = mul(worldPosition, View);  // world to camera xform

    output.Position = mul(viewPosition, Projection); // perspective xform

	output.TextureCoordinate = input.TextureCoordinate;
	output.Normal = input.Normal;
	//WorldPosition_Global = worldPosition;
	output.WorldPosition = worldPosition;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	
	float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);  // get texture color
	//Specular
	float4 normalLight = normalize(LightDir);
	float4 verToCamera = CameraPos - input.WorldPosition;
	verToCamera = normalize(verToCamera);
	float4 Reflection = 2*(dot(input.Normal,normalLight)) * input.Normal - normalLight;
	float4 DotReflection = dot(Reflection, verToCamera);
	float4 specular = pow(DotReflection, 1);
	//float4 normalLightColor = normalize(LightColor);
	specular = saturate(specular);

	//Diffuse
	float4 diffuse = dot(input.Normal, LightDir);
	float4 textureColor_Sat = saturate(textureColor);
	//final calculation
	float4 finalColor = (LightColor * specular) + (textureColor * diffuse) + (textureColor_Sat * Ambient);
	//finalColor = 0.001 * finalColor + (textureColor_Sat * Ambient);
	textureColor.a = 1.0;
	//textureColor.g = 1.0;
	//finalColor = 0.1f * finalColor + input.Normal;
    return saturate(finalColor);
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
