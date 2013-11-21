Shader "Shader_01" { // defines the name of the shader 
	SubShader { // Unity chooses the subshader that fits the GPU best
		
		
		Tags { "Queue" = "Transparent"  "IgnoreProjector"="True" "RenderType"="Transparent" }
		
		Pass { // some shaders require multiple passes
			
			
			ZWrite Off // don't write to depth buffer 
        	Blend OneMinusSrcAlpha SrcAlpha
        	
        	
			CGPROGRAM 
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_particles
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float4 projPos : TEXCOORD1;
			};
			
			
			// Vertex shader
			v2f vert(appdata_t v){
				v2f o;
				
				o.projPos = ComputeScreenPos (o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
				
				//o.objWorldPos = mul( _Object2World, float4(0,0,0,1) ); // Object world position
				// 0.2126 R + 0.7152 G + 0.0722 B - Desaturation
				
				
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
//				o.col = v.vertex;
				return o;
			}
			
			sampler2D _CameraDepthTexture;
			
			float4 frag(v2f i) : COLOR // fragment shader
			{
				float sceneZ = LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
				//return float4(sceneZ,sceneZ,sceneZ,1.0);
				return float4(1.0);
			}
			
			ENDCG  
		}
	}
}