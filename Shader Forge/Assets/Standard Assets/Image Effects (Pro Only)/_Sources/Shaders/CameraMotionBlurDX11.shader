 /*
 	see CameraMotionBlur.shader
 */
 
 Shader "Hidden/CameraMotionBlurDX11" {
	Properties {
		_MainTex ("-", 2D) = "" {}
		_NoiseTex ("-", 2D) = "grey" {}
		_VelTex ("-", 2D) = "black" {}
		_NeighbourMaxTex ("-", 2D) = "black" {}
	}

	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	// noisiness
	#define JITTER_SCALE (0.125f)
	// 's' in paper (# of samples for reconstruction)
	float _SampleCount;
	// 'k' in paper
	float _MaxRadiusOrKInPaper;

	struct v2f {
		float4 pos : POSITION;
		float2 uv  : TEXCOORD0;
	};
				
	sampler2D _MainTex;
	sampler2D _CameraDepthTexture;
	sampler2D _VelTex;
	sampler2D _NeighbourMaxTex;
	sampler2D _NoiseTex;
	
	float4 _MainTex_TexelSize;
	float4 _CameraDepthTexture_TexelSize;
	float4 _VelTex_TexelSize;
	
	float4x4 _InvViewProj;	// inverse view-projection matrix
	float4x4 _PrevViewProj;	// previous view-projection matrix
	float4x4 _ToPrevViewProjCombined; // combined
	
	float _VelocityScale;
	float _DisplayVelocityScale;

	float _MaxVelocity;
	float _MinVelocity;
		
	float _SoftZDistance;
	
	v2f vert(appdata_img v) 
	{
		v2f o;
		o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	}

	// returns vector with largest magnitude
	float2 vmax(float2 a, float2 b)
	{
		float ma = dot(a, a);
		float mb = dot(b, b);
		return (ma > mb) ? a : b;
	}

	// find dominant velocity in each tile
	float4 TileMax(v2f i) : COLOR
	{
		float halfk = _MaxRadiusOrKInPaper * 0.5;

		float2 max = float2(0.0, 0.0);
		float2 srcPos = i.uv - _MainTex_TexelSize.xy * (float2(halfk,halfk)-float2(.5,.5));

		for(int y=0; y<(int)_MaxRadiusOrKInPaper; y++) {
			for(int x=0; x<(int)_MaxRadiusOrKInPaper; x++) {
				float2 v = tex2D(_MainTex, srcPos + float2(x,y) * _MainTex_TexelSize.xy).xy;
				max = vmax(max, v);
		  	}
  	  	}
  	  	return float4(max, 0, 1);
	}

	// find maximum velocity in any adjacent tile
	float4 NeighbourMax(v2f i) : COLOR
	{
		float2 max = float2(0.0, 0.0);
		for(int y=-1; y<=1; y++) {
			for(int x=-1; x<=1; x++) {
				float2 v = tex2D(_MainTex, i.uv + float2(x, y)*_MainTex_TexelSize.xy).xy;
				max = vmax(max, v);
			}
		}
  	  	return float4(max, 0, 1);		
	}	
	 	 	
	float4 Debug(v2f i) : COLOR
	{
		return abs(tex2D(_MainTex, i.uv)) * _DisplayVelocityScale;		
	}

	// classification filters
	float cone(float2 px, float2 py, float2 v)
	{
		// ole: i like the more "cony" result better ... TODO: maybe change cone to clamp(1.0 - (0.75*length(px - py) / length(v)), 0.0, 1.0);
		return clamp(1.0 - (length(px - py) / length(v)), 0.0, 1.0);
	}

	float cylinder(float2 x, float2 y, float2 v)
	{
		float lv = length(v);
		return 1.0 - smoothstep(0.95*lv, 1.05*lv, length(x - y));
	}

	// is zb closer than za?
	float softDepthCompare(float za, float zb)
	{
		return clamp(1.0 - (za - zb) / _SoftZDistance, 0.0, 1.0);
	}

	// reconstruction based blur
	float4 ReconstructFilterBlur(v2f i) : COLOR
	{	
		// uv's

		float2 x = i.uv;
		float2 xf = x;

		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
    		xf.y = 1-xf.y;
		#endif

		float2 x2 = xf;
		
		float2 vn = tex2D(_NeighbourMaxTex, x2).xy;	// largest velocity in neighbourhood
		float4 cx = tex2D(_MainTex, x);				// color at x

#if 0 // DISABLED FOR 3.0 complicance
		float2 vnp = vn*_MainTex_TexelSize.zw;		// vel in pixels
		float velMag = length(vnp);
		if (velMag < _MinVelocity) { 
			// no blur
			return cx;
		}
#endif

		float zx = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, x));
		zx = -Linear01Depth(zx);					// depth at x
		float2 vx = tex2D(_VelTex, xf).xy;			// vel at x 

		// random offset [-0.5, 0.5]
		float j = (tex2D(_NoiseTex, i.uv * 11.0f ).r*2-1) * JITTER_SCALE;

		// sample current pixel
		float weight = 1.0/(1.0+length(vx*_MainTex_TexelSize.zw));
		float4 sum = cx * weight;
 
		int totalSamples = (int)_SampleCount;
		int centerSample = (int)(_SampleCount-1) / 2;
 
		// take S - 1 additional samples
		for(int l=0; l<totalSamples; l++) 
		{
			if (l==centerSample) continue;	// skip center sample

			// Choose evenly placed filter taps along +-vN,
			// but jitter the whole filter to prevent ghosting			

			float t = lerp(-1.0, 1.0, (l + j + 1.0) / ((float)totalSamples + 1.0));	// paper with jitter
			//float t = lerp(-1.0, 1.0, l / (float)(totalSamples - 1)); // simon
			
			float2 y = x + vn*t;

			float2 yf = y;
			#if UNITY_UV_STARTS_AT_TOP
			if (_MainTex_TexelSize.y < 0)
	    		yf.y = 1 - yf.y;
			#endif

			// velocity at y 
			float2 vy = tex2Dlod(_VelTex, float4(yf,0,0)).xy;

			float zy = UNITY_SAMPLE_DEPTH(tex2Dlod (_CameraDepthTexture, float4(y,0,0))); 
			zy = -Linear01Depth(zy);						
			
			float f = softDepthCompare(zx, zy);
			float b = softDepthCompare(zy, zx);
			float alphay = f * cone(y, x, vy) +	// blurry y in front of any x
			               b * cone(x, y, vx) +	// any y behing blurry x; estimate background
			               cylinder(y, x, vy) * cylinder(x, y, vx) * 2.0;	// simultaneous blurry x and y
			
			// accumulate sample 
			float4 cy = tex2Dlod(_MainTex, float4(y,0,0));
			sum += cy * alphay;
			weight += alphay;
		}
		sum /= weight;

		return sum;
	}

		 	 	  	 	  	 	  	 	 		 	 	  	 	  	 	  	 	 		 	 	  	 	  	 	  	 	 
	ENDCG
	
Subshader {

 // pass 0
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
	  #pragma target 5.0
      #pragma vertex vert
      #pragma fragment TileMax
      #pragma fragmentoption ARB_precision_hint_fastest

      ENDCG
  	}

 // pass 1
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
	  #pragma target 5.0
      #pragma vertex vert
      #pragma fragment NeighbourMax
      #pragma fragmentoption ARB_precision_hint_fastest

      ENDCG
  	}

 // pass 2
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
	  #pragma target 5.0
      #pragma vertex vert 
      #pragma fragment ReconstructFilterBlur
      #pragma fragmentoption ARB_precision_hint_fastest

      ENDCG
  	}

  }
  
Fallback off

}