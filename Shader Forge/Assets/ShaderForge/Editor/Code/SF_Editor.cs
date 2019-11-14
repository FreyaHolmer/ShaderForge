using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Reflection;
using System.Net;
using System.Collections;




namespace ShaderForge {


	public delegate T Func<T>();
	
	public enum UpToDateState{UpToDate, OutdatedSoft, OutdatedHard};

	[Serializable]
	public class SF_Editor : EditorWindow {
		[SerializeField]
		public SF_Evaluator shaderEvaluator;
		[SerializeField]
		public SF_PreviewWindow preview;
		[SerializeField]
		public SF_EditorNodeView nodeView;
		[SerializeField]
		public SF_EditorNodeBrowser nodeBrowser;
		[SerializeField]
		public SF_PassSettings ps; // TODO: Move

		[System.NonSerialized]
		public static SF_Editor instance;
		[SerializeField]
		public SFN_Final mainNode;
		[SerializeField]
		public SF_StatusBox statusBox;

		[SerializeField]
		public List<SF_Node> nodes;

		[SerializeField]
		DateTime startTime = DateTime.UtcNow;

		[SerializeField]
		GUIStyle windowStyle;
		[SerializeField]
		GUIStyle titleStyle;
		[SerializeField]
		GUIStyle versionStyle;
		[SerializeField]
		GUIStyle nodeScrollbarStyle;

		[SerializeField]
		public SF_DraggableSeparator separatorLeft;

		[SerializeField]
		public SF_DraggableSeparator separatorRight;

		public Vector2 mousePosition = Vector2.zero;

		[SerializeField]
		public Shader currentShaderAsset;
		[SerializeField]
		public string currentShaderPath;

		[SerializeField]
		public List<SF_EditorNodeData> nodeTemplates;

		[SerializeField]
		private UpToDateState shaderOutdated = UpToDateState.UpToDate;
		public UpToDateState ShaderOutdated{
			get{
				return shaderOutdated;
			}
			set{
				if(shaderOutdated != value){
					//Debug.Log("Changed outdated state to " + value);
					shaderOutdated = value;
				}
			}
		}

		[NonSerialized]
		public bool initialized = false;

		


		public SF_Editor() {
			if(SF_Debug.window)
				Debug.Log( "[SF_LOG] - SF_Editor CONSTRUCTOR SF_Editor()" );
			SF_Editor.instance = this;
		}

		[MenuItem( "Window/Shader Forge" )]
		static void InitEmpty() {
			if( SF_Editor.instance == null )
				Init( null );
			else {
				EditorWindow.GetWindow( typeof( SF_Editor ) ); // Focus
			}
		}
		 
		void OnEnable() {
			SF_Settings.LoadAllFromDisk();
			titleContent = new GUIContent( "Shader Forge", (Texture)SF_GUI.Icon );
			if( this.preview != null )
				preview.OnEnable();
		}

		void OnDisable(){

			if(shaderOutdated != UpToDateState.UpToDate){

				fullscreenMessage = "Saving...";
				Repaint();
				shaderEvaluator.Evaluate();
			}

			if( this.preview != null )
				preview.OnDisable();

			SF_Settings.SaveAllToDisk();

		}


		void OnDestroy(){
			DestroyImmediate( preview.internalMaterial );
		}

		public static bool Init( Shader initShader = null ) {

			// To make sure you get periods as decimal separators
			System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");

			if(SF_Debug.evalFlow || SF_Debug.dynamicNodeLoad)
				Debug.Log( "[SF_LOG] - SF_Editor Init(" + initShader + ")" );
			SF_Editor materialEditor = (SF_Editor)EditorWindow.GetWindow( typeof( SF_Editor ) );
			SF_Editor.instance = materialEditor;
			updateCheck = "";
			bool loaded = materialEditor.InitializeInstance( initShader );
			if( !loaded )
				return false;
			return true;
		}

		public int GetUniqueNodeID() {

			int[] occupiedIDs = nodes.Select( x => x.id ).ToArray();
			int id = -1;
			int limit = 1000000;

			do {
				id = UnityEngine.Random.Range(1,9999);
				limit--;
			} while( occupiedIDs.Contains( id ) && limit > 0 );

			if( limit <= 0 )
				Debug.LogError("Ran out of attempts to find a unique node ID");

			return id;
		}





