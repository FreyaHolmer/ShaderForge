 /*
 	motion blur image effect

	variation of simon green's implementation of "plausible motion blur"
	http://graphics.cs.williams.edu/papers/MotionBlurI3D12/

	and alex vlacho´s motion blur in
	http://www.valvesoftware.com/publications/2008/GDC2008_PostProcessingInTheOrangeBox.pdf

	TODO: add vignetting mask to hide blurring center objects in camera mode
	TODO: add support for dynamic and skinned objects
 */
 
 Shader "Hidden/CameraMotionBlur" {
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
	#define NUM_SAMPLES (13)
	// # samples for valve style blur
	#define MOTION_SAMPLES (16)
	// 'k' in paper
	// (mclamped to MAX_RADIUS)	
	float _MaxRadiusOrKInPaper;

	struct v2f 
	{
		float4 pos : POSITION;
		float2 uv  : TEXCOORD0;
	};
				
	sampler2D _MainTex;
	sampler2D _CameraDepthTexture;
	sampler2D _VelTex;
	sampler2D _NeighbourMaxTex;
	sampler2D _NoiseTex;
	sampler2D _TileTexDebug;
	
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
	
	float4 _BlurDirectionPacked;
	
	float _SoftZDistance;
	
	v2f vert(appdata_img v) 
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	}
	
	// calculate velocity for static scene from depth buffer			
	float4 CameraVelocity(v2f i) : COLOR
	{
		float2 depth_uv = i.uv;

		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			depth_uv.y = 1 - depth_uv.y;	
		#endif

		// read depth
		float d = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, depth_uv));
		
		// calculate position from pixel from depth
		float3 clipPos = float3(i.uv.x*2.0-1.0, (i.uv.y)*2.0-1.0, d);

		// only 1 matrix mul:
		float4 prevClipPos = mul(_ToPrevViewProjCombined, float4(clipPos, 1.0));
		prevClipPos.xyz /= prevClipPos.w;

		/*
		float4 ws = mul(_InvViewProj, float4(clipPos, 1.0));
		ws /= ws.w;
		prevClipPos = mul(_PrevViewProj,ws);
		prevClipPos.xyz /= prevClipPos.w;
		*/

		float2 vel = _VelocityScale * /* exposure * framerate * */ (clipPos.xy - prevClipPos.xy) / 2.f;

		// clamp to maximum velocity (in pixels)
		float maxVel = length(_MainTex_TexelSize.xy*_MaxVelocity);
		if (length(vel) > maxVel) {
			vel = normalize(vel) * maxVel;
		}
		return float4(vel, 0.0, 0.0);
	}

	// returns vector with largest magnitude
	float2 vmax(float2 a, float2 b)
	{
		float ma = dot(a, a);
		float mb = dot(b, b);
		return (ma > mb) ? a : b;
	}

	// find dominant velocity for each tile
	float4 TileMax(v2f i) : COLOR
	{
		// NOTE: weird loop & uv addressing due to circumventing HLSL2GLSL compiler bug

		float halfk = _MaxRadiusOrKInPaper * 0.5;

		float2 uvCorner = i.uv - _MainTex_TexelSize.xy * (float2(halfk,halfk)-float2(.5,.5));
		float2 mx = tex2D(_MainTex, uvCorner);

		// debug:
		//return tex2D(_MainTex, i.uv);

		for(int j=0; j<=(int)(_MaxRadiusOrKInPaper-1)*(_MaxRadiusOrKInPaper-1); j++) 
		{
			float t = ((float)j)/((float)_MaxRadiusOrKInPaper);
			float2 uv = float2(frac(t)*(_MaxRadiusOrKInPaper), (j/(int)_MaxRadiusOrKInPaper)) * _MainTex_TexelSize.xy;
			mx = vmax(mx, tex2Dlod(_MainTex, float4(uvCorner + uv,0,0)).xy);
  	  	}

  	  	return float4(mx, 0, 0);
	}

	// find maximum velocity in any adjacent tile
	float4 NeighbourMax(v2f i) : COLOR
	{
		// NOTE: weird loop & uv addressing due to circumventing HLSL2GLSL compiler bug

		// debug:
		//return tex2D(_MainTex, i.uv);

		float2 x_ = i.uv;

		// to fetch all neighbours, we need 3x3 samples

		float2 mx = tex2D(_MainTex, x_+float2(.9,.9)*_MainTex_TexelSize.xy).xy;
		mx = vmax(mx, tex2D(_MainTex, x_+float2(.9,0)*_MainTex_TexelSize.xy).xy);
		mx = vmax(mx, tex2D(_MainTex, x_+float2(.9,-.9)*_MainTex_TexelSize.xy).xy);

		mx = vmax(mx, tex2D(_MainTex, x_+float2(.0,.9)*_MainTex_TexelSize.xy).xy);
		mx = vmax(mx, tex2D(_MainTex, x_+float2(.0,.0)*_MainTex_TexelSize.xy).xy);
		mx = vmax(mx, tex2D(_MainTex, x_+float2(.0,-.9)*_MainTex_TexelSize.xy).xy);		
		
		mx = vmax(mx, tex2D(_MainTex, x_+float2(-.9,.9)*_MainTex_TexelSize.xy).xy);
		mx = vmax(mx, tex2D(_MainTex, x_+float2(-.9,.0)*_MainTex_TexelSize.xy).xy);
		mx = vmax(mx, tex2D(_MainTex, x_+float2(-.9,-.9)*_MainTex_TexelSize.xy).xy);

  	  	return float4(mx, 0, 0);		
	}
	 	 	
	float4 Debug(v2f i) : COLOR
	{
		//return saturate(tex2D(_NeighbourMaxTex, i.uv).xxxx * _DisplayVelocityScale);
		return saturate( float4(tex2D(_MainTex, i.uv).x,abs(tex2D(_MainTex, i.uv).y),-tex2D(_MainTex, i.uv).xy) * _DisplayVelocityScale);
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

	float4 SimpleBlur (v2f i) : COLOR
	{
		float2 x = i.uv;
		float2 xf = x;

		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
    		xf.y = 1 - xf.y;
		#endif

		float2 vx = tex2D(_VelTex, xf).xy;	// vel at x

		float4 sum = float4(0, 0, 0, 0);
		for(int l=0; l<NUM_SAMPLES; l++) {
			float t = l / (float) (NUM_SAMPLES - 1);
			t = t-0.5;
			float2 y = x - vx*t;
			float4 cy = tex2D(_MainTex, y);
			sum += cy;
		}
		sum /= NUM_SAMPLES;		
		return sum;
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
		float2 vx = tex2D(_VelTex, xf).xy;			// vel at x 

		// debug:
		//return float4(length(vn)-length(vx),0,0,0)*20;

		float zx = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, x));
		zx = -Linear01Depth(zx);					// depth at x

		// random offset [-0.5, 0.5]
		float j = (tex2D(_NoiseTex, i.uv * 11.0f).r*2-1) * JITTER_SCALE;

		// sample current pixel
		float weight = 1.0/(1.0+length(vx*_MainTex_TexelSize.zw));
		float4 sum = cx * weight;
 
		int centerSample = (int)(NUM_SAMPLES-1)/2;
 
		for(int l=0; l<NUM_SAMPLES; l++) 
		{ 
			float contrib = 1.0f;
		#if SHADER_API_D3D11
			if (l==centerSample) continue;	// skip center sample
		#else
			if (l==centerSample) contrib = 0.0f;	// skip center sample
		#endif

			float t = lerp(-1.0, 1.0, (l + j + 1.0) / ((float)NUM_SAMPLES + 1.0));	// paper
			//float t = lerp(-1.0, 1.0, l / (float)(NUM_SAMPLES - 1)); // simon

			float2 y = x + vn * t;

			float2 yf = y;
			#if UNITY_UV_STARTS_AT_TOP
			if (_MainTex_TexelSize.y < 0)
	    		yf.y = 1-yf.y;
			#endif

			// velocity at y 
			float2 vy = tex2D(_VelTex, yf).xy;

			float zy = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, y)); 
			zy = -Linear01Depth(zy);						
			
			float f = softDepthCompare(zx, zy);
			float b = softDepthCompare(zy, zx);
			float alphay = f * cone(y, x, vy) +	// blurry y in front of any x
			               b * cone(x, y, vx) +	// any y behing blurry x; estimate background
			               cylinder(y, x, vy) * cylinder(x, y, vx) * 2.0;	// simultaneous blurry x and y
			// debug:
			//float alphay = 1;

			// accumulate sample 
			float4 cy = tex2D(_MainTex, y);
			sum += cy * alphay * contrib;
			weight += alphay * contrib;
		}
		sum /= weight;

		return sum;
	}


	float4 MotionVectorBlur (v2f i) : COLOR
	{
		float2 x = i.uv;

		float2 insideVector = (x*2-1) * float2(1,_MainTex_TexelSize.w/_MainTex_TexelSize.z);
		float2 rollVector = float2(insideVector.y, -insideVector.x);

		float2 blurDir = _BlurDirectionPacked.x * float2(0,1);
		blurDir += _BlurDirectionPacked.y * float2(1,0);
		blurDir += _BlurDirectionPacked.z * rollVector;
		blurDir += _BlurDirectionPacked.w * insideVector;
		blurDir *= _VelocityScale;
 
		// clamp to maximum velocity (in pixels)
		float velMag = length(blurDir);
		if (velMag > _MaxVelocity) {
			blurDir *= (_MaxVelocity / velMag);
			velMag = _MaxVelocity;
		} 

		float4 centerTap = tex2D(_MainTex, x);
		float4 sum = centerTap;

		blurDir *= smoothstep(_MinVelocity * 0.25f, _MinVelocity * 2.5, velMag);

		blurDir *= _MainTex_TexelSize.xy;
		blurDir /= MOTION_SAMPLES;

		for(int i=0; i<MOTION_SAMPLES; i++) {
			float4 tap = tex2D(_MainTex, x+i*blurDir);
			sum += tap;
		}

		return sum/(1+MOTION_SAMPLES);
	}
		 	 	  	 	  	 	  	 	 		 	 	  	 	  	 	  	 	 		 	 	  	 	  	 	  	 	 
	ENDCG
	
