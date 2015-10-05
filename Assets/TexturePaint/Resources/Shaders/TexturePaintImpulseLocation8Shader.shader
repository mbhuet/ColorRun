//Created by Phillip Heckinger, 2012.
//Distributed only on the Unity Asset Store.
//If purchased or downloaded from any other source,
//then these files were not obtained legally.
//Please support the Indie community and only use these
//files if you have obtained them legally.
//Thanks

Shader "TexturePaint/TexturePaintImpulseLocation8Shader"
{
	Properties
		{	
			fluidTex ("fluidTex", 2D) = "" {}
			tempStorageBuffer ("tempStorageBuffer", 2D) = "" {}

			halfStorageRDX ("halfStorageRDX", float) = 0.015265
			globalStrength ("globalStrength", float) = 1.0
		}
	SubShader {
	Pass{
		CGPROGRAM
		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

sampler2D fluidTex;
sampler2D tempStorageBuffer;

float4 tempOutput;
float4 tempCalc;

float globalStrength;

float texelOffset;

float halfStorageRDX;

int j;

float impulseLocationX;
float impulseLocationY;
float4 impulseColor;
float colorSize;
float colorFalloff;
			
struct appdata
{
	float4 vertex : POSITION;
	float4 texcoord : TEXCOORD0;
};

struct v2f
{
	float4 pos : POSITION;
	float4 color : COLOR;
	float2 uv : TEXCOORD0;
};

v2f vert (appdata v)
{
	v2f o;
	o.pos = mul( UNITY_MATRIX_MVP, v.vertex );
	o.uv = float4( v.texcoord.xy, 0, 0 );
	return o;
}

		float4 frag(v2f i) : COLOR
			{
				tempOutput = tex2D(fluidTex, i.uv);
				
				for(j = 0; j < 8; j++)
				{
					texelOffset = halfStorageRDX + (j / 16.0);
					
					tempCalc = tex2D(tempStorageBuffer, float2(texelOffset, 0.125));
					impulseLocationX = (DecodeFloatRG(tempCalc.xy) * 2.0) - 0.5;
					impulseLocationY = (DecodeFloatRG(tempCalc.zw) * 2.0) - 0.5;
					
					impulseColor = tex2D(tempStorageBuffer, float2(texelOffset, 0.375));
																
					tempCalc = tex2D(tempStorageBuffer, float2(texelOffset, 0.625));
					colorSize = DecodeFloatRG(tempCalc.xy) * 2;
					colorFalloff = tempCalc.z * 150;
				
					tempCalc.a = impulseColor.a * clamp((distance(i.uv, float2(impulseLocationX, impulseLocationY)) - colorSize) * -colorFalloff, 0.0, 1.0) * globalStrength;
					tempOutput = lerp(tempOutput, impulseColor, tempCalc.a);
				}

				return tempOutput;
			}
		ENDCG
		}
	}
}
