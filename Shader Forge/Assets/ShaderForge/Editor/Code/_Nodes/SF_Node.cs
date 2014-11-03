using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System;


namespace ShaderForge {

	public enum NodeUpdateType { Soft, Hard };

	[System.Serializable]
	public class SF_Node : ScriptableObject, IDependable<SF_Node> {

		public const int NODE_SIZE = 96;
		public const int NODE_WIDTH = NODE_SIZE + 3;	// This fits a NODE_SIZE texture inside
		public const int NODE_HEIGHT = NODE_SIZE + 16;	// This fits a NODE_SIZE texture inside

		public int node_width = NODE_WIDTH;
		public int node_height = NODE_HEIGHT;

		public int depth = 0; // Used when deserializing and updating

		public string _variableName;
		public string variableName{
			get{
				if(string.IsNullOrEmpty(_variableName) && GUI.GetNameOfFocusedControl() != VarNameControl() ){
					_variableName = "node_" + id;
				}
				return _variableName;
			}
			set{
				_variableName = value;
				if(IsProperty() && property.overrideInternalName){
					property.UpdateInternalName();
				}
				SF_Tools.FormatSerializableVarName(ref _variableName);
			}
		}

		public bool canAlwaysSetPrecision = false;
		public bool isFloatPrecisionBasedVariable = true;
		public bool lockedVariableName = false;
		public FloatPrecision precision = FloatPrecision.Float;

		string[] _precisionLabels;
		public string[] precisionLabels{
			get{
				if(_precisionLabels == null){
					_precisionLabels = FloatPrecision.Float.DisplayStrings();
				}
				return _precisionLabels;
			}
		}

//		string[] _precisionLabelsSimple;
//		public string[] precisionLabelsSimple{
//			get{
//				if(_precisionLabelsSimple == null){
//					_precisionLabelsSimple = new string[3];
//					for(int i=0;i<3;i++){
//						_precisionLabelsSimple[i] = ((FloatPrecision)i).ToString().ToLower();
//					}
//				}
//				return _precisionLabelsSimple;
//			}
//		}

		public bool isGhost = false;

		public bool selected = false;

		public bool discreteTitle = false;

		public bool varDefined = false; // Whether or not this node has had its variable defined already.
		public bool varPreDefined = false; // Whether or not this variable has done its predefs
		public bool alwaysDefineVariable = false;
		public bool neverDefineVariable = false;
		public bool onlyPreDefine = false;	// If it should only do the pre-define, and skip the regular variable or not (Used in branching)
		public bool availableInDeferredPrePass = true;

		public static Color colorExposed = new Color( 0.8f, 1f, 0.9f );
		public static Color colorExposedDim = new Color( 0.8f, 1f, 0.9f )*0.8f;
		public static Color colorExposedDark = new Color( 0.24f, 0.32f, 0.30f ) * 1.25f;
		public static Color colorExposedDarker = new Color( 0.24f, 0.32f, 0.30f ) * 0.75f;

		public static Color colorGlobal = new Color( 1f, 0.8f, 0.7f); // ( 1f, 0.9f, 0.8f);

		public void UndoRecord(string undoMsg, UpToDateState tempOutdatedState = UpToDateState.OutdatedHard){
			SetDirty(tempOutdatedState); // This will only be in the restored undo state
			Undo.RecordObject(this,undoMsg);
			if(texture != null)
				Undo.RecordObject(texture, undoMsg);
			if(property != null)
				Undo.RecordObject(property, undoMsg);
			if(status != null)
				Undo.RecordObject(status, undoMsg);
			foreach(SF_NodeConnector con in connectors){
				Undo.RecordObject(con, undoMsg);
			}
			SetDirty(UpToDateState.UpToDate); // Might need to comment this for Redo to work, it seems
		}
		

		public Color colorDefault{
			get{
				if(SF_GUI.ProSkin)
					return new Color( 0.8f, 0.8f, 0.8f);
				else
					return new Color( 1f, 1f, 1f );
			}
		}

		public bool showColor;
		public Color displayColor = Color.black;

		public SF_ShaderProperty property = null;

		public SF_NodeStatus status;
		
		public SF_Editor editor;

		public ShaderProgram program = ShaderProgram.Any;

		// User typed comment
		public string comment = "";
		//public bool hasComment;

		public bool showLowerPropertyBox;
		public bool showLowerPropertyBoxAlways;
		public bool showLowerReadonlyValues;
		public bool initialized = false;


		//public int depth = 0; // Used to sort variable initialization

		// public static bool DEBUG = false;


		public SF_NodePreview texture;
		//	public float[] vector;



		public int id;

		public string nodeName;
		private string nodeNameSearch;
		public string SearchName{
			get{
				if(string.IsNullOrEmpty(nodeNameSearch)){
					return nodeName;
				} else {
					return nodeNameSearch;
				}
			}
			set{
				nodeNameSearch = value;
			}
		}

		public Rect rect;
		public Rect rectInner;
		public Rect lowerRect;

		[SerializeField]
		public SF_NodeConnector[] connectors;

		public SF_NodeConnectionGroup conGroup;

		public float extraWidthOutput = 0f;
		public float extraWidthInput = 0f;

		public SF_Node() {
			//Debug.Log("NODE " + GetType());
		}

		// Quick retrieval of connectors
		public SF_NodeConnector this[string s] {
			get {
				return GetConnectorByStringID(s);
			}
		}

		public void OnEnable() {
			base.hideFlags = HideFlags.HideAndDontSave;
		}

		#region IDependable implementation

		private List<SF_Node> dependencies;
		public int iDepth = 0;
		void IDependable<SF_Node>.AddDependency (SF_Node dp){
			(this as IDependable<SF_Node>).Dependencies.Add(dp);
		}

		int IDependable<SF_Node>.Depth {
			get {
				return iDepth;
			}
			set {
				iDepth = value;
			}
		}

		List<SF_Node> IDependable<SF_Node>.Dependencies {
			get {
				if(dependencies == null){
					dependencies = new List<SF_Node>();
				}
				return dependencies;
			}
			set {
				dependencies = value;
			}
		}


		public void ReadDependencies(){
			(this as IDependable<SF_Node>).Dependencies.Clear();
			foreach(SF_NodeConnector c in connectors){
				if(c.conType == ConType.cOutput)
					continue;
				if(!c.IsConnectedAndEnabled())
					continue;
				if(c.inputCon == null)
					continue;
				(this as IDependable<SF_Node>).AddDependency(c.inputCon.node);
			}
		}


		#endregion

		public bool IsProperty() {
			if( property == null )
				return false;
			if( string.IsNullOrEmpty( property.nameType ) ) {
				property = null;
				return false;
			}
			return true;
		}

		public bool IsGlobalProperty(){
			return IsProperty() ? property.global : false;
		}


		// TODO: Matrices & Samplers?
		// TODO: Precision
		public string GetVariableType() {

			int cc = GetEvaluatedComponentCount();
			if(cc == 0)
				cc = texture.CompCount;


			string precisionStr = precision.ToCode();


			if(cc == 1)
				return precisionStr;
			return precisionStr + cc;

			//if( texture.CompCount == 1 )
			//	return "float";
			//return "float" + texture.CompCount;
		}

		public string GetVariableName(bool createIfNull = true) {
			if(IsProperty()){
				if(ShouldDefineVariable() && !neverDefineVariable)
					return property.nameInternal + "_var";
				else if( neverDefineVariable)
					return property.nameInternal;
			}
			if(createIfNull && string.IsNullOrEmpty(variableName))
				variableName = "node_" + id;
			return variableName;
		}

		public virtual void Initialize() {
			// Override
		}


		public SF_NodeConnector[] ConnectedInputs{
			get{
				return connectors.Where(con=>con.IsConnectedAndEnabled() && con.conType == ConType.cInput).Select(con=>con).ToArray();
			}
		}


		// Used for 3D data like normal/view vector, etc.
		public bool vectorDataNode = false;
		public bool displayVectorDataMask = false;

		public void UpdateDisplayVectorDataMask(){
			displayVectorDataMask = CheckIfShouldDisplayVectorDataMask();
		}

