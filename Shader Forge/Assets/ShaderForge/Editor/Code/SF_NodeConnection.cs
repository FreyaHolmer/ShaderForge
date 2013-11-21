using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;


namespace ShaderForge {

	public enum ConType { cInput, cOutput };
	public enum OutChannel { RGB, R, G, B, A, All, RG };

	public enum EnableState { Enabled, Disabled, Hidden };
	public enum AvailableState { Available, Unavailable };

	public enum LinkingMethod { Default, NoUpdate };

	public enum ValueType { VTvPending, VTv1, VTv2, VTv3, VTv4, VTv1v2, VTv1v3, VTv1v4, TexAsset };

	[System.Serializable]
	public class SF_NodeConnection : ScriptableObject {


		public static SF_NodeConnection pendingConnectionSource = null;
		public AvailableState availableState = AvailableState.Available;
		public EnableState enableState = EnableState.Enabled;
		public bool required = false;

		public ConType conType;
		public OutChannel outputChannel = OutChannel.All;
		public ValueType valueType;
		public ValueType valueTypeDefault;
		public string label;
		public SF_NodeConnection inputCon;
		public List<SF_NodeConnection> outputCons;
		public SF_Node node;
		public bool outerLabel = false;
		public Rect rect;
		public int typecastTarget = 0; // 0 = No typecasting

		public string strID = null;

		[SerializeField]
		private ShaderProgram forcedProgram = ShaderProgram.Any;

		[SerializeField]
		private List<PassType> skipPasses;
		public List<PassType> SkipPasses{
			get{
				if(skipPasses == null)
					skipPasses = new List<PassType>();
				return skipPasses;
			}
		}

		public static Color colorEnabledDefault{
			get{
				if(SF_GUI.ProSkin)
					return new Color( 0.6f, 0.6f, 0.6f );
				else
					return new Color( 1f, 1f, 1f );
			}
		}
		public Color color = colorEnabledDefault;
		public string unconnectedEvaluationValue = null;

		public void OnEnable() {
			base.hideFlags = HideFlags.HideAndDontSave;
		}

		public SF_NodeConnection() {
			//Debug.Log("NODE CONNECTION ");
		}


		public static SF_NodeConnection Create( SF_Node node, string strID, string label, ConType conType, ValueType valueType, bool outerLabel = false, string unconnectedEvaluationValue = null ) {
			return ScriptableObject.CreateInstance< SF_NodeConnection>().Initialize(node, strID, label, conType,valueType, outerLabel, unconnectedEvaluationValue);
		}

		public SF_NodeConnection Initialize( SF_Node node, string strID, string label, ConType conType, ValueType valueType, bool outerLabel = false, string unconnectedEvaluationValue = null ) {
			this.node = node;
			this.strID = strID;
			this.label = label;
			this.conType = conType;
			this.valueType = this.valueTypeDefault = valueType;
			this.outerLabel = outerLabel;
			this.unconnectedEvaluationValue = unconnectedEvaluationValue;
			outputCons = new List<SF_NodeConnection>();
			return this;
		}

		// Chaining
		public SF_NodeConnection SetRequired( bool b ) {
			required = b;
			return this;
		}
		public SF_NodeConnection WithColor( Color c ) {
			color = c;
			return this;
		}
		public SF_NodeConnection Outputting( OutChannel channel ) {
			outputChannel = channel;
			return this;
		}
		public SF_NodeConnection TypecastTo(int target) {
			typecastTarget = target;
			//Debug.Log("Typecasting " + label + " to " + target);
			return this;
		}
		public SF_NodeConnection Skip( params PassType[] passes ) {
			SkipPasses.AddRange( passes );
			return this;
		}
		public SF_NodeConnection ForceBlock(ShaderProgram block) {
			forcedProgram = block;
			return this;
		}




		public string ghostType = null;
		public string ghostLinkStrId = null;
		public SF_NodeConnection SetGhostNodeLink( Type ghostType, string ghostLinkStrId ) {
			this.ghostType = ghostType.FullName;
			this.ghostLinkStrId = ghostLinkStrId;
			return this;
		}


