Shader "Custom/Test Shadowed" {

    Properties {
        _Color ("Main Color", Color) = (1,1,1,1) //note: required but not used
    }

    SubShader {

        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        Pass {
            Tags { "LightMode" = "PrepassBase" }
            Blend One One
            Fog { Color(0,0,0,0) }
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd_fullshadows
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

            struct v2f {
                float4 pos : SV_POSITION;
                LIGHTING_COORDS(0,1)
            };

 

            v2f vert (appdata_full v) {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

 

            float4 frag (v2f i) : COLOR {
                return LIGHT_ATTENUATION(i);
            }

            ENDCG
        } //Pass

    } //SubShader

    FallBack "Diffuse" //note: for passes: ForwardBase, ShadowCaster, ShadowCollector

}