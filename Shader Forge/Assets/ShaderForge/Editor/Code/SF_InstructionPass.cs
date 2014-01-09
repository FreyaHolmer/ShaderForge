using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace ShaderForge{


	public class SFIns_PassPlat {
		public RenderPlatform plat;
		public SF_MinMax vert = new SF_MinMax( 0, 0 );
		public SF_MinMax frag = new SF_MinMax( 0, 0 );
		public SF_MinMax vTex = new SF_MinMax( 0, 0 );
		public SF_MinMax fTex = new SF_MinMax( 0, 0 );

		public SFIns_PassPlat(RenderPlatform plat){
			this.plat = plat;
		}

	}


	public class SFIns_Pass {


		public List<SFIns_PassPlat> plats = new List<SFIns_PassPlat>(){
			new SFIns_PassPlat(RenderPlatform.d3d9),
			new SFIns_PassPlat(RenderPlatform.d3d11),
			new SFIns_PassPlat(RenderPlatform.opengl),
			new SFIns_PassPlat(RenderPlatform.gles),
			new SFIns_PassPlat(RenderPlatform.xbox360),
			new SFIns_PassPlat(RenderPlatform.ps3),
			new SFIns_PassPlat(RenderPlatform.flash)
		};

		public void Parse(ShaderProgram prog, string line, bool ignoreMin ) {

			//Debug.Log("Parsing instruction count: line = " + line);

			// String style:
			// "//   opengl - ALU: 29 to 35"
			// "//   opengl - ALU: 7 to 15, TEX: 1 to 3"

			string[] split = line.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);

			if( split.Length != 7 && split.Length != 11 && split.Length != 15 ) {
				Debug.LogError( "Error parsing instruction count. Line did not have 7, 11 or 15 elements [" + line + "]. Length is: " + split.Length );
				return;
			}

			bool hasTex = ( split.Length == 11 );

			int enumID = (int)Enum.Parse( typeof( RenderPlatform ), split[1] );



			if( prog == ShaderProgram.Frag ) {
				if( !ignoreMin )
					plats[enumID].frag.min = IntParse( split[4] );
				plats[enumID].frag.max = IntParse( split[6] );
				if( hasTex ) {
					if( !ignoreMin )
						plats[enumID].fTex.min = IntParse( split[8] );
					plats[enumID].fTex.max = IntParse( split[10] );
				}
			} else if( prog == ShaderProgram.Vert ) {
				if( !ignoreMin )
					plats[enumID].vert.min = IntParse( split[4] );
				plats[enumID].vert.max = IntParse( split[6] );
				if( hasTex ) {
					if( !ignoreMin )
						plats[enumID].vTex.min = IntParse( split[8] );
					plats[enumID].vTex.max = IntParse( split[10] );
				}
			} else {
				Debug.LogError( "Tried to parse things in invalid program [" + prog + "]" );
			}
			
			//Debug.Log("Instr: " + split[1] + " "+ prog + " " + line + " ig: " + ignoreMin);


		}

		public int IntParse( string s ) {
			s = s.Replace(",","");
			return int.Parse(s);
		}

		

	}
	
}