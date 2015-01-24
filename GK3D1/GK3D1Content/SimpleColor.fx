float4x4 WVP;

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float4 Color    : COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 Color    : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position = mul(input.Position, WVP);

	output.Color = input.Color;
  
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return input.Color;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