		// Ghost nodes are default values assigned to unconnected node connectors
		// They are instantiated when the shader is being evaluated, and then removed again
		public void DefineGhostIfNeeded(ref List<SF_Node> ghosts) {

			//Debug.Log("Checking ghosts on " + node.nodeName + "["+this.label+"]" );

			// Skip nodes without ghosts
			if( string.IsNullOrEmpty(ghostType) ) {
				//Debug.Log("Ghost type not defined");
				return;
			}
				

			if( IsConnected() ) // Skip already connected ones
				return;


			SF_Node ghost = null;

			// Search for existing ghost node
			foreach( SF_Node exisGhost in ghosts ) {
				if( exisGhost.GetType().FullName == ghostType ) { // TODO: Make sure serialized data matches too!
					// Found!
					ghost = exisGhost;
					break;
				}
			} 
			 
			// If no ghost was found, create one
			if( ghost == null ) {
				ghost = node.editor.AddNode( ghostType );
				ghosts.Add( ghost );
				//Debug.Log( "Adding ghost of type " + ghostType );
				//Debug.Log( "Ghost in main node list = " + node.editor.nodes.Contains( ghost ) );
			}

			// Just to make sure...
			if( ghost == null ) {
				Debug.LogError( "Ghost is null, this should really not happen. Tried to find type " + ghostType );
			}

			// By this point, ghost is surely an existing node!
			// Link it:

			//Debug.Log( "Linking ghost of type " + ghostType + " on " + this.node.nodeName + " Is COnnected = " + IsConnected());
			ghost.status.leadsToFinal = true;
			ghost[ghostLinkStrId].LinkTo(this,LinkingMethod.NoUpdate);

		}

		// Get the index of this connector in the node array
		public string GetIndex() {
			if( this.HasID() )
				return strID;
			for( int i = 0; i < node.connectors.Length; i++ )
				if( node.connectors[i] == this )
					return i.ToString();
			Debug.LogError( "Couldn't find index of a connector in " + node.nodeName );
			return "0";
		}


		public bool HasID() {
			return !string.IsNullOrEmpty( strID );
		}


		public ShaderProgram GetProgram() {
			if( forcedProgram == ShaderProgram.Any )
				return node.program;
			return forcedProgram;
		}


		public int GetCompCount() {

			if( conType == ConType.cInput ) {
				return inputCon.GetCompCount();
			}


			OutChannel oc = outputChannel;
			if( oc == OutChannel.All )
				return node.texture.CompCount;
			if( oc == OutChannel.RGB )
				return 3;
			if( oc == OutChannel.RG )
				return 2;

			return 1;
		}


		

		public string Evaluate() {

			string s = node.PreEvaluate();

			switch( outputChannel ) {
				case OutChannel.RGB:
					return s + ".rgb";
				case OutChannel.RG:
					return s + ".rg";
				case OutChannel.R:
					return s + ".r";
				case OutChannel.G:
					return s + ".g";
				case OutChannel.B:
					return s + ".b";
				case OutChannel.A:
					return s + ".a";
			}

			return s;

		}


		public bool IsConnected() {
			if( conType == ConType.cInput ) {
				return inputCon != null;
			} else {
				if( outputCons == null )
					return false;
				return ( outputCons.Count != 0 );
			}

		}

		public bool IsConnectedAndEnabled() {
			return IsConnected() && enableState == EnableState.Enabled;
		}



		public bool ConnectionInProgress() {
			return ( SF_NodeConnection.pendingConnectionSource == this && IsConnecting() );
		}

		public static bool IsConnecting() {
			if( SF_NodeConnection.pendingConnectionSource == null )
				return false;
			/*else if( string.IsNullOrEmpty( SF_NodeConnection.pendingConnectionSource.node.name ) ) {
				SF_NodeConnection.pendingConnectionSource=null;
				return false;
			}*/

			return true;
		}

		public bool Hovering(bool world) {
			//if( !node.editor.nodeView.rect.Contains( Event.current.mousePosition ) )
			//	return false;
			Rect r = SF_Tools.GetExpanded( rect, SF_Tools.connectorMargin );
			return r.Contains( world ? Event.current.mousePosition : MousePos() );
		}

		public bool Clicked() {


			bool hovering = Hovering(world:false);
			bool click = ( Event.current.type == EventType.mouseDown && Event.current.button == 0 );
			bool clickedCont = hovering && click;
			//bool clickedCont=cont&&click;
			//Debug.Log();
			return clickedCont;
		}

		public bool Released() {
			bool cont = rect.Contains( MousePos() );
			bool release = ( Event.current.type == EventType.mouseUp );
			return cont && release;
		}


		public void OnClick() {
			Debug.Log( "Clicked Button" );
		}

		public Vector2 MousePos() {
			if( node.editor == null )
				return Vector2.zero;
			return node.editor.nodeView.GetNodeSpaceMousePos();
		}

