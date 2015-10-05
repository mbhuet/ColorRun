//Created by Phillip Heckinger, 2012.
//Distributed only on the Unity Asset Store.
//If purchased or downloaded from any other source,
//then these files were not obtained legally.
//Please support the Indie community and only use these
//files if you have obtained them legally.
//Thanks

Shader "TexturePaint/TexturePaintImpulseLocationTexShader"
{
	Properties
		{	
			fluidTex ("fluidTex", 2D) = "" {}
			colorTexMask ("colorTexMask", 2D) = "" {}
			
			textureData ("textureData", vector) = (0.0, 0.0, 0.0, 0.0)
			
			modColor ("modColor", vector) = (1.0, 1.0, 1.0, 1.0)
			
			globalStrength ("globalStrength", float) = 1.0
		}
	SubShader {
	Pass{
		CGPROGRAM
		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment frag

sampler2D fluidTex;
sampler2D colorTexMask;

float4 textureData;   //x is location x, y is location y, and z is color alpha

float4 modColor;

float4x4 rotationMatrix;

float globalStrength;

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
				float2 rotatedUVs = mul(rotationMatrix, i.uv.xyxy - float4((textureData.x * 2) - 0.5, (textureData.y * 2) - 0.5, 0, 0)).xy;
				
				return lerp(tex2D(fluidTex, i.uv), tex2D(colorTexMask, rotatedUVs + 0.5) * modColor, tex2D(colorTexMask, rotatedUVs + 0.5).a * textureData.z * globalStrength);
			}
		ENDCG
		}
	}
}