		public bool CheckIfShouldDisplayVectorDataMask(){
			if(vectorDataNode){
				return true;
			} else {
				bool disp = false;
				foreach(SF_NodeConnector con in ConnectedInputs){
					if(con.inputCon.node.displayVectorDataMask){
						disp = true;
						break;
					}
				}
				return disp;
			}
		}


		public void Initialize( string name, bool vectorDataTexture = false) {
			editor = SF_Editor.instance; // TODO, pass in a better way
			status = ScriptableObject.CreateInstance<SF_NodeStatus>().Initialize(this);
			Vector2 pos = editor.mousePosition; // TODO: check where to spawn first
			AssignID();
			this.nodeName = name;
			if( SF_Debug.nodes )
				this.nodeName = ( "[" + id + "] " + this.nodeName );
			texture = ScriptableObject.CreateInstance<SF_NodePreview>().Initialize( this );
			texture.Fill( Color.black );


			// Try to find icon

			texture.LoadAndInitializeIcons(this.GetType()); 

			if(vectorDataTexture){
				vectorDataNode = true;
				displayVectorDataMask = true;
				texture.LoadDataTexture(this.GetType());
			}


			pos = editor.nodeView.ScreenSpaceToZoomSpace( pos );
			InitializeDefaultRect( pos );
		}

		public void AssignID() {
			this.id = ( editor.idIncrement++ );
		}

		public virtual void OnPreGetPreviewData() {
			// Override
		}

		public virtual string GetPrepareUniformsAndFunctions(){
			return string.Empty; // Override
		}
		

		public virtual void Update() {
			// Override
		}

		public void InitializeDefaultRect( Vector2 pos ) {


			this.rect = new Rect(
				pos.x - node_width / 2,
				pos.y - node_height / 2,
				node_width,
				( showLowerPropertyBox ? ( node_height ) : ( node_height + 20 ) ) ); // TODO: This seems a bit reversed...
			rectInner = rect;
			rectInner.x = 1;
			rectInner.y = 15;
			rectInner.width = node_width - 3;
			rectInner.height = node_height - 16;

			lowerRect = rectInner;
			lowerRect.y += rectInner.height;
			lowerRect.height = 20;


		}
















		public void UndoableToggle(Rect r, ref bool boolVar, string label, string undoActionName, GUIStyle style){
			if(style == null)
				style = EditorStyles.toggle;
			bool newValue = GUI.Toggle(r,boolVar,label, style);
			if(newValue != boolVar){
				UndoRecord((newValue ? "enable" : "disable") + " " + undoActionName);
				boolVar = newValue;
			}
		}


		public Enum UndoableEnumPopup(Rect r, Enum enumValue, string undoPrefix){
			Enum nextEnum = EditorGUI.EnumPopup( r, enumValue );
			if(nextEnum.ToString() != enumValue.ToString()){
				string undoName = undoPrefix + " to " + nextEnum;
				UndoRecord(undoName);
				enumValue = nextEnum;
			}
			return enumValue;
		}

		public Enum UndoableLabeledEnumPopup(Rect r, string label, Enum enumValue, string undoPrefix){
			Enum nextEnum = SF_GUI.LabeledEnumField( r, label, enumValue, EditorStyles.miniLabel );
			if(nextEnum.ToString() != enumValue.ToString()){
				UndoRecord(undoPrefix + " to " + nextEnum);
				Undo.IncrementCurrentGroup();
				enumValue = nextEnum;
			}
			return enumValue;
		}


		public int UndoablePopup(Rect r, int selected, string[] displayedOptions, string undoPrefix, GUIStyle style = null){
			if(style == null)
				style = EditorStyles.popup;
			int pickedID = EditorGUI.Popup( r, selected, displayedOptions,style);
			if(pickedID != selected){
				UndoRecord(undoPrefix + " to " + displayedOptions[pickedID]);
				selected = pickedID;
			}
			return selected;
		}

		//EditorGUI.FloatField( r, texture.dataUniform[0], SF_Styles.LargeTextField );

		public float UndoableFloatField(Rect r, float value, string undoInfix, GUIStyle style = null){
			if(style == null)
				style = EditorStyles.textField;
			float newValue = EditorGUI.FloatField( r, value, style );
			if(newValue != value){
				if(IsProperty() || IsGlobalProperty()){
					UndoRecord("set " + undoInfix + " of " + (IsGlobalProperty() ? property.nameInternal : property.nameDisplay));
				} else {
					UndoRecord("set " + undoInfix + " of " + nodeName + " node");
				}
				return newValue;
			}
			return value;
		}

		// (r, ref texture.dataUniform.r, "value", SF_Styles.LargeTextField);

		public void UndoableEnterableFloatField(Rect r, ref float value, string undoInfix, GUIStyle style){
			if(style == null)
				style = EditorStyles.textField;
			float previousValue = value;
			SF_GUI.EnterableFloatField(this, r, ref value, style );
			float newValue = value;
			if(previousValue != value){
				value = previousValue;

				UndoRecord("set " + undoInfix + " of " + nodeName + " node");
				value = newValue;
			}
		}

		public float UndoableHorizontalSlider(Rect r, float value, float min, float max, string undoInfix){
			float newValue = GUI.HorizontalSlider( r, value, min, max );
			if(newValue != value){
				if(IsProperty() || IsGlobalProperty()){
					UndoRecord("set " + undoInfix + " of " + (IsGlobalProperty() ? property.nameInternal : property.nameDisplay));
				} else {
					UndoRecord("set " + undoInfix + " of " + nodeName + " node");
				}
				return newValue;
			}
			return value;
		}

		// code = GUI.TextArea(txtRect,code,SF_Styles.CodeTextArea);
		public string UndoableTextArea(Rect r, string value, string undoInfix, GUIStyle style){
			string newValue = EditorGUI.TextArea( r, value, style );
			if(newValue != value){
				if(this is SFN_Code){
					UndoRecord("edit " + undoInfix + " of " + (this as SFN_Code).functionName);
				} else if(IsProperty() || IsGlobalProperty()){
					UndoRecord("edit " + undoInfix + " of " + (IsGlobalProperty() ? property.nameInternal : property.nameDisplay));
				} else {
					UndoRecord("edit " + undoInfix + " of " + nodeName + " node");
				}
				
				return newValue;
			}
			return value;
		}

		public string UndoableTextField(Rect r, string value, string undoInfix, GUIStyle style, bool readPropertyName = true){
			if(style == null)
				style = EditorStyles.textField;
			string newValue = GUI.TextField( r, value, style );
			if(newValue != value){
				if(this is SFN_Code && readPropertyName){
					UndoRecord("edit " + undoInfix + " of " + (this as SFN_Code).functionName);
				} else if( ( IsProperty() || IsGlobalProperty() ) && readPropertyName){
					UndoRecord("edit " + undoInfix + " of " + (IsGlobalProperty() ? property.nameInternal : property.nameDisplay));
				} else {
					UndoRecord("edit " + undoInfix + " of " + nodeName + " node");
				}
				
				return newValue;
			}
			return value;
		}

		public Color UndoableColorField(Rect r, Color color, string undoMsg){
			Color newColor = EditorGUI.ColorField( r, color );
			if(newColor != color){
				UndoRecord(undoMsg);
				return newColor;
			}
			return color;
		}



		// UndoableTextField



		/*
		public int UndoableEnterableTextField(Rect r, ref string str, ){

			SF_GUI.EnterableTextField(this, r,

		}*/















		//	public virtual void OnConnectedNode(){
		//		Debug.Log("OnConnectedNode " + name);
		//	}

