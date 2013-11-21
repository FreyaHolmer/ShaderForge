#ifndef TREE_VERTEXLIT_CG_INCLUDED
#define TREE_VERTEXLIT_CG_INCLUDED

#include "UnityCG.cginc"
#include "TerrainEngine.cginc"

fixed4 _Color;
fixed3 _TranslucencyColor;
fixed _TranslucencyViewDependency;
half _ShadowStrength;

// --- Lighting ----------------------------------------------------------------

fixed3 _LightColor0;
fixed3 ShadeTranslucentMainLight (float4 vertex, float3 normal)
{
	float3 viewDir = normalize(WorldSpaceViewDir(vertex));
	float3 lightDir = normalize(WorldSpaceLightDir(vertex));
	fixed3 lightColor = _LightColor0.rgb;

	float nl = dot (normal, lightDir);
	
	// view dependent back contribution for translucency
	fixed backContrib = saturate(dot(viewDir, -lightDir));
	
	// normally translucency is more like -nl, but looks better when it's view dependent
	backContrib = lerp(saturate(-nl), backContrib, _TranslucencyViewDependency);
	
	// wrap-around diffuse
	fixed diffuse = max(0, nl * 0.6 + 0.4);
	
	return lightColor.rgb * (diffuse + backContrib * _TranslucencyColor);
}


fixed3 ShadeTranslucentLights (float4 vertex, float3 normal)
{
	float3 viewDir = normalize(WorldSpaceViewDir(vertex));
	float3 mainLightDir = normalize(WorldSpaceLightDir(vertex));
	float3 frontlight = ShadeSH9 (float4(normal,1.0));
	float3 backlight = ShadeSH9 (float4(-normal,1.0));
	#ifdef VERTEXLIGHT_ON
	float3 worldPos = mul(_Object2World, vertex).xyz;
	frontlight += Shade4PointLights (
		unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
		unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
		unity_4LightAtten0, worldPos, normal);
	backlight += Shade4PointLights (
		unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
		unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
		unity_4LightAtten0, worldPos, -normal);
	#endif
	
	// view dependent back contribution for translucency using main light as a cue
	fixed backContrib = saturate(dot(viewDir, -mainLightDir));
	backlight = lerp(backlight, backlight * backContrib, _TranslucencyViewDependency);
	
	// as we integrate over whole sphere instead of normal hemi-sphere
	// lighting gets too washed out, so let's half it down
	return 0.5 * (frontlight + backlight * _TranslucencyColor);
}
		

#endif
