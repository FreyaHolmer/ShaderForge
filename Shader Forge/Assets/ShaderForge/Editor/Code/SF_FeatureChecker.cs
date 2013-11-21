using UnityEngine;
using System.Collections;

namespace ShaderForge {
	[System.Serializable]
	public class SF_FeatureChecker : ScriptableObject {

		[SerializeField]
		SF_PassSettings ps;

		[SerializeField]
		public SF_Editor editor;

		public SF_FeatureChecker Initialize( SF_PassSettings ps, SF_Editor editor ) {
			this.ps = ps;
			this.editor = editor;
			return this;
		}

		public void OnEnable() {
			base.hideFlags = HideFlags.HideAndDontSave;
		}



		// Diffuse & Diffuse Power
		// if light mode is not unlit

		// Specular & Gloss
		// if light mode is !(Unlit || Lambert)

		// Transmission
		// if light mode is not Unlit

		// Light Wrapping
		// if light mode is not Unlit

		public void UpdateAvailability() {


			editor.materialOutput.diffuse.SetAvailable( ps.lightMode != SF_PassSettings.LightMode.Unlit );
			editor.materialOutput.diffusePower.SetAvailable( ps.lightMode != SF_PassSettings.LightMode.Unlit );
			editor.materialOutput.specular.SetAvailable( ps.lightMode != SF_PassSettings.LightMode.Unlit && ps.lightMode != SF_PassSettings.LightMode.Lambert );
			editor.materialOutput.gloss.SetAvailable( ps.lightMode != SF_PassSettings.LightMode.Unlit && ps.lightMode != SF_PassSettings.LightMode.Lambert );
			editor.materialOutput.normal.SetAvailable( true );
			editor.materialOutput.alpha.SetAvailable( true );
			editor.materialOutput.alphaClip.SetAvailable( true );
			editor.materialOutput.refraction.SetAvailable( true );
			editor.materialOutput.emissive.SetAvailable( true );
			editor.materialOutput.transmission.SetAvailable( ps.lightMode != SF_PassSettings.LightMode.Unlit );
			editor.materialOutput.lightWrap.SetAvailable( ps.lightMode != SF_PassSettings.LightMode.Unlit );
			editor.materialOutput.displacement.SetAvailable( editor.materialOutput.tessellation.IsConnectedAndEnabled() );
			editor.materialOutput.outlineColor.SetAvailable( editor.materialOutput.outlineWidth.IsConnectedAndEnabled() );

		

			//editor.materialOutput.anisotropicDirection.SetAvailable( false );
			//editor.materialOutput.worldPositionOffset.SetAvailable( false );


		}









	}
}
