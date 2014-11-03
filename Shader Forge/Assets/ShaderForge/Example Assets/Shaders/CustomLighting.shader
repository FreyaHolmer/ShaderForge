// Shader created with Shader Forge Beta 0.37 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.37;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:True,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,arlg:0,hqsc:True,hqlp:False,tesm:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:0,x:34250,y:31953,varname:node_0,prsc:2|normal-83-RGB,custl-64-OUT,olwid-255-OUT;n:type:ShaderForge.SFN_LightAttenuation,id:37,x:33872,y:32026,varname:node_37,prsc:2;n:type:ShaderForge.SFN_Dot,id:40,x:32931,y:32250,varname:node_40,prsc:2,dt:1|A-42-OUT,B-41-OUT;n:type:ShaderForge.SFN_NormalVector,id:41,x:32722,y:32344,prsc:2,pt:True;n:type:ShaderForge.SFN_LightVector,id:42,x:32722,y:32223,varname:node_42,prsc:2;n:type:ShaderForge.SFN_Dot,id:52,x:32931,y:32423,varname:node_52,prsc:2,dt:1|A-41-OUT,B-62-OUT;n:type:ShaderForge.SFN_Add,id:55,x:33872,y:32288,varname:node_55,prsc:2|A-84-OUT,B-187-RGB,C-265-OUT;n:type:ShaderForge.SFN_Power,id:58,x:33133,y:32523,cmnt:Specular Light,varname:node_58,prsc:2|VAL-52-OUT,EXP-244-OUT;n:type:ShaderForge.SFN_HalfVector,id:62,x:32722,y:32483,varname:node_62,prsc:2;n:type:ShaderForge.SFN_LightColor,id:63,x:33872,y:32155,varname:node_63,prsc:2;n:type:ShaderForge.SFN_Multiply,id:64,x:34056,y:32155,varname:node_64,prsc:2|A-37-OUT,B-63-RGB,C-55-OUT;n:type:ShaderForge.SFN_Color,id:80,x:33368,y:32178,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_65,prsc:2,glob:False,c1:0.6544118,c2:0.8426978,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:82,x:33368,y:32002,ptovrint:False,ptlb:Diffuse,ptin:_Diffuse,varname:node_81,prsc:2,tex:8993b617f08498f43adcbd90697f1c5d,ntxv:0,isnm:False|UVIN-272-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:83,x:34056,y:31966,ptovrint:False,ptlb:Normals,ptin:_Normals,varname:node_83,prsc:2,tex:c6dfb00dbee6bc044a8a3bb22e56e064,ntxv:3,isnm:True|UVIN-272-UVOUT;n:type:ShaderForge.SFN_Multiply,id:84,x:33573,y:32160,cmnt:Diffuse Light,varname:node_84,prsc:2|A-82-RGB,B-80-RGB,C-264-OUT;n:type:ShaderForge.SFN_AmbientLight,id:187,x:33573,y:32280,varname:node_187,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:216,x:33133,y:32423,ptovrint:False,ptlb:Bands,ptin:_Bands,varname:node_188,prsc:2,glob:False,v1:8;n:type:ShaderForge.SFN_Slider,id:239,x:31984,y:32591,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:node_217,prsc:2,min:0,cur:0.4511278,max:1;n:type:ShaderForge.SFN_Add,id:240,x:32722,y:32640,varname:node_240,prsc:2|A-242-OUT,B-241-OUT;n:type:ShaderForge.SFN_Vector1,id:241,x:32554,y:32728,varname:node_241,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:242,x:32554,y:32578,varname:node_242,prsc:2|A-239-OUT,B-243-OUT;n:type:ShaderForge.SFN_Vector1,id:243,x:32141,y:32661,varname:node_243,prsc:2,v1:10;n:type:ShaderForge.SFN_Exp,id:244,x:32893,y:32640,varname:node_244,prsc:2,et:1|IN-240-OUT;n:type:ShaderForge.SFN_Vector1,id:255,x:34056,y:32288,varname:node_255,prsc:2,v1:0.05;n:type:ShaderForge.SFN_Posterize,id:264,x:33368,y:32344,varname:node_264,prsc:2|IN-40-OUT,STPS-216-OUT;n:type:ShaderForge.SFN_Posterize,id:265,x:33368,y:32475,varname:node_265,prsc:2|IN-58-OUT,STPS-216-OUT;n:type:ShaderForge.SFN_TexCoord,id:272,x:33121,y:32051,varname:node_272,prsc:2,uv:0;proporder:80-82-83-216-239;pass:END;sub:END;*/

Shader "Shader Forge/Examples/Custom Lighting" {
    SubShader {
        Tags { "RenderType"="Opaque" }
        
        Pass {
            Tags { "LightMode"="ForwardBase" }
            Name "ForwardBase"
            Blend One One
            Fog {Mode Off}
            
            CGPROGRAM
            #define UNITY_PASS_FORWARDBASE
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
            };
            VertexOutput vert(VertexInput v) {
                VertexOutput o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput o) {
                return float3(0,0,0);
            }
            ENDCG
        }
        
        Pass {
            Tags { "LightMode"="ForwardAdd" }
            Name "ForwardAdd"
            Blend One One
            Fog {Mode Off}
            
            CGPROGRAM
            #define UNITY_PASS_FORWARDADD
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
            };
            VertexOutput vert(VertexInput v) {
                VertexOutput o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput o) {
                return float3(0,0,0);
            }
            ENDCG
        }
        
    }
}
