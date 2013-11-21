 Shader "Hidden/tiltShift" {
	Properties {
		_MainTex ("Base", 2D) = "" {}
		_Blurred ("Blurred", 2D) = "" {}
		_Coc ("Coc", 2D) = "" {}
	}

	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
		#if UNITY_UV_STARTS_AT_TOP 
		float2 uv1 : TEXCOORD1;		
		#endif
	};
	
	struct v2f_blur {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
		float4 uv01 : TEXCOORD1;
		float4 uv23 : TEXCOORD2;
		float4 uv45 : TEXCOORD3;
	};
	
	half4 offsets;	
		
	sampler2D _MainTex;
	sampler2D _CameraDepthTexture;
	sampler2D _Blurred;
	sampler2D _Coc;
	
	half4 _MainTex_TexelSize;
	half4 _SimpleDofParams;
	half2 _FgOrBgCoc;
	
	#define focalStart01 _SimpleDofParams.x
	#define focalDistance01 _SimpleDofParams.y
	#define focalEnd01 _SimpleDofParams.z
	#define curve _SimpleDofParams.w
		
	v2f vert (appdata_img v) {
		v2f o;
		o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		o.uv.xy = v.texcoord;
		
		#if UNITY_UV_STARTS_AT_TOP 
		o.uv1.xy = v.texcoord;
		if (_MainTex_TexelSize.y < 0)
			o.uv1.y = 1-o.uv1.y;		
		#else
		
		#endif
		  
		return o;
	} 
	
	half4 fragCocFg (v2f i) : COLOR {
		#if UNITY_UV_STARTS_AT_TOP 
		float d = UNITY_SAMPLE_DEPTH(tex2D (_CameraDepthTexture, i.uv1.xy));
		#else
		float d = UNITY_SAMPLE_DEPTH(tex2D (_CameraDepthTexture, i.uv.xy));
		#endif
		d = Linear01Depth(d);
		 
		half coc;
		
		if (d > focalDistance01) 
			coc = (d - focalDistance01 - 0.0025) * 0.0f;
		else
			coc = (focalDistance01 - d - 0.0025) * 2.0f;
		
		coc = clamp (  ( (coc / (curve))), 0.0, 0.999);	
		return EncodeFloatRGBA (coc);
	}

	half4 fragCocBgAfterFg (v2f i) : COLOR 
	{
		#if UNITY_UV_STARTS_AT_TOP 
		float d = UNITY_SAMPLE_DEPTH(tex2D (_CameraDepthTexture, i.uv1.xy));
		#else
		float d = UNITY_SAMPLE_DEPTH(tex2D (_CameraDepthTexture, i.uv.xy));
		#endif
			
		d = Linear01Depth(d);
		 
		half coc;
		
		if (d > focalDistance01) 
			coc = (d - focalDistance01 - 0.0025) * 2.0f;
		else
			coc = (focalDistance01 - d - 0.0025) * 0.0f;
		
		coc = clamp (  ( (coc / (curve))), 0.0, 0.999);	
		
		#if UNITY_UV_STARTS_AT_TOP 
		half4 cocTex = tex2D (_Coc, i.uv1.xy);
		#else
		half4 cocTex = tex2D (_Coc, i.uv.xy);
		#endif
		
		return max( EncodeFloatRGBA (coc), cocTex);
	}
		
	half4 fragDofApply (v2f i) : COLOR 
	{
		half4 color = tex2D (_MainTex, i.uv.xy);
		
		#if UNITY_UV_STARTS_AT_TOP 
		half4 blurred = tex2D (_Blurred, i.uv1.xy);
		#else
		half4 blurred = tex2D (_Blurred, i.uv.xy);
		#endif
		
		#if UNITY_UV_STARTS_AT_TOP
		half coc = DecodeFloatRGBA (tex2D (_Coc, i.uv1.xy));
		#else
		half coc = DecodeFloatRGBA (tex2D (_Coc, i.uv.xy));
		#endif
		
		return lerp (color, blurred, coc);
	}

	half4 fragDofDebug (v2f i) : COLOR 
	{
		half4 color = tex2D (_MainTex, i.uv.xy);
		
		#if UNITY_UV_STARTS_AT_TOP 
		half4 blurred = tex2D (_Blurred, i.uv1.xy);
		#else
		half4 blurred = tex2D (_Blurred, i.uv.xy);
		#endif		
		
		blurred = (half4(0,1,0,1) + blurred) * 0.5;
		
		#if UNITY_UV_STARTS_AT_TOP
		half coc = DecodeFloatRGBA (tex2D (_Coc, i.uv1.xy));
		#else
		half coc = DecodeFloatRGBA (tex2D (_Coc, i.uv.xy));
		#endif		
		return lerp (color, blurred, coc);
	}	
	
	half4 fragUp (v2f i) : COLOR 
	{
		half4 color = tex2D (_MainTex, i.uv.xy);
		
		half4 cocTex = tex2D (_Coc, i.uv.xy);
		
		return max (color, cocTex);
	}	

	
	v2f_blur vertBlur (appdata_img v) {
		v2f_blur o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

		o.uv.xy = v.texcoord.xy;

		o.uv01 =  v.texcoord.xyxy + offsets.xyxy * float4(1,1, -1,-1);
		o.uv23 =  v.texcoord.xyxy + offsets.xyxy * float4(1,1, -1,-1) * 2.0;
		o.uv45 =  v.texcoord.xyxy + offsets.xyxy * float4(1,1, -1,-1) * 3.0;

		return o;  
	}	

	half4 fragDependentBlur (v2f_blur i) : COLOR 
	{
		half4 color = float4 (0,0,0,0);
		
		// texture dependent texture read ... a little slow
		
		half coc = DecodeFloatRGBA (tex2D (_Coc, i.uv));
		
		i.uv01 = lerp (i.uv.xyxy, i.uv01, coc);
		i.uv23 = lerp (i.uv.xyxy, i.uv23, coc);
		i.uv45 = lerp (i.uv.xyxy, i.uv45, coc);

		color += 0.30 * tex2D (_MainTex, i.uv);
		color += 0.15 * tex2D (_MainTex, i.uv01.xy);
		color += 0.15 * tex2D (_MainTex, i.uv01.zw);
		color += 0.125 * tex2D (_MainTex, i.uv23.xy);
		color += 0.125 * tex2D (_MainTex, i.uv23.zw);
		color += 0.075 * tex2D (_MainTex, i.uv45.xy);
		color += 0.075 * tex2D (_MainTex, i.uv45.zw);	
		
		return color;
	}

	half4 fragBlurMax (v2f_blur i) : COLOR 
	{
		half4 color = float4 (0,0,0,0);
		
		color += 0.30 * tex2D (_MainTex, i.uv);
		color += 0.15 * tex2D (_MainTex, i.uv01.xy);
		color += 0.15 * tex2D (_MainTex, i.uv01.zw);
		color += 0.125 * tex2D (_MainTex, i.uv23.xy);
		color += 0.125 * tex2D (_MainTex, i.uv23.zw);
		color += 0.075 * tex2D (_MainTex, i.uv45.xy);
		color += 0.075 * tex2D (_MainTex, i.uv45.zw);	
		
		return max (color, tex2D (_Coc, i.uv));
	}
	
	half4 fragBlurWeighted (v2f_blur i) : COLOR {
		half4 blurredColor = float4 (0,0,0,0);

		half4 sampleA = tex2D (_MainTex, i.uv.xy);
		half4 sampleB = tex2D (_MainTex, i.uv01.xy);
		half4 sampleC = tex2D (_MainTex, i.uv01.zw);
		half4 sampleD = tex2D (_MainTex, i.uv23.xy);
		half4 sampleE = tex2D (_MainTex, i.uv23.zw);
		
		sampleA.a = DecodeFloatRGBA (tex2D (_Coc, i.uv));
		sampleB.a = DecodeFloatRGBA (tex2D (_Coc, i.uv01.xy));
		sampleC.a = DecodeFloatRGBA (tex2D (_Coc, i.uv01.zw));
		sampleD.a = DecodeFloatRGBA (tex2D (_Coc, i.uv23.xy));
		sampleE.a = DecodeFloatRGBA (tex2D (_Coc, i.uv23.zw));
 		
		half sum = sampleA.a + dot( half4(1,1,1,1), half4 (sampleB.a,sampleC.a,sampleD.a,sampleE.a));
	
		sampleA.rgb = sampleA.rgb * sampleA.a; 
		sampleB.rgb = sampleB.rgb * sampleB.a;
		sampleC.rgb = sampleC.rgb * sampleC.a; 
		sampleD.rgb = sampleD.rgb * sampleD.a; 
		sampleE.rgb = sampleE.rgb * sampleE.a; 
				
		blurredColor += sampleA;
		blurredColor += sampleB;
		blurredColor += sampleC; 
		blurredColor += sampleD; 
		blurredColor += sampleE; 
		
		blurredColor /= sum;
		half4 color = blurredColor;		
		color.a = sampleA.a;
		return color;
	}	
 
	ENDCG
	
