Shader "MyShader/waterfall" {
	 Properties {
	  _MainTex ("MainTexture", 2D) = "white" {}

       
	  _WaterSpeed ("WaterSpeed", Float) = 0.2

      
      _Color ("Tint Color", Color) = (1,1,1,1)
       
       

    }
    SubShader {
      	Tags { "Queue"="Transparent"  "IgnoreProjector"="True" "RenderType" = "Opaque" }
      	ZWrite Off Lighting Off Cull Off Fog { Mode Off } Blend SrcAlpha OneMinusSrcAlpha 
      	LOD 100
		
		CGPROGRAM
		#pragma surface surf Lambert alpha

      struct Input {
        float2 uv_MainTex;
		float3 worldPos;

      };
	  

      sampler2D _MainTex;


	  uniform float _WaterSpeed;
  

	  float4 _Color;


	  
      void surf (Input IN, inout SurfaceOutput o) 
	  {	  
			// top
			_WaterSpeed = _WaterSpeed*_Time;
			
			fixed4 ocol = tex2D(_MainTex, float2(IN.uv_MainTex.x,IN.uv_MainTex.y+_WaterSpeed));
			half3 col_orig = ocol.rgb;			
			
	
			//half3 col1 = tex2D(_MainTex, float2(IN.uv_MainTex.x+col_orig.b,IN.uv_MainTex.y+col_orig.g+_WaterSpeed)).rgb*1;  //水波混色处理


			half3 col = col_orig;
			o.Albedo = col*_Color; 
			o.Alpha = ocol.a;
			//o.Emission = 5;
			
      }
      ENDCG
    }
    Fallback "Diffuse"
}
