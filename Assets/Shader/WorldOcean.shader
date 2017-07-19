// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MyShader/WorldOcean" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ColorTex ("color (RGB)", 2D) = "white" {}
		_Color ("Bg Color", Color) = (1,1,1,1)
		_IslandColor ("Island Color", Color) = (1,1,1,1)

	}
	SubShader {
		Tags {"Queue" = "Transparent-100"}
 		zwrite off
//		ztest less
		blend srcalpha oneminussrcalpha
		pass
		{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			sampler2D _ColorTex;
			float4 _Color;
			float4 _IslandColor;
					
			struct vertexinput
			{
				half4 texcoord:TEXCOORD0;
				half4 vertex:POSITION;
			};
			struct v2f
			{
			 	half4  pos: SV_POSITION;
			 	half4  uv: TEXCOORD0;
			 	float4  uv2: TEXCOORD1;
			};
			v2f vert (vertexinput v)
			{
				v2f o;
				o.uv = v.texcoord;
				float cc = frac(_Time.y*0.001);
				o.uv2 = (v.texcoord-float4(cc,cc,0,0))*30;
				
				o.uv.z = sin(v.vertex.x *0.75+frac(_Time.x*4)*6.2831852);
				o.uv2.z = sin(o.uv.z*3.1415926)*0.5+0.5;				

				o.uv.z = o.uv.z*0.5+0.5;
				o.pos = UnityObjectToClipPos(v.vertex);

				half4 scrPos = ComputeScreenPos(o.pos);
				scrPos.xyz/=scrPos.w;
				scrPos.x = clamp(scrPos.x,0,1);
				scrPos.y = clamp(scrPos.y,0,1);
				half width = (0.5 - abs( 0.5 - scrPos.x))*2;
				o.uv.w = scrPos.y*scrPos.y*scrPos.y*width*width;
				return o;
			}
			half4 frag(v2f i) : COLOR
			{
				half4 tex1 = tex2D(_MainTex, i.uv2.xy);
				half4 tex4 = tex2D(_ColorTex, i.uv.xy);
				half tex = lerp(tex1.r,tex1.g,i.uv2.z *i.uv.z);
				half tex3 = lerp(tex1.b*tex1.b,tex1.a*tex1.a,i.uv2.z);
				tex4.rgb = tex4.rgb*0.15 + tex*0.85;
				tex4.rgb += tex3* i.uv.w *2;
				tex4.rgb*=_IslandColor.rgb; 
				tex4.rgb+=_Color.rgb;
				return tex4;
			}			
			ENDCG
		}
	} 
}
