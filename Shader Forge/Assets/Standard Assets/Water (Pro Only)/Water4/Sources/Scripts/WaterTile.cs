using UnityEngine;

[ExecuteInEditMode]
public class WaterTile : MonoBehaviour 
{
	public PlanarReflection reflection;
	public WaterBase waterBase;
	
	public void Start () 
	{
		AcquireComponents();
	}
	
	private void AcquireComponents() 
	{
		if (!reflection) {
			if (transform.parent)
				reflection = (PlanarReflection)transform.parent.GetComponent<PlanarReflection>();
			else
				reflection = (PlanarReflection)transform.GetComponent<PlanarReflection>();	
		}
		
		if (!waterBase) {
			if (transform.parent)
				waterBase = (WaterBase)transform.parent.GetComponent<WaterBase>();
			else
				waterBase = (WaterBase)transform.GetComponent<WaterBase>();	
		}
	}
	
#if UNITY_EDITOR
	public void Update () 
	{
		AcquireComponents();
	}
#endif
	
	public void OnWillRenderObject() 
	{
		if (reflection)
			reflection.WaterTileBeingRendered(transform, Camera.current);
		if (waterBase)
			waterBase.WaterTileBeingRendered(transform, Camera.current);		
	}
}
