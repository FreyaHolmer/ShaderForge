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


			bool unlit = (ps.lightMode == SF_PassSettings.LightMode.Unlit);
			bool lit = !unlit;
			bool lambert = (ps.lightMode == SF_PassSettings.LightMode.Lambert);

			// Diffuse makes these available: Transmission, Light Wrapping, Ambient lighting, Diffuse Power
			bool diffConnected = editor.materialOutput.diffuse.IsConnectedAndEnabled();
			bool specConnected = editor.materialOutput.specular.IsConnectedAndEnabled();

			editor.materialOutput.diffuse.SetAvailable( lit );
			editor.materialOutput.diffusePower.SetAvailable( lit && diffConnected );
			editor.materialOutput.specular.SetAvailable( lit && !lambert );
			editor.materialOutput.gloss.SetAvailable( lit && !lambert && specConnected );
			editor.materialOutput.normal.SetAvailable( true );
			editor.materialOutput.alpha.SetAvailable( true );
			editor.materialOutput.alphaClip.SetAvailable( true );
			editor.materialOutput.refraction.SetAvailable( true );
			editor.materialOutput.emissive.SetAvailable( true );
			editor.materialOutput.transmission.SetAvailable( lit && diffConnected );

			editor.materialOutput.ambientDiffuse.SetAvailable( lit && diffConnected);
			editor.materialOutput.ambientSpecular.SetAvailable( lit && !lambert && specConnected );
			editor.materialOutput.customLighting.SetAvailable( !lit );

			editor.materialOutput.lightWrap.SetAvailable( lit && diffConnected );
			editor.materialOutput.displacement.SetAvailable( editor.materialOutput.tessellation.IsConnectedAndEnabled() );
			editor.materialOutput.outlineColor.SetAvailable( editor.materialOutput.outlineWidth.IsConnectedAndEnabled() );





		

			//editor.materialOutput.anisotropicDirection.SetAvailable( false );
			//editor.materialOutput.worldPositionOffset.SetAvailable( false );


		}









	}
}