		public virtual void OnUpdateNode( NodeUpdateType updType = NodeUpdateType.Hard, bool cascade = true ) {
			//Debug.Log("Updating " + name);

			

			if( conGroup != null )
				conGroup.Refresh();

			if( !InputsConnected() ) {
				//Debug.Log("Detected missing input on obj " + name);
				texture.OnLostConnection();
			}

			RefreshValue(); // Refresh this value

			if( IsProperty() )
				editor.shaderEvaluator.ApplyProperty(this);

			if( cascade )
				if( connectors != null && connectors.Length > 0 )
					foreach( SF_NodeConnector mCon in connectors ) {
						if( mCon == null )
							continue;
						if( mCon.conType == ConType.cOutput ) {
							for (int i = 0; i < mCon.outputCons.Count; i++) {
								SF_NodeConnector mConOut = mCon.outputCons [i];
								mConOut.node.OnUpdateNode (updType);
								// TODO Null ref
							}
						}
					}

			UpdateDisplayVectorDataMask();

			editor.OnShaderModified( NodeUpdateType.Soft );
			if(!SF_Parser.quickLoad && !isGhost)
				Repaint();

		}

		public void ChainAppendIfConnected(ref string evalStr, string op, params string[] cons ){
			foreach(string con in cons){
				if(GetInputIsConnected(con)){
					evalStr += op + GetConnectorByStringID(con).TryEvaluate();
				}
			}
		}


		public void SetExtensionConnectorChain(params string[] cNames){
			
			SF_NodeConnector con = GetConnectorByStringID(cNames[0]);
			for(int i=1;i<cNames.Length;i++){
				SF_NodeConnector conNew = GetConnectorByStringID(cNames[i]);
				con.SetVisChild(conNew);
				con = conNew;
			}
			
		}


		public float[] VectorCopy( float[] original ) {
			float[] retVec = new float[original.Length];
			for( int i = 0; i < original.Length; i++ ) {
				retVec[i] = original[i];
			}
			return retVec;
		}

		public bool VectorEqual( float[] a, float[] b ) {
			if( a.Length != b.Length )
				return false;
			for( int i = 0; i < a.Length; i++ )
				if( a[i] != b[i] )
					return false;
			return true;
		}



		public bool InputsConnected() {
			foreach( SF_NodeConnector con in connectors ) {
				if( con == null )
					break;
				if( con.conType == ConType.cInput )
					if( !con.IsConnected() && con.required )
						return false;
			}
			return true;
		}

		public bool GetInputIsConnected( string id ) {
			SF_NodeConnector con = GetConnectorByStringID(id);
			if( con == null ) {
				Debug.LogError("Tried to find invalid connector by string ID [" + id + "] in node [" + this.GetType().ToString() + "]");
				return false;
			}
			if( con.IsConnected() )
				return true;
			return false;
		}

		public virtual Color NodeOperator( int x, int y ) {
			return new Color(
				NodeOperator(x,y,0),
				NodeOperator(x,y,1),
				NodeOperator(x,y,2),
				NodeOperator(x,y,3)
			);
		}

		public virtual float NodeOperator( int x, int y, int c ) {
			return 0f; // Override this
		}

		public virtual void RefreshValue() {
			// Override this
		}

		public void RefreshValue( int ia, int ib ) {

			//Debug.Log("Refreshing value of " + name);

			if( connectors == null ) {
				Debug.LogError( "Refreshing node with null connector list on " + this.nodeName );
				return;
			} else if( connectors.Length == 0 ) {
				Debug.LogError( "Refreshing node without connectors on " + this.nodeName );
				return;
			} else if( connectors[0] == null ) {
				Debug.LogError( "Refreshing node with null connectors " + this.nodeName );
				return;
			}

			foreach( SF_NodeConnector nc in connectors ) {
				if( nc.required && nc.conType == ConType.cInput && !nc.IsConnected() ) { // Check only required inputs
					texture.OnLostConnection();
					return;
				}
			}

		
			
			texture.Combine();
			if(!SF_Parser.quickLoad && !isGhost)
				SF_Editor.instance.Repaint();
		}

		public virtual bool IsUniformOutput() {
			return false; // Override
		}


		public void PrepareEvaluation() {
			if(IsProperty()){
				editor.shaderEvaluator.ApplyProperty( this );
			}
			varDefined = false;
			varPreDefined = false;
		}

		public float GetInputData( string id, int x, int y, int c ) {

			SF_NodeConnector con = GetConnectorByStringID(id);

			if(!con.IsConnected()){
				Debug.LogError("GetInputData on " + nodeName + " with unconnected input " + con.strID);
				return 0f;
			}

			SF_NodeConnector inCon = con.inputCon;


			switch( inCon.outputChannel ) {
				case OutChannel.R:
					c = 0;
					break;
				case OutChannel.G:
					c = 1;
					break;
				case OutChannel.B:
					c = 2;
					break;
				case OutChannel.A:
					c = 3;
					break;
			}

			int cc; //GetEvaluatedComponentCount();
			if(SF_Tools.CompCountOf(con.valueType, out cc)){
				if(cc > 1){
					if(c > cc-1){
						return 0f;
					}
				}
			}



			//return GetInputData( id, x, y, c );
			return GetInputData( id ) [ x, y, c ];
		}

		/*
		public SF_NodePreview GetInputData( int id ) {

			if( connectors[id].inputCon == null ) {
				Debug.LogWarning( "Attempt to find input node of connector " + id + " of " + this.nodeName );
			}

			return connectors[id].inputCon.node.texture;
		}*/

		public SF_NodePreview GetInputData( string id ) {

			SF_NodeConnector con = GetConnectorByStringID(id);
			//SF_Node n; // TODO: What was this? Quite recent too. Define and undefine ghosts?

			if( con.inputCon == null ) {

				List<SF_Node> tmpGhosts = new List<SF_Node>();
				con.DefineGhostIfNeeded(ref tmpGhosts);
				//n = tmpGhosts[0];
				tmpGhosts = null;

				Debug.LogWarning( "Attempt to find input node of connector " + id + " of " + this.nodeName );
			}

			//SF_NodePreview ret = con.inputCon.node.texture;




			return con.inputCon.node.texture;
		}

		/*
		public SF_NodeConnection GetInputCon( int id ) {
			if( connectors[id] == null ) {
				Debug.LogError("Failed attempt to find connector [" + id + "] in " + this.nodeName);
				return null;
			}
			if( connectors[id].inputCon == null ) {
				Debug.LogError( "Failed attempt to find node of connector [" + id + "] on " + this.nodeName );
				return null;
			}
			return connectors[id].inputCon;
		}*/

		public SF_NodeConnector GetInputCon( string id ) {
			SF_NodeConnector con = GetConnectorByStringID( id );
			
			if( con == null ) {
				Debug.LogError( "Failed attempt to find connector [" + id + "] in " + this.nodeName );
				return null;
			}
			if(con.inputCon == null) {
				Debug.LogError( "Failed attempt to find input connector of [" + id + "] in " + this.nodeName );
				return null;
			}
			return con.inputCon;
		}


		public float BoundsTop(){

			float top = rect.yMin;

			if(this.IsProperty())
				top -= 20;
			if(HasComment())
				top -= 20;

			return top;
		}

		public float BoundsBottom(){
			return rect.yMax;
		}


		public virtual int GetEvaluatedComponentCount() {
			// Override
			return 0;
		}

		public bool CanEvaluate() {
			//Debug.Log("Checking if can evaluate " + nodeName);
			for( int i = 0; i < connectors.Length; i++ ) {
				if( connectors[i].required )
					if( !connectors[i].IsConnected() )
						return false;
			}
			return true;
		}


		public void CheckForBrokenConnections() {
			foreach( SF_NodeConnector con in connectors ) {
				if( con.IsConnected() && con.conType == ConType.cInput ) {
					if( con.inputCon.IsDeleted() )
						con.inputCon = null;
				}

			}
		}

		//	public MaterialNode MakeDotProductNode(){
		//		connectors = new MaterialNodeConnector[3]{
		//			new MaterialNodeConnector(this,"A",ConType.cInput),
		//			new MaterialNodeConnector(this,"B",ConType.cInput),
		//			new MaterialNodeConnector(this,"Out",ConType.cOutput)
		//		};
		//		return this;
		//	}


		public void DrawConnections() {
			foreach( SF_NodeConnector con in connectors )
				con.CheckConnection( editor );
		}

		public void Repaint() {
			//SF_Editor.instance.Repaint();
		}