		public void InitializeNodeTemplates() {
			nodeTemplates = new List<SF_EditorNodeData>();


			// AddTemplate( typeof( SFN_CommentBox ), "Comment Box" );

			string catArithmetic = "Arithmetic/";
			AddTemplate( typeof( SFN_Abs ), 			catArithmetic + "Abs" );
			AddTemplate( typeof( SFN_Add ), 			catArithmetic + "Add", KeyCode.A );
			AddTemplate( typeof( SFN_Blend ), 			catArithmetic + "Blend", KeyCode.B );
			AddTemplate( typeof( SFN_Ceil ), 			catArithmetic + "Ceil" );
			AddTemplate( typeof( SFN_Clamp ), 			catArithmetic + "Clamp" );
			AddTemplate( typeof( SFN_Clamp01 ), 		catArithmetic + "Clamp 0-1" );
			AddTemplate( typeof( SFN_ConstantClamp ), 	catArithmetic + "Clamp (Simple)",KeyCode.None, "Clamp Simple" );
			AddTemplate( typeof( SFN_Divide ), 			catArithmetic + "Divide", KeyCode.D );
			AddTemplate( typeof( SFN_Exp ), 			catArithmetic + "Exp" );
			AddTemplate( typeof( SFN_Floor ), 			catArithmetic + "Floor" );
			AddTemplate( typeof( SFN_Fmod ), 			catArithmetic + "Fmod" );
			AddTemplate( typeof( SFN_Frac ), 			catArithmetic + "Frac" );
			AddTemplate( typeof( SFN_HsvToRgb ),		catArithmetic + "HSV to RGB" );
			AddTemplate( typeof( SFN_Hue ),				catArithmetic + "Hue" );
			AddTemplate( typeof( SFN_If ), 				catArithmetic + "If", KeyCode.I );
			AddTemplate( typeof( SFN_InverseLerp ),		catArithmetic + "Inverse Lerp" );
			AddTemplate( typeof( SFN_Lerp ), 			catArithmetic + "Lerp", KeyCode.L );
			AddTemplate( typeof( SFN_ConstantLerp ), 	catArithmetic + "Lerp (Simple)", KeyCode.None, "Lerp Simple" );
			AddTemplate( typeof( SFN_Log ), 			catArithmetic + "Log" );
			AddTemplate( typeof( SFN_Max ), 			catArithmetic + "Max" );
			AddTemplate( typeof( SFN_Min ), 			catArithmetic + "Min" );
			AddTemplate( typeof( SFN_Multiply ), 		catArithmetic + "Multiply", KeyCode.M );
			AddTemplate( typeof( SFN_MultiplyMatrix ),  catArithmetic + "Multiply Matrix" );
			AddTemplate( typeof( SFN_Negate ), 			catArithmetic + "Negate" );
			AddTemplate( typeof( SFN_Noise ), 			catArithmetic + "Noise" );
			AddTemplate( typeof( SFN_OneMinus ), 		catArithmetic + "One Minus", KeyCode.O );
			AddTemplate( typeof( SFN_Posterize ), 		catArithmetic + "Posterize" );
			AddTemplate( typeof( SFN_Power ), 			catArithmetic + "Power", KeyCode.E );
			AddTemplate( typeof( SFN_Reciprocal ),		catArithmetic + "Reciprocal" );
			AddTemplate( typeof( SFN_RemapRangeAdvanced),catArithmetic+ "Remap" );
			AddTemplate( typeof( SFN_RemapRange ), 		catArithmetic + "Remap (Simple)", KeyCode.R, "Remap Simple" );
			AddTemplate( typeof( SFN_RgbToHsv ),		catArithmetic + "RGB to HSV" );
			AddTemplate( typeof( SFN_Round ), 			catArithmetic + "Round" );
			AddTemplate( typeof( SFN_Sign ), 			catArithmetic + "Sign" );
			AddTemplate( typeof( SFN_Smoothstep ),		catArithmetic + "Smoothstep" ).MarkAsNewNode();
			AddTemplate( typeof( SFN_Sqrt ), 			catArithmetic + "Sqrt" );
			AddTemplate( typeof( SFN_Step ), 			catArithmetic + "Step (A <= B)", KeyCode.None, "Step"  );
			AddTemplate( typeof( SFN_Subtract ), 		catArithmetic + "Subtract", KeyCode.S );
			AddTemplate( typeof( SFN_Trunc ), 			catArithmetic + "Trunc" );

			string catConstVecs = "Constant Vectors/";
			AddTemplate( typeof( SFN_Vector1 ),		catConstVecs + "Value", KeyCode.Alpha1 );
			AddTemplate( typeof( SFN_Vector2 ),		catConstVecs + "Vector 2", KeyCode.Alpha2 );
			AddTemplate( typeof( SFN_Vector3 ),		catConstVecs + "Vector 3", KeyCode.Alpha3 );
			AddTemplate( typeof( SFN_Vector4 ),		catConstVecs + "Vector 4", KeyCode.Alpha4 );
			AddTemplate( typeof( SFN_Matrix4x4 ),	catConstVecs + "Matrix 4x4" );

			string catProps = "Properties/";
			AddTemplate( typeof( SFN_Color ), 				catProps + "Color" );
			AddTemplate( typeof( SFN_Cubemap ), 			catProps + "Cubemap" );
			AddTemplate( typeof( SFN_Matrix4x4Property ),	catProps + "Matrix 4x4" );
			AddTemplate( typeof( SFN_Slider ), 				catProps + "Slider" );
			AddTemplate( typeof( SFN_SwitchProperty ),		catProps + "Switch" );
			AddTemplate( typeof( SFN_Tex2d ), 				catProps + "Texture 2D", KeyCode.T );
			AddTemplate( typeof( SFN_Tex2dAsset ), 			catProps + "Texture Asset" );
			AddTemplate( typeof( SFN_ToggleProperty ),		catProps + "Toggle" );
			AddTemplate( typeof( SFN_ValueProperty ), 		catProps + "Value" );
			AddTemplate( typeof( SFN_Vector4Property ),		catProps + "Vector 4" );

			//string catBranching = "Branching/"; 
			//AddTemplate( typeof( SFN_StaticBranch ), catBranching + "Static Branch" );

			string catVecOps = "Vector Operations/";
			AddTemplate( typeof( SFN_Append ), 			catVecOps + "Append", KeyCode.Q );
			AddTemplate( typeof( SFN_ChannelBlend ), 	catVecOps + "Channel Blend");
			AddTemplate( typeof( SFN_ComponentMask ),	catVecOps + "Component Mask", KeyCode.C );
			AddTemplate( typeof( SFN_Cross ), 			catVecOps + "Cross Product" );
			AddTemplate( typeof( SFN_Desaturate ), 		catVecOps + "Desaturate" );
			AddTemplate( typeof( SFN_DDX ),				catVecOps + "DDX" );
			AddTemplate( typeof( SFN_DDXY ),			catVecOps + "DDXY" ).MarkAsNewNode();
			AddTemplate( typeof( SFN_DDY ),				catVecOps + "DDY" );
			AddTemplate( typeof( SFN_Distance ), 		catVecOps + "Distance" );
			AddTemplate( typeof( SFN_Dot ), 			catVecOps + "Dot Product" );
			AddTemplate( typeof( SFN_Length ), 			catVecOps + "Length" );
			AddTemplate( typeof( SFN_Normalize ), 		catVecOps + "Normalize", KeyCode.N );
			AddTemplate( typeof( SFN_NormalBlend ), 	catVecOps + "Normal Blend" );
			AddTemplate( typeof( SFN_Reflect ), 		catVecOps + "Reflect" );
			AddTemplate( typeof( SFN_Transform ), 		catVecOps + "Transform" );
			AddTemplate( typeof( SFN_Transpose ),		catVecOps + "Transpose" );
			AddTemplate( typeof( SFN_VectorProjection ),catVecOps + "Vector Projection" );
			AddTemplate( typeof( SFN_VectorRejection ),	catVecOps + "Vector Rejection" );


			string catUvOps = "UV Operations/";
			AddTemplate( typeof( SFN_Panner ), 			catUvOps + "Panner", KeyCode.P );
			AddTemplate( typeof( SFN_Parallax ), 		catUvOps + "Parallax" );
			AddTemplate( typeof( SFN_Rotator ), 		catUvOps + "Rotator" );
			AddTemplate( typeof( SFN_UVTile ),			catUvOps + "UV Tile" );

			string catGeoData = "Geometry Data/";
			AddTemplate( typeof( SFN_Bitangent ), 				catGeoData + "Bitangent Dir.");
			AddTemplate( typeof( SFN_Depth ), 					catGeoData + "Depth");
			AddTemplate( typeof( SFN_FaceSign ),				catGeoData + "Face Sign" );
			AddTemplate( typeof( SFN_Fresnel ), 				catGeoData + "Fresnel", KeyCode.F );
			AddTemplate( typeof( SFN_NormalVector ), 			catGeoData + "Normal Dir." );
			AddTemplate( typeof( SFN_ObjectPosition ), 			catGeoData + "Object Position");
			AddTemplate( typeof( SFN_ObjectScale ),				catGeoData + "Object Scale" );
			AddTemplate( typeof( SFN_ScreenPos ), 				catGeoData + "Screen Position" );
			AddTemplate( typeof( SFN_Tangent ), 				catGeoData + "Tangent Dir." );
			AddTemplate( typeof( SFN_TexCoord ), 				catGeoData + "UV Coordinates", KeyCode.U );
			AddTemplate( typeof( SFN_VertexColor ), 			catGeoData + "Vertex Color", KeyCode.V );
			AddTemplate( typeof( SFN_ViewVector ), 				catGeoData + "View Dir." );
			AddTemplate( typeof( SFN_ViewReflectionVector ), 	catGeoData + "View Refl. Dir.", KeyCode.None, "View Reflection"  );
			AddTemplate( typeof( SFN_FragmentPosition ), 		catGeoData + "World Position", KeyCode.W );

			string catLighting = "Lighting/";
			AddTemplate( typeof( SFN_AmbientLight ), 		catLighting + "Ambient Light" );
			AddTemplate( typeof( SFN_HalfVector ), 			catLighting + "Half Direction", KeyCode.H ).UavailableInDeferredPrePass();
			AddTemplate( typeof( SFN_LightAttenuation ), 	catLighting + "Light Attenuation" ).UavailableInDeferredPrePass();
			AddTemplate( typeof( SFN_LightColor ), 			catLighting + "Light Color" ).UavailableInDeferredPrePass();
			AddTemplate( typeof( SFN_LightVector ), 		catLighting + "Light Direction" ).UavailableInDeferredPrePass();
			AddTemplate( typeof( SFN_LightPosition ), 		catLighting + "Light Position" ).UavailableInDeferredPrePass();
			
			string catExtData = "External Data/";
			AddTemplate( typeof( SFN_PixelSize ),				catExtData + "Pixel Size" );
			AddTemplate( typeof( SFN_ProjectionParameters ), 	catExtData + "Projection Parameters" );
			AddTemplate( typeof( SFN_ScreenParameters ), 		catExtData + "Screen Parameters" );
			AddTemplate( typeof( SFN_Time ), 					catExtData + "Time" );
			AddTemplate( typeof( SFN_ViewPosition ), 			catExtData + "View Position" );

			string catSceneData = "Scene Data/";
			AddTemplate( typeof(SFN_DepthBlend), catSceneData + "Depth Blend" );
			AddTemplate( typeof( SFN_FogColor ), catSceneData + "Fog Color" );
			AddTemplate( typeof(SFN_SceneColor), catSceneData + "Scene Color" );
			AddTemplate( typeof(SFN_SceneDepth), catSceneData + "Scene Depth" );

			string catMathConst = "Math Constants/";
			AddTemplate( typeof( SFN_E ), 		catMathConst + "e", KeyCode.None, "EulersConstant" );
			AddTemplate( typeof( SFN_Phi ), 	catMathConst + "Phi" );
			AddTemplate( typeof( SFN_Pi ), 		catMathConst + "Pi" );
			AddTemplate( typeof( SFN_Root2 ), 	catMathConst + "Root 2" );
			AddTemplate( typeof( SFN_Tau ), 	catMathConst + "Tau (2 Pi)", KeyCode.None, "Tau" );

			string catTrig = "Trigonometry/";
			AddTemplate( typeof( SFN_ArcCos ), 	catTrig + "ArcCos" );
			AddTemplate( typeof( SFN_ArcSin ), 	catTrig + "ArcSin" );
			AddTemplate( typeof( SFN_ArcTan ), 	catTrig + "ArcTan" );
			AddTemplate( typeof( SFN_ArcTan2 ), catTrig + "ArcTan2" );
			AddTemplate( typeof( SFN_Cos ), 	catTrig + "Cos" );
			AddTemplate( typeof( SFN_Sin ), 	catTrig + "Sin" );
			AddTemplate( typeof( SFN_Tan ), 	catTrig + "Tan" );

			string catCode = "Code/";
			AddTemplate( typeof( SFN_Code ), catCode + "Code" );

			string catUtility = "Utility/";
			AddTemplate( typeof( SFN_Relay ),	catUtility + "Relay" );
			AddTemplate( typeof( SFN_Get ),		catUtility + "Get", KeyCode.G ).MarkAsNewNode();
			AddTemplate( typeof( SFN_Set ),		catUtility + "Set" ).MarkAsNewNode();
			


			SF_EditorNodeData ssDiff = TryAddTemplateDynamic( "SFN_SkyshopDiff", "Skyshop/" + "Skyshop Diffuse" );
			if(ssDiff != null)
				ssDiff.MarkAsNewNode();

			SF_EditorNodeData ssSpec = TryAddTemplateDynamic( "SFN_SkyshopSpec", "Skyshop/" + "Skyshop Specular" );
			if( ssSpec != null )
				ssSpec.MarkAsNewNode();
			



		}


		public static bool NodeExistsAndIs(SF_Node node, string nodeName){
			if(NodeExists(nodeName))
				if(node.GetType() == GetNodeType(nodeName))
					return true;
			return false;
		}

		public static bool NodeExists(string nodeName){
			return GetNodeType(nodeName) != null;
		}


		static Assembly editorAssembly;
		public static Assembly EditorAssembly {
			get {
				if( editorAssembly == null ) {

					Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

					foreach( Assembly assembly in assemblies ) {
						if( assembly.FullName.Split( ',' )[0].Trim() == "Assembly-CSharp-Editor" ) {
							editorAssembly = assembly;
							return editorAssembly;
						}
					}
					//if( SF_Debug.dynamicNodeLoad )
					//	Debug.LogError("Unable to find the editor assembly" );
				}
				return editorAssembly;
			}
		}

		 
		public static Type GetNodeType(string nodeName){

			Assembly asm = EditorAssembly;
			if( asm == null )
				return null;
			string fullNodeName = nodeName;
			if(!nodeName.StartsWith("ShaderForge."))
				fullNodeName = "ShaderForge." + nodeName;
			if( SF_Debug.dynamicNodeLoad )
				Debug.Log( "Trying to dynamically load [" + fullNodeName + "]" + " in assembly [" + asm.FullName + "]" );

			return asm.GetType( fullNodeName );
		}

		public SF_EditorNodeData TryAddTemplateDynamic(string type, string label, KeyCode keyCode = KeyCode.None, string searchName = null ){

			Type dynType = GetNodeType(type);

			if(dynType != null){
				if(SF_Debug.dynamicNodeLoad)
					Debug.Log( "TryAddTemplateDynamic of " + type );
				return AddTemplate( dynType, label, keyCode, searchName );
			}
			if( SF_Debug.dynamicNodeLoad )
				Debug.Log( "TryAddTemplateDynamic of " + type + " was null" );
			return null;
		}

