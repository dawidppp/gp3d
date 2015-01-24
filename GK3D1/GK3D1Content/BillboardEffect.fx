float4x4 View;
float4x4 World;
float4x4 Projection;
float3 CameraPosition;
texture ParticleTexture;
sampler2D texSampler = sampler_state {
texture = <ParticleTexture>;
};
float2 Size;
float3 Up; // Camera's 'up' vector
float3 Side; // Camera's 'side' vector
bool AlphaTest = true;
bool AlphaTestGreater = true;
float AlphaTestValue = 0.5f;

// Fog
float FogStart = 300;
float FogEnd = 2000;
float FogEnabled = 0;
float FogPower = 0.5;

//clipping
float ClipPlane = 0;
bool ClipFront = true;

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
	float4 WorldPosition : TEXCOORD1;
	float3 ViewDirection : TEXCOORD2;	
};

float ComputeFogFactor(float d)
{
    return clamp((d - FogStart) / (FogEnd - FogStart), 0, 1) * FogEnabled * FogPower;
}

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	float3 position = input.Position;

	// Determine which corner of the rectangle this vertex
	// represents
	float2 offset = float2((input.UV.x - 0.5f) * 2.0f,
	-(input.UV.y - 0.5f) * 2.0f);

	// Move the vertex along the camera's 'plane' to its corner
	position += offset.x * Size.x * Side + offset.y * Size.y * Up;

	// Transform the position by view and projection
	output.Position = mul(float4(position, 1), mul(View, Projection));
	output.UV = input.UV;

	float4 worldPosition = mul(input.Position, World);
	output.WorldPosition = worldPosition;
	output.ViewDirection = worldPosition - CameraPosition;

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	clip((input.WorldPosition.x < ClipPlane && ClipFront || input.WorldPosition.x >= ClipPlane && !ClipFront) ? -1 : 1);
	float4 color = tex2D(texSampler, input.UV);
	
	if (AlphaTest)
		clip((color.a - AlphaTestValue) * (AlphaTestGreater ? 1 : -1));
	if(AlphaTestGreater == 1)
	{
		float dist = distance(input.WorldPosition + input.ViewDirection, input.WorldPosition);
		float fogFactor = ComputeFogFactor(dist);
		color = lerp(color, float4(1,1,1,1), fogFactor);
	}
	
	return color;
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