using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(WaterBase))]

public class Displace : MonoBehaviour 
{		
	public void Awake() 
	{
		if (enabled)
			OnEnable();
		else
			OnDisable();
	}
	
	public void OnEnable() 
	{
		Shader.EnableKeyword("WATER_VERTEX_DISPLACEMENT_ON");
		Shader.DisableKeyword("WATER_VERTEX_DISPLACEMENT_OFF");		
	}	

	public void OnDisable() 
	{
		Shader.EnableKeyword("WATER_VERTEX_DISPLACEMENT_OFF");
		Shader.DisableKeyword("WATER_VERTEX_DISPLACEMENT_ON");		
	}
	
	/*
	public float GetOffsetAt(Vector3 pos, int displacementMapAmounts = 3)
	{
		return 0.0f;	
	}
	
	public Vector3 GetNormalAt(Vector3 pos, float scale = 1.0F) 
	{
		return Vector3.one;
	}		
	*/
}