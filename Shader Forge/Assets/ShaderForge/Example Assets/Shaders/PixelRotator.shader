// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:2,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:1,x:35749,y:31801,varname:node_1,prsc:2|diff-748-OUT;n:type:ShaderForge.SFN_TexCoord,id:544,x:32941,y:31716,varname:node_544,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:545,x:33168,y:31794,varname:node_545,prsc:2|A-544-UVOUT,B-546-OUT;n:type:ShaderForge.SFN_Vector1,id:546,x:32941,y:31928,varname:node_546,prsc:2,v1:10;n:type:ShaderForge.SFN_Frac,id:556,x:33395,y:31872,varname:node_556,prsc:2|IN-545-OUT;n:type:ShaderForge.SFN_RemapRange,id:559,x:33622,y:31873,varname:node_559,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-556-OUT;n:type:ShaderForge.SFN_Length,id:561,x:34530,y:32037,varname:node_561,prsc:2|IN-585-OUT;n:type:ShaderForge.SFN_Floor,id:562,x:34984,y:32043,varname:node_562,prsc:2|IN-607-OUT;n:type:ShaderForge.SFN_Rotator,id:563,x:34076,y:32056,varname:node_563,prsc:2|UVIN-764-OUT,PIV-574-OUT;n:type:ShaderForge.SFN_Vector2,id:574,x:33849,y:32159,varname:node_574,prsc:2,v1:0,v2:0;n:type:ShaderForge.SFN_Multiply,id:585,x:34303,y:31993,varname:node_585,prsc:2|A-586-OUT,B-563-UVOUT;n:type:ShaderForge.SFN_Vector2,id:586,x:34076,y:31906,varname:node_586,prsc:2,v1:1,v2:3;n:type:ShaderForge.SFN_Clamp01,id:607,x:34757,y:31992,varname:node_607,prsc:2|IN-561-OUT;n:type:ShaderForge.SFN_Vector1,id:641,x:33622,y:32093,varname:node_641,prsc:2,v1:8;n:type:ShaderForge.SFN_Floor,id:736,x:34076,y:31720,varname:node_736,prsc:2|IN-545-OUT;n:type:ShaderForge.SFN_Noise,id:737,x:34530,y:31533,varname:node_737,prsc:2|XY-736-OUT;n:type:ShaderForge.SFN_Noise,id:739,x:34530,y:31701,varname:node_739,prsc:2|XY-743-OUT;n:type:ShaderForge.SFN_Noise,id:741,x:34757,y:31824,varname:node_741,prsc:2|XY-744-OUT;n:type:ShaderForge.SFN_Add,id:743,x:34303,y:31825,varname:node_743,prsc:2|A-736-OUT,B-546-OUT;n:type:ShaderForge.SFN_Add,id:744,x:34530,y:31869,varname:node_744,prsc:2|A-743-OUT,B-546-OUT;n:type:ShaderForge.SFN_Append,id:745,x:34757,y:31656,varname:node_745,prsc:2|A-737-OUT,B-739-OUT;n:type:ShaderForge.SFN_Append,id:746,x:34984,y:31707,varname:node_746,prsc:2|A-745-OUT,B-741-OUT;n:type:ShaderForge.SFN_OneMinus,id:747,x:35211,y:31991,varname:node_747,prsc:2|IN-562-OUT;n:type:ShaderForge.SFN_Multiply,id:748,x:35438,y:31899,varname:node_748,prsc:2|A-753-OUT,B-747-OUT;n:type:ShaderForge.SFN_Power,id:753,x:35211,y:31823,varname:node_753,prsc:2|VAL-746-OUT,EXP-754-OUT;n:type:ShaderForge.SFN_Vector1,id:754,x:34984,y:31909,varname:node_754,prsc:2,v1:2;n:type:ShaderForge.SFN_Posterize,id:764,x:33849,y:31973,varname:node_764,prsc:2|IN-559-OUT,STPS-641-OUT;pass:END;sub:END;*/

