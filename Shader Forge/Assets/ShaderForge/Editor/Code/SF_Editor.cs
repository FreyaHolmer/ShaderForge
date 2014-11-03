using UnityEngine;
using UnityEditor;
using System;
using Holoville.HOEditorUtils; // Third party, to force inspector title icon
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
		public int idIncrement;

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
			HOPanelUtils.SetWindowTitle( this, (Texture)SF_GUI.Icon, "Shader Forge" );
		}

		[MenuItem( "Window/Shader Forge" )]
		static void InitEmpty() {
			//Debug.Log( "[SF_LOG] - SF_Editor InitEmpty()" );
			if( SF_Editor.instance == null )
				Init( null );
			else {
				//Debug.Log( "[SF_LOG] - BEFORE EditorWindow.GetWindow( typeof(SF_Editor) )" );
				EditorWindow.GetWindow( typeof( SF_Editor ) ); // Focus
				//Debug.Log( "[SF_LOG] - AFTER EditorWindow.GetWindow( typeof(SF_Editor) )" );
			}
		}

		//[MenuItem( "Assets/Create/Shader Forge Shader", false, 90 )]
		//public static void CreateGO() {
		//
		//}

		void OnDisable(){



			if(shaderOutdated != UpToDateState.UpToDate){

				fullscreenMessage = "Saving...";
				Repaint();
				shaderEvaluator.Evaluate();
			}

			//Debug.Log("OnDisable editor window");

		}


		void OnDestroy(){

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

		/*
		void OnEnable() {
			Debug.Log( "[SF_LOG] - SF_Editor OnEnable() initialized = " + initialized + " SF_Editor.instance = " + SF_Editor.instance );
			if( !initialized ) {
				SF_Editor.instance = null;
				InitEmpty();
			}
				
		}
		 * */



		public void InitializeNodeTemplates() {
			nodeTemplates = new List<SF_EditorNodeData>();

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
			AddTemplate( typeof( SFN_If ), 				catArithmetic + "If", KeyCode.I );
			AddTemplate( typeof( SFN_Lerp ), 			catArithmetic + "Lerp", KeyCode.L );
			AddTemplate( typeof( SFN_ConstantLerp ), 	catArithmetic + "Lerp (Simple)",KeyCode.None, "Lerp Simple" );
			AddTemplate( typeof( SFN_Log ), 			catArithmetic + "Log" );
			AddTemplate( typeof( SFN_Max ), 			catArithmetic + "Max" );
			AddTemplate( typeof( SFN_Min ), 			catArithmetic + "Min" );
			AddTemplate( typeof( SFN_Multiply ), 		catArithmetic + "Multiply", KeyCode.M );
			AddTemplate( typeof( SFN_Negate ), 			catArithmetic + "Negate" );
			AddTemplate( typeof( SFN_Noise ), 			catArithmetic + "Noise" );
			AddTemplate( typeof( SFN_OneMinus ), 		catArithmetic + "One Minus", KeyCode.O );
			AddTemplate( typeof( SFN_Posterize ), 		catArithmetic + "Posterize" );
			AddTemplate( typeof( SFN_Power ), 			catArithmetic + "Power", KeyCode.E );
			AddTemplate( typeof( SFN_RemapRangeAdvanced),catArithmetic+ "Remap" );
			AddTemplate( typeof( SFN_RemapRange ), 		catArithmetic + "Remap (Simple)", KeyCode.R, "Remap Simple" );
			AddTemplate( typeof( SFN_Round ), 			catArithmetic + "Round" );
			AddTemplate( typeof( SFN_Sign ), 			catArithmetic + "Sign" );
			AddTemplate( typeof( SFN_Sqrt ), 			catArithmetic + "Sqrt" );
			AddTemplate( typeof( SFN_Step ), 			catArithmetic + "Step (A <= B)", KeyCode.None, "Step"  );
			AddTemplate( typeof( SFN_Subtract ), 		catArithmetic + "Subtract", KeyCode.S );
			AddTemplate( typeof( SFN_Trunc ), 			catArithmetic + "Trunc" );

			string catConstVecs = "Constant Vectors/";
			AddTemplate( typeof( SFN_Vector1 ), catConstVecs + "Value", KeyCode.Alpha1 );
			AddTemplate( typeof( SFN_Vector2 ), catConstVecs + "Vector 2", KeyCode.Alpha2 );
			AddTemplate( typeof( SFN_Vector3 ), catConstVecs + "Vector 3", KeyCode.Alpha3 );
			AddTemplate( typeof( SFN_Vector4 ), catConstVecs + "Vector 4", KeyCode.Alpha4 );

			string catProps = "Properties/";
			AddTemplate( typeof( SFN_Color ), 			catProps + "Color" );
			AddTemplate( typeof( SFN_Cubemap ), 		catProps + "Cubemap" );
			AddTemplate( typeof( SFN_Slider ), 			catProps + "Slider" );
			AddTemplate( typeof( SFN_SwitchProperty ),	catProps + "Switch" ).MarkAsNewNode();
			AddTemplate( typeof( SFN_Tex2d ), 			catProps + "Texture 2D", KeyCode.T );
			AddTemplate( typeof( SFN_Tex2dAsset ), 		catProps + "Texture Asset" );
			AddTemplate( typeof( SFN_ToggleProperty ), 	catProps + "Toggle" ).MarkAsNewNode();
			AddTemplate( typeof( SFN_ValueProperty ), 	catProps + "Value" );
			AddTemplate( typeof( SFN_Vector4Property ), catProps + "Vector 4" );

			//string catBranching = "Branching/"; 
			//AddTemplate( typeof( SFN_StaticBranch ), catBranching + "Static Branch" );

			string catVecOps = "Vector Operations/";
			AddTemplate( typeof( SFN_Append ), 			catVecOps + "Append", KeyCode.Q );
			AddTemplate( typeof( SFN_ChannelBlend ), 	catVecOps + "Channel Blend");
			AddTemplate( typeof( SFN_ComponentMask ),	catVecOps + "Component Mask", KeyCode.C );
			AddTemplate( typeof( SFN_Cross ), 			catVecOps + "Cross Product" );
			AddTemplate( typeof( SFN_Desaturate ), 		catVecOps + "Desaturate" );
			AddTemplate( typeof( SFN_Distance ), 		catVecOps + "Distance" );
			AddTemplate( typeof( SFN_Dot ), 			catVecOps + "Dot Product" );
			AddTemplate( typeof( SFN_Length ), 			catVecOps + "Length" );
			AddTemplate( typeof( SFN_Normalize ), 		catVecOps + "Normalize", KeyCode.N );
			AddTemplate( typeof( SFN_NormalBlend ), 	catVecOps + "Normal Blend" );
			AddTemplate( typeof( SFN_Reflect ), 		catVecOps + "Reflect" );
			AddTemplate( typeof( SFN_Transform ), 		catVecOps + "Transform" );
			AddTemplate( typeof( SFN_VectorProjection ),catVecOps + "Vector Projection" );
			AddTemplate( typeof( SFN_VectorRejection ),	catVecOps + "Vector Rejection" );


			string catUvOps = "UV Operations/";
			AddTemplate( typeof( SFN_Panner ), 		catUvOps + "Panner", KeyCode.P );
			AddTemplate( typeof( SFN_Parallax ), 	catUvOps + "Parallax" );
			AddTemplate( typeof( SFN_Rotator ), 	catUvOps + "Rotator" );

			string catGeoData = "Geometry Data/";
			AddTemplate( typeof( SFN_Binormal ), 				catGeoData + "Binormal Dir.");
			AddTemplate( typeof( SFN_Depth ), 					catGeoData + "Depth");
			AddTemplate( typeof( SFN_Fresnel ), 				catGeoData + "Fresnel", KeyCode.F );
			AddTemplate( typeof( SFN_NormalVector ), 			catGeoData + "Normal Dir." );
			AddTemplate( typeof( SFN_ObjectPosition ), 			catGeoData + "Object Position");
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
			AddTemplate( typeof( SFN_ProjectionParameters ), 	catExtData + "Projection Parameters" );
			AddTemplate( typeof( SFN_ScreenParameters ), 		catExtData + "Screen Parameters" );
			AddTemplate( typeof( SFN_Time ), 					catExtData + "Time" );
			AddTemplate( typeof( SFN_ViewPosition ), 			catExtData + "View Position" );

			string catSceneData = "Scene Data/";
			AddTemplate( typeof(SFN_DepthBlend), catSceneData + "Depth Blend" );
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
			AddTemplate( typeof( SFN_Relay ), catUtility + "Relay" );


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
		static Assembly EditorAssembly {
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
			return Event.current.type == EventType.mouseDown;
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



			if(fps > 40)
				return; // Wait for target FPS
			

			prevFrameTime = now;

			preview.UpdateRot();
			
			
			
			for (int i = nodes.Count - 1; i >= 0; i--) {
				if(nodes[i] == null)
					nodes.Remove(nodes[i]);
				else
					nodes[i].Update();
			}
			

				

			if( ShaderOutdated == UpToDateState.OutdatedHard && SF_Settings.AutoRecompile && nodeView.GetTimeSinceChanged() >= 1f) {
				shaderEvaluator.Evaluate();
			}


			if( !Application.isPlaying ) { // In order to animate shaders when game is not running

				Shader.SetGlobalVector( "_TimeEditor", new Vector4( // TODO: Make this only run if a Time node is present
						(float)now / 20f,
				        (float)now,
				        (float)now * 2f,
				        (float)now * 3f
					)
				); 
			}

			


			//UpdateCameraZoomValue();
			if(focusedWindow == this)
				Repaint(); // Update GUI every frame if focused

		}

	

		MethodInfo isDockedMethod;
		public bool Docked{
			get{
				if(isDockedMethod == null){
					BindingFlags fullBinding = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
					isDockedMethod = typeof( EditorWindow ).GetProperty( "docked", fullBinding ).GetGetMethod( true );
				}
				return ( bool ) isDockedMethod.Invoke(this, null);
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

			Rect fullRect = new Rect( 0, 0, Screen.width, Screen.height );
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


			if(SF_Settings.ShowNodeSidebar)
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

			if( Event.current.rawType == EventType.keyUp ){
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
			if(SF_Settings.ShowNodeSidebar){
				separatorRight.MinX = (int)fullRect.width - 150;
				separatorRight.MaxX = (int)fullRect.width - 32;
				separatorRight.Draw( (int)pRect.y, (int)pRect.height );


				pRect.x += pRect.width + separatorRight.rect.width;
				pRect.width = fullRect.width - separatorRight.rect.x - separatorRight.rect.width;

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


			if(Event.current.type == EventType.repaint)
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
						preview.DrawMesh(previewRect);

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
					GUI.Label(crBottom, SF_Tools.versionStage + " " + SF_Tools.version + " - http://u3d.as/6cc");
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


		private enum MainMenuState{Main, Credits}

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
					GUILayout.Label( SF_Tools.versionStage + " " + SF_Tools.version, EditorStyles.boldLabel );
					if(outOfDate)
						GUI.color = Color.white;
				});


				if(menuState == MainMenuState.Main){
					minSize = new Vector2(500,400);
					DrawPrimaryMainMenuGUI();
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
				GUILayout.Label( "Joachim 'Acegikmo' Holm" + '\u00e9' + "r", EditorStyles.boldLabel);
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
			CreditsLine( "Tim 'Stramit' Cooper & David 'Texel' Jones", "For giving helpful tips");
			CreditsLine( "Sander 'Zerot' Homan", "For helping out stealing Unity's internal RT code");
			CreditsLine( "Carlos 'Darkcoder' Wilkes", "For helping out with various serialization issues");
			CreditsLine( "Ville 'wiliz' Mäkynen", "For helping out with the undo system");
			CreditsLine( "Daniele Giardini", "For his editor window icon script (also, check out his plugin HOTween!)");
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

		public void DrawPrimaryMainMenuGUI(){

			
			
			FlexHorizontal(()=>{
				GUI.color = new Color( 0.7f, 0.7f, 0.7f );
				if( GUILayout.Button( '\u00a9' + " Joachim 'Acegikmo' Holm" + '\u00e9' + "r", EditorStyles.miniLabel ) ) {
					Application.OpenURL("https://twitter.com/JoachimHolmer");
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
						bool created = TryCreateNewShader();
						if( created )
							return;
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
			
			FlexHorizontal(()=>{
				if( GUILayout.Button( SF_Tools.bugReportLabel, GUILayout.Height( 32f ), GUILayout.Width( 190f ) ) ) {
					Application.OpenURL( SF_Tools.bugReportURL );
				}
			});
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



		public bool TryCreateNewShader() {
			string savePath = EditorUtility.SaveFilePanel(
				"Save new shader",
				"Assets",
				"NewShader",
				"shader"
			);

			if( string.IsNullOrEmpty( savePath ) ) {
				return false;
			}

			//Debug.Log( "savePath:" + savePath );


			// So we now have the path to save it, let's save
			//StreamWriter sw;
			if( !File.Exists( savePath ) ) {
				StreamWriter sw = File.CreateText( savePath );
				sw.Write("Shader \"\" { SubShader { Pass { } } }");
				sw.Flush();
				sw.Close();
				AssetDatabase.Refresh();
			}

			// Shorten it to a relative path
			string dataPath = Application.dataPath;
			//Debug.Log( "dataPath = " + dataPath );
			string assetPath = "Assets/" + savePath.Substring( dataPath.Length + 1 );
			//Debug.Log( "assetPath = " + assetPath );

			// Assign a reference to the file
			//AssetDatabase.Refresh();
			currentShaderAsset = (Shader)AssetDatabase.LoadAssetAtPath( assetPath, typeof( Shader ) );

			if( currentShaderAsset == null ) {
				Debug.LogError( "Couldn't load shader asset" );
				Debug.Break();
				return false;
			}

			// Extract name of the file to put in the shader path
			string[] split = savePath.Split( '/' );
			currentShaderPath = split[split.Length - 1].Split( '.' )[0];
			currentShaderPath = "Shader Forge/" + currentShaderPath;

			//Debug.Log( "Shader path = " + currentShaderPath );

			//EditorUtility.DisplayDialog( "Inconvenient, right?", "Later, you'll be able to create new\nshaders without saving them first", "Ok" );


			// Make sure the preview material is using the shader
			preview.InternalMaterial.shader = currentShaderAsset;

			// That's about it for the file/asset management.
			CreateOutputNode();
			shaderEvaluator.Evaluate(); // And we're off!

			nodeView.CenterCamera();

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

			SF_Settings.AutoRecompile = GUI.Toggle( btnRect, SF_Settings.AutoRecompile, "Auto" );

			btnRect.y += 4;



			// SETTINGS EXPANSION
			if(displaySettings){
				btnRect.y += btnRect.height;
				btnRect.x = r.x + 2;
				btnRect.width = r.width / 4f;
				btnRect.x += btnRect.width;
				btnRect.width *= 2.5f;
				SF_Settings.HierarcyMove = GUI.Toggle( btnRect, SF_Settings.HierarcyMove, "Hierarchal Node Move" );
				btnRect = btnRect.MovedDown();
				SF_Settings.DrawNodePreviews = GUI.Toggle( btnRect, SF_Settings.DrawNodePreviews, "Auto-Update Node Previews" );
				btnRect = btnRect.MovedDown();
				SF_Settings.QuickPickWithWheel = GUI.Toggle( btnRect, SF_Settings.QuickPickWithWheel, "Use scroll in the quickpicker" );
				btnRect = btnRect.MovedDown();
				SF_Settings.ShowVariableSettings = GUI.Toggle( btnRect, SF_Settings.ShowVariableSettings, "Show variable name & precision" );
				btnRect = btnRect.MovedDown();
				SF_Settings.ShowNodeSidebar = GUI.Toggle( btnRect, SF_Settings.ShowNodeSidebar, "Show node browser panel" );
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

		int previewButtonHeightOffset;

		public void OnPressBackToMenuButton(){
			shaderEvaluator.SaveShaderAsset();
			Close();
			Init();
		}


		public void OnPressSettingsButton(){

		}







		public void OnShaderEvaluated() {
			statusBox.UpdateInstructionCount( preview.InternalMaterial.shader );
		}



		public void CheckForBrokenConnections() {
			foreach( SF_Node node in nodes )
				node.CheckForBrokenConnections();
		}

	}
}