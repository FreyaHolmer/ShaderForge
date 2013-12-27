using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace ShaderForge {

	[System.Serializable]
	public class SFN_Append : SF_Node {

		public SFN_Append() {
			/*
			Initialize("Append");
			base.showColor = true;
			UseLowerReadonlyValues(true);
		
			connectors = new SF_NodeConnection[]{
				new SF_NodeConnection(this,"",ConType.cOutput,ValueType.VTvPending,false),
				new SF_NodeConnection(this,"A",ConType.cInput,ValueType.VTvPending,false).SetRequired(true),
				new SF_NodeConnection(this,"B",ConType.cInput,ValueType.VTvPending,false).SetRequired(true)
			};

			base.conGroup = new SFNCG_Append( connectors[0], connectors[1], connectors[2] );
			*/
		}

		public override void Initialize() {
			base.Initialize( "Append" );
			base.showColor = true;
			UseLowerReadonlyValues( true );

			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTvPending,false),
				SF_NodeConnector.Create(this,"A","A",ConType.cInput,ValueType.VTvPending,false).SetRequired(true),
				SF_NodeConnector.Create(this,"B","B",ConType.cInput,ValueType.VTvPending,false).SetRequired(true)
			};

			base.conGroup = ScriptableObject.CreateInstance<SFNCG_Append>().Initialize( connectors[0], connectors[1], connectors[2] );
		}

		public override int GetEvaluatedComponentCount() {
			return ( (SFNCG_Append)conGroup ).GetOutputComponentCount();
		}

		public override bool IsUniformOutput() {
			return ( GetInputData( "A" ).uniform && GetInputData( "B" ).uniform );
		}


		// New system
		public override void RefreshValue() {
			RefreshValue( 1, 2 );
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {

			string varName = "float";
			int compCount = GetEvaluatedComponentCount();
			if( compCount > 1 )
				varName += compCount;

			return varName + "(" + GetConnectorByStringID( "A" ).TryEvaluate() + "," + GetConnectorByStringID( "B" ).TryEvaluate() + ")";
		}

		public override float NodeOperator( int x, int y, int c ) {

			int evcc = GetEvaluatedComponentCount();
			int acc = GetInputCon( "A" ).GetCompCount();

			if( c < acc )
				return GetInputData( "A", x, y, c );
			else if( evcc > c )
				return GetInputData( "B", x, y, c - acc );
			else
				return 0f;
		}


	}
}