Shader "FULL TEMPLATE NORMAL" {
//
//    Properties {
//        _MainTex ("Base (RGB)", 2D) = "white" {}
//        _BumpMap ("Bumpmap", 2D) = "bump" {}
//    }
//
//    SubShader {
//
//        Tags { "RenderType"="Opaque" }
//        LOD 200
//        Pass {
//
//            //ForwardBase pass lighting: //
//            // 1 lightmap
//            // 1 directional pixel light
//            //OR
//            // 1 directional pixel light
//            // 9 spherical harmonic coefficients
//            // (optionally) 4 point vertex lights
//
//            Name "FORWARD"
//
//            Tags { "LightMode" = "ForwardBase" }
//
//            CGPROGRAM
//            #pragma vertex vert_surf
//            #pragma fragment frag_surf
//            #pragma fragmentoption ARB_precision_hint_fastest
//            #pragma multi_compile_fwdbase
//            #include "HLSLSupport.cginc"
//            #define UNITY_PASS_FORWARDBASE
//            #include "UnityCG.cginc"
//            #include "Lighting.cginc"
//            #include "AutoLight.cginc"
//
//            sampler2D _MainTex;
//            sampler2D _BumpMap;
//
//            #ifdef LIGHTMAP_OFF
//	            struct v2f_surf {
//	                float4 pos : SV_POSITION;
//	                float2 pack0 : TEXCOORD0;
//	                fixed3 normal : TEXCOORD1;
//	                fixed3 vlight : TEXCOORD2; //vertex + SH lighting results
//	                LIGHTING_COORDS(3,4)
//	            };
//            #endif
//            #ifndef LIGHTMAP_OFF
//	            struct v2f_surf {
//	                float4 pos : SV_POSITION;
//	                float2 pack0 : TEXCOORD0;
//	                float2 lmap : TEXCOORD1;
//	                LIGHTING_COORDS(2,3)
//	            };
//            #endif
//            #ifndef LIGHTMAP_OFF
//            	float4 unity_LightmapST;
////            	float4 unity_ShadowFadeCenterAndType; // HACK; Commented out due to error
//            #endif
//            float4 _MainTex_ST;
//            v2f_surf vert_surf (appdata_full v) {
//
//                v2f_surf o;
//
//                // Transform vertex into normalized device coordinates
//                o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
//                // Apply scaling/tiling to UV coordinates
//                o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
//                #ifndef LIGHTMAP_OFF
//	                // Apply scaling/tiling to UV2 coordinates for light map
//	                o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
//                #endif
//                // Calculate world normal
//                float3 worldN = mul((float3x3)_Object2World, SCALED_NORMAL);
//                #ifdef LIGHTMAP_OFF
//                	o.normal = worldN;
//                #endif
//                #ifdef LIGHTMAP_OFF
//	                // Spherical harmonic contribution
//	                float3 shlight = ShadeSH9 (float4(worldN,1.0));
//	                o.vlight = shlight;
//	                #ifdef VERTEXLIGHT_ON
//		                float3 worldPos = mul(_Object2World, v.vertex).xyz;
//		                // Vertex light contribution
//		                o.vlight += Shade4PointLights (
//		                    unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
//		                    unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
//		                    unity_4LightAtten0, worldPos, worldN );
//	                #endif // VERTEXLIGHT_ON
//                #endif // LIGHTMAP_OFF
//                // Shadow coordinates
//                TRANSFER_VERTEX_TO_FRAGMENT(o);
//                return o;
//
//            }
//
//            #ifndef LIGHTMAP_OFF
//            	sampler2D unity_Lightmap;
//            #endif
//
//            fixed4 frag_surf (v2f_surf IN) : COLOR {
//            
//            	//
//                // SHADER CODE GOES HERE
//                //
//                /////////////////////////////////////////////////////////////////////////////////////////
//                SurfaceOutput o;
//                o.Albedo = 0.0;
//                o.Emission = 0.0;
//                o.Specular = 0.0;
//                o.Alpha = 0.0;
//                o.Gloss = 0.0;
//                #ifdef LIGHTMAP_OFF
//                	o.Normal = UnpackNormal(tex2D (_BumpMap, IN.pack0.xy));
////                	o.Normal = tex2D (_BumpMap, IN.pack0.xy);
//                #endif
//                
//                // EX:
//                half4 tex = tex2D (_MainTex, IN.pack0.xy);
//                o.Albedo = tex.rgb;
//                o.Alpha = tex.a;
//                
//                /////////////////////////////////////////////////////////////////////////////////////////
//                //
//				//
//				//
//				
//				
//                // Surface function normally be called here
//                fixed atten = LIGHT_ATTENUATION(IN);
//                fixed4 c = 0;
//                #ifdef LIGHTMAP_OFF
//	                // Directional light
//	                c = LightingLambert (o, _WorldSpaceLightPos0.xyz, atten);
//                #endif // LIGHTMAP_OFF
//                #ifdef LIGHTMAP_OFF
//	                // Vertex and SH light
//	                c.rgb += o.Albedo * IN.vlight;
//                #endif // LIGHTMAP_OFF
//                #ifndef LIGHTMAP_OFF
//	                // Light map
//	                fixed4 lmtex = tex2D(unity_Lightmap, IN.lmap.xy);
//	                fixed3 lm = DecodeLightmap (lmtex);
//	                #ifdef SHADOWS_SCREEN
//		                #if defined(SHADER_API_GLES) && defined(SHADER_API_MOBILE)
//		                	c.rgb += o.Albedo * min(lm, atten*2);
//		                #else
//		                	c.rgb += o.Albedo * max(min(lm,(atten*2)*lmtex.rgb), lm*atten);
//		                #endif
//	                #else // SHADOWS_SCREEN
//	               		c.rgb += o.Albedo * lm;
//	                #endif // SHADOWS_SCREEN
//	                c.a = o.Alpha;
//
//           		#endif // LIGHTMAP_OFF
//                return c;
//            }
//            ENDCG
//        }
//
//        Pass {
//
//            //ForwardAdd pass lighting:
//            //
//            // 1 additional pixel light
//
//            Name "FORWARD"
//            Tags { "LightMode" = "ForwardAdd" }
//            ZWrite Off
//            Blend One One
//            Fog { Color (0,0,0,0) }
//
//            CGPROGRAM
//            #pragma vertex vert_surf
//            #pragma fragment frag_surf
//            #pragma fragmentoption ARB_precision_hint_fastest
//            #pragma multi_compile_fwdadd
//            #include "HLSLSupport.cginc"
//            #define UNITY_PASS_FORWARDADD
//            #include "UnityCG.cginc"
//            #include "Lighting.cginc"
//            #include "AutoLight.cginc"
//
//            
//
//            sampler2D _MainTex;
//            sampler2D _BumpMap;
//
//            
//            struct v2f_surf {
//                float4 pos : SV_POSITION;
//                float2 pack0 : TEXCOORD0;
//                fixed3 normal : TEXCOORD1;
//                half3 lightDir : TEXCOORD2;
//                LIGHTING_COORDS(3,4)
//            };
//
//            float4 _MainTex_ST;
//            v2f_surf vert_surf (appdata_full v) {
//                v2f_surf o;
//                // Transform vertex into normalized device coordinates
//                o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
//                // Apply scaling/tiling to UV coordinates
//                o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
//                // Calculate world normal
//                o.normal = mul((float3x3)_Object2World, SCALED_NORMAL);
//                // World light direction calculation depends on light type
//                float3 lightDir = WorldSpaceLightDir( v.vertex );
//                o.lightDir = lightDir;
//                // Shadow coordinates
//                TRANSFER_VERTEX_TO_FRAGMENT(o);
//                return o;
//            }
//
//            fixed4 frag_surf (v2f_surf IN) : COLOR {
//            
//            	//
//                // SHADER CODE GOES HERE
//                //
//                /////////////////////////////////////////////////////////////////////////////////////////
//                // Surface setup
//                SurfaceOutput o;
//                o.Albedo = 0.0;
//                o.Emission = 0.0;
//                o.Specular = 0.0;
//                o.Alpha = 0.0;
//                o.Gloss = 0.0;
//                o.Normal = UnpackNormal(tex2D (_BumpMap, IN.pack0.xy));
////                o.Normal = tex2D (_BumpMap, IN.pack0.xy);
//                
//                // EX:
//                half4 tex = tex2D (_MainTex, IN.pack0.xy);
//                o.Albedo = tex.rgb;
//                o.Alpha = tex.a;
//                
//                /////////////////////////////////////////////////////////////////////////////////////////
//                //
//				//
//				//
//
//                // Surface function normally be called here
//                #ifndef USING_DIRECTIONAL_LIGHT
//                	fixed3 lightDir = normalize(IN.lightDir);
//                #else
//                	fixed3 lightDir = IN.lightDir;
//                #endif
//
//                // Apply lighting
//                fixed4 c = LightingLambert (o, lightDir, LIGHT_ATTENUATION(IN));
//                c.a = 0.0;
//                return c;
//            }
//
//            
//
//            ENDCG
//
//        }
//
//        Pass {
//
//            //PrePassBase:
//            //
//            // Packs world normal into RGB
//            // Specular power into A
//
//            Name "PREPASS"
//            Tags { "LightMode" = "PrePassBase" }
//            Fog {Mode Off}
//
//            CGPROGRAM
//            #pragma vertex vert_surf
//            #pragma fragment frag_surf
//            #pragma fragmentoption ARB_precision_hint_fastest
//
//            #include "HLSLSupport.cginc"
//            #define UNITY_PASS_PREPASSBASE
//            #include "UnityCG.cginc"
//            #include "Lighting.cginc"
//
//            
//
//            struct v2f_surf {
//                float4 pos : SV_POSITION;
//                fixed3 normal : TEXCOORD0;
//                fixed2 uv;
//                float3 tangentWorld : TEXCOORD2;
//                float3 normalWorld : TEXCOORD3;
//                float3 binormalWorld : TEXCOORD4;
////                float3 tangentSpaceLightDir;
//                
//            };
//
//            v2f_surf vert_surf (appdata_full v) {
//                v2f_surf o;
//                // Transform vertex into normalized device coordinates
//                o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
//                // Calculate world normal
//                o.uv = TRANSFORM_UV(0);
//                
//                
//                o.tangentWorld = normalize( float3(mul(_Object2World, float4(float3(v.tangent.xyz),0.0)).xyz));
//                o.normalWorld = normalize(mul(float4(v.normal.xyz,0.0),_World2Object).xyz);
//                o.binormalWorld = normalize(cross(o.normalWorld,o.tangentWorld).xyz*v.tangent.w);
//                
//                
//                
//                o.normal = v.normal;
//                
////				float3 tangentSpaceLightDir;
////				float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
////				float3 binormal = cross( normalize(v.normal), normalize(v.tangent.xyz) );
////				float3x3 rotation = float3x3( v.tangent.xyz, binormal, v.normal );
////				o.tangentSpaceLightDir = mul(rotation, viewDir);
//                
//                // Tangent space rotation
////                TANGENT_SPACE_ROTATION; // creates the rotation matrix
////				o.lightDir = mul (rotation, ObjSpaceLightDir(v.vertex));
////				o.viewDir = mul (rotation, ObjSpaceViewDir(v.vertex));
//                
////                o.normal = mul((float3x3)_Object2World, SCALED_NORMAL);
//                return o;
//
//            }
//            
//            sampler2D _BumpMap;
//            float4 _BumpMap_ST;
//
//            fixed4 frag_surf (v2f_surf IN) : COLOR {
//				//
//                // SHADER CODE GOES HERE
//                //
//                /////////////////////////////////////////////////////////////////////////////////////////
//                // Surface setup
//				SurfaceOutput o;
////                o.Albedo = 0.0;
////                o.Emission = 0.0;
//                o.Specular = 0.0;
////                o.Alpha = 0.0;
////                o.Gloss = 0.0;
//
//				float4 texN = tex2D(_BumpMap,_BumpMap_ST.xy*IN.tex.xy+_BumpMap_ST.zw);
//				float3 localCoords = float3(2.0*texN.ag-float2(1.0,1.0),0.0);
//				localCoords.z = 1.0-0.5*dot(localCoords,localCoords);
//				
//				
//				float3x3 local2WorldTranspose = float3x3(
//					IN.tangentWorld,
//					IN.binormalWorld,
//					IN.normalWorld);
//				
//				float3 normalDir = normalize(mul(localCoords,local2WorldTranspose));
//				
//				
//
////				half3 tangentSpaceNormal = (tex2D(_BumpMap, IN.uv.xy).rgb * 2.0) - 1.0;
////				o.Normal = normalize(tangentSpaceNormal);
////				o.Normal = UnpackNormal(tex2D (_BumpMap, IN.uv.xy));
//                o.Normal = normalDir;//IN.normal;//-UnpackNormal(tex2D (_BumpMap, IN.texcoord.xy));
////                o.Normal = tex2D (_BumpMap, IN.normal.xy);
//                
//                /////////////////////////////////////////////////////////////////////////////////////////
//                //
//				//
//				//
//                
//                // Surface function normally be called here
//                fixed4 res;
//                // Pack normal
//                res.rgb = o.Normal * 0.5 + 0.5;
//                // Specular goes in alpha channel
//                res.a = o.Specular;
//                return res;
//            }
//            ENDCG
//
//        }
//
//        Pass {
//
//            //PrePassFinal pass lighting:
//            //
//            // 2 light maps
//            // Any number of pixel lights
//
//            Name "PREPASS"
//            Tags { "LightMode" = "PrePassFinal" }
//            ZWrite Off
//            CGPROGRAM
//            #pragma vertex vert_surf
//            #pragma fragment frag_surf
//            #pragma fragmentoption ARB_precision_hint_fastest
//            #pragma multi_compile_prepassfinal
//            #include "HLSLSupport.cginc"
//            #define UNITY_PASS_PREPASSFINAL
//            #include "UnityCG.cginc"
//            #include "Lighting.cginc"
//
//            
//
//            sampler2D _MainTex;
//
//            struct v2f_surf {
//                float4 pos : SV_POSITION;
//                float2 pack0 : TEXCOORD0;
//                float4 screen : TEXCOORD1;
//	            #ifndef LIGHTMAP_OFF
//	                float2 lmap : TEXCOORD2;
//	                float4 lmapFadePos : TEXCOORD3;
//	            #endif
//            };
//            #ifndef LIGHTMAP_OFF
//            	float4 unity_LightmapST;
////          float4 unity_ShadowFadeCenterAndType;	// HACK: Commented out due to error
//            #endif
//
//            float4 _MainTex_ST;
//            v2f_surf vert_surf (appdata_full v) {
//
//                v2f_surf o;
//                // Transform vertex into normalized device coordinates
//                o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
//                // Apply scaling/tiling to UV coordinates
//                o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
//                // Screen coordinates from position
//                o.screen = ComputeScreenPos (o.pos);
//
//            #ifndef LIGHTMAP_OFF
//                // Apply scaling/tiling to UV2 coordinates for light map
//                o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
//
//                // Dual lightmap fading factors
//                o.lmapFadePos.xyz = (mul(_Object2World, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
//                o.lmapFadePos.w = (-mul(UNITY_MATRIX_MV, v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
//
//            #endif
//                return o;
//
//            }
//
//            sampler2D _LightBuffer;
//
//            #ifndef LIGHTMAP_OFF
//	            sampler2D unity_Lightmap;
//	            sampler2D unity_LightmapInd;
//	            float4 unity_LightmapFade;
//            #endif
//
//            fixed4 unity_Ambient;
//
//            fixed4 frag_surf (v2f_surf IN) : COLOR {
//
//				//
//                // SHADER CODE GOES HERE
//                //
//                /////////////////////////////////////////////////////////////////////////////////////////
//                // Set up surface
//                SurfaceOutput o;
//                o.Albedo = 0.0;
//                o.Emission = 0.0;
//                o.Specular = 0.0;
//                o.Alpha = 0.0;
//                o.Gloss = 0.0;
//                
//                // EX:
//                half4 tex = tex2D (_MainTex, IN.pack0.xy);
//                o.Albedo = tex.rgb;
//                o.Alpha = tex.a;
//                
//                /////////////////////////////////////////////////////////////////////////////////////////
//                //
//				//
//				//
//                // Surface function normally be called here
//                
//                // Sample light buffer
//                half4 light = tex2Dproj (_LightBuffer, UNITY_PROJ_COORD(IN.screen));
//
//	            #if defined (SHADER_API_GLES)
//	                light = max(light, half4(0.001));
//	            #endif
//                // Light buffer values are encoded logarithmically
//                light = -log2(light);
//	            #ifndef LIGHTMAP_OFF
//	                // Sample both light maps and interpolate between them
//	                half3 lmFull = DecodeLightmap (tex2D(unity_Lightmap, IN.lmap.xy));
//	                half3 lmIndirect = DecodeLightmap (tex2D(unity_LightmapInd, IN.lmap.xy));
//	                float lmFade = length (IN.lmapFadePos) * unity_LightmapFade.z + unity_LightmapFade.w;
//	                half3 lm = lerp (lmIndirect, lmFull, saturate(lmFade));
//	                light.rgb += lm;
//	            #else
//	                // Ambient light only if there is no light map
//	                light.rgb += unity_Ambient.rgb;
//	            #endif
//
//                // Apply light buffer to material
//                half4 col = LightingLambert_PrePass (o, light);
//                return col;
//
//            }
//
//            ENDCG
//
//        }   // Pass to render object as a shadow caster
//
//        Pass {
//            Name "ShadowCaster"
//            Tags { "LightMode" = "ShadowCaster" }
//            Fog {Mode Off}
//            ZWrite On
//            ZTest Less
//            Cull Off
//            Offset 1, 1
//
//            CGPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag
//            #pragma multi_compile_shadowcaster
//            #pragma fragmentoption ARB_precision_hint_fastest
//            #include "UnityCG.cginc"
//
//            struct v2f { 
//                V2F_SHADOW_CASTER;
//            };
//
//
//            v2f vert( appdata_base v ) {
//                v2f o;
//                TRANSFER_SHADOW_CASTER(o)
//                return o;
//            }
//
//            float4 frag( v2f i ) : COLOR {
//                SHADOW_CASTER_FRAGMENT(i)
//            }
//
//            ENDCG
//
//        } // Pass
//
//    
//
//        // Pass to render object as a shadow collector
//        Pass {
//
//            Name "ShadowCollector"
//            Tags { "LightMode" = "ShadowCollector" }
//            Fog {Mode Off}
//            ZWrite On
//            ZTest Less
//
// 
//
//            CGPROGRAM
//
//            #pragma vertex vert
//            #pragma fragment frag
//            #pragma fragmentoption ARB_precision_hint_fastest
//            #pragma multi_compile_shadowcollector
//
//            #define SHADOW_COLLECTOR_PASS
//            #include "UnityCG.cginc"
//
//            struct appdata {
//                float4 vertex : POSITION;
//            };
//
//            struct v2f {
//                V2F_SHADOW_COLLECTOR;
//            };
//
//            v2f vert (appdata v){
//                v2f o;
//                TRANSFER_SHADOW_COLLECTOR(o)
//                return o;
//            }
//
//            fixed4 frag (v2f i) : COLOR {
//                SHADOW_COLLECTOR_FRAGMENT(i)
//            }
//
//            ENDCG
//        } // Pass
//    } // SubShader
} // Shader