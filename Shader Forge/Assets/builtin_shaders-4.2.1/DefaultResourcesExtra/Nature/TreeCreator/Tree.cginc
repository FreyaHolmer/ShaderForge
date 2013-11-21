#ifndef TREE_CG_INCLUDED
#define TREE_CG_INCLUDED

#include "TerrainEngine.cginc"

fixed4 _Color;
fixed3 _TranslucencyColor;
fixed _TranslucencyViewDependency;
half _ShadowStrength;

struct LeafSurfaceOutput {
	fixed3 Albedo;
	fixed3 Normal;
	fixed3 Emission;
	fixed Translucency;
	half Specular;
	fixed Gloss;
	fixed Alpha;
};

inline half4 LightingTreeLeaf (LeafSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
{
	half3 h = normalize (lightDir + viewDir);
	
	half nl = dot (s.Normal, lightDir);
	
	half nh = max (0, dot (s.Normal, h));
	half spec = pow (nh, s.Specular * 128.0) * s.Gloss;
	
	// view dependent back contribution for translucency
	fixed backContrib = saturate(dot(viewDir, -lightDir));
	
	// normally translucency is more like -nl, but looks better when it's view dependent
	backContrib = lerp(saturate(-nl), backContrib, _TranslucencyViewDependency);
	
	fixed3 translucencyColor = backContrib * s.Translucency * _TranslucencyColor;
	
	// wrap-around diffuse
	nl = max(0, nl * 0.6 + 0.4);
	
	fixed4 c;
	c.rgb = s.Albedo * (translucencyColor * 2 + nl);
	c.rgb = c.rgb * _LightColor0.rgb + spec;
	
	// For directional lights, apply less shadow attenuation
	// based on shadow strength parameter.
	#if defined(DIRECTIONAL) || defined(DIRECTIONAL_COOKIE)
	c.rgb *= lerp(2, atten * 2, _ShadowStrength);
	#else
	c.rgb *= 2*atten;
	#endif
	
	return c;
}

#endif