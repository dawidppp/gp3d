#define NUMLIGHTS 3

float4x4 World;
float4x4 View;
float4x4 Projection;

// Point lights
float3 PointLightPosition = float3(200, -300, 0);
float3 PointLightColor = float3(1, 0, 0);
float PointLightAttenuation = 900;
float PointLightFalloff = 10;
float PointLightSpecularPower = 300;
float3 PointLightSpecularColor = float3(1, 1, 1);

// Spot lights
float3 DiffuseColor = float3(.85, .85, .85);
float3 LightPosition[NUMLIGHTS];
float3 LightDirection[NUMLIGHTS];
float3 LightColor[NUMLIGHTS];
float ConeAngle = 90;
float LightFalloff = 200;
float SpecularPower = 300;
float3 SpecularColor = float3(1, 1, 1);

float3 AmbientLightColor = float3(.15, .15, .15);
float3 CameraPosition;
texture BasicTexture;
bool TextureEnabled = true;

sampler BasicTextureSampler = sampler_state {
	texture = <BasicTexture>;
};


///////////////////////////////////////////////////// Technique for textured objects

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
	float4 WorldPosition : TEXCOORD2;
	float3 ViewDirection : TEXCOORD3;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);

	output.Position = mul(viewPosition, Projection);
	output.WorldPosition = worldPosition;
	output.ViewDirection = worldPosition - CameraPosition;
	output.UV = input.UV;
	output.Normal = mul(input.Normal, World);

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 diffuseColor = DiffuseColor;
	if (TextureEnabled)
		diffuseColor *= tex2D(BasicTextureSampler, input.UV).rgb;

	float3 totalLight = float3(0, 0, 0);
	totalLight += AmbientLightColor;

	// Point light
	float3 lightDir = normalize(PointLightPosition - input.WorldPosition);
	float diffuse = dot(normalize(input.Normal), lightDir);
	float d = distance(PointLightPosition, input.WorldPosition);
	float att = 1 - pow(clamp(d / PointLightAttenuation, 0, 1),	PointLightFalloff);
	float3 normal = normalize(input.Normal);
	float3 refl = reflect(lightDir, normal);
	float3 view = normalize(input.ViewDirection);
	totalLight += pow(saturate(dot(refl, view)), PointLightSpecularPower) * PointLightSpecularColor;
	totalLight += diffuse * att * PointLightColor;

	// Perform lighting calculations per spot light
	for (int i = 0; i < NUMLIGHTS; i++)
	{
		float3 lightDir = normalize(LightPosition[i] - input.WorldPosition);
		float diffuse = saturate(dot(normalize(input.Normal), lightDir));
		// (dot(p - lp, ld) / cos(a))^f
		float d = dot(-lightDir, normalize(LightDirection[i]));
		float a = cos(ConeAngle);
		float att = 0;

		//float3 lightDir = normalize(LightDirection);
		float3 normal = normalize(input.Normal);
		float3 refl = reflect(lightDir, normal);
		float3 view = normalize(input.ViewDirection);

		if (a < d){
			att = 1 - pow(clamp(a / d, 0, 1), LightFalloff);
			//specular
			totalLight += pow(saturate(dot(refl, view)), SpecularPower) * SpecularColor;
		}

		totalLight += diffuse * att * LightColor[i];
	}

	return float4(diffuseColor * totalLight, 1);
}

technique Technique1
{
    pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}

///////////////////////////////////////////////////// Technique for colored objects

struct VertexShaderInputColor
{
	float4 Position : POSITION;
	float4 Color : COLOR;
	float3 Normal : NORMAL;
};

struct VertexShaderOutputColor
{
	float4 Position : POSITION;
	float4 Color : COLOR;
	float3 Normal : TEXCOORD0;
	float4 WorldPosition : TEXCOORD1;
	float3 ViewDirection : TEXCOORD3;
};

VertexShaderOutputColor VertexShaderFunctionColor(VertexShaderInputColor input)
{
	VertexShaderOutputColor output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.ViewDirection = worldPosition - CameraPosition;
	output.WorldPosition = worldPosition;
	output.Color = input.Color;
	output.Normal = mul(input.Normal, World);
	return output;
}

float4 PixelShaderFunctionColor(VertexShaderOutputColor input) : COLOR0
{
	float3 diffuseColor = DiffuseColor;
	//if (TextureEnabled)
		diffuseColor *= input.Color ;

	float3 totalLight = float3(0, 0, 0);
	totalLight += AmbientLightColor;

	// Point light
	float3 lightDir = normalize(PointLightPosition - input.WorldPosition);
	float diffuse = dot(normalize(input.Normal), lightDir);
	float d = distance(PointLightPosition, input.WorldPosition);
	float att = 1 - pow(clamp(d / PointLightAttenuation, 0, 1),	PointLightFalloff);
	float3 normal = normalize(input.Normal);
	float3 refl = reflect(lightDir, normal);
	float3 view = normalize(input.ViewDirection);
	if( att > 0.9f)
		totalLight += pow(saturate(dot(refl, view)), PointLightSpecularPower) * PointLightSpecularColor;
	totalLight += diffuse * att * PointLightColor;


	// Perform lighting calculations per light
	for (int i = 0; i < NUMLIGHTS; i++)
	{
		float3 lightDir = normalize(LightPosition[i] - input.WorldPosition);
		float diffuse = saturate(dot(normalize(input.Normal), lightDir));
		// (dot(p - lp, ld) / cos(a))^f
		float d = dot(-lightDir, normalize(LightDirection[i]));
		float a = cos(ConeAngle);
		float att = 0;

		float3 normal = normalize(input.Normal);
		float3 refl = reflect(lightDir, normal);
		float3 view = normalize(input.ViewDirection);

		if (a < d){
			att = 1 - pow(clamp(a / d, 0, 1), LightFalloff);
			totalLight += pow(saturate(dot(refl, view)), SpecularPower) * SpecularColor;
		}

		totalLight += diffuse * att * LightColor[i];
	}

	return float4(diffuseColor * totalLight, 1);
}

technique Color
{
    pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunctionColor();
		PixelShader = compile ps_3_0 PixelShaderFunctionColor();
	}
}