		// TODO: Pass nodes into actual line draw thingy
		public float GetConnectionCenterY( SF_NodeConnection cA, SF_NodeConnection cB ) {
			Rect a = cA.node.rect;
			Rect b = cB.node.rect;
			if( cA.GetConnectionPoint().y > cB.GetConnectionPoint().y )
				return 0.5f * ( a.yMax + b.yMin );
			else
				return 0.5f * ( b.yMax + a.yMin );
		}

		public void CheckConnection( SF_Editor editor ) {



			if( enableState == EnableState.Hidden )
				return;


			

			if( conType == ConType.cInput && inputCon != null ) {
				DrawConnection( editor );
			}

			if( enableState == EnableState.Disabled || availableState == AvailableState.Unavailable )
				return;

			if( Clicked() ) {
				SF_NodeConnection.pendingConnectionSource = this;
				editor.nodeView.selection.DeselectAll();
				Event.current.Use();
			}



			if( !ConnectionInProgress() ) {
				if( Released() )
					TryMakeConnection();
				return;
			}

			node.Repaint();

			if( conType == ConType.cInput )
				GUILines.DrawStyledConnection( editor, GetConnectionPoint(), MousePos(), 1, GetConnectionLineColor() );
			else
				GUILines.DrawStyledConnection( editor, MousePos(), GetConnectionPoint(), GetCompCount(), GetConnectionLineColor() );

			//Drawing.DrawLine(rect.center,MousePos(),Color.white,2,true);


		}

		private Color GetConnectionLineColor() {

			Color def = EditorGUIUtility.isProSkin ? new Color( 1f, 1f, 1f, 0.3f ) : new Color( 0f, 0f, 0f, 0.4f );

			
			

			Color sel = SF_GUI.selectionColor;

			if( inputCon == null )
				return def;
			else if( inputCon.color == SF_Node.colorExposed )
				def = SF_Node.colorExposedDark;

			if( node.selected || inputCon.node.selected )
				return sel;


			if( !DisplayAsValid() )
				def.a = 0.1f;

			return def;
		}

		void DrawConnection( SF_Editor editor ) {


			Vector2 a = GetConnectionPoint();
			Vector2 b = inputCon.GetConnectionPoint();
			int cc = GetCompCount();

			GUILines.DrawStyledConnection( editor, a, b, cc, GetConnectionLineColor() );

		}

		public bool CanEvaluate() {
			if( !IsConnected() )
				if( !string.IsNullOrEmpty( unconnectedEvaluationValue ) )
					return true;
				else
					return false; // TODO: Something
			if(conType == ConType.cInput)
				if( !inputCon.node.CanEvaluate() )
					return false;
			return true;
		}

		public bool CanEvaluateAs( int target ) {
			if( !CanEvaluate() )
				return false;
			//int source = inputCon.GetCompCount();
			//if( target < source ) // TODO: Allow this?
			//	return false;
			return true;
		}


		public string TryEvaluate() {
			
			//Debug.Log("TryEvaluate " + label + " typecast = " + typecastTarget);
			
			if( !IsConnected() )
				if( !string.IsNullOrEmpty( unconnectedEvaluationValue ) )
					return unconnectedEvaluationValue;
			if( !CanEvaluate() )
				return null;

			if( typecastTarget == 0 ){
				if(conType == ConType.cInput)
					return inputCon.Evaluate();
				else
					return Evaluate();
			} else {
				//Debug.Log("Trying to evaluate node " + this.label + " on node " + this.node.nodeName);
				return TryEvaluateAs( typecastTarget );
			}
				
		}

		public string TryEvaluateAs(int target) {
			if( !CanEvaluateAs(target) )
				return null; // TODO: Throw errors etc

			int source = inputCon.GetCompCount();

			int diff = target - source;



			if(diff == 0) // Same value type
				return inputCon.Evaluate();
			if( diff < 0 ) { // Lowering component count, mask components
				switch( target ) {
					case 1:
						return inputCon.Evaluate() + ".r";
					case 2:
						return inputCon.Evaluate() + ".rg";
					case 3:
						return inputCon.Evaluate() + ".rgb";
				}
			}

			// Increasing component count:

			string cast;


			if( source != 1 ) { // Evaluate + append zeroes
				cast = "float" + target + "(" + inputCon.Evaluate();
				for( int i = 0; i < diff; i++ )
					cast += ",0.0";
			} else {
				inputCon.node.DefineVariable();
				cast = "float" + target + "(" + inputCon.Evaluate();
				for( int i = 0; i < diff; i++ )
					cast += "," + inputCon.Evaluate();
			}

			

			cast += ")";
			return cast;
		}


