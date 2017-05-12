using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.IO;

namespace ShaderForge{
	public class NodesToShaders : MonoBehaviour {

		// _A ("A", 2D) = "white" {}

		static string[] template = new string[]{
			"Shader \"Hidden/Shader Forge/{0}\" [[[",
			"    Properties [[[",
			"        _OutputMask (\"Output Mask\", Vector) = (1,1,1,1)",
			"{1}",
			"    ]]]",
			"    SubShader [[[",
			"        Tags [[[",
			"            \"RenderType\"=\"Opaque\"",
			"        ]]]",
			"        Pass [[[",
			"        CGPROGRAM",
			"            #pragma vertex vert",
			"            #pragma fragment frag",
			"            #define UNITY_PASS_FORWARDBASE",
			"            #include \"UnityCG.cginc\"",
			"            #pragma target 3.0",
			"            uniform float4 _OutputMask;",
			"{2}",
			"",
			"            struct VertexInput [[[",
			"                float4 vertex : POSITION;",
			"                float2 texcoord0 : TEXCOORD0;",
			"            ]]];",
			"            struct VertexOutput [[[",
			"                float4 pos : SV_POSITION;",
			"                float2 uv : TEXCOORD0;",
			"            ]]];",
			"            VertexOutput vert (VertexInput v) [[[",
			"                VertexOutput o = (VertexOutput)0;",
			"                o.uv = v.texcoord0;",
			"                o.pos = UnityObjectToClipPos( v.vertex );",
			"                return o;",
			"            ]]]",
			"            float4 frag(VertexOutput i) : COLOR [[[",
			"",
			"                // Read inputs",
			"{3}",
			"",
			"                // Operator{4}",
			"                float4 outputColor = {5};",
			"",
			"                // Return",
			"                return outputColor * _OutputMask;",
			"            ]]]",
			"            ENDCG",
			"        ]]]",
			"    ]]]",
			"]]]"
		};

		static string templateMerged = "";

		void Start () {
			
		}
	
		void WriteShader (  ) {
		
		}

		[MenuItem("CODEGEN/Generate blit shaders")]
		public static void AnalyzeSelection() {

			templateMerged = string.Join( "\n", template );
			Debug.Log( "Merged = " + templateMerged );

			/*
			System.Type[] types = (
				from domainAssembly in System.AppDomain.CurrentDomain.GetAssemblies()
				from assemblyType in domainAssembly.GetTypes()
				where typeof(SF_Node).IsAssignableFrom(assemblyType) && assemblyType != typeof(SF_Node)
				select assemblyType).ToArray();
			 * */

			List<SF_EditorNodeData> types = SF_Editor.instance.nodeTemplates;

			for( int i = 0; i < types.Count; i++ ) {

				SF_Node node = types[i].CreateInstance();
				bool manual = node.shaderGenMode == ShaderGenerationMode.Manual || node.shaderGenMode == ShaderGenerationMode.ManualModal;
				bool offUniform = node.shaderGenMode == ShaderGenerationMode.OffUniform;
				bool shouldGenerate = !(manual || offUniform );
				if( shouldGenerate ) {
					
					if( node.shaderGenMode == ShaderGenerationMode.ModularInput ) {
						for( int j = 2; j <= 5; j++ ) { // 2 to 5
							Generate( node, node.GetType().Name, j );
						}
					} else if( node.shaderGenMode == ShaderGenerationMode.Modal ) {

						string[] modes = node.GetModalModes();
						for( int j = 0; j < modes.Length; j++ ) {
							Generate( node, node.GetType().Name, -1, modes[j] );
						}
						
					} else {
						Generate( node, node.GetType().Name );
					}
				} else {
					Debug.LogFormat( "Skipping code gen for {0} ({1})", node.nodeName, node.shaderGenMode );
				}
				DestroyImmediate( node );
			}

			AssetDatabase.Refresh( ImportAssetOptions.DontDownloadFromCacheServer );

		}

