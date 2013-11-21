
#pragma strict

@CustomEditor (TiltShift)
class TiltShiftEditor extends Editor 
{	
	var serObj : SerializedObject;	
		
	var focalPoint : SerializedProperty;
	var smoothness : SerializedProperty;
	var visualizeCoc : SerializedProperty;

	var renderTextureDivider : SerializedProperty;
	var blurIterations : SerializedProperty;
	var foregroundBlurIterations : SerializedProperty;	
	var maxBlurSpread : SerializedProperty;	
	var enableForegroundBlur : SerializedProperty;

	function OnEnable () {
		serObj = new SerializedObject (target);
		
		focalPoint = serObj.FindProperty ("focalPoint");
		smoothness = serObj.FindProperty ("smoothness");
		visualizeCoc = serObj.FindProperty ("visualizeCoc");
		
		renderTextureDivider = serObj.FindProperty ("renderTextureDivider");
		blurIterations = serObj.FindProperty ("blurIterations");
		foregroundBlurIterations = serObj.FindProperty ("foregroundBlurIterations");
		maxBlurSpread = serObj.FindProperty ("maxBlurSpread");	
		enableForegroundBlur = serObj.FindProperty ("enableForegroundBlur");	
	} 
    		
    function OnInspectorGUI () {         
    	serObj.Update ();
    	
    	var go : GameObject = (target as TiltShift).gameObject;
    	
    	if (!go)
    		return;
    		
    	if (!go.camera)
    		return;
    		    		
		GUILayout.Label ("Current: "+go.camera.name+", near "+go.camera.nearClipPlane+", far: "+go.camera.farClipPlane+", focal: "+focalPoint.floatValue, EditorStyles.miniBoldLabel);
    	    	
    	GUILayout.Label ("Focal Settings", EditorStyles.boldLabel);    	
		EditorGUILayout.PropertyField (visualizeCoc, new GUIContent("Visualize"));  		
   		focalPoint.floatValue = EditorGUILayout.Slider ("Distance", focalPoint.floatValue, go.camera.nearClipPlane, go.camera.farClipPlane);
   		EditorGUILayout.PropertyField (smoothness, new GUIContent("Smoothness"));
   		
   		EditorGUILayout.Separator ();
   		
   		GUILayout.Label ("Background Blur", EditorStyles.boldLabel);  
   		renderTextureDivider.intValue = EditorGUILayout.Slider ("Downsample", renderTextureDivider.intValue, 1, 3);
   		blurIterations.intValue = EditorGUILayout.Slider ("Iterations", blurIterations.intValue, 1, 4);
   		EditorGUILayout.PropertyField (maxBlurSpread, new GUIContent("Max blur spread"));
		
		EditorGUILayout.Separator ();
		
		GUILayout.Label ("Foreground Blur", EditorStyles.boldLabel);  
   		EditorGUILayout.PropertyField (enableForegroundBlur, new GUIContent("Enable")); 
   		
   		if (enableForegroundBlur.boolValue)
			foregroundBlurIterations.intValue = EditorGUILayout.Slider ("Iterations", foregroundBlurIterations.intValue, 1, 4);
   				
   		//GUILayout.Label ("Background options");
   		//edgesOnly.floatValue = EditorGUILayout.Slider ("Edges only", edgesOnly.floatValue, 0.0, 1.0);
   		//EditorGUILayout.PropertyField (edgesOnlyBgColor, new GUIContent ("Background"));    		
    	    	
    	serObj.ApplyModifiedProperties();
    }
}