Subshader {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }    	
 
 Pass { // 0 

      CGPROGRAM
      
      #pragma fragmentoption ARB_precision_hint_fastest
      // not enough temporary registers for flash
      #pragma exclude_renderers flash
      #pragma vertex vert
      #pragma fragment fragCocFg

      ENDCG
  	}

 Pass { // 1  

      CGPROGRAM
      
      #pragma fragmentoption ARB_precision_hint_fastest
      // not enough temporary registers for flash
      #pragma exclude_renderers flash
      #pragma vertex vert
      #pragma fragment fragDofApply

      ENDCG
  	}
  	
 Pass { // 2 

      CGPROGRAM
      
      #pragma fragmentoption ARB_precision_hint_fastest
      // not enough temporary registers for flash
      #pragma exclude_renderers flash
      #pragma vertex vertBlur
      #pragma fragment fragDependentBlur

      ENDCG
  	}  	
  	
 Pass { // 3

      CGPROGRAM
      
      #pragma fragmentoption ARB_precision_hint_fastest
      // not enough temporary registers for flash
      #pragma exclude_renderers flash
      #pragma vertex vertBlur
      #pragma fragment fragBlurMax

      ENDCG
  	}    	

 Pass { // 4  

      CGPROGRAM
      
      #pragma fragmentoption ARB_precision_hint_fastest
      // not enough temporary registers for flash
      #pragma exclude_renderers flash
      #pragma vertex vert
      #pragma fragment fragDofDebug

      ENDCG
  	}  
  	
 Pass { // 5
		
      CGPROGRAM
      
      #pragma fragmentoption ARB_precision_hint_fastest
      // not enough temporary registers for flash
      #pragma exclude_renderers flash
      #pragma vertex vert
      #pragma fragment fragCocBgAfterFg

      ENDCG
  	}
  	
 Pass { // 6

      CGPROGRAM
      
      #pragma fragmentoption ARB_precision_hint_fastest
      // not enough temporary registers for flash
      #pragma exclude_renderers flash
      #pragma vertex vertBlur
      #pragma fragment fragBlurWeighted

      ENDCG
  	} 

 Pass { // 7

      CGPROGRAM
      
      #pragma fragmentoption ARB_precision_hint_fastest
      // not enough temporary registers for flash
      #pragma exclude_renderers flash
      #pragma vertex vert
      #pragma fragment fragUp

      ENDCG
  	} 
  	  	
  }
  
Fallback off

}