		public static void Generate( SF_Node node, string nodeName, int modularInputCount = -1, string modalSuffix = "" ) {


			bool modal = node.shaderGenMode == ShaderGenerationMode.Modal || node.shaderGenMode == ShaderGenerationMode.ManualModal;
			

			string pathRoot = SF_Resources.InternalResourcesPath + SF_Resources.pGpuRendering;

			string folder = pathRoot + nodeName + "/";

			bool directoryExists = Directory.Exists( folder );

			if( directoryExists == false ) {
				Directory.CreateDirectory( folder );
			}
			

			// 0:	Shader Name				
			// 1:	Properties				_A ("A", 2D) = "white" {}
			// 2:	Uniform declarations	uniform sampler2D _A;
			// 3:	Input read				float4 a = tex2D( _A, i.uv );
			// 4:	Operation

			string shaderName = nodeName;	// 0

			// _A ("A", 2D) = "white" {}
			//  uniform sampler2D _A;
			SF_NodeConnector[] inputs = node.connectors.Where( x => x.conType == ConType.cInput ).ToArray();
			if( modularInputCount != -1 ) {
				inputs = inputs.Take( modularInputCount ).ToArray();
			}
			int iCount = inputs.Length;
			string[] inputIDs		= inputs.Select( x => "_" + x.strID		).ToArray();
			string[] displayNames	= inputs.Select( x => x.label			).ToArray();
			string[] varNames		= inputs.Select( x => "_" + x.strID.ToLower() ).ToArray();

			int addedPropsCount = 0;
			string[] addedProps = node.ExtraPassedFloatProperties();
			if( addedProps != null ) {
				addedPropsCount = addedProps.Length;
			}

			string[] properties = new string[iCount + addedPropsCount];
			string[] uniDecs = new string[iCount + addedPropsCount];
			string[] inputSamplers = new string[iCount];
			for( int i = 0; i < iCount; i++ ) {
				properties[i] = string.Format(		"        {0} (\"{1}\", 2D) = \"black\" ", inputIDs[i], displayNames[i]) + "{}";
				uniDecs[i] =						"            uniform sampler2D " + inputIDs[i] + ";";
				inputSamplers[i] = string.Format(	"                float4 {0} = tex2D( {1}, i.uv );", varNames[i], inputIDs[i] );
			}
			for( int i = iCount; i < iCount + addedPropsCount; i++ ) {
				string dispName = addedProps[i - iCount];
				string vName = "_" + dispName.ToLower();
				properties[i] = string.Format("        {0} (\"{1}\", Float) = 0 ", vName, dispName);
				uniDecs[i] = "            uniform float " + vName + ";";
			}


			string[] preOutputLines = new string[]{};
			string outputLine = "";
			if( node.shaderGenMode == ShaderGenerationMode.SimpleFunction ) {

				outputLine = nodeName.Substring( 4 ).ToLower() + "(" + string.Join( ", ", varNames ) + ")";

			} else if( modal || node.shaderGenMode == ShaderGenerationMode.CustomFunction || node.shaderGenMode == ShaderGenerationMode.ValuePassing ) {

				string[] allOutputLines;
				if( modal ) {
					allOutputLines = node.GetBlitOutputLines( modalSuffix );
				} else {
					allOutputLines = node.GetBlitOutputLines();
				} 
				
				preOutputLines = new string[allOutputLines.Length-1];
				for (int i = 0; i < allOutputLines.Length-1; i++){ // All but last
					preOutputLines[i] = allOutputLines[i];
				}
				outputLine = allOutputLines[allOutputLines.Length - 1]; // Last

			} else if( node.shaderGenMode == ShaderGenerationMode.ModularInput ) {

				string prefix, infix, suffix;
				node.GetModularShaderFixes( out prefix, out infix, out suffix );
				outputLine = prefix + varNames[0] + infix + varNames[1] + suffix;
				for( int i = 2; i < modularInputCount; i++ ) {
					outputLine = prefix + outputLine + infix + varNames[i] + suffix;
				}

			}

			if( modularInputCount != -1 )
				shaderName += "_" + modularInputCount;
			if( modalSuffix != "" )
				shaderName += "_" + modalSuffix;

			string shaderCode = GetShaderCode( shaderName, properties, uniDecs, inputSamplers, preOutputLines, outputLine );

			
			string filePath = folder + nodeName;
			if(modularInputCount != -1)
				filePath += "_" + modularInputCount;
			if( modalSuffix != "" )
				filePath += "_" + modalSuffix;
			filePath += ".shader";

			File.WriteAllLines( filePath, shaderCode.Split( '\n' ) );

		}



		static string GetShaderCode( string shaderName, string[] properties, string[] uniDecs, string[] inputSamplers, string[] preOutputLines, string outputLine ) {
			string preOutputString = "";
			if( preOutputLines.Length > 0 ) {
				preOutputString = "\n" + string.Join( "\n                ", preOutputLines );
			}
			string code = string.Format( templateMerged,
				shaderName,
				string.Join( "\n", properties ),
				string.Join( "\n", uniDecs ),
				string.Join( "\n", inputSamplers ),
				preOutputString,
				outputLine
			);
			code = code.Replace( "[[[", "{" ); // string.Format doesn't like curlies without integers...
			code = code.Replace( "]]]", "}" );
			return code;
		}










	}

}