		public bool IsFocused() {
			return rect.Contains( Event.current.mousePosition );
		}

		/*
		public bool CheckIfDeleted() {

			if( Event.current.keyCode == KeyCode.Delete && Event.current.type == EventType.keyDown && selected ) {
				Delete(true,"delete " + nodeName);
				return true;
			}
			return false;

		}*/

		public void PrepareWindowColor() {

			if(IsProperty()){
				if(property.global){
					GUI.color = colorGlobal;
				} else {
					GUI.color = colorExposed; // colorExposed
				}
			} else {
				GUI.color = colorDefault;
			}
		}

		public void ResetWindowColor() {
			GUI.color = colorDefault;
		}


		public void OnPress(){
			if( MouseOverNode( world: true ) && Event.current.isMouse ) {
				editor.ResetRunningOutdatedTimer();
				if( !selected && !SF_GUI.MultiSelectModifierHeld() )
					editor.nodeView.selection.DeselectAll(registerUndo:true);
				
				StartDragging();
				
				//if(!selected)
				Event.current.Use();
				//Select();
			}
				
		}

		public void OnRelease() {


			if(isDragging){
				isDragging = false;
				Vector2 tmp = new Vector2(rect.x, rect.y);
				rect.x = dragStart.x;
				rect.y = dragStart.y;
				UndoRecord("move " + nodeName + " node");
				rect.x = tmp.x;
				rect.y = tmp.y;

			}
			//isDragging = false;

			if( SF_NodeConnector.pendingConnectionSource != null )
				return;

			bool hover = MouseOverNode( world: true );
			bool stationary = dragDelta.sqrMagnitude < SF_Tools.stationaryCursorRadius;
			bool placingNew = editor.nodeBrowser.IsPlacing();
			
			if( hover && stationary && !placingNew ) { // If you released on the node without dragging
				if( SF_GUI.MultiSelectModifierHeld() ) {
					if( selected )
						Deselect(registerUndo:true);
					else
						Select(registerUndo:true);
					Event.current.Use();
				} else if(!selected) {
					editor.nodeView.selection.DeselectAll(registerUndo:true);
					Select(registerUndo:true);
					Event.current.Use();
				}
			}

		}


		public bool isDragging = false;
		bool isEditingNodeTextField = false;
		public static bool isEditingAnyNodeTextField = false;

		public void ContextClick( object o ) {
			string picked = o as string;
			switch(picked){
			case "prop_global_toggle":
				property.ToggleGlobal();
				break;
			case "doc_open":
				SF_Web.OpenDocumentationForNode(this);
				break;
			case "cmt_edit":
				editor.Defocus(deselectNodes:true);
				GUI.FocusControl("node_comment_" + id);
				isEditingNodeTextField = true;
				SF_Node.isEditingAnyNodeTextField = true;
				break;
			}
		}

		public bool HasComment(){
			return !string.IsNullOrEmpty(comment);
		}

		public bool UnavailableInThisRenderPath(){
			return editor.ps.catLighting.renderPath == SFPSC_Lighting.RenderPath.DeferredPrePass && !availableInDeferredPrePass;
		}

		float commentYposTarget;
		float commentYposCurrent;


