#define NUMLIGHTS 3

float4x4 World;
float4x4 View;
float4x4 Projection;

// Point lights
float3 PointLightPosition = float3(200, -300, 0);
float4 PointLightColor = float4(1, 0, 0, 1);
float PointLightAttenuation = 900;
float PointLightFalloff = 10;
float PointLightSpecularPower = 300;
float4 PointLightSpecularColor = float4(1, 1, 1, 1);

// Spot lights
float4 DiffuseColor = float4(.85, .85, .85, 1);
float3 LightPosition[NUMLIGHTS];
float3 LightDirection[NUMLIGHTS];
float4 LightColor[NUMLIGHTS];
float ConeAngle = 90;
float LightFalloff = 200;
float SpecularPower = 300;
float4 SpecularColor = float4(1, 1, 1, 1);

// Fog
float FogStart = 300;
float FogEnd = 2000;
float FogEnabled = 0;
float FogPower = 0.5;

//clipping
float ClipPlane = 100;
bool ClipFront = false;

float4 AmbientLightColor = float4(.15, .15, .15, 1);
float3 CameraPosition;
texture BasicTexture;
texture OtherTexture;
bool TextureEnabled = true;
bool IsOtherTextureEnabled = false;

sampler BasicTextureSampler = sampler_state {
	texture = <BasicTexture>;
	AddressU = MIRROR;
};

sampler OtherTextureSampler = sampler_state {
	texture = <OtherTexture>;
	magfilter = LINEAR;
};

float ComputeFogFactor(float d)
{
	
    return clamp((d - FogStart) / (FogEnd - FogStart), 0, 1) * FogEnabled * FogPower;
}


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
	if(input.WorldPosition.x < ClipPlane && ClipFront || input.WorldPosition.x >= ClipPlane && !ClipFront)
		return float4(0,0,0,0);

	//clip((input.WorldPosition.x < ClipPlane && ClipFront || input.WorldPosition.x >= ClipPlane && !ClipFront) ? -1 : 1);
	float4 diffuseColor = DiffuseColor;
	float4 otherTextureColor;// = DiffuseColor;
	if (TextureEnabled)
	{
		diffuseColor *= tex2D(BasicTextureSampler, input.UV);
		if(IsOtherTextureEnabled)
			diffuseColor += tex2D(OtherTextureSampler, input.UV);
	}

	float4 totalLight = float4(0, 0, 0, 1);
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

	float4 finalColor = diffuseColor * totalLight;
	float dist = distance(input.WorldPosition + input.ViewDirection, input.WorldPosition);
	float fogFactor = ComputeFogFactor(dist);
	finalColor.rgb = lerp(finalColor.rgb, float4(1,1,1,1), fogFactor);

	return finalColor;
}

technique Technique1
{
	pass Pass1
	{
		AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
		FillMode = Solid;
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
	pass Pass2
	{
		AlphaBlendEnable = False;
		FillMode = WireFrame;
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

	//if(input.WorldPosition.x < ClipPlane && ClipFront || input.WorldPosition.x >= ClipPlane && !ClipFront)
		//return input.Color;
	clip((input.WorldPosition.x < ClipPlane && ClipFront || input.WorldPosition.x >= ClipPlane && !ClipFront) ? -1 : 1);
	float4 diffuseColor = DiffuseColor;
	//if (TextureEnabled)
		diffuseColor *= input.Color ;

	float4 totalLight = float4(0, 0, 0, 1);
	totalLight += AmbientLightColor;

	// Point light
	float3 lightDir = normalize(PointLightPosition - input.WorldPosition);
	float diffuse = dot(normalize(input.Normal), lightDir);
	float d = distance(PointLightPosition, input.WorldPosition);
	
		float att = 1 - pow(clamp(d / PointLightAttenuation, 0, 1),	PointLightFalloff);
		float3 normal = normalize(input.Normal);
		float3 refl = reflect(lightDir, normal);
		float3 view = normalize(input.ViewDirection);
		if( att > 0.8f)
			totalLight += pow(saturate(dot(refl, view)), PointLightSpecularPower) * PointLightSpecularColor;
	
		totalLight += diffuse * PointLightColor * att;
	


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

	float4 finalColor = diffuseColor * totalLight;
	float dist = distance(input.WorldPosition + input.ViewDirection, input.WorldPosition);
	float fogFactor = ComputeFogFactor(dist);
	finalColor.rgb = lerp(finalColor.rgb, float4(1,1,1,1), fogFactor);

	return finalColor;
}

technique Color
{
    pass Pass1
	{
		AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
		FillMode = Solid;
		VertexShader = compile vs_3_0 VertexShaderFunctionColor();
		PixelShader = compile ps_3_0 PixelShaderFunctionColor();
	}
	pass Pass2
	{
		AlphaBlendEnable = False;
		FillMode = WireFrame;
		VertexShader = compile vs_3_0 VertexShaderFunctionColor();
		PixelShader = compile ps_3_0 PixelShaderFunctionColor();
	}
}