		public SF_EditorNodeData AddTemplate( Type type, string label, KeyCode keyCode = KeyCode.None, string searchName = null ) {
			SF_EditorNodeData item = ScriptableObject.CreateInstance<SF_EditorNodeData>().Initialize( type.FullName, label, keyCode );

			if(!string.IsNullOrEmpty(searchName)){
				item.SearchName = searchName;
			}

			this.nodeTemplates.Add( item );
			return item;
		}



		public SF_EditorNodeData GetTemplate<T>() {
			foreach( SF_EditorNodeData sft in nodeTemplates ) {
				if(  sft.type == typeof(T).FullName )
					return sft;
			}
			return null;
		}

		public SF_EditorNodeData GetTemplate( string typeName ) {
			foreach( SF_EditorNodeData sft in nodeTemplates ) {
				if( sft.type == typeName )
					return sft;
			}
			return null;
		}


		public void OnShaderModified(NodeUpdateType updType) {
			//Debug.Log("OnShaderModified: " + updType.ToString() );
			if( updType == NodeUpdateType.Hard && nodeView.treeStatus.CheckCanCompile() ){
				nodeView.lastChangeTime = (float)EditorApplication.timeSinceStartup;
				ShaderOutdated = UpToDateState.OutdatedHard;
			}
			if(updType == NodeUpdateType.Soft && ShaderOutdated == UpToDateState.UpToDate)
				ShaderOutdated = UpToDateState.OutdatedSoft;

			ps.fChecker.UpdateAvailability();
			ps.UpdateAutoSettings();
		}

		public void ResetRunningOutdatedTimer(){
			if(ShaderOutdated == UpToDateState.UpToDate)
				return;
			if(ShaderOutdated == UpToDateState.OutdatedSoft) // Might not want to have this later
				return;

			nodeView.lastChangeTime = (float)EditorApplication.timeSinceStartup;

		}

		/*
		public Vector3 GetMouseWorldPos( Vector3 playerPos ) {

			Vector3 camDir = Camera.main.transform.forward;
			Ray r = Camera.main.ScreenPointToRay( Input.mousePosition );
			Plane p = new Plane( camDir * -1, playerPos );

			float dist = 0f;
			if( p.Raycast( r, out dist ) ) {
				return r.GetPoint( dist );
			}

			Debug.LogError( "Mouse ray did not hit the plane" );
			return Vector3.zero;
		}*/

		public bool InitializeInstance( Shader initShader = null ) {
			if(SF_Debug.evalFlow)
				Debug.Log( "[SF_LOG] - SF_Editor InitializeInstance(" + initShader + ")" );
			//this.title = ;

			SF_Settings.InitializeSettings();
			this.initialized = true;
			this.ps = ScriptableObject.CreateInstance<SF_PassSettings>().Initialize( this );
			this.shaderEvaluator = new SF_Evaluator( this );
			this.preview = new SF_PreviewWindow( this );
			this.statusBox = new SF_StatusBox( /*this*/ );
			statusBox.Initialize(this);

			InitializeNodeTemplates();

			windowStyle = new GUIStyle( EditorStyles.textField );
			windowStyle.margin = new RectOffset( 0, 0, 0, 0 );
			windowStyle.padding = new RectOffset( 0, 0, 0, 0 );

			titleStyle = new GUIStyle( EditorStyles.largeLabel );
			titleStyle.fontSize = 24;

			versionStyle = new GUIStyle( EditorStyles.miniBoldLabel );
			versionStyle.alignment = TextAnchor.MiddleLeft;
			versionStyle.fontSize = 9;
			versionStyle.normal.textColor = Color.gray;
			versionStyle.padding.left = 1;
			versionStyle.padding.top = 1;
			versionStyle.padding.bottom = 1;
			versionStyle.margin.left = 1;
			versionStyle.margin.top = 3;
			versionStyle.margin.bottom = 1;

			this.nodes = new List<SF_Node>();

			// Create main output node and add to list
			this.nodeView = ScriptableObject.CreateInstance<SF_EditorNodeView>().Initialize( this );
			this.ps.catConsole.treeStatus = this.nodeView.treeStatus;
			this.nodeBrowser = ScriptableObject.CreateInstance<SF_EditorNodeBrowser>().Initialize( this );
			this.separatorLeft = ScriptableObject.CreateInstance<SF_DraggableSeparator>();
			this.separatorRight = ScriptableObject.CreateInstance<SF_DraggableSeparator>();

			separatorLeft.rect = new Rect(340, 0, 0, 0);
			separatorRight.rect = new Rect(Screen.width - 130f, 0, 0, 0);

			this.previousPosition = position;

			if( initShader == null ) {
				// TODO: New menu etc
				//CreateOutputNode();
			} else {
				currentShaderAsset = initShader;
				
				bool loaded = SF_Parser.ParseNodeDataFromShader( this, initShader );
				if( !loaded ) {
					initShader = null;
					DestroyImmediate( this );
					return false;
				}
					
				// Make preview material use this shader
				//preview.material.shader = currentShaderAsset;
				Material m = preview.InternalMaterial;
				SF_Tools.AssignShaderToMaterialAsset( ref m, currentShaderAsset );
			}

			// Load data if it was set to initialize things
			return true; // Successfully loaded
		}





		public SF_Node CreateOutputNode() {
			//Debug.Log ("Creating output node");
			this.mainNode = ScriptableObject.CreateInstance<SFN_Final>().Initialize( this );//new SFN_Final();
			this.nodes.Add( mainNode );
			return mainNode;
		}

		public SF_Node GetNodeByID( int id ) {
			for( int i = 0; i < nodes.Count; i++ ) {
				if( nodes[i].id == id )
					return nodes[i];
			}
			return null;
		}





		public void UpdateKeyHoldEvents(bool mouseOverSomeNode) {
			if( nodeTemplates == null || nodeTemplates.Count == 0 ) {
				InitializeNodeTemplates();
			}

			//Debug.Log( "nodeTemplates.Count = " + nodeTemplates.Count );

			foreach( SF_EditorNodeData nData in nodeTemplates ) {

				if( nData == null ) {
					InitializeNodeTemplates();
					return;
				}
				SF_EditorNodeData requestedNode = nData.CheckHotkeyInput(mouseOverSomeNode);
				if( requestedNode != null ) {
					AddNode( requestedNode, true );
					return;
				}
			}
			/*foreach(KeyValuePair<SF_EditorNodeData, Func<SF_Node>> entry in inputInstancers){
				if(entry.Key.CheckHotkeyInput()){
					AddNode( entry.Key );
				}
			}*/
		}

		public T AddNode<T>() where T:SF_Node {
			return AddNode(GetTemplate<T>()) as T;
		}

		public SF_Node AddNode(string typeName) {
			//Debug.Log( "Searching for " + typeName );
			return AddNode( GetTemplate( typeName ) );
		}

		public SF_Node AddNode( SF_EditorNodeData nodeData, bool registerUndo = false ) {

			if( nodeData == null ){
				Debug.Log("Null node data passed into AddNode");
			}

			SF_Node node = nodeData.CreateInstance();

			if( SF_Debug.dynamicNodeLoad ) {
				if( node == null )
					Debug.Log( "nodeData failed to create a node of full path: " + nodeData.fullPath );
				else
					Debug.Log( "Created a node of full path: " + nodeData.fullPath );
			}

			if(registerUndo){
				Undo.RecordObject(this, "add node " + node.nodeName);
			}


			nodes.Add( node );
			if(Event.current != null)
				Event.current.Use();
			//Repaint();
			return node;
		}


		bool Clicked() {
			return Event.current.type == EventType.MouseDown;
		}
		
		float fps = 0;
		double prevFrameTime = 1;
		public double deltaTime = 0.02;






		List<IEnumerator> coroutines = new List<IEnumerator>();

		//double corLastTime;
	//	double corDeltaTime;
		void UpdateCoroutines(){
			//corDeltaTime = EditorApplication.timeSinceStartup - corLastTime;
			//corLastTime = EditorApplication.timeSinceStartup;
			for(int i = 0; i < coroutines.Count; i++){
				IEnumerator routine = coroutines[i];
				if(!routine.MoveNext()){
					coroutines.RemoveAt(i--);
				}
			}
		}
		void StartCoroutine (IEnumerator routine){
			coroutines.Add(routine);
		}




		void Update() {



			if( closeMe ) {
				base.Close();
				return;
			}
			
			
			double now = Now();
			double deltaTime = now-prevFrameTime;
			fps = 1f/(float)deltaTime;


			
			if(fps > 60)
				return; // Wait for target FPS
			

			prevFrameTime = now;

			preview.UpdateRot();

			
			
			for (int i = nodes.Count - 1; i >= 0; i--) {
				if(nodes[i] == null)
					nodes.Remove(nodes[i]);
				else
					nodes[i].Update();
			}


			// Refresh node previews
			int maxUpdatesPerFrame = 80;
			int updatedNodes = 0;

			while( updatedNodes < maxUpdatesPerFrame ) {
				bool anyUpdated = false;
				for( int i = 0; i < nodes.Count; i++ ) {
					if( nodes[i].CheckIfDirty() ) {
						anyUpdated = true;
						updatedNodes++;
					}
				}
				if( !anyUpdated ) {
					break;
				}
			}

			
			

				

			if( ShaderOutdated == UpToDateState.OutdatedHard && SF_Settings.autoCompile && nodeView.GetTimeSinceChanged() >= 1f) {
				shaderEvaluator.Evaluate();
			}


			//UpdateCameraZoomValue();
			if(focusedWindow == this)
				Repaint(); // Update GUI every frame if focused

		}

	

