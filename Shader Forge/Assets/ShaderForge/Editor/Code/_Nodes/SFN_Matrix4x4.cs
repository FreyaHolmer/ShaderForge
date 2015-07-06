using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ShaderForge {

	public class SFN_Matrix4x4 : SF_Node {


		public SFN_Matrix4x4() {

		}

		public Matrix4x4 mtx = Matrix4x4.identity;

		public override void Initialize() {
			node_height = NODE_HEIGHT;
			base.Initialize( "Matrix 4x4" );
			base.showColor = false;
			base.UseLowerPropertyBox( false );
			base.texture.uniform = true;
			base.texture.CompCount = 4;
			base.canAlwaysSetPrecision = true;
			base.alwaysDefineVariable = true;
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTm4x4,false)
			};
		}

		public override bool IsUniformOutput() {
			return true;
		}

		public override string Evaluate( OutChannel channel = OutChannel.All ) {
			return mtx.ToCgMatrix();
		}

		public override void NeatWindow() {
			PrepareWindowColor();
			GUI.BeginGroup( rect );
			Rect r = new Rect( rectInner );
			r = r.Pad( 4 );
			r.height = 20;

			DrawGrabHandle( r );


			Rect tRect = rectInner.Pad( 2 );
			tRect.yMin += 28;

			tRect.width /= 4;
			tRect.height /= 4;
			tRect.height = Mathf.FloorToInt( tRect.height );

			for( int i=0; i < 4; i++ ) {
				UndoableEnterableFloatFieldMtx( tRect, i, 0);
				tRect.x += tRect.width;
				UndoableEnterableFloatFieldMtx( tRect, i, 1 );
				tRect.x += tRect.width;
				UndoableEnterableFloatFieldMtx( tRect, i, 2 );
				tRect.x += tRect.width;
				UndoableEnterableFloatFieldMtx( tRect, i, 3 );
				tRect.x -= tRect.width*3;
				tRect.y += tRect.height;
			}


			GUI.EndGroup();
			ResetWindowColor();

		}


		public void UndoableEnterableFloatFieldMtx(Rect r, int row, int column ) {
			float val = mtx[row,column];
			UndoableEnterableFloatField( r, ref val, "matrix [" + row + "," + column + "]", null );
			mtx[row, column] = val;
		}

		public override string SerializeSpecialData() {
			return mtx.SerializeToCSV();
		}

		public override void DeserializeSpecialData( string key, string value ) {
			mtx = mtx.DeserializeKeyValue( key, value );
		}





	}
}