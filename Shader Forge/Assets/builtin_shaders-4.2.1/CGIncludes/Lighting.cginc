#ifndef LIGHTING_INCLUDED
#define LIGHTING_INCLUDED

struct SurfaceOutput {
	fixed3 Albedo;
	fixed3 Normal;
	fixed3 Emission;
	half Specular;
	fixed Gloss;
	fixed Alpha;
};

#ifndef USING_DIRECTIONAL_LIGHT
#if defined (DIRECTIONAL_COOKIE) || defined (DIRECTIONAL)
#define USING_DIRECTIONAL_LIGHT
#endif
#endif


// NOTE: you would think using half is fine here, but that would make
// Cg apply some precision emulation, making the constants in the shader
// much different; and do some other stupidity that actually increases ALU
// count on d3d9 at least. So we use float.
//
// Also, the numbers in many components should be the same, but are changed
// very slightly in the last digit, to prevent Cg from mis-optimizing
// the shader (it tried to be super clever at saving one shader constant
// at expense of gazillion extra scalar moves). Saves about 6 ALU instructions
// on d3d9 SM2.
#define UNITY_DIRBASIS \
const float3x3 unity_DirBasis = float3x3( \
  float3( 0.81649658,  0.0,        0.57735028), \
  float3(-0.40824830,  0.70710679, 0.57735027), \
  float3(-0.40824829, -0.70710678, 0.57735026) \
);


inline half3 DirLightmapDiffuse(in half3x3 dirBasis, fixed4 color, fixed4 scale, half3 normal, bool surfFuncWritesNormal, out half3 scalePerBasisVector)
{
	half3 lm = DecodeLightmap (color);
	
	// will be compiled out (and so will the texture sample providing the value)
	// if it's not used in the lighting function, like in LightingLambert
	scalePerBasisVector = DecodeLightmap (scale);

	// will be compiled out when surface function does not write into o.Normal
	if (surfFuncWritesNormal)
	{
		half3 normalInRnmBasis = saturate (mul (dirBasis, normal));
		lm *= dot (normalInRnmBasis, scalePerBasisVector);
	}

	return lm;
}


fixed4 _LightColor0;
fixed4 _SpecColor;

inline fixed4 LightingLambert (SurfaceOutput s, fixed3 lightDir, fixed atten)
{
	fixed diff = max (0, dot (s.Normal, lightDir));
	
	fixed4 c;
	c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten * 2);
	c.a = s.Alpha;
	return c;
}


inline fixed4 LightingLambert_PrePass (SurfaceOutput s, half4 light)
{
	fixed4 c;
	c.rgb = s.Albedo * light.rgb;
	c.a = s.Alpha;
	return c;
}

inline half4 LightingLambert_DirLightmap (SurfaceOutput s, fixed4 color, fixed4 scale, bool surfFuncWritesNormal)
{
	UNITY_DIRBASIS
	half3 scalePerBasisVector;
	
	half3 lm = DirLightmapDiffuse (unity_DirBasis, color, scale, s.Normal, surfFuncWritesNormal, scalePerBasisVector);
	
	return half4(lm, 0);
}


// NOTE: some intricacy in shader compiler on some GLES2.0 platforms (iOS) needs 'viewDir' & 'h'
// to be mediump instead of lowp, otherwise specular highlight becomes too bright.
inline fixed4 LightingBlinnPhong (SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten)
{
	half3 h = normalize (lightDir + viewDir);
	
	fixed diff = max (0, dot (s.Normal, lightDir));
	
	float nh = max (0, dot (s.Normal, h));
	float spec = pow (nh, s.Specular*128.0) * s.Gloss;
	
	fixed4 c;
	c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * _SpecColor.rgb * spec) * (atten * 2);
	c.a = s.Alpha + _LightColor0.a * _SpecColor.a * spec * atten;
	return c;
}

inline fixed4 LightingBlinnPhong_PrePass (SurfaceOutput s, half4 light)
{
	fixed spec = light.a * s.Gloss;
	
	fixed4 c;
	c.rgb = (s.Albedo * light.rgb + light.rgb * _SpecColor.rgb * spec);
	c.a = s.Alpha + spec * _SpecColor.a;
	return c;
}

inline half4 LightingBlinnPhong_DirLightmap (SurfaceOutput s, fixed4 color, fixed4 scale, half3 viewDir, bool surfFuncWritesNormal, out half3 specColor)
{
	UNITY_DIRBASIS
	half3 scalePerBasisVector;
	
	half3 lm = DirLightmapDiffuse (unity_DirBasis, color, scale, s.Normal, surfFuncWritesNormal, scalePerBasisVector);
	
	half3 lightDir = normalize (scalePerBasisVector.x * unity_DirBasis[0] + scalePerBasisVector.y * unity_DirBasis[1] + scalePerBasisVector.z * unity_DirBasis[2]);
	half3 h = normalize (lightDir + viewDir);

	float nh = max (0, dot (s.Normal, h));
	float spec = pow (nh, s.Specular * 128.0);
	
	// specColor used outside in the forward path, compiled out in prepass
	specColor = lm * _SpecColor.rgb * s.Gloss * spec;
	
	// spec from the alpha component is used to calculate specular
	// in the Lighting*_Prepass function, it's not used in forward
	return half4(lm, spec);
}



#ifdef UNITY_CAN_COMPILE_TESSELLATION
struct UnityTessellationFactors {
    float edge[3] : SV_TessFactor;
    float inside : SV_InsideTessFactor;
};
#endif // UNITY_CAN_COMPILE_TESSELLATION

#endif