		MethodInfo isDockedMethod;
		const float dockedCheckInterval = 1f;
		public float dockedLastUpdate = -100f;
		public bool _docked = false;
		public bool Docked{
			get{
				if( EditorApplication.timeSinceStartup - dockedLastUpdate > dockedCheckInterval ) {
					dockedLastUpdate = (float)EditorApplication.timeSinceStartup;
					if( isDockedMethod == null ) {
						BindingFlags fullBinding = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
						isDockedMethod = typeof( EditorWindow ).GetProperty( "docked", fullBinding ).GetGetMethod( true );
					}
					_docked = ( bool ) isDockedMethod.Invoke(this, null);
				}
				return _docked;
			}
		}

		public int TabOffset{
			get{
				return Docked ? 19 : 22;
			}
		}


		
		public double Now(){
			TimeSpan t = ( DateTime.UtcNow - startTime );
			return t.TotalSeconds;
		}




		void OnWindowResized( int deltaXsize, int deltaYsize ) {
			if(separatorRight == null)
				ForceClose();
			separatorRight.rect.x += deltaXsize;
		}

		void ForceClose() {
			//Debug.Log("Force close");
			closeMe = true;
			GUIUtility.ExitGUI();
		} 

		void AddDependenciesHierarchally(SF_Node node, DependencyTree<SF_Node> tree){
			node.ReadDependencies();
			tree.Add(node);
			foreach(SF_Node n in ((IDependable<SF_Node>)node).Dependencies){
				AddDependenciesHierarchally(n, tree);
			}
		}

		public List<SF_Node> GetDepthSortedDependencyTreeForConnectedNodes(bool reverse = false){
			DependencyTree<SF_Node> tree = new DependencyTree<SF_Node>();
			
			AddDependenciesHierarchally(mainNode, tree);
			//Debug.Log(tree.tree.Count);
			tree.Sort();

			List<SF_Node> list = tree.tree.Select(x=>(SF_Node)x).ToList();
			if(reverse)
				list.Reverse();
			return list;
		}

		string fullscreenMessage = "";
		public Rect previousPosition;
		public bool closeMe = false;
		void OnGUI() {

			//Debug.Log("SF_Editor OnGUI()");

			//SF_AllDependencies.DrawDependencyTree(new Rect(0, 0, Screen.width, Screen.height));
			//return;

//			if(Event.current.keyCode == KeyCode.Space && Event.current.type == EventType.keyDown){
//				Debug.Log("Beep");
//				Event.current.Use();
//
//
//
//			}

			if(SF_Parser.quickLoad) // Don't draw while loading
				return;
			
			if(SF_Debug.performance)
				GUI.Label(new Rect(500,64,128,64),"fps: "+fps.ToString());

			if( position != previousPosition ) {
				OnWindowResized( (int)(position.width - previousPosition.width), (int)(position.height - previousPosition.height) );
				previousPosition = position;
			}

						Rect fullRect = new Rect( 0, 0, Screen.width, Screen.height);
			//Debug.Log( fullRect );

			if( currentShaderAsset == null ) {
				DrawMainMenu();
				return;
			}

			if(!string.IsNullOrEmpty(fullscreenMessage)){
				GUI.Box(fullRect,fullscreenMessage);
				return;
			}



			//UpdateCameraZoomInput();


			if(Event.current.rawType == EventType.ValidateCommand && Event.current.commandName == "UndoRedoPerformed"){
				Defocus(deselectNodes:false);
				CheckForDirtyNodes(); // When undoing, some nodes will come back as dirty, which means they need to update their values
				shaderEvaluator.ps.fChecker.UpdateAvailability();
				ResetRunningOutdatedTimer();
			}


			if( nodes != null ) {

				//foreach( SF_Node n in nodes ) {
				for( int i = 0; i < nodes.Count;i++ ) {
					SF_Node n = nodes[i];

					if( n == null ) {
						// THIS MEANS YOU STARTED UNITY WITH SF OPEN
						ForceClose();
						return;
					} else{
						n.DrawConnections();
					}
				}
					
			}

			if(separatorLeft == null){
				// THIS MEANS YOU STARTED UNITY WITH SF OPEN
				ForceClose();
				return;
			}




			//EditorGUILayout.BeginHorizontal();
			//{
			//float wPreview = leftSeparator;
			//float wNodeBrowser = 130;

			Rect pRect = new Rect( fullRect );
			pRect.height /= EditorGUIUtility.pixelsPerPoint;
			pRect.width /= EditorGUIUtility.pixelsPerPoint;
			pRect.width = separatorLeft.rect.x;
			SF_GUI.FillBackground( pRect );
			DrawPreviewPanel( pRect );
			Rect previewPanelRect = pRect;

			//pRect.x += leftWidth;
			//pRect.width = wSeparator;
			//VerticalSeparatorDraggable(ref leftWidth, pRect );
			separatorLeft.MinX = 320;
			separatorLeft.MaxX = (int)( fullRect.width / 2f - separatorLeft.rect.width );
			separatorLeft.Draw( (int)pRect.y, (int)pRect.height );
			pRect.x = separatorLeft.rect.x + separatorLeft.rect.width;


			if(SF_Settings.showNodeSidebar)
				pRect.width = separatorRight.rect.x - separatorLeft.rect.x - separatorLeft.rect.width;
			else
				pRect.width = Screen.width - separatorLeft.rect.x - separatorLeft.rect.width;
			//GUI.Box( new Rect( 300, 0, 512, 32 ), pRect.ToString() );

			if( SF_Debug.nodes ) {
				Rect r = pRect; r.width = 256; r.height = 16;
				for( int i = 0; i < nodes.Count; i++ ) {
					GUI.Label( r, "Node[" + i + "] at {" + nodes[i].rect.x + ", " + nodes[i].rect.y + "}", EditorStyles.label ); // nodes[i]
					r = r.MovedDown();
				}
			}

			if( Event.current.rawType == EventType.KeyUp ){
				foreach(SF_EditorNodeData nd in nodeTemplates){
					nd.holding = false;
				}
			}


			nodeView.OnLocalGUI( pRect.PadTop(TabOffset) ); // 22 when not docked, 19 if docked
			//GUI.EndGroup();

			//pRect.yMin -= 3; // if docked



			

			//pRect.x += pRect.width;
			//pRect.width = wSeparator;
			//VerticalSeparatorDraggable(ref rightWidth, pRect );
			if(SF_Settings.showNodeSidebar){
				separatorRight.MinX = (int)(fullRect.width / EditorGUIUtility.pixelsPerPoint) - 150;
				separatorRight.MaxX = (int)(fullRect.width /  EditorGUIUtility.pixelsPerPoint) - 32;
				separatorRight.Draw( (int)pRect.y, (int)pRect.height );

				pRect.x += pRect.width + separatorRight.rect.width;
				pRect.width = (fullRect.width / EditorGUIUtility.pixelsPerPoint) - separatorRight.rect.x - separatorRight.rect.width;

				SF_GUI.FillBackground( pRect );
				nodeBrowser.OnLocalGUI( pRect );
			}




			// Last thing, right?

			ssButtonColor = Color.Lerp(ssButtonColor,ssButtonColorTarget, (float)deltaTime*ssButtonFadeSpeed);

			if(previewPanelRect.Contains(Event.current.mousePosition)){

				ssButtonColorTarget = Color.white;
				ssButtonFadeSpeed = 0.4f;


			} else {
				ssButtonColorTarget = new Color(1f,1f,1f,0f); // TODO LERP
				ssButtonFadeSpeed = 1.5f;
			}
			Rect ssRect = new Rect(8,previewButtonHeightOffset,32,19);
			GUI.color = ssButtonColor;
			if(GUI.Button(ssRect, SF_GUI.Screenshot_icon)){
				GenericMenu menu = new GenericMenu();
				menu.AddItem( new GUIContent("Take screenshot of node tree"), false, ContextClickScreenshot, "ss_standard" );
				menu.AddItem( new GUIContent("Take screenshot of node tree without 3D preview"), false, ContextClickScreenshot, "ss_nopreview" );
				menu.ShowAsContext();
				
			}
			GUI.color = Color.white;

			//Rect ssRectIcon = new Rect(0f, 0f, SF_GUI.Screenshot_icon.width, SF_GUI.Screenshot_icon.height);
			////ssRectIcon.center = ssRect.center;
			//GUI.DrawTexture(ssRectIcon, SF_GUI.Screenshot_icon);


			if(Event.current.type == EventType.Repaint)
				UpdateCoroutines();


			DrawTooltip();

		}


		public void CheckForDirtyNodes(){

			for(int i=0;i<nodes.Count;i++){
				nodes[i].CheckIfDirty();
			}

		}









		Color ssButtonColor = Color.black;
		Color ssButtonColorTarget = Color.black;
		float ssButtonFadeSpeed = 0.5f;
	

		public void ContextClickScreenshot( object o ) {
			string picked = o as string;
			switch(picked){
			case "ss_standard":
				StartCoroutine(CaptureScreenshot(includePreview:true));
				break;
			case "ss_nopreview":
				StartCoroutine(CaptureScreenshot(includePreview:false));
				break;
			}
		}




		public bool screenshotInProgress = false;
		public bool firstFrameScreenshotInProgress = false;

		public float preScreenshotZoom = 1f;