		public void DrawWindow() {


			

			//Vector2 prev = new Vector2( rect.x, rect.y );
			//int prevCont = GUIUtility.hotControl;

			if(Event.current.type == EventType.repaint){
				commentYposCurrent = Mathf.Lerp(commentYposCurrent, commentYposTarget, 0.4f);
			}


			if(UnavailableInThisRenderPath())
				GUI.color = new Color(1f,1f,1f,0.5f);
			GUI.Box( rect, nodeName, discreteTitle ? SF_Styles.NodeStyleDiscrete : SF_Styles.NodeStyle );


			// Draw lock
			if(UnavailableInThisRenderPath()){
				SF_GUI.DrawLock(rect.PadTop(3), "This node is only available in forward rendering");
			}


			if(!UnavailableInThisRenderPath())
				GUI.color = Color.white;


			 

			ResetWindowColor();
			//rect = GUI.Window( id, rect, NeatWindow, nodeName );
			NeatWindow();

			// If you didn't interact with anything inside...
			if( SF_GUI.PressedLMB() ) {
				OnPress();
			} else if( SF_GUI.ReleasedRawLMB() ) {
				OnRelease();
			} else if( Event.current.type == EventType.ContextClick ) {
				//Vector2 mousePos = Event.current.mousePosition;
				if( MouseOverNode( world: true ) ) {
					// Now create the menu, add items and show it
					GenericMenu menu = new GenericMenu();
					editor.ResetRunningOutdatedTimer();
					if(IsProperty() && property.CanToggleGlobal()){
						if(property.global){
							menu.AddItem( new GUIContent("Make local"), false, ContextClick, "prop_global_toggle" );
						} else {
							menu.AddItem( new GUIContent("Make global"), false, ContextClick, "prop_global_toggle" );
						}
					}
					menu.AddItem( new GUIContent("Edit Comment"), false, ContextClick, "cmt_edit" );
					menu.AddItem( new GUIContent("What does " + nodeName + " do?"), false, ContextClick, "doc_open" );
					menu.ShowAsContext();
					Event.current.Use();
				}
			}




			if( isDragging && Event.current.isMouse)
				OnDraggedWindow( Event.current.delta );



			string focusName = "namelabel" + this.id;
			if( Event.current.type == EventType.keyDown && ( Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter ) && GUI.GetNameOfFocusedControl() == focusName ) {
				editor.Defocus();
			}

			bool codeNode = this is SFN_Code;

			bool mouseOver = rect.Contains( Event.current.mousePosition );

			if( IsProperty() || codeNode ) {
				PrepareWindowColor();
				Rect nameRect = new Rect( rect );
				nameRect.height = 20;
				nameRect.y -= nameRect.height;
				nameRect.xMax -= 1; // Due to reasons
				//GUI.color = SF_Styles.nodeNameLabelBackgroundColor;
				GUI.Box( nameRect, "", EditorStyles.textField );
				GUI.color = EditorGUIUtility.isProSkin ? Color.white : Color.black;
				string oldName = codeNode ? (this as SFN_Code).functionName : IsGlobalProperty() ? property.nameInternal : property.nameDisplay;
				
				GUI.SetNextControlName(focusName);
				//Debug.Log();

				string newName;
				//if(codeNode)
				//	newName = UndoableTextField( nameRect, oldName, SF_Styles.GetNodeNameLabelText() ); // newName = GUI.TextField( nameRect, oldName, SF_Styles.GetNodeNameLabelText() );
				//else if(IsGlobalProperty())
				//	newName = GUI.TextField( nameRect, oldName, SF_Styles.GetNodeNameLabelText() );
				//else


				string labelType = codeNode ? "function name" : !IsGlobalProperty() ? "property name" : "internal name";
				newName = UndoableTextField( nameRect, oldName, labelType, SF_Styles.GetNodeNameLabelText(), readPropertyName:false );

				if(codeNode)
					newName = SF_ShaderProperty.FormatInternalName(newName);
				else
					SF_Tools.FormatSerializable( ref newName );


				
				
				if( oldName != newName ){
					if(codeNode)
						(this as SFN_Code).functionName = newName.Replace(" ",string.Empty);
					else if(IsGlobalProperty())
						property.SetBothNameAndInternal( newName );
					else
						property.SetName( newName );
				}

				bool focusedField = GUI.GetNameOfFocusedControl() == focusName;

				mouseOver = nameRect.Contains( Event.current.mousePosition ) || rect.Contains( Event.current.mousePosition );

				if( focusedField )
					editor.nodeView.selection.DeselectAll(registerUndo:true);

				if( selected || focusedField || mouseOver && !editor.screenshotInProgress ) {
					GUI.color = new Color(1f,1f,1f,0.6f);
					nameRect.x += nameRect.width;
					if(!IsGlobalProperty() && !codeNode){
						GUI.Label( nameRect, property.nameInternal, EditorStyles.boldLabel );
					}
					nameRect.y -= 12;

					// Right:
					if(!IsGlobalProperty() && !codeNode){ // Global ones *only* have internal names, display as main instead
						GUI.color = new Color( 1f, 1f, 1f, 0.3f );
						GUI.Label( nameRect, "Internal name:", EditorStyles.miniLabel);
					}

					
					// Upper:
					nameRect = new Rect( rect );
					nameRect.height = 20;
					nameRect.y -= 33;
					GUI.color = new Color( 1f, 1f, 1f, 0.6f );
					GUI.Label( nameRect, codeNode ? "Function name:" : !IsGlobalProperty() ? "Property label:" : "Internal name:", EditorStyles.miniLabel );


					GUI.color = Color.white;
				}
				ResetWindowColor();
				
			}


			Rect cr = rect;
			if(HasComment() || isEditingNodeTextField){
				GUI.color = Color.white;

				cr.height = SF_Styles.GetNodeCommentLabelTextField().fontSize + 4;
				cr.width = 2048;
				cr.y -= cr.height + 2;


				commentYposTarget = cr.y;

				//commentYposCurrent = 

				if( IsProperty() || this is SFN_Code ){
					commentYposTarget -= 19;
					if( mouseOver || selected ){
						commentYposTarget -= 8;
					}
				}

				cr.y = Mathf.Round(commentYposCurrent);

				if(isEditingNodeTextField){


					bool clicked = Event.current.rawType == EventType.mouseDown && Event.current.button == 0;
					bool clickedOutside = clicked && !cr.Contains(Event.current.mousePosition);
					bool pressedReturn = Event.current.rawType == EventType.KeyDown && Event.current.keyCode == KeyCode.Return;

					bool defocus = pressedReturn || clickedOutside;

					if( defocus ){
						isEditingNodeTextField = false;
						SF_Node.isEditingAnyNodeTextField = false;
						editor.Defocus();
					}
					string fieldStr = "node_comment_" + id;
					GUI.SetNextControlName(fieldStr);
					Rect tmp = cr;
					tmp.width = 256;
					//comment = GUI.TextField(tmp, comment, SF_Styles.GetNodeCommentLabelTextField());
					comment = UndoableTextField(tmp, comment, "comment", SF_Styles.GetNodeCommentLabelTextField());

					SF_Tools.FormatSerializableComment(ref comment);


					if(!defocus){
						GUI.FocusControl(fieldStr);
					}

				} else {
					GUI.Label(cr, "// " + comment, SF_Styles.GetNodeCommentLabelText());
				}



			}



			Rect ur = rect;

			ur = ur.MovedDown();


			// See how tall/which ones we should use on this node
			bool showPrecision = ((ShouldDefineVariable() || IsProperty()) && isFloatPrecisionBasedVariable) || canAlwaysSetPrecision;
			bool showVarname = !IsGlobalProperty() && (ShouldDefineVariable() || IsProperty()) && !lockedVariableName ;
			bool optionalVarname = IsProperty();
			bool showPanel = SF_Settings.ShowVariableSettings && (showPrecision || showVarname);




			ur.height = (showPrecision && showVarname) ? 46 : 26;
			ur.y += 1;
			if(ur.width != NODE_WIDTH){
				ur.x += (rect.width - NODE_WIDTH)/2f;
				ur.width = NODE_WIDTH;
			}


			

			// #precision #variablename

			if( showPanel ){

				// Background
				PrepareWindowColor();
				GUI.Label(ur, string.Empty, SF_Styles.NodeStyle);
				GUI.color = Color.white;
			

				Rect varNameRect = ur.Pad(4);
				Rect precisionRect = ur.Pad(4);

				if(showPrecision){

					if(showVarname){
						Rect[] split = ur.SplitVertical(0.5f, padding:4);
						precisionRect = split[0];
						varNameRect = split[1];
					}

					precision = (FloatPrecision)UndoablePopup(precisionRect,(int)precision,precisionLabels,"variable precision",SF_Styles.BoldEnumField);

					//GUI.SetNextControlName(VarPrecisionControl());
					//string[] labels = split[0].Contains(Event.current.mousePosition) ? precisionLabels : precisionLabelsSimple;

				}




				if( showVarname ){

					if( optionalVarname ){
						Rect[] split = varNameRect.SplitFromLeft((int)varNameRect.height);
						varNameRect = split[1];
						UndoableToggle(split[0],ref property.overrideInternalName,string.Empty,"override internal name", EditorStyles.toggle);
						GUI.enabled = property.overrideInternalName;
					}

					GUI.SetNextControlName(VarNameControl());
					variableName = UndoableTextField(varNameRect, (IsProperty() && !property.overrideInternalName) ? property.nameInternal : variableName, (IsProperty() ? "variable" : "internal") + " name", EditorStyles.textField, false);
					GUI.enabled = true;

				}


			
				
			}


			//GUI.Label( nameRect, "Test", EditorStyles.toolbarTextField );

		}


//		public bool ShowPrecisionEditField(){
//
//		}
//
//		public bool ShowVarnameEditField(){
//			
//		}
//
//
//		public int GetHeightOfLowerPanel(){
//
//		}


		public bool CanCustomizeVariable(){
			return ( ShouldDefineVariable() || IsProperty() ) && !lockedVariableName;
		}

		public string VarNameControl(){
			return "ctrl_" + id + "_varname";
		}
		public string VarPrecisionControl(){
			return "ctrl_" + id + "_precision";
		}


		public void UpdateNeighboringConnectorLines(){
			foreach(SF_NodeConnector con in connectors){
				if(!con.IsConnected())
					continue;
				if(con.conType == ConType.cOutput){
					foreach(SF_NodeConnector conOut in con.outputCons){
						conOut.conLine.ReconstructShapes();
					}
				} else if(con.conType == ConType.cInput){
					con.conLine.ReconstructShapes();
				}
			}
		}


		public void StartDragging() {
			isDragging = true;
			dragStart = new Vector2( rect.x, rect.y );
			dragDelta = Vector2.zero;
		}


		public static int snapThreshold = 10;
		public static int snapDistance = 256;
		public static Color snapColor = new Color(1f,1f,1f,0.5f);
		public Vector2 dragStart;
		public Vector2 dragDelta;

		public void OnDraggedWindow( Vector2 delta ) {



			editor.ResetRunningOutdatedTimer();

			//UndoRecord("move " + nodeName + " node");

			dragDelta += delta;
			Vector2 finalDelta = new Vector2( rect.x, rect.y );
			rect.x = dragStart.x + dragDelta.x;
			rect.y = dragStart.y + dragDelta.y;
			Event.current.Use();


			UpdateNeighboringConnectorLines();

			

			if(!SF_Settings.HierarcyMove) // TODO: Snap toggle + make it work properly with hierarchal on
				foreach(SF_Node n in editor.nodes){
					if( n == this )
						continue;
					if( SF_Tools.DistChebyshev( rect.center, n.rect.center ) > snapDistance )
						continue;
					if( n.selected ) // Don't snap to selected nodes
						continue;
					if( Mathf.Abs( n.rect.x - rect.x ) < snapThreshold ) { // LEFT SIDE SNAP
						delta.x -= rect.x - n.rect.x;
						rect.x = n.rect.x;
					} else if( Mathf.Abs( n.rect.y - rect.y ) < snapThreshold ) { // TOP SIDE SNAP
						delta.y -= rect.y - n.rect.y;
						rect.y = n.rect.y;
					} else if( Mathf.Abs( n.rect.center.x - rect.center.x ) < snapThreshold ) { // CENTER HORIZONTAL SNAP
						delta.x -= rect.center.x - n.rect.center.x;
						Vector2 tmp = rect.center;
						tmp.x = n.rect.center.x;
						rect.center = tmp;

						//GUILines.DrawLine( rect.center, n.rect.center, snapColor, snapThreshold * 2, true );

					} else if( Mathf.Abs( n.rect.center.y - rect.center.y ) < snapThreshold ) { // CENTER VERTICAL SNAP
						delta.y -= rect.center.y - n.rect.center.y;
						Vector2 tmp = rect.center;
						tmp.y = n.rect.center.y;
						rect.center = tmp;

						//GUILines.DrawLine( editor.nodeView.AddNodeWindowOffset( rect.center ), editor.nodeView.AddNodeWindowOffset( n.rect.center ), Color.white, snapThreshold * 2, true );
			
					}
				}

			finalDelta =  new Vector2( rect.x, rect.y ) - finalDelta;
			
			editor.nodeView.selection.MoveSelection(finalDelta, ignore:this);

			if( delta != Vector2.zero && SF_Settings.HierarcyMove && ( GetType() != typeof( SFN_Final ) ) ) {
				MoveUnselectedChildren( delta );
			}

		}

