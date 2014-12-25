// Shader created with Shader Forge v1.00 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.00;sub:START;pass:START;ps:flbk:,lico:0,lgpr:1,nrmq:1,limd:0,uamb:False,mssp:True,lmpd:False,lprd:True,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:0,x:34043,y:32815,varname:node_0,prsc:2|emission-585-OUT;n:type:ShaderForge.SFN_VertexColor,id:4,x:33627,y:32845,varname:node_4,prsc:2;n:type:ShaderForge.SFN_Slider,id:38,x:33095,y:32967,ptovrint:False,ptlb:Divisions,ptin:_Divisions,varname:_Divisions,prsc:2,min:0,cur:0.5639098,max:1;n:type:ShaderForge.SFN_Power,id:42,x:33443,y:32990,varname:node_42,prsc:2|VAL-38-OUT,EXP-44-OUT;n:type:ShaderForge.SFN_Vector1,id:44,x:33252,y:33034,varname:node_44,prsc:2,v1:2;n:type:ShaderForge.SFN_ConstantLerp,id:137,x:33627,y:32990,varname:node_137,prsc:2,a:0.5,b:12|IN-42-OUT;n:type:ShaderForge.SFN_Posterize,id:585,x:33845,y:32915,varname:node_585,prsc:2|IN-4-RGB,STPS-137-OUT;proporder:38;pass:END;sub:END;*/

Shader "Shader Forge/Examples/Vertex Color Rounding" {
    Properties {
        _Divisions ("Divisions", Range(0, 1)) = 0.5639098
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH_PROBE ( defined (LIGHTMAP_OFF) )
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float _Divisions;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.vertexColor = v.vertexColor;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
/////// Vectors:
////// Lighting:
////// Emissive:
                float node_137 = lerp(0.5,12,pow(_Divisions,2.0));
                float3 emissive = floor(i.vertexColor.rgb * node_137) / (node_137 - 1);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