		/*
		public bool IsFocused() {
			return rect.Contains( Event.current.mousePosition );
		}*/

		public bool CheckIfDeleted() {
			if( (Event.current.keyCode == KeyCode.Delete || Event.current.keyCode == KeyCode.Backspace) && Event.current.type == EventType.keyDown && Hovering( world: true ) ) {
				Disconnect();
				return true;
			}
			return false;
		}




		public bool IsDeleted() {
			return ( node == null );
		}

		public void Disconnect( bool force = false, bool callback = true ) {

			//Debug.Log( "Attempt to disconnect: " + node.name + "[" + label + "]" );

			if( !IsConnected() ) {
				//Debug.Log( "Aborted " + node.name + "[" + label + "]" );
				return;
			}


			if( conType == ConType.cInput ) {
				//Debug.Log( "Input disconnecting " + node.name + "[" + label + "]" );
				ResetValueType();
				if( inputCon != null ) {
					inputCon.outputCons.Remove( this );
					//Debug.Log( "Disconnecting " + label + "<--" + inputCon.label );
				}
				inputCon = null;
				if( callback )
					node.OnUpdateNode();
			} else {
				//Debug.Log( "Output disconnecting " + node.name + "[" + label + "]" );
				SF_NodeConnection[] outputsArr = outputCons.ToArray();
				for( int i = 0; i < outputsArr.Length; i++ ) {
					//Debug.Log( "Disconnecting " + outputsArr[i].label + "<--" + label );
					outputsArr[i].Disconnect( true, callback );
				}
				outputCons.Clear();
			}

			//		AceMatEditor.instance.CheckForBrokenConnections();

			//		node = null; // What?
		}

		public Vector2 GetConnectionPoint() {
			if( conType == ConType.cInput )
				return new Vector2( rect.xMax, rect.center.y );
			else
				return new Vector2( rect.xMin+1, rect.center.y );
		}


		public bool CanConnectTo(SF_NodeConnection other) {
			if( other == null )
				return false;

			if( other.node == node )
				return false; // Disallow connecting to self

			if( other.conType == this.conType )
				return false; // Disallow connecting same types (i <- i & o <- o)

			return true;
		}

		public bool CanValidlyConnectTo(SF_NodeConnection other) {
			if(!CanConnectTo(other))
				return false;

			if(this.conType == ConType.cInput)
				return SFNCG_Arithmetic.CompatibleTypes( this.valueTypeDefault, other.valueType );
			else
				return SFNCG_Arithmetic.CompatibleTypes( other.valueTypeDefault, this.valueType );


		}


		public void TryMakeConnection() {

			if( !CanConnectTo( SF_NodeConnection.pendingConnectionSource ) ) {
				return; // TODO: Display message
			}

			LinkTo( SF_NodeConnection.pendingConnectionSource );

			SF_NodeConnection.pendingConnectionSource = null;
		}

		public void ThrowLinkError() {
			Debug.LogError( "Attempt to connect invalid types" );
		}




		public void ResetValueType() {
			//Debug.Log("Resetting value type on " + this.label);
			valueType = valueTypeDefault;
		}

		public void LinkTo( SF_NodeConnection other, LinkingMethod linkMethod = LinkingMethod.Default ) {

			if( this.conType == other.conType ) {
				Debug.Log("Invalid IO linking: " + other.node.nodeName);
				return;
			}

			if( conType == ConType.cInput ) {
				other.LinkTo( this, linkMethod ); // Reverse connection if dragged other way
				return;
			}

			// Other is the input node
			// [other] <---- [this]

			// Verify, if default. Not if it's without update
			if( linkMethod == LinkingMethod.Default ) {

				if( !SFNCG_Arithmetic.CompatibleTypes( other.valueTypeDefault, this.valueType ) ) {
					Debug.LogError( "Incompatible types: Type A: " + other.valueTypeDefault + " Type B: " + this.valueType );
					//ThrowLinkError();
					//other.ResetValueType();
					return;
				}

				// In case there's an existing one
				if( other.IsConnected() )
					other.Disconnect();

			}

			//Debug.Log("Linking " + other.node.nodeName + "["+other.label+"] <--- ["+ this.label +"]" + this.node.nodeName );

			// Connect
			other.valueType = this.valueType;
			other.inputCon = this;

			// TODO: Force types in connector group!
			//if( linkMethod == LinkingMethod.Default ) {
				if( other.node.conGroup != null )
					other.node.conGroup.Refresh();
			//}


			this.outputCons.Add( other );

			if( linkMethod == LinkingMethod.Default ) {
				node.RefreshValue();// OnUpdateNode( NodeUpdateType.Soft, false ); // Update this value
				other.node.OnUpdateNode(); // Update other, and following
				
			}
			
		}