		public IEnumerator CaptureScreenshot(bool includePreview){



			screenshotInProgress = true;
			firstFrameScreenshotInProgress = true;

			preScreenshotZoom = nodeView.zoomTarget;
			nodeView.SetZoom (1f);
			nodeView.zoomTarget = 1f;
			yield return null;


			Rect r = nodeView.rect.PadBottom(24);
			Vector2 startCamPos = nodeView.cameraPos;
			Rect nodeWrap = nodeView.GetNodeEncapsulationRect().Margin(32);

			// Calculate tiles needed
			int xTiles;
			int yTiles;

			xTiles = Mathf.CeilToInt(nodeWrap.width/r.width);
			yTiles = Mathf.CeilToInt(nodeWrap.height/r.height);
			//int bottomAlign = (int)((r.height*(yTiles)) - nodeWrap.height);



			int leftAlign = -(int)separatorLeft.rect.xMax;

			Texture2D tex = new Texture2D((int)r.width*xTiles, (int)r.height*yTiles,TextureFormat.RGB24,false);
			tex.hideFlags = HideFlags.HideAndDontSave;

			float previewRadius = 64f;
			Vector2 optimalPreviewPoint = CalculateOptimalPlacement(nodeWrap, out previewRadius);
			int ssMargin = 64;
			previewRadius = previewRadius*2 - ssMargin;

			float creditsRadius = 32;
			Vector2 optimalCreditsPoint;
			if(includePreview){
				float tr = previewRadius-ssMargin;
				optimalCreditsPoint = CalculateOptimalPlacement(nodeWrap, out creditsRadius,
				        new Rect(optimalPreviewPoint.x-tr/2+ssMargin/2,optimalPreviewPoint.y-tr/2+ssMargin/2,tr, tr)
		        );
			} else {
				optimalCreditsPoint = optimalPreviewPoint;
				creditsRadius = previewRadius-ssMargin;
			}


			string shaderTitle = "";

			if(!string.IsNullOrEmpty(currentShaderPath)){
				if(currentShaderPath.Contains('/')){
					string[] split = currentShaderPath.Split('/');
					if(split.Length > 0){
						shaderTitle = split[split.Length-1];
					}
				}
			}
			
			
			
			
			
			for(int ix=0;ix<xTiles;ix++){
				for(int iy=0;iy<yTiles;iy++){
					r = nodeView.rect.PadBottom(24);

					nodeView.cameraPos = nodeWrap.TopLeft() + new Vector2(ix*r.width,iy*r.height) - new Vector2(leftAlign,0f);
					//nodeWrap = nodeView.GetNodeEncapsulationRect();
					// PUT LOADING INDICATOR HERE
					yield return null;
					if(SF_Debug.screenshot)
						GUI.Label(r,"(" + ix + ", " + iy + ")");

				//	Debug.Log("R: " + r + " OptPt: " + optimalPreviewPoint);

					if(includePreview){
						Rect previewRect = new Rect(0f,0f,previewRadius,previewRadius);
						//previewRect.center = new Vector2(optimalPreviewPoint.x-nodeView.cameraPos.x,optimalPreviewPoint.y-nodeView.cameraPos.y);
						previewRect.center = nodeView.ZoomSpaceToScreenSpace(optimalPreviewPoint);

						//Rect previewLabelRect = previewRect;
						//previewLabelRect.height = (28);
						//previewLabelRect.x += 4;
						//previewLabelRect.y += 2;

						GUI.Box( previewRect.Margin(2).PadTop(-16), string.Empty, SF_Styles.NodeStyle );
						preview.DrawMeshGUI(previewRect);

						if(shaderTitle != string.Empty){
							Rect previewLabelRect = previewRect;
							previewLabelRect.height = 16;
							previewLabelRect.Margin(-1);
							previewLabelRect.y -= 16;

							GUI.Label(previewLabelRect,shaderTitle,SF_Styles.GetNodeScreenshotTitleText());
						}
					}

					Rect creditsLineRect = nodeWrap;
					creditsLineRect.height = 32;
					creditsLineRect.x -= nodeView.cameraPos.x;
					creditsLineRect.y -= nodeView.cameraPos.y;
					creditsLineRect = creditsLineRect.Margin(-8);

					Color tmp  = SF_GUI.ProSkin ? Color.white : Color.black;
					tmp.a = 0.6f;
					GUI.color = tmp;
					//GUI.Label(creditsLineRect, "Created with Shader Forge " + SF_Tools.versionStage + " " + SF_Tools.version + " - A node-based shader editor for Unity - http://u3d.as/6cc", EditorStyles.boldLabel);


					Rect creditsRect = new Rect(0f,0f,Mathf.Min(creditsRadius*1.5f,SF_GUI.Logo.width),0f);
					creditsRect.height = creditsRect.width*((float)SF_GUI.Logo.height/SF_GUI.Logo.width);
					creditsRect.center = nodeView.ZoomSpaceToScreenSpace(optimalCreditsPoint);
					GUI.DrawTexture(creditsRect, SF_GUI.Logo);
					Rect crTop = creditsRect;
					crTop.height = 16;
					crTop = crTop.MovedUp();
					GUI.Label(crTop, "Created using");
					Rect crBottom = creditsRect;
					crBottom = crBottom.MovedDown();
					crBottom.height = 16;
					//crBottom.width += 256;
					TextClipping prevClip = GUI.skin.label.clipping;
					//GUI.skin.label.alignment = TextAnchor.MiddleLeft;
					GUI.skin.label.clipping = TextClipping.Overflow;
					GUI.Label(crBottom, SF_Tools.versionStage + " v" + SF_Tools.version + " - http://u3d.as/6cc");
					GUI.skin.label.clipping = prevClip;

					if(SF_Debug.screenshot){
						GUI.color = new Color(1f,0f,0f,0.4f);
						GUI.DrawTexture(crBottom, EditorGUIUtility.whiteTexture);
						GUI.color = new Color(0f,1f,0f,0.4f);
						GUI.DrawTexture(creditsRect, EditorGUIUtility.whiteTexture);
						GUI.color = new Color(0f,0f,1f,0.4f);
						GUI.DrawTexture(crTop, EditorGUIUtility.whiteTexture);
					}

					//GUI.color = Color.white;
					GUI.color = Color.white;


					//float clampedX = Mathf.Min(r.width, nodeWrap.width - ix*r.width);
					//float clampedY = Mathf.Min(r.height, nodeWrap.height - iy*r.height);

					Rect readRect = new Rect(r.x, r.y, r.width, r.height);
					//Rect readRect = new Rect(r.x, r.y, clampedX, clampedY);

					tex.ReadPixels(readRect,(int)(ix*r.width),(int)(tex.height-(iy+1)*r.height));
					firstFrameScreenshotInProgress = false;

					//Debug.Log(nodeView.cameraPos - startCamPos);



				}
			}

			//tex.ReadPixels(new Rect(preview.),)


			nodeView.cameraPos = startCamPos;

			nodeView.SetZoom(preScreenshotZoom);
			nodeView.zoomTarget = preScreenshotZoom;


			// Crop the texture down to fit the nodes + margins


			Color[] croppedBlock = tex.GetPixels(0,tex.height - (int)nodeWrap.height,(int)nodeWrap.width, (int)nodeWrap.height);
			DestroyImmediate(tex);
			tex = new Texture2D((int)nodeWrap.width, (int)nodeWrap.height);
			tex.hideFlags = HideFlags.HideAndDontSave;
			tex.SetPixels(croppedBlock);


			// EditorUtility.OpenFolderPanel("things",Application.dataPath,"Default");


			/*Vector2[] nodePoints = new Vector2[nodes.Count];
			for(int i=0;i<nodePoints.Length;i++){
				nodePoints[i] = nodes[i].rect.center;
			}*/













			// Mask with the new mask thing
			/*
			Color[] oldPixels = tex.GetPixels();
			Color[] newPixels = new Color[oldPixels.Length];
			for (int i = 0; i < oldPixels.Length; i+=1) {
				Color pixel = oldPixels [i];

				Vector2 pt = new Vector2(i%tex.width, tex.height - Mathf.FloorToInt((float)i/tex.width));

				Vector2 maskPt = (pt / distSampleResF);
				maskPt.x /= mask.width;
				maskPt.y /= mask.height;
				maskPt.y = 1f-maskPt.y;



				Vector2 testPt = pt + nodeWrap.TopLeft();

				//pixel *= Mathf.Clamp01((testPt - nodeWrap.TopLeft()).magnitude/256f);
				//pixel *= Mathf.Clamp01((nodePoints[0] - testPt).magnitude/256f);
				//pixel *= Mathf.Clamp01(testPt.ShortestChebyshevDistanceToPoints(nodePoints)/256f);
				/*
				float dist2rect = testPt.ShortestManhattanDistanceToRects(nodeRects.ToArray());
				float dist2line = float.MaxValue;

				foreach(SF_NodeConnectionLine line in lines){
					dist2line = Mathf.Min(dist2line, SF_Tools.DistanceToLine(line.pointsBezier0[0],line.pointsBezier0[line.pointsBezier0.Length-1],testPt));
				}


				float shortest = Mathf.Min(dist2rect, dist2line);

				//pixel = Color.white * Mathf.Clamp01(shortest/(Mathf.Max(tex.width,tex.height)*0.2f));

				//pixel.a = 1f;
				newPixels[i] = pixel * mask.GetPixelBilinear(maskPt.x, maskPt.y);
			}
			tex.SetPixels(newPixels);
			*/



			tex.Apply();

			shaderTitle = CleanFileName(shaderTitle+"_"+DateTime.Now.ToShortDateString()).Replace(" ","_").ToLower();

			string projPath = Application.dataPath.Substring(0, Application.dataPath.Length-6);
			string filePath = projPath + "sf_"+shaderTitle+".png";
			File.WriteAllBytes(filePath, tex.EncodeToPNG());
			DestroyImmediate(tex);
			screenshotInProgress = false;
			if(Application.platform == RuntimePlatform.OSXEditor)
				EditorUtility.RevealInFinder(filePath);
			else
				System.Diagnostics.Process.Start("explorer.exe", "/select,"+filePath.Replace("/","\\"));
		}

		public static string CleanFileName(string filename){
			filename.Replace("/","");
			return new String(filename.Except(System.IO.Path.GetInvalidFileNameChars()).ToArray());
		}   



