using UnityEngine;
using UnityEditor;
using System;
using Holoville.HOEditorUtils; // Third party, to force inspector title icon
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;




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

		[NonSerialized]
		public static SF_Editor instance;
		[SerializeField]
		public SFN_Final materialOutput;
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
					Debug.Log("Changed outdated state to " + value);
					shaderOutdated = value;
				}
			}
		}

		[NonSerialized]
		public bool initialized = false;


		public SF_Editor() {
			//Debug.Log( "[SF_LOG] - SF_Editor CONSTRUCTOR SF_Editor()" );
			SF_Editor.instance = this;
			//Texture tabIcon = (Texture2D)Resources.LoadAssetAtPath( SF_Paths.pInterface + "icon_normal.tga", typeof( Texture2D ) );
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

		[MenuItem( "Assets/Create/Shader Forge Shader", false, 90 )]
		public static void CreateGO() {

		}

		void OnDisable(){



			if(shaderOutdated != UpToDateState.UpToDate){

				fullscreenMessage = "Saving...";
				Repaint();
				shaderEvaluator.Evaluate();
			}

			//Debug.Log("OnDisable editor window");

		}


		void OnDestroy(){

			//Debug.Log ("Destroyed the editor window");




		}

		public static void Init( Shader initShader = null ) {
			//Debug.Log( "[SF_LOG] - SF_Editor Init(" + initShader + ")" );
			SF_Editor materialEditor = (SF_Editor)EditorWindow.GetWindow( typeof( SF_Editor ) );
			SF_Editor.instance = materialEditor;
			materialEditor.InitializeInstance( initShader );
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
			AddTemplate( typeof( SFN_Add ), catArithmetic + "Add", KeyCode.A );
			AddTemplate( typeof( SFN_Subtract ), catArithmetic + "Subtract", KeyCode.S );
			AddTemplate( typeof( SFN_Multiply ), catArithmetic + "Multiply", KeyCode.M );
			AddTemplate( typeof( SFN_Divide ), catArithmetic + "Divide", KeyCode.D );
			AddTemplate( typeof( SFN_Power ), catArithmetic + "Power", KeyCode.E );
			AddTemplate( typeof( SFN_Sqrt ), catArithmetic + "Sqrt" );
			AddTemplate( typeof( SFN_Log ), catArithmetic + "Log" );
			AddTemplate( typeof( SFN_Min ), catArithmetic + "Min" );
			AddTemplate( typeof( SFN_Max ), catArithmetic + "Max" );
			AddTemplate( typeof( SFN_Abs ), catArithmetic + "Abs" );
			AddTemplate( typeof( SFN_Sign ), catArithmetic + "Sign" );
			AddTemplate( typeof( SFN_Ceil ), catArithmetic + "Ceil" );
			AddTemplate( typeof( SFN_Round ), catArithmetic + "Round" );
			AddTemplate( typeof( SFN_Floor ), catArithmetic + "Floor" );
			AddTemplate( typeof( SFN_Trunc ), catArithmetic + "Trunc" );
			AddTemplate( typeof( SFN_Step ), catArithmetic + "Step (A <= B)" );
			AddTemplate( typeof( SFN_If ), catArithmetic + "If", KeyCode.I );
			AddTemplate( typeof( SFN_Frac ), catArithmetic + "Frac" );
			AddTemplate( typeof( SFN_Fmod ), catArithmetic + "Fmod" );
			AddTemplate( typeof( SFN_Clamp ), catArithmetic + "Clamp" );
			AddTemplate( typeof( SFN_ConstantClamp ), catArithmetic + "Constant Clamp" );
			AddTemplate( typeof( SFN_Clamp01 ), catArithmetic + "Clamp 0-1" );
			AddTemplate( typeof( SFN_Lerp ), catArithmetic + "Lerp", KeyCode.L );
			AddTemplate( typeof( SFN_ConstantLerp ), catArithmetic + "Constant Lerp" );
			AddTemplate( typeof( SFN_OneMinus ), catArithmetic + "One Minus", KeyCode.O );
			AddTemplate( typeof( SFN_Negate ), catArithmetic + "Negate" );
			AddTemplate( typeof( SFN_Exp ), catArithmetic + "Exp" );
			
			string catConstVecs = "Constant Vectors/";
			AddTemplate( typeof( SFN_Vector1 ), catConstVecs+"Value", KeyCode.Alpha1 );
			AddTemplate( typeof( SFN_Vector2 ), catConstVecs+"Vector 2", KeyCode.Alpha2 );
			AddTemplate( typeof( SFN_Vector3 ), catConstVecs+"Vector 3", KeyCode.Alpha3 );
			AddTemplate( typeof( SFN_Vector4 ), catConstVecs+"Vector 4", KeyCode.Alpha4 );

			string catProps = "Properties/";
			
			AddTemplate( typeof( SFN_Tex2d ), catProps + "Texture 2D", KeyCode.T );
			AddTemplate( typeof( SFN_Tex2dAsset ), catProps + "Texture Asset" );
			AddTemplate( typeof( SFN_ValueProperty ), catProps + "Value" );
			AddTemplate( typeof( SFN_Vector4Property ), catProps + "Vector 4" );
			AddTemplate( typeof( SFN_Color ), catProps + "Color" );
			AddTemplate( typeof( SFN_Cubemap ), catProps + "Cubemap" );
			AddTemplate( typeof( SFN_Slider ), catProps + "Slider" );

			string catVecOps = "Vector Operations/";
			AddTemplate( typeof( SFN_Dot ), catVecOps+"Dot Product" );
			AddTemplate( typeof( SFN_Cross ), catVecOps + "Cross Product" );
			AddTemplate( typeof( SFN_Reflect ), catVecOps + "Reflect" );
			AddTemplate( typeof( SFN_Normalize ), catVecOps + "Normalize", KeyCode.N );
			AddTemplate( typeof( SFN_Append ), catVecOps + "Append", KeyCode.Q );
			AddTemplate( typeof( SFN_Desaturate ), catVecOps + "Desaturate" );
			AddTemplate( typeof( SFN_Distance ), catVecOps + "Distance" );
			AddTemplate( typeof( SFN_Length ), catVecOps + "Length" );
			AddTemplate( typeof( SFN_ComponentMask ), catVecOps + "Component Mask", KeyCode.C );
			AddTemplate( typeof( SFN_Transform ), catVecOps + "Transform" );
			
			string catUvOps = "UV Operations/";
			AddTemplate( typeof( SFN_Panner ), catUvOps + "Panner", KeyCode.P );
			AddTemplate( typeof( SFN_Rotator ), catUvOps + "Rotator" );
			AddTemplate( typeof( SFN_Parallax ), catUvOps + "Parallax" );
			
			string catGeoData = "Geometry Data/";
			AddTemplate( typeof( SFN_TexCoord ), catGeoData + "UV Coordinates", KeyCode.U );
			AddTemplate( typeof( SFN_ObjectPosition ), catGeoData + "Object Position");
			AddTemplate( typeof( SFN_ScreenPos ), catGeoData + "Screen Position" );
			AddTemplate( typeof( SFN_FragmentPosition ), catGeoData + "World Position" );
			AddTemplate( typeof( SFN_VertexColor ), catGeoData + "Vertex Color", KeyCode.V );
			AddTemplate( typeof( SFN_Fresnel ), catGeoData + "Fresnel", KeyCode.F );
			AddTemplate( typeof( SFN_NormalVector ), catGeoData + "Normal Dir." );
			AddTemplate( typeof( SFN_Binormal ), catGeoData + "Binormal Dir.", KeyCode.B );
			AddTemplate( typeof( SFN_Tangent ), catGeoData + "Tangent Dir." );
			AddTemplate( typeof( SFN_ViewVector ), catGeoData + "View Dir." );
			AddTemplate( typeof( SFN_ViewReflectionVector ), catGeoData + "View Refl. Dir.", KeyCode.R );

			string catLighting = "Lighting/";
			AddTemplate( typeof( SFN_LightColor ), catLighting + "Light Color" );
			AddTemplate( typeof( SFN_LightAttenuation ), catLighting + "Light Attenuation" );
			AddTemplate( typeof( SFN_AmbientLight ), catLighting + "Ambient Light" );
			AddTemplate( typeof( SFN_LightVector ), catLighting + "Light Direction" );
			AddTemplate( typeof( SFN_HalfVector ), catLighting + "Half Direction" );
			AddTemplate( typeof( SFN_LightPosition ), catLighting + "Light Position" );
			
			string catExtData = "External Data/";
			AddTemplate( typeof( SFN_Time ), catExtData + "Time" );
			AddTemplate( typeof( SFN_ViewPosition ), catExtData + "View Position" );
			AddTemplate( typeof( SFN_ProjectionParameters ), catExtData + "Projection Parameters" );
			AddTemplate( typeof( SFN_ScreenParameters ), catExtData + "Screen Parameters" );

			string catSceneData = "Scene Data/";
			AddTemplate( typeof(SFN_SceneColor), catSceneData + "Scene Color" ).MarkAsNewNode();
			

			string catMathConst = "Math Constants/";
			AddTemplate( typeof( SFN_Pi ), catMathConst + "Pi" );
			AddTemplate( typeof( SFN_Tau ), catMathConst + "Tau (2 Pi)" );
			AddTemplate( typeof( SFN_Phi ), catMathConst+"Phi" );
			AddTemplate( typeof( SFN_Root2 ), catMathConst + "Root 2" );
			AddTemplate( typeof( SFN_E ), catMathConst + "e" );

			string catTrig = "Trigonometry/";
			AddTemplate( typeof( SFN_Sin ), catTrig + "Sin" );
			AddTemplate( typeof( SFN_Cos ), catTrig + "Cos" );
			AddTemplate( typeof( SFN_Tan ), catTrig + "Tan" );
			AddTemplate( typeof( SFN_ArcSin ), catTrig + "ArcSin" );
			AddTemplate( typeof( SFN_ArcCos ), catTrig + "ArcCos" );
			AddTemplate( typeof( SFN_ArcTan ), catTrig + "ArcTan" );
			AddTemplate( typeof( SFN_ArcTan2 ), catTrig + "ArcTan2" );

		}

		public SF_EditorNodeData AddTemplate( Type type, string label, KeyCode keyCode = KeyCode.None ) {
			SF_EditorNodeData item = ScriptableObject.CreateInstance<SF_EditorNodeData>().Initialize( type.FullName, label, keyCode );
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

		public void InitializeInstance( Shader initShader = null ) {
			//Debug.Log( "[SF_LOG] - SF_Editor InitializeInstance(" + initShader + ")" );
			//this.title = ;


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
				SF_Parser.ParseNodeDataFromShader( this, initShader );
				// Make preview material use this shader
				//preview.material.shader = currentShaderAsset;
				SF_Tools.AssignShaderToMaterialAsset( ref preview.material, currentShaderAsset );
			}





			// Load data if it was set to initialize things

		}





		public SF_Node CreateOutputNode() {
			//Debug.Log ("Creating output node");
			this.materialOutput = ScriptableObject.CreateInstance<SFN_Final>().Initialize( this );//new SFN_Final();
			this.nodes.Add( materialOutput );
			return materialOutput;
		}

		public SF_Node GetNodeByID( int id ) {
			for( int i = 0; i < nodes.Count; i++ ) {
				if( nodes[i].id == id )
					return nodes[i];
			}
			return null;
		}





		void UpdateKeyHoldEvents() {
			if( nodeTemplates == null || nodeTemplates.Count == 0 ) {
				InitializeNodeTemplates();
			}

			//Debug.Log( "nodeTemplates.Count = " + nodeTemplates.Count );

			foreach( SF_EditorNodeData nData in nodeTemplates ) {

				if( nData == null ) {
					InitializeNodeTemplates();
					return;
				}
					

				if( nData.CheckHotkeyInput() ) {
					AddNode( nData );
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

		public SF_Node AddNode( SF_EditorNodeData nodeData ) {
			//Debug.Log( "nodeData: "+nodeData.type );

			SF_Node node = nodeData.CreateInstance();
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
		float prevFrameTime = 1f;

		void Update() {
			
			if( closeMe ) {
				base.Close();
				return;
			}
			
			
			float now = Now();
			fps = 1f/(now-prevFrameTime);
			
			if(fps > 30)
				return; // Wait for target FPS
			
			prevFrameTime = now;
			
			
			
			

			

			foreach( SF_Node n in nodes ) {
				n.Update();
			}
				

			if( ShaderOutdated == UpToDateState.OutdatedHard && nodeView.autoRecompile && nodeView.GetTimeSinceChanged() >= 1f) {
				shaderEvaluator.Evaluate();
			}


			if( !Application.isPlaying ) { // In order to animate shaders when game is not running

				Shader.SetGlobalVector( "_TimeEditor", new Vector4( // TODO: Make this only run if a Time node is present
						now / 20f,
						now,
						now * 2f,
						now * 3f
					)
				); 
			}

			


			//UpdateCameraZoomValue();
			if(focusedWindow == this)
				Repaint(); // Update GUI every frame if focused

		}
		
		
		public float Now(){
			TimeSpan t = ( DateTime.UtcNow - startTime );
			return (float)t.TotalSeconds;
		}




		void OnWindowResized( int deltaXsize, int deltaYsize ) {
			if(separatorRight == null)
				ForceClose();
			separatorRight.rect.x += deltaXsize;
		}

		void ForceClose() {
			closeMe = true;
			GUIUtility.ExitGUI();
		}

		string fullscreenMessage = "";
		public Rect previousPosition;
		public bool closeMe = false;
		void OnGUI() {
			//Debug.Log("SF_Editor OnGUI()");
			
			if(SF_Node.DEBUG)
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


			UpdateKeyHoldEvents();

			//UpdateCameraZoomInput();

			if( nodes != null ) {

				//foreach( SF_Node n in nodes ) {
				for( int i = 0; i < nodes.Count;i++ ) {
					SF_Node n = nodes[i];

					if( n == null ) {
						// THIS MEANS YOU STARTED UNITY WITH SF OPEN
						ForceClose();
						return;
					} else
						n.DrawConnections();
				}
					
			}

			if(separatorLeft == null){
				// THIS MEANS YOU STARTED UNITY WITH SF OPEN
				ForceClose();
				return;
			}


			if(nodeView != null)
				nodeView.selection.DrawBoxSelection();

			//EditorGUILayout.BeginHorizontal();
			//{


			//float wPreview = leftSeparator;
			//float wNodeBrowser = 130;

			Rect pRect = new Rect( fullRect );
			pRect.width = separatorLeft.rect.x;
			SF_GUI.FillBackground( pRect );
			DrawPreviewPanel( pRect );

			//pRect.x += leftWidth;
			//pRect.width = wSeparator;
			//VerticalSeparatorDraggable(ref leftWidth, pRect );
			separatorLeft.MinX = 320;
			separatorLeft.MaxX = (int)( fullRect.width / 2f - separatorLeft.rect.width );
			separatorLeft.Draw( (int)pRect.y, (int)pRect.height );
			pRect.x = separatorLeft.rect.x + separatorLeft.rect.width;



			pRect.width = separatorRight.rect.x - separatorLeft.rect.x - separatorLeft.rect.width;
			//GUI.Box( new Rect( 300, 0, 512, 32 ), pRect.ToString() );

			if( SF_Node.DEBUG ) {
				Rect r = pRect; r.width = 256; r.height = 16;
				for( int i = 0; i < nodes.Count; i++ ) {
					GUI.Label( r, "Node[" + i + "] at {" + nodes[i].rect.x + ", " + nodes[i].rect.y + "}", EditorStyles.label );// nodes[i]
					r.y += r.height;
				}
			}

			

			nodeView.OnLocalGUI( pRect );



			

			//pRect.x += pRect.width;
			//pRect.width = wSeparator;
			//VerticalSeparatorDraggable(ref rightWidth, pRect );

			separatorRight.MinX = (int)fullRect.width - 150;
			separatorRight.MaxX = (int)fullRect.width - 32;
			separatorRight.Draw( (int)pRect.y, (int)pRect.height );


			pRect.x += pRect.width + separatorRight.rect.width;
			pRect.width = fullRect.width - separatorRight.rect.x - separatorRight.rect.width;
			SF_GUI.FillBackground( pRect );
			nodeBrowser.OnLocalGUI( pRect );

			DrawTooltip();


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
			GUI.FocusControl("defocus");
			if( deselectNodes )
				nodeView.selection.DeselectAll();
		}


		public bool DraggingAnySeparator() {
			return separatorLeft.dragging || separatorRight.dragging;
		}

		
		
		
		

		
		public void DrawMainMenu() {
			
			
			
			
			

			GUILayout.BeginVertical();
			{
				GUILayout.FlexibleSpace();


				GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();
				{
					GUILayout.Label( SF_GUI.Logo );
					GUILayout.Label( SF_Tools.versionStage + " " + SF_Tools.version, EditorStyles.boldLabel );
				}
				GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();


				GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();
				{
					GUI.color = new Color( 0.7f, 0.7f, 0.7f );
					GUILayout.Label( '\u00a9' + " Joachim 'Acegikmo' Holm" + '\u00e9' + "r", EditorStyles.miniLabel );
					GUI.color = Color.white;
				}
				GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();

				EditorGUILayout.Separator();

				GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();
				{
					if( GUILayout.Button(SF_Tools.manualLabel , GUILayout.Height( 32f ), GUILayout.Width( 190f ) ) ) {
						Application.OpenURL( SF_Tools.manualURL );
					}
				}
				GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();
				

				GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();
				{
					
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
				}
				GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();
				{
					if( GUILayout.Button( "Polycount thread" ) ) {
						Application.OpenURL( "http://www.polycount.com/forum/showthread.php?t=123439" );
					}
					if( GUILayout.Button( "Unity thread" ) ) {
						Application.OpenURL( "http://forum.unity3d.com/threads/191595-Shader-Forge-A-visual-node-based-shader-editor" );
					}
					if( GUILayout.Button( SF_Tools.featureListLabel ) ) {
						Application.OpenURL( SF_Tools.featureListURL );
					}
				}
				GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();
				{
					if( GUILayout.Button( SF_Tools.bugReportLabel, GUILayout.Height( 32f ), GUILayout.Width( 190f ) ) ) {
						Application.OpenURL( SF_Tools.bugReportURL );
					}
				}
				GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();
				
				

				

				GUILayout.FlexibleSpace();
			}
			GUILayout.EndVertical();


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
				File.CreateText( savePath );
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
			preview.material.shader = currentShaderAsset;

			// That's about it for the file/asset management.
			CreateOutputNode();
			shaderEvaluator.Evaluate(); // And we're off!

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



		public void DrawPreviewPanel( Rect r ) {
			// Left side shader preview

			Rect logoRect = new Rect( 5, 0, SF_GUI.Logo.width, SF_GUI.Logo.height );

			GUI.DrawTexture( logoRect, SF_GUI.Logo );
			GUI.Box( new Rect(203,10,128,19), SF_Tools.versionStage+" "+SF_Tools.version, versionStyle );
			int previewOffset = preview.OnGUI( SF_GUI.Logo.height, (int)r.width );
			int statusBoxOffset = statusBox.OnGUI( previewOffset, (int)r.width );
			ps.OnLocalGUI(statusBoxOffset, (int)r.width );
			if( SF_Node.DEBUG ) {
				GUILayout.Label( "Node count: " + nodes.Count );
			}

		}






		public void OnShaderEvaluated() {
			statusBox.UpdateInstructionCount( preview.material.shader );
		}



		public void CheckForBrokenConnections() {
			foreach( SF_Node node in nodes )
				node.CheckForBrokenConnections();
		}

	}
}