		public void MoveUnselectedChildren( Vector2 delta ) {
			// Find all child nodes
			// TODO: On click or on connect, not every frame
			List<SF_Node> children = new List<SF_Node>();
			children.AddRange( editor.nodeView.selection.Selection );
			children.Add( this );
			AppendUnselectedChildren( children );
			foreach( SF_Node n in editor.nodeView.selection.Selection ) {
				n.AppendUnselectedChildren( children );
			}
			foreach(SF_Node n in editor.nodeView.selection.Selection){
				children.Remove( n );
			}
			children.Remove( this );

			for( int i = 0; i < children.Count; i++ ) {
				children[i].rect.x += delta.x;
				children[i].rect.y += delta.y;
			}
		}

		public void AppendUnselectedChildren( List<SF_Node> list ) {

			// Search all connected
			for( int i = 0; i < connectors.Length; i++ ) {
				if( connectors[i].conType == ConType.cOutput )
					continue;
				if( connectors[i].IsConnected() && !list.Contains( connectors[i].inputCon.node ) ) {
					//if( connectors[i].inputCon.node.ConnectedOutputCount() > 1 )
					//	continue; // Only unique children
					//if( OutputsToAnyOutside( list ) )
					//	continue; // Only unique children
					if( !connectors[i].inputCon.node.selected )
						list.Add( connectors[i].inputCon.node );
					connectors[i].inputCon.node.AppendUnselectedChildren( list );
				}
			}
		}


		/*
		public bool OutputsToAnyOutside( List<SF_Node> list ) {
			foreach( SF_NodeConnection nc in connectors ) {
				if( nc.conType == ConType.cInput )
					continue;
				foreach(SF_NodeConnection con in nc.outputCons){
					if( !list.Contains( con.inputCon.node ) )
						return true;
				}
			}
			return false;
		}*/

		public int ConnectedOutputCount() {
			int count = 0;
			foreach( SF_NodeConnector nc in connectors ) {
				if(nc.conType == ConType.cInput)
					continue;
				count += nc.outputCons.Count;
			}
			return count;
		}

		public void UndoRecordSelectionState(string undoMsg){
			UndoRecord(undoMsg, UpToDateState.OutdatedSoft);
			Undo.RecordObject(editor.nodeView.selection, undoMsg);
		}

		public void Select(bool registerUndo) {
			if( !editor.nodeView.selection.Selection.Contains( this ) ) {
				if(registerUndo)
					UndoRecordSelectionState("select");
				editor.nodeView.selection.Add( this );
				selected = true;
			}
		}

		public void Deselect(bool registerUndo, string undoMsg = null) {
			if(!selected)
				return;
			if(undoMsg == null)
				undoMsg = "deselect";
			if(registerUndo)
				UndoRecordSelectionState(undoMsg);
			editor.nodeView.selection.Remove( this );
			selected = false;
		}

		public void DrawHighlight() {
			
			//if( Event.current.type == EventType.repaint )
			if( selected ) {

				Rect r = new Rect( rect );
				r.xMax -= 1;
				if( IsProperty() )
					r.yMin -= 20;
				GUILines.Highlight( r, offset: 1, strength: 2 );
			}
		}

		/*
		public void OnSelectedWindow() {
			Debug.Log("Beep!");
		}*/


		public void ProcessInput() {
			/*
			if( IsFocused() )
				Debug.Log( "Mouse over " + nodeName + " rawType = " + Event.current.rawType );
			if( Event.current.rawType == EventType.mouseDown && Event.current.button == 0 ) {
				if( !selected ) {
					Debug.Log("SELECTED");
					Debug.Log("Rect: " + rect + " mPos: " + Event.current.mousePosition);
					editor.nodeView.selection.DeselectAll();
					Select();
				}
					
			}*/
		}


		public virtual bool Draw() {


			ProcessInput();


			DrawHighlight();

			//if(status != null)
			
			

			

			PrepareWindowColor();

			if( showLowerPropertyBox )
				if( showLowerPropertyBoxAlways || ( showLowerPropertyBox && CanEvaluate() && IsUniformOutput() ) ) {
					rect.height = ( node_height + 20 );
				} else {
					rect.height = node_height;
				}


			DrawWindow();

			ResetWindowColor();

			return true;
		}



		public virtual void NeatWindow( ) {
			GUI.BeginGroup( rect );

			if(UnavailableInThisRenderPath())
				GUI.color = new Color(1f,1f,1f,0.5f);
			else
				GUI.color = Color.white;
			GUI.skin.box.clipping = TextClipping.Overflow;


			if( showColor ) {
				texture.Draw( rectInner, UnavailableInThisRenderPath() );
				GUI.color = Color.white;

				if( SF_Debug.nodes ) {
					Rect r = new Rect( 0, 16, 96, 20 );
					GUI.color = Color.white;
					GUI.skin.box.normal.textColor = Color.white;
					GUI.Box( r, "ID: " + id );
					r.y += r.height;
					//GUI.Box( r, "Cmps: " + texture.CompCount );
					//r.y += r.height;
					//GUI.Box( r, "Unif: " + texture.dataUniform );

				}


			}

			if( showLowerPropertyBox ) {
				GUI.color = Color.white;
				DrawLowerPropertyBox();
			}

			//GUI.DragWindow();


			GUI.EndGroup( );
			

			//if(rect.center.x)
		}


		public Rect LocalRect() {
			Rect r = new Rect( rect );
			r.x = 0;
			r.y = 0;
			return r;
		}

		public bool MouseOverNode(bool world = false) {

			if(!editor.nodeView.MouseInsideNodeView(offset:true))
				return false;

			if( world ) {
				return rect.Contains( Event.current.mousePosition );
			}
			else
				return LocalRect().Contains( Event.current.mousePosition );
		}

		public void ColorPickerCorner( Rect r ) {
			//bool prevEnabledState = GUI.enabled;
			//GUI.enabled = MouseOverNode(false);

			//try {
			Rect pickRect = new Rect( r );
			pickRect.height = 15;
			pickRect.width = 45;
			pickRect.y -= pickRect.height + 1;
			pickRect.x += 1;
			Rect pickBorder = new Rect( pickRect );
			pickBorder.xMax -= 18;
			//pickBorder.xMin -= 1;
			//pickBorder.yMax += 1;
			//pickBorder.yMin -= 1;

			float grayscale = texture.dataUniform.grayscale;
			Color borderColor = Color.white - new Color( grayscale, grayscale, grayscale );
			borderColor.a = GUI.enabled ? 1f : 0.25f;
			GUI.color = borderColor;
			GUI.DrawTexture( pickBorder, EditorGUIUtility.whiteTexture );
			GUI.color = Color.white;

			
			
			Color pickedColor = EditorGUI.ColorField( pickRect, texture.ConvertToDisplayColor( texture.dataUniform ) );
			SetColor( pickedColor, true );
			
		}

		public void SetColor(Color c, bool registerUndo = false) {
			if( c != texture.dataUniform ) {
				Color newColor = texture.ConvertToDisplayColor( c );
				if(registerUndo){
					if(IsProperty()){
						UndoRecord("set color of " + property.nameDisplay);
					} else {
						UndoRecord("set color of " + nodeName);
					}

				}

				texture.dataUniform = newColor;
				if( IsProperty() ) {
					if( property is SFP_Color ) {
						( this as SFN_Color ).OnUpdateValue();
					}
				}
			}
		}