		public Vector2 CalculateOptimalPlacement(Rect nodeWrap, out float radius, params Rect[] extraRects ){




			List<Rect> nodeRects = new List<Rect>();
			List<SF_NodeConnectionLine> lines = new List<SF_NodeConnectionLine>();
			for(int i=0;i<nodes.Count;i++){
				nodeRects.Add( nodes[i].rect.PadTop((int)(nodes[i].BoundsTop()-nodes[i].rect.yMin)) );
				foreach(SF_NodeConnector con in nodes[i].connectors){
					nodeRects.Add(con.rect);
					if(con.conType == ConType.cOutput || !con.IsConnectedAndEnabled())
						continue;
					lines.Add(con.conLine);
					con.conLine.ReconstructShapes();
					for (int j = 0; j < con.conLine.pointsBezier0.Length; j++) {
						
						con.conLine.pointsBezier0[j] = con.conLine.pointsBezier0[j] ; //+ new Vector2(nodeWrap.width, nodeWrap.height) - new Vector2(r.width*0.5f,r.height*0.5f);//new Vector2(600,1330);
						
					}
				}
				
			}
			if(extraRects != null)
				nodeRects.AddRange(extraRects);
			
			
			Rect[] borderRects = new Rect[]{
				nodeWrap.MovedRight(),
				nodeWrap.MovedLeft(),
				nodeWrap.MovedUp(),
				nodeWrap.MovedDown()
			};
			
			for (int i = 0; i < 4; i++) {
				nodeRects.Add(borderRects[i]);
			}
			
			
			
			int distSampleRes = 16;
			float distSampleResF = distSampleRes;
			
			
			//Texture2D mask = new Texture2D(Mathf.CeilToInt(nodeWrap.width/distSampleResF),Mathf.CeilToInt(nodeWrap.height/distSampleResF),TextureFormat.RGB24,false);
			//mask.hideFlags = HideFlags.HideAndDontSave;

			int width = Mathf.CeilToInt(nodeWrap.width/distSampleResF);
			int height = Mathf.CeilToInt(nodeWrap.height/distSampleResF);
			
			float longestDist = float.MinValue;
			Vector2 longestDistPt = Vector2.zero;
			
			
			// GENERATE MASK
			Color[] newMaskPixels = new Color[width*height];
			for (int i = 0; i < newMaskPixels.Length; i+=1) {
				
				
				
				//Color pixel = Color.white;
				
				
				
				Vector2 testPt = new Vector2(i%width, height - Mathf.FloorToInt((float)i/width))*distSampleResF + nodeWrap.TopLeft();
				
				//pixel *= Mathf.Clamp01((testPt - nodeWrap.TopLeft()).magnitude/256f);
				//pixel *= Mathf.Clamp01((nodePoints[0] - testPt).magnitude/256f);
				//pixel *= Mathf.Clamp01(testPt.ShortestChebyshevDistanceToPoints(nodePoints)/256f);
				
				float dist2rect = testPt.ShortestManhattanDistanceToRects(nodeRects.ToArray());
				float dist2line = float.MaxValue;
				
				foreach(SF_NodeConnectionLine line in lines){
					dist2line = Mathf.Min(dist2line, SF_Tools.DistanceToLine(line.pointsBezier0[0],line.pointsBezier0[line.pointsBezier0.Length-1],testPt));
				}
				
				
				float shortest = Mathf.Min(dist2rect, dist2line);
				
				if(shortest > longestDist){
					longestDist = shortest;
					longestDistPt = testPt;
					//pixel = Color.red;
				}// else {
					//pixel = Color.white * Mathf.Clamp01(shortest/(Mathf.Max(nodeWrap.width,nodeWrap.height)*0.2f));
				//}
				
				
				
				//pixel.a = 1f;
				//newMaskPixels[i] = pixel;
			}
			//mask.SetPixels(newMaskPixels);
			//mask.Apply();
			radius = longestDist;
			return longestDistPt;
		}




		// TOOLTIP, Draw this last
		public void DrawTooltip() {
			/*
			if( !string.IsNullOrEmpty( GUI.tooltip ) ) {
				//Debug.Log( "TOOLTIP" );
				GUIStyle tooltipStyle = EditorStyles.miniButton;
				GUI.Box(
					new Rect(
						Event.current.mousePosition.x + 32,
						Event.current.mousePosition.y,
						tooltipStyle.CalcSize( new GUIContent( GUI.tooltip ) ).x * 1.1f,
						tooltipStyle.CalcSize( new GUIContent( GUI.tooltip ) ).y * 1.2f
					),
					GUI.tooltip, tooltipStyle
				);
			}
			GUI.tooltip = null;*/
		}

		public void Defocus(bool deselectNodes = false) {
			//Debug.Log("DEFOCUS");
//			string currentFocus = GUI.GetNameOfFocusedControl();
//			if( currentFocus != "defocus"){
				GUI.FocusControl("null");
//			}

			if( deselectNodes )
				nodeView.selection.DeselectAll(registerUndo:true);
		}


		public bool DraggingAnySeparator() {
			return separatorLeft.dragging || separatorRight.dragging;
		}

		

		public void FlexHorizontal(Action func){
			GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();
			func();
			GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();
		}

		public void FlexHorizontal(Action func, float width){
			GUILayout.BeginHorizontal(GUILayout.Width(width)); GUILayout.Space(Screen.width/2f - 335);
			func();
			GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();
		}


		public static string updateCheck = "";
		public static bool outOfDate = false;

		public static void CheckForUpdates(){
			updateCheck = "Checking for updates...";
			//Debug.Log(updateCheck);

			WebClient wc = new WebClient();

			string latestVersion;

			try{
				latestVersion = wc.DownloadString("http://www.acegikmo.com/shaderforge/latestversion.php");
				string[] split = latestVersion.Split('.');
				int latestMajor = int.Parse(split[0]);
				int latestMinor = int.Parse(split[1]);

				if(latestMajor > SF_Tools.versionNumPrimary){
					outOfDate = true;
				} else if(latestMajor == SF_Tools.versionNumPrimary && latestMinor > SF_Tools.versionNumSecondary){
					outOfDate = true;
				} else {
					outOfDate = false;
				}

				if(outOfDate){
					updateCheck = "Shader Forge is out of date!\nYou are running " + SF_Tools.version + ", the latest version is " + latestVersion;
				} else {
					updateCheck = "Shader Forge is up to date!";
				}




			} catch ( WebException e){
				updateCheck = "Couldn't check for updates: " + e.Status;
			}
		

		}


		private enum MainMenuState{Main, Credits, PresetPick}

		private MainMenuState menuState = MainMenuState.Main;

		
		public void DrawMainMenu() {


			//SF_AllDependencies.DrawDependencyTree(new Rect(0f,0f,Screen.width,Screen.height));
			//return;
			
			if(string.IsNullOrEmpty(updateCheck)){
				CheckForUpdates();
			}

			GUILayout.BeginVertical();
			{
				GUILayout.FlexibleSpace();


				FlexHorizontal(()=>{
					GUILayout.Label( SF_GUI.Logo );
					if(outOfDate)
						GUI.color = Color.red;
					GUILayout.Label( SF_Tools.versionStage + " v" + SF_Tools.version, EditorStyles.boldLabel );
					if(outOfDate)
						GUI.color = Color.white;
				});


				if(menuState == MainMenuState.Main){
					minSize = new Vector2(500,400);
					DrawPrimaryMainMenuGUI();
				} else if( menuState == MainMenuState.PresetPick ) {
					minSize = new Vector2( 128*(shaderPresetNames.Length + 1), 560 );
					DrawPresetPickGUI();
				} else if(menuState == MainMenuState.Credits){

					//Vector2 centerPrev = position.center;

					minSize = new Vector2(740,560);

					//Rect rWnd = position;
					//rWnd.center = new Vector2( 800,800);
					//position = rWnd;


					DrawCreditsGUI();
				}
				

				

				GUILayout.FlexibleSpace();
			}
			GUILayout.EndVertical();


		}

		public void DrawCreditsGUI(){
			EditorGUILayout.Separator();
			FlexHorizontal(()=>{
				GUILayout.Label( "Thanks for purchasing Shader Forge <3" );
			});
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			FlexHorizontal(()=>{
				GUILayout.Label( "Created by ", SF_Styles.CreditsLabelText);
				GUILayout.Label( "Freya 'Acegikmo' Holm" + '\u00e9' + "r", EditorStyles.boldLabel);
			});
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			FlexHorizontal(()=>{
				GUILayout.Label( "Special thanks:", EditorStyles.boldLabel );
			});
			CreditsLine("All of the alpha & beta testers","For their amazing feedback during the early days!" );
			CreditsLine( "Jenny 'sranine' Nordenborg", "For creating the Shader Forge logo and for supporting me throughout the development time!" );
			CreditsLine( "Peter Cornelius", "For convincing me that I should have started creating SF in the first place" );
			CreditsLine( "Robert Briscoe", "For actively testing SF and providing excellent feedback" );
			CreditsLine( "Thomas Pasieka", "For helping out immensely in getting the word out, as well as motivating me to continue" );
			CreditsLine( "Aras Pranckevi" +'\u010D'+ "ius", "For helping out with various shader code issues");
			CreditsLine( "Renaldas 'ReJ' Zioma", "For assisting in the Unity 5 transition" );
			CreditsLine( "Tim 'Stramit' Cooper & David 'Texel' Jones", "For giving helpful tips");
			CreditsLine( "Sander 'Zerot' Homan", "For helping out stealing Unity's internal RT code");
			CreditsLine( "Carlos 'Darkcoder' Wilkes", "For helping out with various serialization issues");
			CreditsLine( "Ville 'wiliz' Mäkynen", "For helping out with the undo system");
			CreditsLine( "Daniele Giardini", "For his editor window icon script (also, check out his plugin DOTween!)");
			CreditsLine( "Beck Sebenius", "For helping out getting coroutines to run in the Editor");
			CreditsLine( "James 'Farfarer' O'Hare", "For asking all the advanced shader questions on the forums so I didn't have to");
			CreditsLine( "Tenebrous", "For helping with... Something... (I can't remember)");
			CreditsLine( "Alex Telford", "For his fragment shader tutorials");
			CreditsLine( "Shawn White", "For helping out finding how to access compiled shaders from code");
			CreditsLine( "Colin Barr"+ '\u00e9' +"-Brisebois & Stephen Hill", "For their research on normal map blending");
			CreditsLine( "Andrew Baldwin", "For his articles on pseudorandom numbers" );


			EditorGUILayout.Separator();
			FlexHorizontal(()=>{
				if( GUILayout.Button( "Return to menu", GUILayout.Height( 30f ), GUILayout.Width( 190f ) ) ) {
					menuState = MainMenuState.Main;
				}
			});
		}

