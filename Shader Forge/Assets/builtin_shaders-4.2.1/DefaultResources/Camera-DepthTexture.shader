Shader "Hidden/Camera-DepthTexture" {
Properties {
	_MainTex ("", 2D) = "white" {}
	_Cutoff ("", Float) = 0.5
	_Color ("", Color) = (1,1,1,1)
}
Category {
	Fog { Mode Off }

SubShader {
	Tags { "RenderType"="Opaque" }
	Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
struct v2f {
    float4 pos : POSITION;
	#ifdef UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE
    float2 depth : TEXCOORD0;
	#endif
};
v2f vert( appdata_base v ) {
    v2f o;
    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
    UNITY_TRANSFER_DEPTH(o.depth);
    return o;
}
fixed4 frag(v2f i) : COLOR {
    UNITY_OUTPUT_DEPTH(i.depth);
}
ENDCG
	}
}

SubShader {
	Tags { "RenderType"="TransparentCutout" }
	Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
struct v2f {
    float4 pos : POSITION;
	float2 uv : TEXCOORD0;
	#ifdef UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE
    float2 depth : TEXCOORD1;
	#endif
};
uniform float4 _MainTex_ST;
v2f vert( appdata_base v ) {
    v2f o;
    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
    UNITY_TRANSFER_DEPTH(o.depth);
    return o;
}
uniform sampler2D _MainTex;
uniform fixed _Cutoff;
uniform fixed4 _Color;
fixed4 frag(v2f i) : COLOR {
	fixed4 texcol = tex2D( _MainTex, i.uv );
	clip( texcol.a*_Color.a - _Cutoff );
    UNITY_OUTPUT_DEPTH(i.depth);
}
ENDCG
	}
}

SubShader {
	Tags { "RenderType"="TreeBark" }
	Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma glsl_no_auto_normalization
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "TerrainEngine.cginc"
struct v2f {
    float4 pos : POSITION;
	#ifdef UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE
	float2 depth : TEXCOORD0;
	#endif
};
v2f vert( appdata_full v ) {
    v2f o;
    TreeVertBark(v);
	
	o.pos = mul( UNITY_MATRIX_MVP, v.vertex );
    UNITY_TRANSFER_DEPTH(o.depth);
    return o;
}
fixed4 frag(v2f i) : COLOR {
	UNITY_OUTPUT_DEPTH(i.depth);
}
ENDCG
	}
}

SubShader {
	Tags { "RenderType"="TreeLeaf" }
	Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma glsl_no_auto_normalization
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "TerrainEngine.cginc"
struct v2f {
	float4 pos : POSITION;
	float2 uv : TEXCOORD0;
	#ifdef UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE
	float2 depth : TEXCOORD1;
	#endif
};
v2f vert( appdata_full v ) {
    v2f o;
    TreeVertLeaf(v);
	
	o.pos = mul( UNITY_MATRIX_MVP, v.vertex );
	o.uv = v.texcoord.xy;
    UNITY_TRANSFER_DEPTH(o.depth);
    return o;
}
uniform sampler2D _MainTex;
uniform fixed _Cutoff;
fixed4 frag(v2f i) : COLOR {
	half alpha = tex2D(_MainTex, i.uv).a;

	clip (alpha - _Cutoff);
	UNITY_OUTPUT_DEPTH(i.depth);
}
ENDCG
	}
}

SubShader {
	Tags { "RenderType"="TreeOpaque" }
	Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "TerrainEngine.cginc"
struct v2f {
	float4 pos : POSITION;
	#ifdef UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE
	float2 depth : TEXCOORD0;
	#endif
};
struct appdata {
    float4 vertex : POSITION;
    fixed4 color : COLOR;
};
v2f vert( appdata v ) {
	v2f o;
	TerrainAnimateTree(v.vertex, v.color.w);
	o.pos = mul( UNITY_MATRIX_MVP, v.vertex );
    UNITY_TRANSFER_DEPTH(o.depth);
	return o;
}
fixed4 frag( v2f i ) : COLOR {
    UNITY_OUTPUT_DEPTH(i.depth);
}
ENDCG
	}
} 

