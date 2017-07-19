// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MyShader/GridLine" {
	Properties {

       _MainTex ("Font Texture", 2D) = "white" {}

       _Color ("Tint Color", Color) = (1,1,1,1)
       
       _Alpha ("_Alpha",Range(0,1)) = 1
       
    }

	
	SubShader
	{
		Tags {"Queue"="Transparent+80" "IgnoreProjector"="True" "RenderType"="Transparent"}
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
			float _Alpha;

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
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = v.texcoord;
				return o;
			}

			fixed4 frag_mult(v2f_vct i) : COLOR
			{
				float4 newColor = float4(_Color.r,_Color.g,_Color.b,_Alpha)*i.color;
				fixed4 col = tex2D(_MainTex, i.texcoord)  *newColor;
				return col;
			}
			
			ENDCG
		} 
	}
   
}
