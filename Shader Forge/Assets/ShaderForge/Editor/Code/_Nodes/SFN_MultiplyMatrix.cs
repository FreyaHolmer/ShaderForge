using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_MultiplyMatrix : SF_Node_Arithmetic {

		public Matrix4x4 mtx;

		public SFN_MultiplyMatrix() {

		}

		public override void Initialize() {
			node_height = 58;
			base.Initialize( "Multiply Matrix" );
			base.showColor = false;
			base.UseLowerPropertyBox( false, true );
			//UseLowerReadonlyValues( true );
			
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv4m4x4,false),
				SF_NodeConnector.Create(this,"A","A",ConType.cInput,ValueType.VTv4m4x4,false).SetRequired(true),
				SF_NodeConnector.Create(this,"B","B",ConType.cInput,ValueType.VTv4m4x4,false).SetRequired(true)
			};

			base.conGroup = ScriptableObject.CreateInstance<SFNCG_MatrixMultiply>().Initialize( connectors[0], connectors[1], connectors[2] );
			
		}
		
		
		
		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			string evalStr = "";
			evalStr += "mul(" + GetConnectorByStringID( "A" ).TryEvaluate() + "," + GetConnectorByStringID( "B" ).TryEvaluate() + ")";
			return evalStr;
		}

		public override Vector4 EvalCPU() {

			return Color.black;
			/*
			SF_NodeConnector a = ConnectedInputs[0];
			SF_NodeConnector b = ConnectedInputs[1];

			if( !a.IsConnected() || !b.IsConnected() ) {
				return Color.black;
			}


			bool am = a.inputCon.valueType == ValueType.VTm4x4;
			bool bm = b.inputCon.valueType == ValueType.VTm4x4;

			Matrix4x4 mtx;
			if( am && bm ) {
				return Color.black;
			} else if(am){
				mtx = ( a.inputCon.node as SFN_Matrix4x4 ).mtx;
				return mtx * GetInputData( "B" )[x,y];
			} else if( bm ) {
				mtx = ( b.inputCon.node as SFN_Matrix4x4 ).mtx;
				return mtx.transpose * GetInputData( "A" )[x, y];
			}
			return Color.black;*/
		}



		public override void NeatWindow() {
			PrepareWindowColor();
			GUI.BeginGroup( rect );
			Rect r = new Rect( rectInner );
			r = r.Pad( 4 );

			Rect texCoords = new Rect( r );
			texCoords.width /= 7;
			texCoords.height /= 3;
			texCoords.x = texCoords.y = 0;
			GUI.DrawTextureWithTexCoords( r, SF_GUI.Handle_drag, texCoords, alphaBlend: true );

			GUI.EndGroup();
			ResetWindowColor();

		}


	}
}