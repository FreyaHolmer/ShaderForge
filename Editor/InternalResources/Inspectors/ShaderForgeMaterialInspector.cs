

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using ShaderForge;

public class ShaderForgeMaterialInspector : MaterialEditor {
	
	// this is the same as the ShaderProperty function, show here so 
	// you can see how it works
	/*
	private void ShaderPropertyImpl(Shader shader, int propertyIndex)
	{
		int i = propertyIndex;
		string label = ShaderUtil.GetPropertyDescription(shader, i);
		string propertyName = ShaderUtil.GetPropertyName(shader, i);




		switch (ShaderUtil.GetPropertyType(shader, i))
		{
			case ShaderUtil.ShaderPropertyType.Range: // float ranges
			{
				GUILayout.BeginHorizontal();
				float v2 = ShaderUtil.GetRangeLimits(shader, i, 1);
				float v3 = ShaderUtil.GetRangeLimits(shader, i, 2);



				RangeProperty(propertyName, label, v2, v3);
				GUILayout.EndHorizontal();
				
				break;
			}
			case ShaderUtil.ShaderPropertyType.Float: // floats
			{
				FloatProperty(propertyName, label);
				break;
			}
			case ShaderUtil.ShaderPropertyType.Color: // colors
			{
				ColorProperty(propertyName, label);
				break;
			}
			case ShaderUtil.ShaderPropertyType.TexEnv: // textures
			{
				ShaderUtil.ShaderPropertyTexDim desiredTexdim = ShaderUtil.GetTexDim(shader, i);
				TextureProperty(propertyName, label, desiredTexdim);
				
				GUILayout.Space(6);
				break;
			}
			case ShaderUtil.ShaderPropertyType.Vector: // vectors
			{
				VectorProperty(propertyName, label);
				break;
			}
			default:
			{
				GUILayout.Label("Unknown property " + label + " : " + ShaderUtil.GetPropertyType(shader, i));
				break;
			}
		}
	}*/



	public override void OnInspectorGUI()
	{
		base.serializedObject.Update();
		var theShader = serializedObject.FindProperty ("m_Shader");


		if (isVisible && !theShader.hasMultipleDifferentValues && theShader.objectReferenceValue != null )
		{

			Shader shader = theShader.objectReferenceValue as Shader;


			// SHADER FORGE BUTTONS
			if( GUILayout.Button( "Open shader in Shader Forge" ) ) {
				SF_Editor.Init( shader );
			}
			if( SF_Tools.advancedInspector ) {
				GUILayout.BeginHorizontal();
				{
					GUIStyle btnStyle = "MiniButton";
					if( GUILayout.Button( "Open shader code", btnStyle ) ) {
						UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal( AssetDatabase.GetAssetPath( shader ), 1 );
					}
					//if( GUILayout.Button( "Open compiled shader", btnStyle ) ) {
					//	ShaderForgeInspector.OpenCompiledShader( shader );
					//}
				}
				GUILayout.EndHorizontal();
			}

			Material mat = target as Material;
			

			mat.globalIlluminationFlags = (MaterialGlobalIlluminationFlags)EditorGUILayout.EnumPopup( "Emission GI", mat.globalIlluminationFlags);
			
			GUILayout.Space(6);




			if(this.PropertiesGUI())
				this.PropertiesChanged();
		}
	}


	/*
	public override void OnInspectorGUI ()
	{
		serializedObject.Update ();
		var theShader = serializedObject.FindProperty ("m_Shader");	
		if (isVisible && !theShader.hasMultipleDifferentValues && theShader.objectReferenceValue != null)
		{
			float controlSize = 80;
			
			EditorGUIUtility.LookLikeControls(Screen.width - controlSize - 20);
			
			EditorGUI.BeginChangeCheck();
			Shader shader = theShader.objectReferenceValue as Shader;


			// SHADER FORGE BUTTONS
			if( GUILayout.Button( "Open shader in Shader Forge" ) ) {
				SF_Editor.Init( shader );
			}
			if( SF_Tools.advancedInspector ) {
				GUILayout.BeginHorizontal();
				{
					GUIStyle btnStyle = "MiniButton";
					if( GUILayout.Button( "Open source", btnStyle ) ) {
						UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal( AssetDatabase.GetAssetPath( shader ), 1 );
					}
					if( GUILayout.Button( "Open compiled", btnStyle ) ) {
						ShaderForgeInspector.OpenCompiledShader( shader );
					}
				}
				GUILayout.EndHorizontal();
			}

			GUILayout.Space(6);


			//GUILayout.Box("Material Properties:",EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
			//GUI.color = Color.white;
			// END SF BUTTONS


			//GUI.color = SF_Node.colorExposed;
			for (int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
			{
				ShaderPropertyImpl(shader, i);
			}
			//GUI.color = Color.white;
			
			if (EditorGUI.EndChangeCheck())
				PropertiesChanged ();
		}
	}
	*/
}