		public string FloatArrToString( float[] arr ) {
			string s = "";
			for( int i = 0; i < arr.Length; i++ )
				s += arr[i] + " ";
			return s;
		}

		public void UseLowerReadonlyValues( bool use ) {
			UseLowerPropertyBox( use );
			showLowerReadonlyValues = use;
		}

		public void UseLowerPropertyBox( bool use, bool always = false ) {
			rect.height = ( use ? ( node_height + 20 ) : ( node_height ) );
			showLowerPropertyBox = use;
			if( always )
				showLowerPropertyBoxAlways = use;
		}

		public virtual void DrawLowerPropertyBox() {
			if( showLowerReadonlyValues )
				DrawLowerReadonlyValues();
		}

		public void DrawLowerReadonlyValues() {

			if( !texture.uniform )
				return;

			if( !InputsConnected() || !texture.uniform ) {
				GUI.enabled = false;
				GUI.skin.label.alignment = TextAnchor.MiddleCenter;
				GUI.Label( lowerRect, "<Input missing>" );
				GUI.skin.label.alignment = TextAnchor.MiddleLeft;
				GUI.enabled = true;
				return;
			}

			Rect tmp = lowerRect;
			tmp.width /= texture.CompCount;
			GUI.enabled = false;
			for( int i = 0; i < texture.CompCount; i++ ) {
				GUI.Box( tmp, "" );
				EditorGUI.SelectableLabel( tmp, texture.dataUniform[i].ToString() );
				tmp.x += tmp.width;
			}
			GUI.enabled = true;
		}

		public virtual void OnDelete() {
			// Override
		}

		[SerializeField]
		public UpToDateState dirtyState = UpToDateState.UpToDate;

		public void CheckIfDirty(){

			if(dirtyState == UpToDateState.UpToDate)
				return;



			//Debug.Log("Cleaning up " + nodeName);

			NodeUpdateType updType = NodeUpdateType.Hard;
			if(dirtyState == UpToDateState.OutdatedHard)
				updType = NodeUpdateType.Hard;
			if(dirtyState == UpToDateState.OutdatedSoft)
				updType = NodeUpdateType.Soft;


			OnUpdateNode(updType, true);
			dirtyState = UpToDateState.UpToDate;
		}

		public void SetDirty(UpToDateState dirtyState){
			this.dirtyState = dirtyState;
		}


		// CURRENTLY ONLY USED BY GHOST NODES
		public void DeleteGhost(bool registerUndo = false, string undoMsg = "") {

			if( this is SFN_Final )
				return;

			//bool leadsToFinal = status.leadsToFinal;


			if(SF_Debug.nodeActions)
				Debug.Log("Deleting node " + nodeName);



			OnDelete();


			Deselect(registerUndo:false);
			editor.nodes.Remove( this );
			if( editor.nodeView.treeStatus.propertyList.Contains( this ) )
				editor.nodeView.treeStatus.propertyList.Remove( this );

			for( int i = 0; i < connectors.Length; i++ ) {
				connectors[i].Disconnect(true, false);
				//connectors[i] = null; // TODO
			}
			//connectors = null; // TODO


			//SF_Editor.instance.CheckForBrokenConnections();
			//SF_Editor.instance.Repaint();

			texture.DestroyTexture();




			DestroyImmediate( texture );
			ScriptableObject.DestroyImmediate( status );
			
			ScriptableObject.DestroyImmediate(this);




			editor.OnShaderModified(NodeUpdateType.Soft);

			//if(leadsToFinal){
			//	editor.ShaderOutdated = UpToDateState.OutdatedHard; // TODO: Only if connected
			//}
		}



		// TODO: Channels etc
		// Override if this node has unconnected, required inputs
		public virtual string Evaluate( OutChannel channel = OutChannel.All ) {
			return GetVariableName();
		}


		// Used to see if it's an already defined variable or not
		public string PreEvaluate() {

			if(varDefined)
				return GetVariableName();

			// If it shouldn't be defined, get raw value
			if( !ShouldDefineVariable() ) {
				return Evaluate();
			} else if( !varDefined && !neverDefineVariable ) { // If it's not defined yet, define it! Append a new row
				DefineVariable();
			}

			return GetVariableName();
			
		}

		public void DefineVariable() {

			//if(this is SFN_If)
				//Debug.Log("Defining variable");

			if( varDefined || neverDefineVariable ) {
				//Debug.Log( "Already defined!" );
				return;
			}
			PreDefine();

			if(onlyPreDefine){
				varDefined = true;
				return;
			}
			
			string s = GetVariableType() + " " + GetVariableName() + " = " + Evaluate() + ";";

			if(HasComment()){
				s += " // " + comment;
			}

			SF_Editor.instance.shaderEvaluator.App( s );
			varDefined = true;
		}

		public virtual string[] TryGetMultiCompilePragmas( out int group ){
			group = 0;
			return null; // Override
		}


		public void DefineGhostsIfNeeded(ref List<SF_Node> ghosts) {

			//Debug.Log("Checking if ghosts should be defined on " + nodeName + "...");


			// Super duper ultra weird and shouldn't be here. Find real issue later // TODO
			if(this == null)
				return;
			
			// TODO: This will prevent multi-ghosting
			/*
			if( editor.shaderEvaluator.ghostNodes.Contains(this) ){
				if(SF_Debug.The(DebugType.GhostNodes))
					Debug.Log("Skipping ghost define for " + nodeName);
				return;
			}

			if(Connectors == null){
				Debug.Log("CHK. GHOST: [" + nodeName + "] Connector count = NULL");
				Debug.Log("WHAT? this = " + this);
				if(this == null)
					return;
			} else
				Debug.Log("CHK. GHOST: [" + nodeName + "] Connector count = " + Connectors.Length);
				*/

			foreach(SF_NodeConnector con in connectors){
				if( con.conType == ConType.cOutput) {
					//Debug.LogError("Ghost node defined on an output: "+nodeName+"[" + con.label + "]");
					continue;
				}
				con.DefineGhostIfNeeded( ref ghosts );
			}
		}


		public void PreDefine() {
			if( varDefined || varPreDefined )
				return;

			string[] preDefs = GetPreDefineRows();
			if( preDefs != null ) {
				foreach( string row in preDefs ) {
					SF_Editor.instance.shaderEvaluator.App( row );
				}
			}
			varPreDefined = true;
		}

		public virtual string[] GetPreDefineRows() {
			return null; // Override this
		}



		public bool ShouldDefineVariable() {
			if(neverDefineVariable)
				return false;
			return ((UsedMultipleTimes() || alwaysDefineVariable) /*&& !varDefined*/);
		}


		public bool UsedMultipleTimes() {
			return ( GetOutputCount() > 1 );
		}

		public int GetOutputCount() {
			int n = 0;
			foreach( SF_NodeConnector con in connectors ) {
				if( con.conType == ConType.cInput )
					continue;
				if( con.IsConnected() ) {
					foreach(SF_NodeConnector inCon in con.outputCons){
						n += inCon.usageCount; // Make sure it counts some as multiple uses
					}
				}
			}
			return n;
		}


		public virtual string SerializeSpecialData() {
			return null; // Override!
		}

		public virtual void DeserializeSpecialData( string key, string value ) {
			return; // Override!
		}