		public void CreditsLine(string author, string reason){
			FlexHorizontal(()=>{
				GUILayout.Label( author, EditorStyles.boldLabel );
				GUILayout.Label(" - ", SF_Styles.CreditsLabelText );
				GUILayout.Label( reason, SF_Styles.CreditsLabelText );
			},400f);
		}

		public enum ShaderPresets { Unlit, LitPBR, LitBasic, Custom, Sprite, ParticleAdditive, ParticleAlphaBlended, ParticleMultiplicative, Sky, PostEffect }
		public string[] shaderPresetNames = new string[] {
			"Unlit",
			"Lit\n(PBR)",
			"Lit\n(Basic)",
			"Custom Lighting",
			"Sprite",
			"Particle\n(Additive)",
			"Particle\n(Alpha-Blended)",
			"Particle\n(Multiplicative)",
			"Sky",
			"Post-Effect"
		};

		public string[] shaderPresetShaders = new string[] {
			"Unlit",
			"PBR",
			"Basic",
			"CustomLighting",
			"Sprite",
			"ParticleAdditive",
			"ParticleAlphaBlended",
			"ParticleMultiplicative",
			"Sky",
			"PostEffect"
		};

		public string GetShaderPresetPath(ShaderPresets preset) {
			int i = (int)preset;
			string file = "preset" + shaderPresetShaders[i] + ".shader";
			return SF_Resources.InternalResourcesPath + "Shader Presets/" + file;
		}


		public string[] shaderPresetDescriptions = new string[] { 
			"Unlit means that light sources will not affect this shader, it will simply have the color you give it, regardless of the scene setup.",
			"Lit (PBR) is set up to match Unity's Physically Based shader, affected by lightmaps, light probes, reflection probes etc.",
			"Lit (Basic) is the old-school Blinn-Phong lighting model. Direct lighting only, no lightmap or probe data.",
			"Custom Lighting is set up with a simple example of how you can create your own lighting models. The initial setup is a Blinn-Phong shader.",
			"Sprite is for creating 2D shaders to be used on sprites. These will have the pixel-perfect option and sort properly with other 2D sprites.",
			"Particle (Additive) is generally for glow effects, lightshafts, sparks etc. Primarily used in particle systems.",
			"Particle (Alpha-Blended) is generally for debris effects, dusty smoke etc. Primarily used in particle systems.",
			"Particle (Multiplicative) is generally for darkening effects, black smoke, evil-looking anti-glow etc. Primarily used in particle systems.",
			"Sky is for creating shaders to be used with a sky material in your scene. It will render behind everything else.",
			"Post-Effect is for creating shaders meant to render full-screen rather than at a position in the world, commonly used for post-process effects"
		};

		string desc = "";

		public void DrawPresetPickGUI() {

			GUIStyle centerLabel = new GUIStyle( EditorStyles.boldLabel );
			GUIStyle centerLabelSmall = new GUIStyle( EditorStyles.miniLabel );
			centerLabel.alignment = centerLabelSmall.alignment = TextAnchor.MiddleCenter;


			EditorGUILayout.Separator();
			FlexHorizontal( () => {
				GUILayout.BeginVertical();
				GUILayout.Label( "What kind of shader do you want to forge?", centerLabel );
				GUI.color = new Color(1f,1f,1f,0.4f);
				GUILayout.Label( "This will simply affect the initial configuration of the shader. It will not \"lock-in\" any features", centerLabelSmall );
				GUI.color = Color.white;
				GUILayout.EndVertical();
			} );
			EditorGUILayout.Separator();

			

			FlexHorizontal( () => {

				GUILayoutOption[] btnLayout = new GUILayoutOption[2] { GUILayout.Width( 128 ), GUILayout.Height( 128 ) };

				GUIStyle style = new GUIStyle( EditorStyles.boldLabel );
				style.alignment = TextAnchor.UpperCenter;

				//if( Event.current.type == EventType.mouseMove)
					//desc = "";

				//GUILayout.BeginVertical();
				for(int i=0;i<shaderPresetNames.Length;i++){
					
					GUILayout.Label( GetShaderPresetIcon( (ShaderPresets)i ), btnLayout );

					Rect r = GUILayoutUtility.GetLastRect();

					GUI.Label( r.MovedDown(), shaderPresetNames[i], style );

					if(r.Contains(Event.current.mousePosition) ){
						GUI.DrawTexture(r, SF_GUI.Shader_preset_icon_highlight, ScaleMode.ScaleToFit, true);
						desc = shaderPresetDescriptions[i];
					}

					



					GUI.color = Color.clear;
					


					if( GUI.Button( r, "" ) ) {
						
						bool created = TryCreateNewShader( (ShaderPresets)i );
						if( created )
							return;
					}
					GUI.color = Color.white;
				}
				//GUILayout.EndVertical();

			});
		
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();

			FlexHorizontal( () => {
				GUILayout.Label( desc, centerLabelSmall );
			});

			EditorGUILayout.Separator();
			EditorGUILayout.Separator();

			FlexHorizontal( () => {
				if( GUILayout.Button( "Back" ) ) {
					menuState = MainMenuState.Main;
				}
			} );



			
		}


		public Texture2D GetShaderPresetIcon(ShaderPresets preset) {

			switch( preset ) {

				case ShaderPresets.Custom:
					return SF_GUI.Shader_preset_icon_custom;
				case ShaderPresets.LitBasic:
					return SF_GUI.Shader_preset_icon_litbasic;
				case ShaderPresets.LitPBR:
					return SF_GUI.Shader_preset_icon_litpbr;
				case ShaderPresets.ParticleAdditive:
					return SF_GUI.Shader_preset_icon_particleadditive;
				case ShaderPresets.ParticleAlphaBlended:
					return SF_GUI.Shader_preset_icon_particlealphablended;
				case ShaderPresets.ParticleMultiplicative:
					return SF_GUI.Shader_preset_icon_particlemultiplicative;
				case ShaderPresets.Sky:
					return SF_GUI.Shader_preset_icon_sky;
				case ShaderPresets.Sprite:
					return SF_GUI.Shader_preset_icon_sprite;
				case ShaderPresets.Unlit:
					return SF_GUI.Shader_preset_icon_unlit;
				case ShaderPresets.PostEffect:
					return SF_GUI.Shader_preset_icon_posteffect;

			}

			Debug.LogError("No preset icon found");

			return null;


		}


		public void DrawPrimaryMainMenuGUI(){

			
			
			FlexHorizontal(()=>{
				GUI.color = new Color( 0.7f, 0.7f, 0.7f );
				if( GUILayout.Button( '\u00a9' + " Freya 'Acegikmo' Holm" + '\u00e9' + "r", EditorStyles.miniLabel ) ) {
					Application.OpenURL("https://twitter.com/FreyaHolmer");
				}
				
				SF_GUI.AssignCursorForPreviousRect( MouseCursor.Link );
				GUI.color = Color.white;
			});
			
			EditorGUILayout.Separator();
			
			/*
				FlexHorizontal(()=>{
					if( GUILayout.Button(SF_Tools.manualLabel , GUILayout.Height( 32f ), GUILayout.Width( 190f ) ) ) {
						Application.OpenURL( SF_Tools.manualURL );
					}
				});
			*/
			
			FlexHorizontal(()=>{
				
				if(SF_Tools.CanRunShaderForge()){
					if( GUILayout.Button( "New Shader", GUILayout.Width( 128 ), GUILayout.Height( 64 ) ) ) {
						menuState = MainMenuState.PresetPick;
					}
					if( GUILayout.Button( "Load Shader", GUILayout.Width( 128 ), GUILayout.Height( 64 ) ) ) {
						OpenLoadDialog();
					}
				} else {
					GUILayout.BeginVertical();
					SF_Tools.UnityOutOfDateGUI();
					GUILayout.EndVertical();
				}
			});

			
			
			FlexHorizontal(()=>{
				if( GUILayout.Button( "Polycount thread" ) ) {
					Application.OpenURL( "http://www.polycount.com/forum/showthread.php?t=123439" );
				}
				if( GUILayout.Button( "Unity thread" ) ) {
					Application.OpenURL( "http://forum.unity3d.com/threads/222049-Shader-Forge-A-visual-node-based-shader-editor" );
				}
				if( GUILayout.Button( SF_Tools.documentationLabel ) ) {
					Application.OpenURL( SF_Tools.documentationURL );
				}
				if( GUILayout.Button( "Wiki" ) ) {
					Application.OpenURL( "http://acegikmo.com/shaderforge/wiki" );
				}
				if( GUILayout.Button("Credits") ){
					menuState = MainMenuState.Credits;
				}
			});

			
			FlexHorizontal( () => {
				if( GUILayout.Button( SF_Tools.bugReportLabel, GUILayout.Height( 32f ), GUILayout.Width( 180f ) ) ) {
					Application.OpenURL( SF_Tools.bugReportURL );
				}
			} );

			FlexHorizontal( () => {
				if( GUILayout.Button( "Forums", GUILayout.Height( 32f ), GUILayout.Width( 120f ) ) ) {
					Application.OpenURL( "http://neatcorporation.com/forums/viewforum.php?f=1" );
				}
			} );
			
			EditorGUILayout.Separator();
			FlexHorizontal(()=>{
				GUILayout.Label(updateCheck);
			});
			if(outOfDate){
				float t = (Mathf.Sin((float)EditorApplication.timeSinceStartup*Mathf.PI*2f)*0.5f)+0.5f;
				GUI.color = Color.Lerp(Color.white, new Color(0.4f,0.7f,1f),t);
				FlexHorizontal(()=>{
					if(GUILayout.Button("Download latest version")){
						Application.OpenURL( "https://www.assetstore.unity3d.com/#/content/14147" );
					}
				});
				t = (Mathf.Sin((float)EditorApplication.timeSinceStartup*Mathf.PI*2f-1)*0.5f)+0.5f;
				GUI.color = Color.Lerp(Color.white, new Color(0.4f,0.7f,1f),t);
				FlexHorizontal(()=>{
					if(GUILayout.Button("What's new?")){
						Application.OpenURL( "http://acegikmo.com/shaderforge/changelog/" );
					}
				});
				GUI.color = Color.green;
			}
		}
		
		
		
