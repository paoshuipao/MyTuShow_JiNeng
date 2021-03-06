// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Scene Manager/Cinema Effect" {
	Properties {
		_TintOffset ("Tint Offset", range(0,1)) = 0		// 0 == color, 1 == gray
		_FadeOffset ("Fade Offset", range(0,1)) = 0		// 0 == image, 1 == black
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
	
	sampler2D _ScreenContent;
	half4 _ScreenContent_ST;
	float _TintOffset;
	float _FadeOffset;
							
	struct v2f {
		half4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
	};
	
	v2f vert(appdata_full v) {
		v2f o;	
		o.pos = UnityObjectToClipPos (v.vertex);	
		o.uv.xy = TRANSFORM_TEX(v.texcoord, _ScreenContent);	
		#if UNITY_UV_STARTS_AT_TOP
		o.uv.y = 1 - o.uv.y;
		#endif		
		return o; 
	}
	
	fixed4 frag(v2f i) : COLOR {	
		float4 screenColor = tex2D(_ScreenContent, i.uv.xy);
		return lerp(screenColor, Luminance(screenColor), _TintOffset) * (1 - _FadeOffset);
	}
	
	ENDCG        
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		Lighting Off
		LOD 200
	
		GrabPass { "_ScreenContent" }
	
		Pass {
			CGPROGRAM	
			#pragma vertex vert
			#pragma fragment frag	
			ENDCG
		}            
	}
	
	FallBack "Diffuse"
} 