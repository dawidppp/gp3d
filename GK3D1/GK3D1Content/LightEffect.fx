float4x4 World;
float4x4 View;
float4x4 Projection;

float3 ClipPoint;
float3 ClipVector;
float4 ClipPos;
float3 CameraPosition;
texture BasicTexture;
sampler BasicTextureSampler = sampler_state 
{
	texture = <BasicTexture>;
	MinFilter = Anisotropic; // Minification Filter
	MagFilter = Anisotropic; // Magnification Filter
	MipFilter = Linear; // Mip-mapping
	AddressU = Wrap; // Address Mode for U Coordinates
	AddressV = Wrap; // Address Mode for V Coordinates
};

bool TextureEnabled = false;

float3 DiffuseColor = float3(1, 1, 1);
float3 AmbientColor = float3(0.1, 0.1, 0.1);
float3 LightDirection = float3(0, 0, -1);
float3 LightColor = float3(0.9, 0.9, 0.9);

float SpecularPower = 256;
float3 SpecularColor = float3(1, 1, 1);


float3 DiffuseColor2 = float3(1, 1, 1);
float3 LightPosition2 = float3(0, 0, 50);
float3 LightDirection2 = float3(0, 0, 1);
//float ConeAngle2 =90;
float3 LightColor2 = float3(1, 1, 1);
//float LightFalloff2 = 200;
float SpecularPower2 = 32;
float3 SpecularColor2 = float3(1, 1, 1);
float Spot1Con = 32;
float Spot1Int =1.0f;

//float3 AmbientLightColor2 = float3(0, 0, 0);
float3 DiffuseColor3 = float3(1, 1, 1);
float3 LightPosition3 = float3(0, 0, 50);
float3 LightDirection3 = float3(0, 0.5f, 1);
//float ConeAngle2 =90;
float3 LightColor3 = float3(1, 0, 0);
//float LightFalloff2 = 200;
float Spot2Con = 64;
float Spot2Int =1.0f;

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
	float3 Normal : NORMAL0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
	float3 Normal : TEXCOORD1;
	float3 ViewDirection : TEXCOORD2;
	float4 clipDistances : TEXCOORD3;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4x4 viewProjection = mul(View, Projection);
	output.Position = mul(worldPosition, viewProjection);
	output.UV = input.UV;
	output.Normal = mul(input.Normal, World);
	output.ViewDirection = worldPosition - CameraPosition;
	output.clipDistances.x = dot(input.Position, ClipPos); 
	output.clipDistances.y = 0; 
	output.clipDistances.z = 0; 
	output.clipDistances.w = 0;
	return output;
}


float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{

	// Start with diffuse color
	float3 color = DiffuseColor;

	// Texture if necessary
	if (TextureEnabled)
		color *= tex2D(BasicTextureSampler, input.UV);

	// Start with ambient lighting
	float3 lighting = AmbientColor;
	float3 lightDir = normalize(LightDirection);
	float3 normal = normalize(input.Normal);

	// Add lambertian lighting
	lighting += saturate(dot(lightDir, normal)) * LightColor;

	float3 refl = reflect(lightDir, normal);
	float3 view = normalize(input.ViewDirection);

	// Add specular highlights
	lighting += pow(saturate(dot(refl, view)), SpecularPower) *	SpecularColor;

	// Calculate final color
	float3 output = saturate(lighting) * color;

	return float4(output, 1);
}


technique Technique1
{
	pass Pass1
	{
		VertexShader = compile vs_1_1 VertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}