//Created by Phillip Heckinger, 2012.
//Distributed only on the Unity Asset Store.
//If purchased or downloaded from any other source,
//then these files were not obtained legally.
//Please support the Indie community and only use these
//files if you have obtained them legally.
//Thanks

Shader "TexturePaint/TexturePaintInitializeToTextureShader"
{
	Properties
		{
			initialTexture ("initialTexture", 2D) = "" {}
		}
	SubShader {
	Pass{
		CGPROGRAM
		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment frag

sampler2D initialTexture;
			
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
				return tex2D(initialTexture, i.uv);
			}
		ENDCG
		}
	}
}
