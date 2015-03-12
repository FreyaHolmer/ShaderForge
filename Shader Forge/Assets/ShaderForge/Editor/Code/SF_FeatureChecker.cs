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

			bool deferredPp = ps.catLighting.renderPath == SFPSC_Lighting.RenderPath.DeferredPrePass;
			bool unlit = (ps.catLighting.lightMode == SFPSC_Lighting.LightMode.Unlit);
			bool pbr = (ps.catLighting.lightMode == SFPSC_Lighting.LightMode.PBL);
			bool lit = !unlit;

			// Diffuse makes these available: Transmission, Light Wrapping, Ambient lighting, Diffuse Power
			bool diffConnected = editor.mainNode.diffuse.IsConnectedAndEnabled();
			bool specConnected = editor.mainNode.specular.IsConnectedAndEnabled();

			bool bakedData = ps.catLighting.bakedLight;
			bool usesAmbient = ps.catLighting.useAmbient;
			bool ambDiffConnected = ps.HasAmbientDiffuse();
			bool ambSpecConnected = ps.HasAmbientSpecular();



			editor.mainNode.diffuse.SetAvailable( lit );
			editor.mainNode.diffusePower.SetAvailable( lit && diffConnected && !deferredPp );
			editor.mainNode.specular.SetAvailable( lit );
			editor.mainNode.gloss.SetAvailable( lit && ( specConnected || ( diffConnected && pbr ) ));
			editor.mainNode.normal.SetAvailable( true );
			editor.mainNode.alpha.SetAvailable( !deferredPp  );
			editor.mainNode.alphaClip.SetAvailable( true );
			editor.mainNode.refraction.SetAvailable( !deferredPp  );
			editor.mainNode.emissive.SetAvailable( true );
			editor.mainNode.transmission.SetAvailable( lit && diffConnected && !deferredPp  );





			editor.mainNode.diffuseOcclusion.SetAvailable( lit && diffConnected && ( bakedData || usesAmbient || ambDiffConnected ) );
			editor.mainNode.specularOcclusion.SetAvailable( lit && specConnected && ( bakedData || ambSpecConnected ) ); // Masks ambient spec & directional lightmaps

			editor.mainNode.ambientDiffuse.SetAvailable( lit && diffConnected);
			editor.mainNode.ambientSpecular.SetAvailable( lit && specConnected );
			editor.mainNode.customLighting.SetAvailable( !lit && !deferredPp  );

			editor.mainNode.lightWrap.SetAvailable( lit && diffConnected && !deferredPp  );
			editor.mainNode.displacement.SetAvailable( editor.mainNode.tessellation.IsConnectedAndEnabled() );
			editor.mainNode.outlineColor.SetAvailable( editor.mainNode.outlineWidth.IsConnectedAndEnabled() && !deferredPp  );
			editor.mainNode.outlineWidth.SetAvailable( !deferredPp  );





		

			//editor.materialOutput.anisotropicDirection.SetAvailable( false );
			//editor.materialOutput.worldPositionOffset.SetAvailable( false );


		}









	}
}
