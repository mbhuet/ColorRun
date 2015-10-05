//Created by Phillip Heckinger, 2012.
//Distributed only on the Unity Asset Store.
//If purchased or downloaded from any other source,
//then these files were not obtained legally.
//Please support the Indie community and only use these
//files if you have obtained them legally.
//Thanks

Shader "TexturePaint/TexturePaintColorDissipationTexShader"
{
	Properties
		{	
			fluidTex ("fluidTex", 2D) = "" {}
			dissipationTex ("dissipationTex", 2D) = "" {}
			
			dissipationColor ("dissipationColor", vector) = (0,0,0,0)
			
			dissipation ("dissipation", float) = 0.01
		}
	SubShader {
	Pass{
		CGPROGRAM
		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

sampler2D fluidTex;
sampler2D dissipationTex;

float4 tempOutput;

float dissipation;

float4 dissipationColor;
			
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
				tempOutput = lerp(tex2D(fluidTex, i.uv), dissipationColor, tex2D(dissipationTex, i.uv).x * dissipation);

				return tempOutput;
			}
		ENDCG
		}
	}
}