SubShader {
	Tags { "RenderType"="TreeTransparentCutout" }
	Pass {
		Cull Off
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "TerrainEngine.cginc"

struct v2f {
	float4 pos : POSITION;
	float2 uv : TEXCOORD0;
	#ifdef UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE
	float2 depth : TEXCOORD1;
	#endif
};
struct appdata {
    float4 vertex : POSITION;
    fixed4 color : COLOR;
    float4 texcoord : TEXCOORD0;
};
v2f vert( appdata v ) {
	v2f o;
	TerrainAnimateTree(v.vertex, v.color.w);
	o.pos = mul( UNITY_MATRIX_MVP, v.vertex );
	o.uv = v.texcoord.xy;
    UNITY_TRANSFER_DEPTH(o.depth);
	return o;
}
uniform sampler2D _MainTex;
uniform fixed _Cutoff;
fixed4 frag( v2f i ) : COLOR {
	half alpha = tex2D(_MainTex, i.uv).a;

	clip (alpha - _Cutoff);
    UNITY_OUTPUT_DEPTH(i.depth);
}
ENDCG
	}
}

SubShader {
	Tags { "RenderType"="TreeBillboard" }
	Pass {
		Cull Off
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "TerrainEngine.cginc"
struct v2f {
	float4 pos : POSITION;
	float2 uv : TEXCOORD0;
	#ifdef UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE
	float2 depth : TEXCOORD1;
	#endif
};
v2f vert (appdata_tree_billboard v) {
	v2f o;
	TerrainBillboardTree(v.vertex, v.texcoord1.xy, v.texcoord.y);
	o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
	o.uv.x = v.texcoord.x;
	o.uv.y = v.texcoord.y > 0;
    UNITY_TRANSFER_DEPTH(o.depth);
	return o;
}
uniform sampler2D _MainTex;
fixed4 frag( v2f i ) : COLOR {
	fixed4 texcol = tex2D( _MainTex, i.uv );
	clip( texcol.a - 0.001 );
    UNITY_OUTPUT_DEPTH(i.depth);
}
ENDCG
	}
}

SubShader {
	Tags { "RenderType"="GrassBillboard" }
	Pass {
		Cull Off		
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "TerrainEngine.cginc"
#pragma glsl_no_auto_normalization

struct v2f {
	float4 pos : POSITION;
	fixed4 color : COLOR;
	float2 uv : TEXCOORD0;
	#ifdef UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE
	float2 depth : TEXCOORD1;
	#endif
};

v2f vert (appdata_full v) {
	v2f o;
	WavingGrassBillboardVert (v);
	o.color = v.color;
	o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
	o.uv = v.texcoord.xy;
    UNITY_TRANSFER_DEPTH(o.depth);
	return o;
}
uniform sampler2D _MainTex;
uniform fixed _Cutoff;
fixed4 frag( v2f i ) : COLOR {
	fixed4 texcol = tex2D( _MainTex, i.uv );
	fixed alpha = texcol.a * i.color.a;
	clip( alpha - _Cutoff );
    UNITY_OUTPUT_DEPTH(i.depth);
}
ENDCG
	}
}

SubShader {
	Tags { "RenderType"="Grass" }
	Pass {
		Cull Off
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "TerrainEngine.cginc"
struct v2f {
	float4 pos : POSITION;
	fixed4 color : COLOR;
	float2 uv : TEXCOORD0;
	#ifdef UNITY_MIGHT_NOT_HAVE_DEPTH_TEXTURE
	float2 depth : TEXCOORD1;
	#endif
};
v2f vert (appdata_full v) {
	v2f o;
	WavingGrassVert (v);
	o.color = v.color;
	o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
	o.uv = v.texcoord;
    UNITY_TRANSFER_DEPTH(o.depth);
	return o;
}
uniform sampler2D _MainTex;
uniform fixed _Cutoff;
fixed4 frag(v2f i) : COLOR {
	fixed4 texcol = tex2D( _MainTex, i.uv );
	fixed alpha = texcol.a * i.color.a;
	clip( alpha - _Cutoff );
    UNITY_OUTPUT_DEPTH(i.depth);
}
ENDCG
	}
}
}
Fallback Off
}