Shader "Shader Forge/Examples/Pixel Rotator" {
    Properties {
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform float4 _LightColor0;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                UNITY_LIGHT_ATTENUATION(attenuation,i, i.posWorld.xyz);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float node_546 = 10.0;
                float2 node_545 = (i.uv0*node_546);
                float2 node_736 = floor(node_545);
                float2 node_737_skew = node_736 + 0.2127+node_736.x*0.3713*node_736.y;
                float2 node_737_rnd = 4.789*sin(489.123*(node_737_skew));
                float node_737 = frac(node_737_rnd.x*node_737_rnd.y*(1+node_737_skew.x));
                float2 node_743 = (node_736+node_546);
                float2 node_739_skew = node_743 + 0.2127+node_743.x*0.3713*node_743.y;
                float2 node_739_rnd = 4.789*sin(489.123*(node_739_skew));
                float node_739 = frac(node_739_rnd.x*node_739_rnd.y*(1+node_739_skew.x));
                float2 node_744 = (node_743+node_546);
                float2 node_741_skew = node_744 + 0.2127+node_744.x*0.3713*node_744.y;
                float2 node_741_rnd = 4.789*sin(489.123*(node_741_skew));
                float node_741 = frac(node_741_rnd.x*node_741_rnd.y*(1+node_741_skew.x));
                float4 node_6513 = _Time;
                float node_563_ang = node_6513.g;
                float node_563_spd = 1.0;
                float node_563_cos = cos(node_563_spd*node_563_ang);
                float node_563_sin = sin(node_563_spd*node_563_ang);
                float2 node_563_piv = float2(0,0);
                float node_641 = 8.0;
                float2 node_563 = (mul(floor((frac(node_545)*2.0+-1.0) * node_641) / (node_641 - 1)-node_563_piv,float2x2( node_563_cos, -node_563_sin, node_563_sin, node_563_cos))+node_563_piv);
                float3 diffuseColor = (pow(float3(float2(node_737,node_739),node_741),2.0)*(1.0 - floor(saturate(length((float2(1,3)*node_563))))));
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 psp2 n3ds wiiu 
            #pragma target 3.0
            uniform float4 _LightColor0;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                UNITY_LIGHT_ATTENUATION(attenuation,i, i.posWorld.xyz);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float node_546 = 10.0;
                float2 node_545 = (i.uv0*node_546);
                float2 node_736 = floor(node_545);
                float2 node_737_skew = node_736 + 0.2127+node_736.x*0.3713*node_736.y;
                float2 node_737_rnd = 4.789*sin(489.123*(node_737_skew));
                float node_737 = frac(node_737_rnd.x*node_737_rnd.y*(1+node_737_skew.x));
                float2 node_743 = (node_736+node_546);
                float2 node_739_skew = node_743 + 0.2127+node_743.x*0.3713*node_743.y;
                float2 node_739_rnd = 4.789*sin(489.123*(node_739_skew));
                float node_739 = frac(node_739_rnd.x*node_739_rnd.y*(1+node_739_skew.x));
                float2 node_744 = (node_743+node_546);
                float2 node_741_skew = node_744 + 0.2127+node_744.x*0.3713*node_744.y;
                float2 node_741_rnd = 4.789*sin(489.123*(node_741_skew));
                float node_741 = frac(node_741_rnd.x*node_741_rnd.y*(1+node_741_skew.x));
                float4 node_9092 = _Time;
                float node_563_ang = node_9092.g;
                float node_563_spd = 1.0;
                float node_563_cos = cos(node_563_spd*node_563_ang);
                float node_563_sin = sin(node_563_spd*node_563_ang);
                float2 node_563_piv = float2(0,0);
                float node_641 = 8.0;
                float2 node_563 = (mul(floor((frac(node_545)*2.0+-1.0) * node_641) / (node_641 - 1)-node_563_piv,float2x2( node_563_cos, -node_563_sin, node_563_sin, node_563_cos))+node_563_piv);
                float3 diffuseColor = (pow(float3(float2(node_737,node_739),node_741),2.0)*(1.0 - floor(saturate(length((float2(1,3)*node_563))))));
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