Subshader {
 
 // pass 0
 Pass {
	  ZTest Always Cull Off ZWrite On Blend Off
	  Fog { Mode off }      

      CGPROGRAM
	  #pragma target 3.0
      #pragma vertex vert
      #pragma fragment CameraVelocity
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma glsl
      #pragma exclude_renderers d3d11_9x 

      ENDCG
  	}

 // pass 1
 Pass {
	  ZTest Always Cull Off ZWrite Off Blend Off
	  Fog { Mode off }      

      CGPROGRAM
	  #pragma target 3.0
      #pragma vertex vert
      #pragma fragment Debug
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma glsl
      #pragma exclude_renderers d3d11_9x 

      ENDCG
  	}

 // pass 2
 Pass {
	  ZTest Always Cull Off ZWrite Off Blend Off
	  Fog { Mode off }      

      CGPROGRAM
	  #pragma target 3.0
      #pragma vertex vert
      #pragma fragment TileMax
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma glsl
      #pragma exclude_renderers d3d11_9x       

      ENDCG
  	}

 // pass 3
 Pass {
	  ZTest Always Cull Off ZWrite Off Blend Off
	  Fog { Mode off }      

      CGPROGRAM
	  #pragma target 3.0
      #pragma vertex vert
      #pragma fragment NeighbourMax
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma glsl
      #pragma exclude_renderers d3d11_9x       

      ENDCG
  	}

 // pass 4
 Pass {
	  ZTest Always Cull Off ZWrite Off Blend Off
	  Fog { Mode off }      

      CGPROGRAM
	  #pragma target 3.0
      #pragma vertex vert 
      #pragma fragment ReconstructFilterBlur
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma glsl
      #pragma exclude_renderers d3d11_9x       

      ENDCG
  	}

 // pass 5
 Pass {
	  ZTest Always Cull Off ZWrite Off Blend Off
	  Fog { Mode off }      
 
      CGPROGRAM
	  #pragma target 3.0
      #pragma vertex vert
      #pragma fragment SimpleBlur
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma glsl
      #pragma exclude_renderers d3d11_9x       

      ENDCG
  	}

  // pass 6
 Pass {
	  ZTest Always Cull Off ZWrite Off Blend Off
	  Fog { Mode off }      
 
      CGPROGRAM
	  #pragma target 3.0
      #pragma vertex vert
      #pragma fragment MotionVectorBlur
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma glsl
      #pragma exclude_renderers d3d11_9x       

      ENDCG
  	}
  }
  
Fallback off

}