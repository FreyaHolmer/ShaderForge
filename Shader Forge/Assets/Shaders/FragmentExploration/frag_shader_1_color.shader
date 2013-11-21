Shader "Fragment Exploration/frag_shader_1" {
	Properties {
		_Color ("Color", Color) = (1.0,1.0,1.0,1.0)
	}
	SubShader {
		Pass {
			Tags { "LightMode"="ForwardBase" }
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			
			uniform float4 _Color;
			
			
			struct VertexInput {
				float4 vertex : POSITION;
			};
			
			struct VertexOutput {
				float4 pos : SV_POSITION;
			};
			
			VertexOutput vert (VertexInput v) {
				VertexOutput o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				return o;
			}
			
			fixed4 frag(VertexOutput i) : COLOR {
				return _Color;
			}

			ENDCG
		} 
	}
	FallBack "Diffuse"
}