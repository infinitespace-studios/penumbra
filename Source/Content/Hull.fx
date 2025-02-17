﻿cbuffer cbPerObject : register(c0)
{
	float4 Color;
};

cbuffer cbPerFrame : register(c1)
{
	float4x4 ViewProjection;
};

struct VertexIn
{
	float2 Position : SV_POSITION0;
};

struct VertexOut
{
	float4 Position : SV_POSITION;
};

VertexOut VS(VertexIn vin)
{
	VertexOut vout;

	vout.Position = mul(float4(vin.Position.x, vin.Position.y, 0.0, 1.0), ViewProjection);

	return vout;
}

float4 PS(VertexOut pin) : SV_TARGET
{
	return Color;
}

technique Main
{
	pass P0
	{		
		VertexShader = compile vs_4_0_level_9_1 VS();
		PixelShader = compile ps_4_0_level_9_1 PS();
	}
}
