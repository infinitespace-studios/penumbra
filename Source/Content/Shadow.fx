﻿cbuffer cbConstant : register(c0)
{
	float4 Color; // Used in debugging.
};

cbuffer cbPerFrame : register(c1)
{
	float4x4 ViewProjection;
};

cbuffer cbPerObject : register(c5)
{		
	float2 LightPosition;
	float LightRadius;
};

struct VertexIn
{	
	float2 SegmentA : TEXCOORD0;
	float2 SegmentB : TEXCOORD1;
	float2 Occlusion : SV_POSITION0;	
};

struct VertexOut
{
	float4 Position : SV_POSITION;
	float4 Penumbra : TEXCOORD1;
	float ClipValue : TEXCOORD2;
};

float2x2 Invert(float2x2 m)
{
	return float2x2(m._m11, -m._m01, -m._m10, m._m00) / determinant(m);
}

VertexOut VS(VertexIn vin)
{
	// Segments are in CCW order.
	// Occlusion.x determines if we are dealing with segment vertex A or vertex B.	
	// Occlusion.y determines if we are projecting the vertex or not.	
	
	float2 toSegmentA = vin.SegmentA - LightPosition;
	float2 toSegmentB = vin.SegmentB - LightPosition;

	// Find radius offsets 90deg left and right from light source relative to vertex.
	float2 toLightOffsetA = float2(-LightRadius, LightRadius)*normalize(toSegmentA).yx;
	float2 toLightOffsetB = float2(LightRadius, -LightRadius)*normalize(toSegmentB).yx;
	float2 lightOffsetA = LightPosition + toLightOffsetA; // 90 CCW.
	float2 lightOffsetB = LightPosition + toLightOffsetB; // 90 CW.

	// From each edge, project a quad. We have 4 vertices per edge.	
	float2 position = lerp(vin.SegmentA, vin.SegmentB, vin.Occlusion.x);
	float2 projectionOffset = lerp(lightOffsetA, lightOffsetB, vin.Occlusion.x);
	float4 projected = float4(position - projectionOffset*vin.Occlusion.y, 0.0, 1.0 - vin.Occlusion.y);

	// Transform to ndc.
	float4 clipPosition = mul(projected, ViewProjection);
	
	float2 penumbraA = mul(projected.xy - (vin.SegmentA)*projected.w, Invert(float2x2(toLightOffsetA, toSegmentA)));
	float2 penumbraB = mul(projected.xy - (vin.SegmentB)*projected.w, Invert(float2x2(toLightOffsetB, toSegmentB)));

	float2 clipNormal = normalize(vin.SegmentB - vin.SegmentA).yx*float2(-1.0, 1.0);
	// 90 CCW. ClipValue > 0 means the projection is pointing towards light => no shadow should be generated.
	float clipValue = dot(clipNormal, projected.xy - projected.w*position);
	
	VertexOut vout;
	vout.Position = clipPosition;
	vout.Penumbra = float4(penumbraA, penumbraB);
	vout.ClipValue = clipValue;

	return vout;
}

float4 PS(VertexOut pin) : SV_TARGET
{
	// If clipvalue > 0, don't shadow. We are clipping shadows cast from edges which normals are pointing
	// towards the light.
	clip(-pin.ClipValue);

	float2 p = clamp(pin.Penumbra.xz / pin.Penumbra.yw, -1.0, 1.0);
	float2 value = lerp(p*(3.0 - p*p)*0.25 + 0.5, 1.0, step(pin.Penumbra.yw, 0.0));	
	float occlusion = (value.x + value.y - 1.0);
	return float4(0.0, 0.0, 0.0, occlusion);
}

float4 PSDebug(VertexOut pin) : SV_TARGET
{
	clip(-pin.ClipValue);
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

technique Debug
{
	pass P0
	{
		VertexShader = compile vs_4_0_level_9_1 VS();
		PixelShader = compile ps_4_0_level_9_1 PSDebug();
	}
}
