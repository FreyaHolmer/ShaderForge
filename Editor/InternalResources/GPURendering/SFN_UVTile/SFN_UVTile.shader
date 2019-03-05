Shader "Hidden/Shader Forge/SFN_UVTile" {
    Properties {
        _OutputMask ("Output Mask", Vector) = (1,1,1,1)
        _UVIN ("UV", 2D) = "black" {}
        _WDT ("Wid", 2D) = "black" {}
        _HGT ("Hei", 2D) = "black" {}
        _TILE ("Tile", 2D) = "black" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma target 3.0
            uniform float4 _OutputMask;
            uniform sampler2D _UVIN;
            uniform sampler2D _WDT;
            uniform sampler2D _HGT;
            uniform sampler2D _TILE;

            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {

                // Read inputs
                float4 _uvin = tex2D( _UVIN, i.uv );
                float4 _wdt = tex2D( _WDT, i.uv );
                float4 _hgt = tex2D( _HGT, i.uv );
                float4 _tile = tex2D( _TILE, i.uv );

                // Operator
float2 tcrcp = float2(1.0,1.0)/float2( _wdt.x, _hgt.x );
                float ty = floor(_tile.x * tcrcp.x);
                float tx = _tile.x - _wdt.x * ty;
                float4 outputColor = float4((_uvin.xy + float2(tx, ty)) * tcrcp,0,0);

                // Return
                return outputColor * _OutputMask;
            }
            ENDCG
        }
    }
}
