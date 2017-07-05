﻿// unlit, vertex colour, alpha blended
// cull off

Shader "MyShader/BuildGroundShader" 
{
	Properties {

       _MainTex ("Font Texture", 2D) = "white" {}

       _Color ("Tint Color", Color) = (1,1,1,1)

    }

	
	SubShader
	{
		Tags {"Queue"="Transparent+100" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off Lighting Off Cull Off Fog { Mode Off } Blend SrcAlpha OneMinusSrcAlpha 
		LOD 110
		
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert_vct
			#pragma fragment frag_mult 
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;

			struct vin_vct 
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f_vct
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			v2f_vct vert_vct(vin_vct v)
			{
				v2f_vct o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord = v.texcoord;
				return o;
			}

			fixed4 frag_mult(v2f_vct i) : COLOR
			{
				fixed4 col = tex2D(_MainTex, i.texcoord) *_Color;				
				col*=fixed4(i.color.g,i.color.g,i.color.g,i.color.a);
				col+=(col*(1-i.color.r));
				//if(i.color.r==1&&i.color.b<1)
				//{
				//	col+=(col*(1-i.color.b));
				//}
				//else
				//col*= i.color;
				return col;
			}
			
			ENDCG
		} 
	}
}
