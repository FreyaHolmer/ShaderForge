Shader "Hidden/TerrainEngine/Details/Vertexlit" {
Properties {
	_MainTex ("Main Texture", 2D) = "white" {  }
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 200

CGPROGRAM
#pragma surface surf Lambert

sampler2D _MainTex;

struct Input {
	float2 uv_MainTex;
	fixed4 color : COLOR;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
	o.Albedo = c.rgb;
	o.Alpha = c.a;
}

ENDCG
}
SubShader {
	Tags { "RenderType"="Opaque" }
	Pass {
		Tags { "LightMode" = "Vertex" }
		ColorMaterial AmbientAndDiffuse
		Lighting On
		SetTexture [_MainTex] {
			combine texture * primary DOUBLE, texture * primary
		} 
	}
	Pass {
		Tags { "LightMode" = "VertexLMRGBM" }
		ColorMaterial AmbientAndDiffuse
		BindChannels {
			Bind "Vertex", vertex
			Bind "texcoord1", texcoord0 // lightmap uses 2nd uv
			Bind "texcoord", texcoord1 // main uses 1st uv
		}
		SetTexture [unity_Lightmap] {
			matrix [unity_LightmapMatrix]
			combine texture * texture alpha DOUBLE
		}
		SetTexture [_MainTex] { combine texture * previous QUAD, texture }
	}
}

Fallback "VertexLit"
}
