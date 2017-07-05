Shader "MyShader/Ocean" {
	 Properties {
	  _MainTex ("MainTexture", 2D) = "white" {}
      //_BumpTex ("BumpTexture", 2D) = "white" {}
      
	  _DirectionX("DirectionX",float) = 0   //x轴流向
      _DirectionZ("DirectionZ",float) = 0   //z轴流向
       
	  _WaterSpeed ("WaterSpeed", Float) = 0.2
	  _BumpSpeed ("BumpSpeed", Float) = 0.2
	  _BottomDepth ("BottomDepth", Float) = 0.05  //模拟的二层水的深度
	  
	  _SelfLight("SelfLight",Range(0,1)) = 0.5  //自发光;
	  _Alpha("Alpha",Range(0,1)) = 0.5
	  _LightStep("LightStep",Range(1,10)) = 1    //模拟光照;
	
      
      
      _Color ("Tint Color", Color) = (1,1,1,1)
       
       

    }
    SubShader {
      	Tags { "Queue"="Transparent-10"  "IgnoreProjector"="True" "RenderType" = "Opaque" }
      	LOD 100
		
		CGPROGRAM
		#pragma surface surf Lambert alpha

      struct Input {
        float2 uv_MainTex;
        float2 uv_BumpTex;
		float3 worldPos;

      };
	  

      sampler2D _MainTex;
      //sampler2D _BumpTex;

	  uniform float _WaterSpeed;
	  uniform float _BumpSpeed;
	  uniform float _BottomDepth;	  
	  float _SelfLight;
	  float _Alpha;
	  float4 _Color;
	  float _LightStep;
	  float _DirectionX;
	  float _DirectionZ;

	  
      void surf (Input IN, inout SurfaceOutput o) 
	  {	  
			// top
			float waveslidex = _WaterSpeed*_Time*_DirectionX;
			float waveslidez = _WaterSpeed*_Time*_DirectionZ;
			half3 col_orig = tex2D(_MainTex, float2(IN.uv_MainTex.x+waveslidex,IN.uv_MainTex.y+waveslidez)).rgb*_BottomDepth;			
			
			float waveslidex2 = _BumpSpeed*_Time*_DirectionX;
			float waveslidez2 = _BumpSpeed*_Time*_DirectionZ;
			half3 col1 = tex2D(_MainTex, float2(IN.uv_MainTex.x+col_orig.b+waveslidex2,IN.uv_MainTex.y+col_orig.g+waveslidez2)).rgb*1;  //水波混色处理
			//fixed4 bump = tex2D(_BumpTex,float2(IN.uv_BumpTex.x+waveslidex2,IN.uv_BumpTex.y+waveslidez2));
			//fixed4 bumpnormal =float4(bump.xyz, 1.0);

			half3 col = col1*_LightStep;
			o.Albedo = col*_Color; 
			o.Alpha = _Alpha;
			//o.Normal = bumpnormal;
			//o.Normal = normalize(o.Normal);
			o.Emission = _SelfLight; 
			
      }
      ENDCG
    }
    Fallback "Diffuse"
}
