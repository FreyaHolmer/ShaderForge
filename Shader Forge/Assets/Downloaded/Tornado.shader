// Shader created with Shader Forge Alpha 0.14 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.14;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,uamb:True,ufog:False,aust:False,igpj:True,qofs:0,lico:0,qpre:3,flbk:,rntp:2,lmpd:False,enco:False,frtr:True,vitr:True,dbil:False;n:type:ShaderForge.SFN_Final,id:1,x:32456,y:32705|alpha-152-OUT,emission-260-OUT,voffset-274-OUT;n:type:ShaderForge.SFN_Power,id:2,x:32983,y:32635|VAL-4-OUT,EXP-3-OUT;n:type:ShaderForge.SFN_Vector1,id:3,x:33140,y:32669,v1:0.8;n:type:ShaderForge.SFN_Multiply,id:4,x:33140,y:32544|A-16-OUT,B-5-OUT;n:type:ShaderForge.SFN_Lerp,id:5,x:33334,y:32756|A-9-OUT,B-7-OUT,T-6-R;n:type:ShaderForge.SFN_VertexColor,id:6,x:33515,y:32905;n:type:ShaderForge.SFN_Multiply,id:7,x:33515,y:32777|A-15-OUT,B-8-XYZ;n:type:ShaderForge.SFN_Vector4Property,id:8,x:33712,y:32859,ptlb:Layer 3 color,v1:0.7,v2:0.68,v3:0.65,v4:0;n:type:ShaderForge.SFN_Lerp,id:9,x:33515,y:32650|A-10-OUT,B-11-OUT,T-14-OUT;n:type:ShaderForge.SFN_Multiply,id:10,x:33695,y:32521|A-12-XYZ,B-29-R;n:type:ShaderForge.SFN_Multiply,id:11,x:33695,y:32663|A-13-XYZ,B-31-R;n:type:ShaderForge.SFN_Vector4Property,id:12,x:33892,y:32473,ptlb:Layer 1 color,v1:0.2,v2:0.15,v3:0.1,v4:0;n:type:ShaderForge.SFN_Vector4Property,id:13,x:33892,y:32634,ptlb:Layer 2 color,v1:0.55,v2:0.5,v3:0.45,v4:0;n:type:ShaderForge.SFN_ConstantClamp,id:14,x:34782,y:33079,min:0,max:1|IN-22-OUT;n:type:ShaderForge.SFN_Power,id:15,x:34213,y:32847|VAL-18-OUT,EXP-17-OUT;n:type:ShaderForge.SFN_Lerp,id:16,x:35566,y:32158|A-24-OUT,B-25-OUT,T-26-R;n:type:ShaderForge.SFN_Vector1,id:17,x:34383,y:32894,v1:3;n:type:ShaderForge.SFN_Divide,id:18,x:34383,y:32774|A-19-OUT,B-20-OUT;n:type:ShaderForge.SFN_Add,id:19,x:34559,y:32774|A-21-OUT,B-35-R;n:type:ShaderForge.SFN_Vector1,id:20,x:34559,y:32894,v1:2;n:type:ShaderForge.SFN_Add,id:21,x:34720,y:32774|A-29-R,B-31-R;n:type:ShaderForge.SFN_Multiply,id:22,x:34952,y:33079|A-35-R,B-23-OUT;n:type:ShaderForge.SFN_Vector1,id:23,x:35128,y:33113,v1:1.5;n:type:ShaderForge.SFN_Vector1,id:24,x:35732,y:32158,v1:0.4;n:type:ShaderForge.SFN_Vector1,id:25,x:35732,y:32213,v1:1;n:type:ShaderForge.SFN_Tex2d,id:26,x:35732,y:32278,tex:091c45f9ad5a0af41946801f860ac8d7|UVIN-41-UVOUT,TEX-27-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:27,x:35881,y:32049,ptlb:Noise map,tex:091c45f9ad5a0af41946801f860ac8d7;n:type:ShaderForge.SFN_Tex2d,id:29,x:35732,y:32452,tex:091c45f9ad5a0af41946801f860ac8d7|UVIN-94-UVOUT,TEX-27-TEX;n:type:ShaderForge.SFN_Tex2d,id:31,x:35732,y:32653,tex:091c45f9ad5a0af41946801f860ac8d7|UVIN-102-UVOUT,TEX-27-TEX;n:type:ShaderForge.SFN_Tex2d,id:35,x:35732,y:32844,tex:091c45f9ad5a0af41946801f860ac8d7|UVIN-116-UVOUT,TEX-27-TEX;n:type:ShaderForge.SFN_Panner,id:41,x:35900,y:32278,spu:0.23,spv:-0.057|UVIN-80-OUT,DIST-341-OUT;n:type:ShaderForge.SFN_Time,id:42,x:36257,y:32012;n:type:ShaderForge.SFN_Multiply,id:80,x:36068,y:32278|A-81-OUT,B-82-OUT;n:type:ShaderForge.SFN_Vector2,id:81,x:36233,y:32276,v1:0.5,v2:0.125;n:type:ShaderForge.SFN_Add,id:82,x:36395,y:32299|A-84-OUT,B-83-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:83,x:36580,y:32420,uv:0;n:type:ShaderForge.SFN_Multiply,id:84,x:36555,y:32280|A-85-OUT,B-86-OUT;n:type:ShaderForge.SFN_Vector2,id:85,x:36717,y:32280,v1:-0.35,v2:0;n:type:ShaderForge.SFN_OneMinus,id:86,x:36819,y:32430|IN-149-V;n:type:ShaderForge.SFN_Panner,id:94,x:35900,y:32452,spu:0.07,spv:-0.063|DIST-341-OUT;n:type:ShaderForge.SFN_Multiply,id:95,x:36068,y:32452|A-96-OUT,B-97-OUT;n:type:ShaderForge.SFN_Vector2,id:96,x:36247,y:32452,v1:0.352,v2:1.5;n:type:ShaderForge.SFN_Add,id:97,x:36395,y:32477|A-83-UVOUT,B-98-OUT;n:type:ShaderForge.SFN_Multiply,id:98,x:36555,y:32556|A-86-OUT,B-100-OUT;n:type:ShaderForge.SFN_Vector2,id:100,x:36712,y:32574,v1:-1,v2:0.7;n:type:ShaderForge.SFN_Panner,id:102,x:35900,y:32653,spu:0.11,spv:-0.059|UVIN-103-OUT,DIST-341-OUT;n:type:ShaderForge.SFN_Multiply,id:103,x:36068,y:32653|A-104-OUT,B-97-OUT;n:type:ShaderForge.SFN_Vector2,id:104,x:36247,y:32653,v1:0.39,v2:0.75;n:type:ShaderForge.SFN_Panner,id:116,x:35900,y:32844,spu:0.17,spv:-0.089|UVIN-119-OUT,DIST-341-OUT;n:type:ShaderForge.SFN_Multiply,id:119,x:36068,y:32844|A-120-OUT,B-97-OUT;n:type:ShaderForge.SFN_Vector2,id:120,x:36247,y:32844,v1:0.313,v2:0.678;n:type:ShaderForge.SFN_ConstantClamp,id:123,x:33001,y:33369,min:0,max:1|IN-124-OUT;n:type:ShaderForge.SFN_Power,id:124,x:33168,y:33369|VAL-126-OUT,EXP-125-OUT;n:type:ShaderForge.SFN_Vector1,id:125,x:33328,y:33428,v1:1.5;n:type:ShaderForge.SFN_Multiply,id:126,x:33328,y:33303|A-128-OUT,B-127-OUT;n:type:ShaderForge.SFN_Add,id:127,x:33487,y:33428|A-128-OUT,B-140-OUT;n:type:ShaderForge.SFN_Multiply,id:128,x:33653,y:33306|A-130-OUT,B-129-A;n:type:ShaderForge.SFN_VertexColor,id:129,x:33827,y:33343;n:type:ShaderForge.SFN_OneMinus,id:130,x:33827,y:33216|IN-131-OUT;n:type:ShaderForge.SFN_Abs,id:131,x:33988,y:33216|IN-132-OUT;n:type:ShaderForge.SFN_Power,id:132,x:34156,y:33216|VAL-136-OUT,EXP-134-OUT;n:type:ShaderForge.SFN_Vector1,id:134,x:34322,y:33355,v1:3;n:type:ShaderForge.SFN_ViewReflectionVector,id:135,x:34633,y:33206;n:type:ShaderForge.SFN_ComponentMask,id:136,x:34322,y:33206,cc1:0,cc2:4,cc3:4,cc4:4|IN-139-XYZ;n:type:ShaderForge.SFN_Transform,id:139,x:34476,y:33206,s:9|IN-135-OUT;n:type:ShaderForge.SFN_Lerp,id:140,x:34610,y:33445|A-14-OUT,B-142-OUT,T-141-R;n:type:ShaderForge.SFN_VertexColor,id:141,x:34783,y:33614;n:type:ShaderForge.SFN_ConstantClamp,id:142,x:34783,y:33465,min:0,max:1|IN-143-OUT;n:type:ShaderForge.SFN_Power,id:143,x:34959,y:33465|VAL-145-OUT,EXP-144-OUT;n:type:ShaderForge.SFN_Vector1,id:144,x:35122,y:33600,v1:2;n:type:ShaderForge.SFN_Multiply,id:145,x:35122,y:33465|A-147-OUT,B-146-OUT;n:type:ShaderForge.SFN_Vector1,id:146,x:35298,y:33617,v1:2;n:type:ShaderForge.SFN_ConstantClamp,id:147,x:35298,y:33465,min:0,max:1|IN-148-OUT;n:type:ShaderForge.SFN_Subtract,id:148,x:35457,y:33465|A-29-R,B-31-R;n:type:ShaderForge.SFN_TexCoord,id:149,x:36989,y:32392,uv:0;n:type:ShaderForge.SFN_ComponentMask,id:152,x:32841,y:33369,cc1:0,cc2:4,cc3:4,cc4:4|IN-123-OUT;n:type:ShaderForge.SFN_Vector4Property,id:253,x:34107,y:31652,ptlb:Light 1 direction,v1:-1,v2:0.85,v3:0,v4:1;n:type:ShaderForge.SFN_Normalize,id:254,x:33927,y:31652|IN-253-XYZ;n:type:ShaderForge.SFN_NormalVector,id:255,x:34230,y:31771,pt:False;n:type:ShaderForge.SFN_Dot,id:256,x:33727,y:31747,dt:0|A-254-OUT,B-255-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:257,x:33563,y:31747,min:0,max:1|IN-256-OUT;n:type:ShaderForge.SFN_Vector4Property,id:258,x:33564,y:31603,ptlb:Light 1 color,v1:1,v2:1,v3:0.9,v4:1;n:type:ShaderForge.SFN_Multiply,id:259,x:33366,y:31685|A-258-XYZ,B-257-OUT;n:type:ShaderForge.SFN_Multiply,id:260,x:32763,y:32066|A-265-OUT,B-2-OUT;n:type:ShaderForge.SFN_Add,id:265,x:32917,y:31836|A-273-OUT,B-266-OUT;n:type:ShaderForge.SFN_Vector1,id:266,x:33083,y:31926,v1:0.2;n:type:ShaderForge.SFN_Vector4Property,id:267,x:34100,y:31949,ptlb:Light 2 direction,v1:1,v2:0.8,v3:0,v4:1;n:type:ShaderForge.SFN_Normalize,id:268,x:33928,y:31949|IN-267-XYZ;n:type:ShaderForge.SFN_Dot,id:269,x:33727,y:31949,dt:0|A-268-OUT,B-255-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:270,x:33563,y:31949,min:0,max:1|IN-269-OUT;n:type:ShaderForge.SFN_Multiply,id:271,x:33371,y:31949|A-270-OUT,B-272-XYZ;n:type:ShaderForge.SFN_Vector4Property,id:272,x:33563,y:32133,ptlb:Light 2 color,v1:0.1,v2:0.1,v3:0.15,v4:0;n:type:ShaderForge.SFN_Add,id:273,x:33192,y:31796|A-259-OUT,B-271-OUT;n:type:ShaderForge.SFN_Multiply,id:274,x:32763,y:33668|A-275-OUT,B-278-OUT;n:type:ShaderForge.SFN_Multiply,id:275,x:33018,y:33947|A-277-OUT,B-276-OUT;n:type:ShaderForge.SFN_Vector3,id:276,x:33185,y:33989,v1:0.0625,v2:0,v3:0.0625;n:type:ShaderForge.SFN_Append,id:277,x:33185,y:33866|A-282-OUT,B-295-OUT;n:type:ShaderForge.SFN_Lerp,id:278,x:33018,y:34086|A-279-OUT,B-280-OUT,T-306-OUT;n:type:ShaderForge.SFN_Vector1,id:279,x:33207,y:34086,v1:0.3;n:type:ShaderForge.SFN_Vector1,id:280,x:33207,y:34134,v1:1;n:type:ShaderForge.SFN_Append,id:282,x:33360,y:33866|A-285-OUT,B-283-OUT;n:type:ShaderForge.SFN_Vector1,id:283,x:33548,y:33991,v1:0;n:type:ShaderForge.SFN_TexCoord,id:284,x:33343,y:34190,uv:0;n:type:ShaderForge.SFN_Sin,id:285,x:33552,y:33866|IN-309-OUT;n:type:ShaderForge.SFN_Vector1,id:287,x:33883,y:33989,v1:0.933;n:type:ShaderForge.SFN_Add,id:288,x:34064,y:34028|A-294-OUT,B-289-OUT;n:type:ShaderForge.SFN_Sin,id:289,x:34236,y:33998|IN-307-OUT;n:type:ShaderForge.SFN_Vector1,id:291,x:34563,y:34019,v1:7.945;n:type:ShaderForge.SFN_Time,id:292,x:34828,y:34003;n:type:ShaderForge.SFN_TexCoord,id:293,x:34404,y:33754,uv:0;n:type:ShaderForge.SFN_OneMinus,id:294,x:34233,y:33795|IN-293-V;n:type:ShaderForge.SFN_Sin,id:295,x:33548,y:34176|IN-310-OUT;n:type:ShaderForge.SFN_Vector1,id:297,x:33883,y:34298,v1:1.973;n:type:ShaderForge.SFN_Add,id:298,x:34064,y:34309|A-294-OUT,B-305-OUT;n:type:ShaderForge.SFN_Vector1,id:300,x:34563,y:34365,v1:5.137;n:type:ShaderForge.SFN_Tau,id:302,x:34858,y:34123;n:type:ShaderForge.SFN_Sin,id:305,x:34236,y:34309|IN-308-OUT;n:type:ShaderForge.SFN_OneMinus,id:306,x:33185,y:34190|IN-284-V;n:type:ShaderForge.SFN_Divide,id:307,x:34392,y:33998|A-330-OUT,B-291-OUT;n:type:ShaderForge.SFN_Divide,id:308,x:34407,y:34309|A-330-OUT,B-300-OUT;n:type:ShaderForge.SFN_Divide,id:309,x:33715,y:33866|A-337-OUT,B-287-OUT;n:type:ShaderForge.SFN_Divide,id:310,x:33714,y:34176|A-340-OUT,B-297-OUT;n:type:ShaderForge.SFN_Abs,id:321,x:35503,y:32295|IN-35-R;n:type:ShaderForge.SFN_Multiply,id:330,x:34666,y:34070|A-292-T,B-302-OUT;n:type:ShaderForge.SFN_Multiply,id:337,x:33883,y:33866|A-339-OUT,B-288-OUT;n:type:ShaderForge.SFN_Tau,id:339,x:34080,y:33841;n:type:ShaderForge.SFN_Multiply,id:340,x:33883,y:34176|A-339-OUT,B-298-OUT;n:type:ShaderForge.SFN_Multiply,id:341,x:36071,y:32012|A-346-OUT,B-42-T;n:type:ShaderForge.SFN_Vector1,id:346,x:36257,y:31950,v1:2;proporder:12-13-8-27-253-267-258-272;pass:END;sub:END;*/