		public bool PropertyNameTaken(SF_ShaderProperty sProp){
			foreach(SF_Node n in nodes){
				if(n == sProp.node)
					continue;
				if(n.IsProperty())
					if(n.property.nameDisplay == sProp.nameDisplay || n.property.nameInternal == sProp.nameInternal)
						return true;
			}
			return false;
		}
		
		
		public void OpenLoadDialog(){
			string path = EditorUtility.OpenFilePanel(
							"Load Shader",
							"Assets",
							"shader"
						);

						if( string.IsNullOrEmpty( path ) ) {
							//Debug.LogError("No path selected");
							return;
						} else {

							// Found file! Make sure it's a shader

							path = SF_Tools.PathFromAbsoluteToProject( path );
							Shader loadedShader = (Shader)AssetDatabase.LoadAssetAtPath(path, typeof(Shader));
							if( loadedShader == null ) {
								Debug.LogError( "Selected shader not found" );
								return;
							}



							bool isSFshader = SF_Parser.ContainsShaderForgeData(loadedShader);

							bool allowEdit = isSFshader;
							if(!allowEdit)
								allowEdit = SF_GUI.AcceptedNewShaderReplaceDialog();

							
							if( allowEdit ) {
								SF_Editor.Init( loadedShader );
							} else {
								//Debug.LogError( "User cancelled loading operation" );
							}
								
						}
			
		}



		public bool TryCreateNewShader(SF_Editor.ShaderPresets preset) {


			
			

			//Shader s = (Shader)AssetDatabase.LoadAssetAtPath( presetPath, typeof(Shader) );
			//Debug.Log( s);

	

			string savePath = EditorUtility.SaveFilePanel(
				"Save new shader",
				"Assets",
				"NewShader",
				"shader"
			);

			if( string.IsNullOrEmpty( savePath ) ) {
				return false;
			}

			string presetPath = GetShaderPresetPath( preset );
			StreamReader presetReader = new StreamReader( Application.dataPath + presetPath.Substring( 6 ) );

			// So we now have the path to save it, let's save
			StreamWriter sw;
			if( !File.Exists( savePath ) ) {
				sw = File.CreateText( savePath );
			} else {
				sw = new StreamWriter(savePath);
			}

			// Read from preset
			string[] presetLines = presetReader.ReadToEnd().Split( '\n' );
			for( int i=0; i < presetLines.Length; i++ ) {
				if( presetLines[i].StartsWith( "Shader \"Hidden/" ) ) {

					// Extract name of the file to put in the shader path
					string[] split = savePath.Split( '/' );
					currentShaderPath = split[split.Length - 1].Split( '.' )[0];
					currentShaderPath = "Shader Forge/" + currentShaderPath;

					// Write to the line
					presetLines[i] = "Shader \"" + currentShaderPath + "\" {";

					break;
				}
			}

			// Read from the preset
			for( int i=0; i < presetLines.Length; i++ ) {
				sw.WriteLine( presetLines[i] );
			}

			sw.Flush();
			sw.Close();
			presetReader.Close();
			AssetDatabase.Refresh();

			// Shorten it to a relative path
			string dataPath = Application.dataPath;
			string assetPath = "Assets/" + savePath.Substring( dataPath.Length + 1 );

			// Assign a reference to the file
			currentShaderAsset = (Shader)AssetDatabase.LoadAssetAtPath( assetPath, typeof( Shader ) );

			if( currentShaderAsset == null ) {
				Debug.LogError( "Couldn't load shader asset" );
				Debug.Break();
				return false;
			}

			

			// Make sure the preview material is using the shader
			preview.InternalMaterial.shader = currentShaderAsset;

			// That's about it for the file/asset management.
			//CreateOutputNode();
			SF_Editor.Init( currentShaderAsset );
			//shaderEvaluator.Evaluate(); // And we're off!

			//nodeView.CenterCamera();

			return true;
		}

		public string GetShaderFilePath() {

			if( currentShaderAsset == null ) {
				Debug.LogError( "Tried to find path of null shader asset!" );
				Debug.Break();
				return null;
			}
			return AssetDatabase.GetAssetPath( currentShaderAsset );
		}

		public bool displaySettings = false;

		public void DrawPreviewPanel( Rect r ) {
			// Left side shader preview

			//Rect logoRect = new Rect( 1, 0, SF_GUI.Logo.width, SF_GUI.Logo.height );

			//GUI.DrawTexture( logoRect, SF_GUI.Logo );

			Rect btnRect = new Rect(r);
			btnRect.y += 4;
			btnRect.x += 2;
			//btnRect.xMin += logoRect.width;

			int wDiff = 8;

			btnRect.height = 17;
			btnRect.width /= 4;
			btnRect.width += wDiff;

			GUIStyle btnStyle = EditorStyles.miniButton;

			if(GUI.Button(btnRect,"Return to menu",btnStyle)){
				OnPressBackToMenuButton();
			}
			btnRect.x += btnRect.width;
			btnRect.xMax -= wDiff*2;
			btnRect.width *= 0.75f;
			displaySettings = GUI.Toggle(btnRect, displaySettings, "Settings",btnStyle);
		
			btnRect.x += btnRect.width;
			btnRect.width *= 2f;

			GUI.color = SF_GUI.outdatedStateColors[(int)ShaderOutdated];
			if( GUI.Button( btnRect, "Compile shader", btnStyle ) ) {
				if(nodeView.treeStatus.CheckCanCompile())
					shaderEvaluator.Evaluate();
			}
			GUI.color = Color.white;
			
			nodeView.DrawRecompileTimer(btnRect);
			btnRect.x += btnRect.width;
			btnRect.width *= 0.5f;

			SF_Settings.autoCompile = GUI.Toggle( btnRect, SF_Settings.autoCompile, "Auto" );

			btnRect.y += 4;



			// SETTINGS EXPANSION
			if(displaySettings){
				btnRect.y += btnRect.height;
				btnRect.x = r.x - 4;
				btnRect.width = r.width / 4f;
				btnRect.x += btnRect.width;
				btnRect.width *= 2.55f;
				
				/*Rect[] splitRects = btnRect.SplitHorizontal( 0.5f, 1 ); // Node render mode control
				GUI.Label( splitRects[1], "Node rendering" );
				EditorGUI.BeginChangeCheck();
				SF_Settings.nodeRenderMode = (NodeRenderMode)EditorGUI.EnumPopup( splitRects[0], SF_Settings.nodeRenderMode );
				if( EditorGUI.EndChangeCheck() ) {
					RegenerateNodeBaseData();
				}
				btnRect = btnRect.MovedDown();*/
				if( SF_Settings.nodeRenderMode == NodeRenderMode.Viewport ) {
					EditorGUI.BeginDisabledGroup( true );
					GUI.Toggle( btnRect, true, "Real-time node rendering" );
					EditorGUI.EndDisabledGroup();
				} else {
					EditorGUI.BeginChangeCheck();
					SF_Settings.realtimeNodePreviews = GUI.Toggle( btnRect, SF_Settings.realtimeNodePreviews, "Real-time node rendering" );
					if( EditorGUI.EndChangeCheck() ) {
						RegenerateNodeBaseData();
					}
				}
				
				btnRect = btnRect.MovedDown();
				SF_Settings.quickPickScrollWheel = GUI.Toggle( btnRect, SF_Settings.quickPickScrollWheel, "Use scroll in the quickpicker" );
				btnRect = btnRect.MovedDown();
				SF_Settings.showVariableSettings = GUI.Toggle( btnRect, SF_Settings.showVariableSettings, "Show variable name & precision" );
				btnRect = btnRect.MovedDown();
				SF_Settings.showNodeSidebar = GUI.Toggle( btnRect, SF_Settings.showNodeSidebar, "Show node browser panel" );
				btnRect = btnRect.MovedDown();
				if( SF_GUI.HoldingControl() ) {
					EditorGUI.BeginDisabledGroup( true );
					GUI.Toggle( btnRect, !SF_Settings.hierarchalNodeMove, "Hierarchal Node Move" );
					EditorGUI.EndDisabledGroup();
				} else {
					SF_Settings.hierarchalNodeMove = GUI.Toggle( btnRect, SF_Settings.hierarchalNodeMove, "Hierarchal Node Move" );
				}
				
				btnRect.y += 4;
			}




			//GUI.Box( new Rect(203,10,128,19), SF_Tools.versionStage+" "+SF_Tools.version, versionStyle );
			previewButtonHeightOffset = (int)btnRect.yMax + 24;
			int previewOffset = preview.OnGUI( (int)btnRect.yMax, (int)r.width );
			int statusBoxOffset = statusBox.OnGUI( previewOffset, (int)r.width );


			ps.OnLocalGUI(statusBoxOffset, (int)r.width );
			if( SF_Debug.nodes ) {
				GUILayout.Label( "Node count: " + nodes.Count );
			}

		}

		void RegenerateNodeBaseData() {
			for( int i = 0; i < nodes.Count; i++ ) {
				nodes[i].GenerateBaseData();
			}
		}

		int previewButtonHeightOffset;

		public void OnPressBackToMenuButton(){
			shaderEvaluator.SaveShaderAsset();
			Close();
			Init();
		}


		public void OnPressSettingsButton(){

		}







		public void OnShaderEvaluated() {
			// statusBox.UpdateInstructionCount( preview.InternalMaterial.shader );
		}



		public void CheckForBrokenConnections() {
			foreach( SF_Node node in nodes )
				node.CheckForBrokenConnections();
		}

	}
}