		public Color GetConnectorColorRGB() {
			if( enableState != EnableState.Enabled )
				return Color.gray;

			//if( IsConnected() ) // DEBUG
			//	return Color.yellow;

			Color unselected = color;

			if( node.selected )
				return SF_GUI.selectionColor;//Color.Lerp(unselected, SF_GUI.selectionColor, 0.75f);
			return unselected;
		}

		public Color GetConnectorColor() {
			Color c = GetConnectorColorRGB();
			if( DisplayAsValid() )
				c.a = SF_GUI.ProSkin ? 1f : 0.5f;
			else
				c.a = SF_GUI.ProSkin ? 0.25f : 0.125f;
			return c;
		}


		public bool DisplayAsValid() {
			return enableState == EnableState.Enabled && availableState == AvailableState.Available && !UnconnectableToPending();
		}


		public bool UnconnectableToPending() {
			if( SF_NodeConnection.pendingConnectionSource != null ) {
				if( SF_NodeConnection.pendingConnectionSource != this )
					if( !CanValidlyConnectTo( SF_NodeConnection.pendingConnectionSource ) )
						return true;
			}
			return false;
		}


		public void Draw( Vector2 pos ) {

			if( enableState == EnableState.Hidden )
				return;


			// Don't draw if invalid
			
				

			rect = new Rect( pos.x, pos.y, 25, 14 );

			if( conType == ConType.cOutput ) {
				rect.xMin -= node.extraWidthOutput;
			} else {
				rect.width += node.extraWidthInput;
			}


			/*if( IsConnected() && conType == ConType.cOutput ) {
				Rect tRect = new Rect(rect);
				tRect.width = 100;
				tRect.x -= 200;
				GUI.Label( tRect, outputCons[0].node.nodeName );
			}*/
				


			if( conType == ConType.cOutput ) {
				rect.x -= node.rect.width + rect.width - node.extraWidthOutput;
			}






			
			//GUIStyle cStyle = conType == ConType.cInput ? EditorStyles.miniButtonRight : EditorStyles.miniButtonLeft;
			//GUIStyle cStyle = (GUIStyle)"ShurikenModuleTitle";
			
			GUI.color = GetConnectorColor();
			GUI.Box( rect, string.Empty );
			if( SF_GUI.ProSkin ) {
				GUI.Box( rect, string.Empty );
				GUI.Box( rect, string.Empty );
				GUI.Box( rect, string.Empty );
			}

			GUI.color = DisplayAsValid() ? Color.white : Color.grey;

			if( HasErrors() ) {
				Rect iconRect = new Rect( rect );
				iconRect.x += iconRect.width;
				iconRect.height = iconRect.width = 16;
				iconRect.y -= 1;
				GUI.DrawTexture( iconRect, SF_Styles.IconErrorSmall );
			}

			

			Rect labelRect = rect;


			if( SF_Node.DEBUG ) {
				Rect typeRect = rect;
				typeRect.width *= 3f;

				if( conType == ConType.cInput ) {
					GUI.skin.label.alignment = TextAnchor.MiddleLeft;
					typeRect.x += rect.width;
				} else {
					GUI.skin.label.alignment = TextAnchor.MiddleRight;
					typeRect.x -= typeRect.width;
				}

				GUI.Label( typeRect, valueType.ToString() );
			}

			if( outerLabel ) {
				labelRect.width = node.rect.width;
				labelRect.x -= EditorStyles.miniLabel.CalcSize( new GUIContent( label ) ).x + 4;
			}

			GUI.Label( labelRect, label, SF_Styles.MiniLabelOverflow );
			
			CheckIfDeleted();

			GUI.color = Color.white;
		}


		public void SetAvailable( bool b ) {
			availableState = b ? AvailableState.Available : AvailableState.Unavailable;
		}

		public bool HasErrors() {
			if( required && !IsConnected() ) {
				return true;
			}
			return false;
		}






	}
}