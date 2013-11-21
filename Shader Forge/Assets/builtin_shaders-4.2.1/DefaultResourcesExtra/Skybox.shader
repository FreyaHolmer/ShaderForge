Shader "RenderFX/Skybox" {
Properties {
	_Tint ("Tint Color", Color) = (.5, .5, .5, .5)
	_FrontTex ("Front (+Z)", 2D) = "white" {}
	_BackTex ("Back (-Z)", 2D) = "white" {}
	_LeftTex ("Left (+X)", 2D) = "white" {}
	_RightTex ("Right (-X)", 2D) = "white" {}
	_UpTex ("Up (+Y)", 2D) = "white" {}
	_DownTex ("down (-Y)", 2D) = "white" {}
}

SubShader {
	Tags { "Queue"="Background" "RenderType"="Background" }
	Cull Off ZWrite Off Fog { Mode Off }
	
	CGINCLUDE
	#include "UnityCG.cginc"

	fixed4 _Tint;
	
	struct appdata_t {
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
	};
	struct v2f {
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
	};
	v2f vert (appdata_t v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.texcoord = v.texcoord;
		return o;
	}
	fixed4 skybox_frag (v2f i, sampler2D smp)
	{
		fixed4 tex = tex2D (smp, i.texcoord);
		fixed4 col;
		col.rgb = tex.rgb + _Tint.rgb - unity_ColorSpaceGrey;
		col.a = tex.a * _Tint.a;
		return col;
	}
	ENDCG
	
	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest
		sampler2D _FrontTex;
		fixed4 frag (v2f i) : COLOR { return skybox_frag(i,_FrontTex); }
		ENDCG 
	}
	Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest
		sampler2D _BackTex;
		fixed4 frag (v2f i) : COLOR { return skybox_frag(i,_BackTex); }
		ENDCG 
	}
	Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest
		sampler2D _LeftTex;
		fixed4 frag (v2f i) : COLOR { return skybox_frag(i,_LeftTex); }
		ENDCG
	}
	Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest
		sampler2D _RightTex;
		fixed4 frag (v2f i) : COLOR { return skybox_frag(i,_RightTex); }
		ENDCG
	}	
	Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest
		sampler2D _UpTex;
		fixed4 frag (v2f i) : COLOR { return skybox_frag(i,_UpTex); }
		ENDCG
	}	
	Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest
		sampler2D _DownTex;
		fixed4 frag (v2f i) : COLOR { return skybox_frag(i,_DownTex); }
		ENDCG
	}
}	

SubShader {
	Tags { "Queue"="Background" "RenderType"="Background" }
	Cull Off ZWrite Off Fog { Mode Off }
	Color [_Tint]
	Pass {
		SetTexture [_FrontTex] { combine texture +- primary, texture * primary }
	}
	Pass {
		SetTexture [_BackTex]  { combine texture +- primary, texture * primary }
	}
	Pass {
		SetTexture [_LeftTex]  { combine texture +- primary, texture * primary }
	}
	Pass {
		SetTexture [_RightTex] { combine texture +- primary, texture * primary }
	}
	Pass {
		SetTexture [_UpTex]    { combine texture +- primary, texture * primary }
	}
	Pass {
		SetTexture [_DownTex]  { combine texture +- primary, texture * primary }
	}
}
}