		// n:type:SFN_Multiply,id:8,x:33794,y:32535|1-9-0,2-7-0;
		// Deserialize is in SF_Parser
		public string Serialize(bool skipExternalLinks = false, bool useSuffixPrefix = false) {
			
			string s = "";
			if(useSuffixPrefix)
				s = "n:";


			s += "type:" + this.GetType().ToString() + ",";
			s += "id:" + this.id + ",";
			s += "x:" + (int)rect.x + ",";
			s += "y:" + (int)rect.y;
			if(IsProperty()){
				s += ",ptovrint:" + property.overrideInternalName;
				s += ",ptlb:" + property.nameDisplay;
				s += ",ptin:" + property.nameInternal;
			}
			if(HasComment())
				s += ",cmnt:" + comment;
			if(!string.IsNullOrEmpty(variableName) && !lockedVariableName){
				s += ",varname:" + variableName;
			}
			if(isFloatPrecisionBasedVariable)
				s += ",prsc:" + (int)precision;
			
			
			//
			string specialData = SerializeSpecialData(); // <-- This is the unique data for each node
			if( !string.IsNullOrEmpty( specialData ) ) {
				s += "," + specialData;
			}
			//

			if( HasAnyInputConnected(skipExternalLinks) ) {
				s += "|";
				int linkCount = 0;
				int i = 0;
				foreach( SF_NodeConnector con in connectors ) { // List connections, connected inputs only
					if( con.conType == ConType.cOutput ) { i++; continue; }
					if( !con.IsConnected() ) { i++; continue; }
					
					if(skipExternalLinks)
						if(!con.inputCon.node.selected){ i++; continue; }


					string link = con.GetIndex() + "-" + connectors[i].inputCon.node.id + "-" + connectors[i].inputCon.GetIndex();

					if( linkCount > 0 )
						s += ",";
					s += link;

					linkCount++;
					i++;
				}
			}
			
			if(useSuffixPrefix)
				s += ";";

			return s;
		}

		// This is the data per-node
		// n:type:SFN_Final,id:6,x:33383,y:32591|0-8-0;
		public static SF_Node Deserialize( string row, ref List<SF_Link> linkList) {
			
			
			bool isLinked = row.Contains( "|" );
			
			string linkData = "";
			
			// Grab connections, if any, and remove them from the main row
			if( isLinked ) {
				string[] split = row.Split( '|' );
				row = split[0];
				linkData = split[1];
			}
			
			
			string[] nData = row.Split( ',' ); // Split the node data
			SF_Node node = null;
			
			// This is the data in a single node, without link information
			// type:SFN_Final,id:6,x:33383,y:32591
			foreach( string s in nData ) {
				if(SF_Debug.deserialization)
					Debug.Log("Deserializing node: " + s);
				string[] split = s.Split( ':' );
				string dKey = split[0];
				string dValue = split[1];
				
				switch( dKey ) {
				case "type":
					//Debug.Log( "Deserializing " + dValue );
					node = TryCreateNodeOfType( dValue );
					if( node == null ) {
						if(SF_Debug.dynamicNodeLoad)
							Debug.LogError( "Node not found, returning..." );
						return null;
					}
					break;
				case "id":
					node.id = int.Parse( dValue );
					SF_Editor.instance.idIncrement = Mathf.Max( SF_Editor.instance.idIncrement, node.id + 1 );
					break;
				case "x":
					node.rect.x = int.Parse( dValue );
					break;
				case "y":
					node.rect.y = int.Parse( dValue );
					break;
				case "ptovrint":
					node.property.overrideInternalName = bool.Parse(dValue);
					break;
				case "ptlb":
					node.property.SetName( dValue );
					break;
				case "ptin":
					node.property.nameInternal = dValue;
					break;
				case "cmnt":
					node.comment = dValue;
					break;
				case "varname":
					node.variableName = dValue;
					break;
				case "prsc":
					node.precision = (FloatPrecision)int.Parse(dValue);
					break;
				default:
					//Debug.Log("Deserializing KeyValue: " +dKey + " v: " + dValue);
					node.DeserializeSpecialData( dKey, dValue );
					break;
				}
			}
			
			// Add links to link data, if it's connected
			if( isLinked ) {
				string[] parsedLinks = linkData.Split( ',' );
				foreach( string s in parsedLinks )
					linkList.Add( new SF_Link( node.id, s ) );
			}
			
			
			return node;
			
		}


		private static SF_Node TryCreateNodeOfType( string nodeType ) {
			SF_Node node = null;
			if( nodeType == "ShaderForge.SFN_Final" ) {
				node = SF_Editor.instance.CreateOutputNode();
			} else {
				foreach( SF_EditorNodeData tmp in SF_Editor.instance.nodeTemplates ) {
					if( tmp.type == nodeType ) {		// 1 is the type
						node = SF_Editor.instance.AddNode( tmp );	// Create the node
						break;
					}
				}
			}
			if( node == null && SF_Debug.dynamicNodeLoad ) {
				Debug.LogError( "Type [" + nodeType + "] not found!" );
			}
			return node;
		}


		public void TrySerialize( XmlWriter xml, string key, object val ) {
			if( val == null )
				return;
			string str = val.ToString();
			if( string.IsNullOrEmpty( str ) )
				return;
			xml.WriteElementString( key, str );
		}

		/*
		public virtual string SerializeCustomData() {
			return ""; // Override
		}*/

		public void DrawConnectors() {



			int yOut = 0;
			int yIn = 0;

			int spacing = 20;

			if( connectors != null ) {
				for( int i = 0; i < connectors.Length; i++ ) {
					Vector2 pos = new Vector2( rect.width + rect.x, 16 + rect.y );


					if( connectors[i].conType == ConType.cInput ) {
						pos.y += yIn * spacing;
						yIn++;
					} else {
						pos.y += yOut * spacing;
						yOut++;
					}

					connectors[i].Draw( pos );


				}
			}



			/*if( DEBUG ) {
				Rect tmp = new Rect( rect );
				tmp.height = 20;
				tmp.width = 250;
				tmp.y -= tmp.height;
				GUI.Box( tmp, depth.ToString(), EditorStyles.largeLabel );
				tmp.y -= tmp.height;
				GUI.Box( tmp, "cCons: " + CalcConnectionCount().ToString(), EditorStyles.largeLabel );
				tmp.y -= tmp.height;
				GUI.Box( tmp, "Conctrs: " + connectors.Length, EditorStyles.largeLabel );
				tmp.y -= tmp.height;
				GUI.Box( tmp, "Editor: " + ( editor != null ), EditorStyles.largeLabel );
				tmp.y -= tmp.height;
				GUI.Box( tmp, "Property: " + IsProperty(), EditorStyles.miniLabel );
				tmp.y -= tmp.height;
				if( conGroup != null ) {
					GUI.Box( tmp, "C Group out: " + conGroup.output, EditorStyles.miniLabel );
					tmp.y -= tmp.height;
					GUI.Box( tmp, "C Group ins: " + conGroup.inputs.Length, EditorStyles.miniLabel );
					tmp.y -= tmp.height;
					GUI.Box( tmp, "C Group hash: " + conGroup.GetHashCode(), EditorStyles.miniLabel );
					tmp.y -= tmp.height;
				} else {
					GUI.Box( tmp, "C Group is NULL", EditorStyles.miniLabel );
					tmp.y -= tmp.height;
				}
				GUI.Box( tmp, "Type: " + GetType().ToString(), EditorStyles.miniLabel );
				tmp.y -= tmp.height;
				GUI.Box( tmp, "Hash: " + GetHashCode(), EditorStyles.miniLabel );
				tmp.y -= tmp.height;
				if(texture != null)
					GUI.Box( tmp, "Unif: " + texture.uniform );
				tmp.y -= tmp.height;
			}*/

		}

		public SF_NodeConnector GetConnectorByID(string s) {
			int number;
			if( int.TryParse( s, out number ) ) {
				return connectors[number];
			} else {
				return GetConnectorByStringID(s);
			}
		}

		public SF_NodeConnector GetConnectorByStringID(string s) {
			foreach( SF_NodeConnector con in connectors ) {
				if( !con.HasID() )
					continue;
				if( s == con.strID )
					return con;
			}

			Debug.LogError("Unsuccessfully tried to find connector by string ID [" + s + "] in node " + nodeName);
			return null;
		}

		public bool HasAnyInputConnected(bool skipExternalLinks = false) {
			foreach( SF_NodeConnector con in connectors )
				if( con.IsConnected() && con.conType == ConType.cInput ){
					if(skipExternalLinks){
						if(con.inputCon.node.selected)
							return true;
					} else {
						return true;
					}
				}
					
			return false;
		}

		public int CalcConnectionCount() {
			int i = 0;
			foreach( SF_NodeConnector con in connectors ) {
				if( con.IsConnected() )
					i++;
			}
			return i;
		}
	}

}