Shader "OMD/Tornado" {
    Properties {
        _Layer1color ("Layer 1 color", Vector) = (0.2,0.15,0.1,0)
        _Layer2color ("Layer 2 color", Vector) = (0.55,0.5,0.45,0)
        _Layer3color ("Layer 3 color", Vector) = (0.7,0.68,0.65,0)
        _Noisemap ("Noise map", 2D) = "white" {}
        _Light1direction ("Light 1 direction", Vector) = (-1,0.85,0,1)
        _Light2direction ("Light 2 direction", Vector) = (1,0.8,0,1)
        _Light1color ("Light 1 color", Vector) = (1,1,0.9,1)
        _Light2color ("Light 2 color", Vector) = (0.1,0.1,0.15,0)
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers d3d11 gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float4 _Layer3color;
            uniform float4 _Layer1color;
            uniform float4 _Layer2color;
            uniform sampler2D _Noisemap;
            uniform float4 _Light1direction;
            uniform float4 _Light1color;
            uniform float4 _Light2direction;
            uniform float4 _Light2color;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.vertexColor = v.vertexColor;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float node_339 = 6.28318530718;
                float node_294 = (1.0 - o.uv0.g);
                float4 node_292 = _Time + _TimeEditor;
                float node_330 = (node_292.g*6.28318530718);
                v.vertex.xyz += ((float3(float2(sin(((node_339*(node_294+sin((node_330/7.945))))/0.933)),0.0),sin(((node_339*(node_294+sin((node_330/5.137))))/1.973)))*float3(0.0625,0,0.0625))*lerp(0.3,1.0,(1.0 - o.uv0.g)));
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.posWorld = mul(_Object2World, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = normalize(i.normalDir);
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 node_255 = i.normalDir;
                float node_86 = (1.0 - i.uv0.g);
                float2 node_83 = i.uv0;
                float4 node_42 = _Time + _TimeEditor;
                float node_341 = (2.0*node_42.g);
                float4 node_29 = tex2D(_Noisemap,(i.uv0.rg+node_341*float2(0.07,-0.063)));
                float2 node_97 = (node_83.rg+(node_86*float2(-1,0.7)));
                float4 node_31 = tex2D(_Noisemap,((float2(0.39,0.75)*node_97)+node_341*float2(0.11,-0.059)));
                float4 node_35 = tex2D(_Noisemap,((float2(0.313,0.678)*node_97)+node_341*float2(0.17,-0.089)));
                float node_14 = clamp((node_35.r*1.5),0,1);
                float node_128 = ((1.0 - abs(pow(mul( tangentTransform, viewReflectDirection ).rgb.r,3.0)))*i.vertexColor.a);
                return fixed4(((((_Light1color.rgb*clamp(dot(normalize(_Light1direction.rgb),node_255),0,1))+(clamp(dot(normalize(_Light2direction.rgb),node_255),0,1)*_Light2color.rgb))+0.2)*pow((lerp(0.4,1.0,tex2D(_Noisemap,((float2(0.5,0.125)*((float2(-0.35,0)*node_86)+node_83.rg))+node_341*float2(0.23,-0.057))).r)*lerp(lerp((_Layer1color.rgb*node_29.r),(_Layer2color.rgb*node_31.r),node_14),(pow((((node_29.r+node_31.r)+node_35.r)/2.0),3.0)*_Layer3color.rgb),i.vertexColor.r)),0.8))+UNITY_LIGHTMODEL_AMBIENT.xyz,clamp(pow((node_128*(node_128+lerp(node_14,clamp(pow((clamp((node_29.r-node_31.r),0,1)*2.0),2.0),0,1),i.vertexColor.r))),1.5),0,1).r);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCollector"
            Tags {
                "LightMode"="ShadowCollector"
            }
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCOLLECTOR
            #define SHADOW_COLLECTOR_PASS
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcollector
            #pragma exclude_renderers d3d11 gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_COLLECTOR;
                float4 uv0 : TEXCOORD5;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                float node_339 = 6.28318530718;
                float node_294 = (1.0 - o.uv0.g);
                float4 node_292 = _Time + _TimeEditor;
                float node_330 = (node_292.g*6.28318530718);
                v.vertex.xyz += ((float3(float2(sin(((node_339*(node_294+sin((node_330/7.945))))/0.933)),0.0),sin(((node_339*(node_294+sin((node_330/5.137))))/1.973)))*float3(0.0625,0,0.0625))*lerp(0.3,1.0,(1.0 - o.uv0.g)));
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_SHADOW_COLLECTOR(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                SHADOW_COLLECTOR_FRAGMENT(i)
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Cull Off
            Offset 1, 1
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma exclude_renderers d3d11 gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float4 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                float node_339 = 6.28318530718;
                float node_294 = (1.0 - o.uv0.g);
                float4 node_292 = _Time + _TimeEditor;
                float node_330 = (node_292.g*6.28318530718);
                v.vertex.xyz += ((float3(float2(sin(((node_339*(node_294+sin((node_330/7.945))))/0.933)),0.0),sin(((node_339*(node_294+sin((node_330/5.137))))/1.973)))*float3(0.0625,0,0.0625))*lerp(0.3,1.0,(1.0 - o.uv0.g)));
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
