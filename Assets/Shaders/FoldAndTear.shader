// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Sprite/FoldAndTear"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
		_FoldAngle("FoldAngle", Float) = 0
		_FoldDark("FoldDark", Range( 0 , 1)) = 0.7
		_TearMask("TearMask", 2D) = "white" {}
		[Toggle]_Tearing("Tearing", Float) = 0
		[Toggle]_UseGreenMask("UseGreenMask", Float) = 0
		[Toggle]_AfterTeared("AfterTeared", Float) = 0
		_SmileTex("SmileTex", 2D) = "white" {}

	}

	SubShader
	{
		LOD 0

		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		
		
		Pass
		{
		CGPROGRAM
			
			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"
			#define ASE_NEEDS_FRAG_COLOR


			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord1 : TEXCOORD1;
			};
			
			uniform fixed4 _Color;
			uniform float _EnableExternalAlpha;
			uniform sampler2D _MainTex;
			uniform sampler2D _AlphaTex;
			uniform sampler2D _SmileTex;
			uniform float TIME_FADE;
			uniform float _Tearing;
			uniform float3 DRAG_POINT;
			uniform float _FoldAngle;
			uniform float _UseGreenMask;
			uniform sampler2D _TearMask;
			uniform float _FoldDark;
			uniform float _AfterTeared;
			uniform int HOLLOW;
			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			

			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				float3 ase_worldPos = mul(unity_ObjectToWorld, IN.vertex).xyz;
				OUT.ase_texcoord1.xyz = ase_worldPos;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				OUT.ase_texcoord1.w = 0;
				
				IN.vertex.xyz +=  float3(0,0,0) ; 
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if ETC1_EXTERNAL_ALPHA
				// get the color from an external texture (usecase: Alpha support for ETC1 on android)
				fixed4 alpha = tex2D (_AlphaTex, uv);
				color.a = lerp (color.a, alpha.r, _EnableExternalAlpha);
#endif //ETC1_EXTERNAL_ALPHA

				return color;
			}
			
			fixed4 frag(v2f IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float2 texCoord3 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float4 tex2DNode4 = tex2D( _MainTex, texCoord3 );
				float temp_output_91_0 = ( ( TIME_FADE * 2.0 ) - 1.0 );
				float2 texCoord90 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float simplePerlin2D93 = snoise( texCoord90*2.0 );
				simplePerlin2D93 = simplePerlin2D93*0.5 + 0.5;
				float smoothstepResult94 = smoothstep( temp_output_91_0 , ( temp_output_91_0 + 1.0 ) , simplePerlin2D93);
				float smileFade95 = smoothstepResult94;
				float4 lerpResult98 = lerp( tex2D( _SmileTex, texCoord3 ) , tex2DNode4 , smileFade95);
				float3 ase_worldPos = IN.ase_texcoord1.xyz;
				float2 appendResult39 = (float2(( ase_worldPos - DRAG_POINT ).xy));
				float temp_output_29_0 = ( _FoldAngle * ( UNITY_PI / 180.0 ) );
				float2 appendResult33 = (float2(sin( temp_output_29_0 ) , cos( temp_output_29_0 )));
				float2 angle36 = appendResult33;
				float dotResult40 = dot( appendResult39 , angle36 );
				float4 tex2DNode56 = tex2D( _TearMask, texCoord3 );
				float mask57 = (( _UseGreenMask )?( tex2DNode56.g ):( tex2DNode56.r ));
				float lerpResult59 = lerp( 1.0 , step( dotResult40 , 0.0 ) , mask57);
				clip( (( _Tearing )?( lerpResult59 ):( 1.0 )) - 0.5);
				float4 break44 = lerpResult98;
				float3 appendResult45 = (float3(break44.r , break44.g , break44.b));
				float lerpResult54 = lerp( _FoldDark , 1.0 , saturate( pow( -dotResult40 , 0.5 ) ));
				float lerpResult63 = lerp( lerpResult54 , 1.0 , ( 1.0 - mask57 ));
				float shadow69 = lerpResult63;
				float texAlpha83 = tex2DNode4.a;
				float temp_output_85_0 = ( ( 1.0 - mask57 ) * texAlpha83 );
				float lerpResult87 = lerp( (( _AfterTeared )?( temp_output_85_0 ):( break44.a )) , temp_output_85_0 , (float)HOLLOW);
				float4 appendResult46 = (float4(( appendResult45 * shadow69 ) , lerpResult87));
				
				fixed4 c = ( IN.color * appendResult46 );
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18935
-1788;194;1518;738;2158.152;733.3044;2.707626;True;False
Node;AmplifyShaderEditor.CommentaryNode;35;-2189.454,518.3019;Inherit;False;1093.426;554.3004;Comment;8;33;34;26;29;31;30;28;36;GetVector;1,1,1,1;0;0
Node;AmplifyShaderEditor.PiNode;30;-2139.454,759.7344;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-2104.91,616.3018;Inherit;False;Property;_FoldAngle;FoldAngle;0;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;31;-1951.453,759.7344;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;180;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-1809.453,625.7344;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;106;988.6466,804.6523;Inherit;False;1943.989;459.386;Comment;8;95;94;93;92;91;90;89;88;DissolveMask;1,1,1,1;0;0
Node;AmplifyShaderEditor.SinOpNode;26;-1617.509,568.3019;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CosOpNode;34;-1624.453,748.7344;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;22;-1210.797,177.4065;Inherit;False;Global;DRAG_POINT;DRAG_POINT;1;0;Create;True;0;0;0;False;0;False;0,0,0;13.572,-9.271042,5;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;33;-1447.453,617.7344;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldPosInputsNode;20;-1202.2,11.34202;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;88;1038.647,1116.184;Inherit;False;Global;TIME_FADE;TIME_FADE;5;0;Create;True;0;0;0;False;0;False;0;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-2381.421,-712.3756;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;1262.289,1118.924;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;36;-1315.29,614.1981;Inherit;False;angle;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;38;-1003.055,90.44231;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;39;-846.0567,92.44232;Inherit;False;FLOAT2;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;56;-1757.026,-1092.319;Inherit;True;Property;_TearMask;TearMask;2;0;Create;True;0;0;0;False;0;False;-1;None;70c11798c6b0d15418f7278c162920bc;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;37;-867.6208,227.6661;Inherit;False;36;angle;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;90;1066.986,860.6014;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;91;1406.615,1117.55;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;93;1312.54,854.6524;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;1,1;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;68;-594.3892,812.2878;Inherit;False;1403.197;524.5459;Comment;9;63;54;61;52;41;60;55;51;69;Apply ShadowMask;1,1,1,1;0;0
Node;AmplifyShaderEditor.ToggleSwitchNode;79;-1328.105,-1069.871;Inherit;False;Property;_UseGreenMask;UseGreenMask;4;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;92;1711.932,1113.42;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;40;-674.3652,139.7723;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;57;-895.4195,-1064.483;Inherit;False;mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;51;-544.3892,959.0353;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;94;1859.36,861.1915;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;58;-524.7493,294.1042;Inherit;False;57;mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;6;-480.2592,144.7152;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;95;2106.134,857.3146;Inherit;True;smileFade;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;55;-396.8046,1090.807;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;2;-1458.242,-359.655;Inherit;False;0;0;_MainTex;Shader;False;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;97;-1389.745,-626.7878;Inherit;True;Property;_SmileTex;SmileTex;6;0;Create;True;0;0;0;False;0;False;-1;None;420b63dda9b1d69438f5a866610f4997;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;41;-276.0056,965.5242;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;4;-1270.277,-349.379;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;60;164.2952,1133.15;Inherit;False;57;mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;99;-396.7882,-245.0917;Inherit;False;95;smileFade;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-137.0852,862.2878;Inherit;False;Property;_FoldDark;FoldDark;1;0;Create;True;0;0;0;False;0;False;0.7;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;59;-318.576,252.6553;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;61;163.2952,1056.15;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;98;-365.3605,-370.8965;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;81;22.707,361.3615;Inherit;False;57;mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;54;160.5303,919.7072;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;72;-322.3463,150.0431;Inherit;False;Property;_Tearing;Tearing;3;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;83;-890.2426,-220.3661;Inherit;False;texAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClipNode;13;-7.20389,133.1881;Inherit;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;63;393.3556,970.0953;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;82;194.9276,367.0802;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;84;189.158,463.9243;Inherit;False;83;texAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;355.158,408.9243;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;69;563.0995,966.071;Inherit;False;shadow;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;44;239.0913,133.5967;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.IntNode;86;731.7609,513.0228;Inherit;False;Global;HOLLOW;HOLLOW;6;0;Create;True;0;0;0;False;0;False;0;0;False;0;1;INT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;80;516.0134,281.5939;Inherit;False;Property;_AfterTeared;AfterTeared;5;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;45;480.3535,-29.054;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;70;462.3626,100.4516;Inherit;False;69;shadow;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;634.8148,24.91425;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;87;728.9528,386.7236;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;7;1026.11,-11.74754;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;46;806.0751,142.733;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;1254.497,119.1779;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;1404.981,119.4208;Float;False;True;-1;2;ASEMaterialInspector;0;6;Sprite/FoldAndTear;0f8ba0101102bb14ebf021ddadce9b49;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;False;True;3;1;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;31;0;30;0
WireConnection;29;0;28;0
WireConnection;29;1;31;0
WireConnection;26;0;29;0
WireConnection;34;0;29;0
WireConnection;33;0;26;0
WireConnection;33;1;34;0
WireConnection;89;0;88;0
WireConnection;36;0;33;0
WireConnection;38;0;20;0
WireConnection;38;1;22;0
WireConnection;39;0;38;0
WireConnection;56;1;3;0
WireConnection;91;0;89;0
WireConnection;93;0;90;0
WireConnection;79;0;56;1
WireConnection;79;1;56;2
WireConnection;92;0;91;0
WireConnection;40;0;39;0
WireConnection;40;1;37;0
WireConnection;57;0;79;0
WireConnection;51;0;40;0
WireConnection;94;0;93;0
WireConnection;94;1;91;0
WireConnection;94;2;92;0
WireConnection;6;0;40;0
WireConnection;95;0;94;0
WireConnection;55;0;51;0
WireConnection;97;1;3;0
WireConnection;41;0;55;0
WireConnection;4;0;2;0
WireConnection;4;1;3;0
WireConnection;59;1;6;0
WireConnection;59;2;58;0
WireConnection;61;0;60;0
WireConnection;98;0;97;0
WireConnection;98;1;4;0
WireConnection;98;2;99;0
WireConnection;54;0;52;0
WireConnection;54;2;41;0
WireConnection;72;1;59;0
WireConnection;83;0;4;4
WireConnection;13;0;98;0
WireConnection;13;1;72;0
WireConnection;63;0;54;0
WireConnection;63;2;61;0
WireConnection;82;0;81;0
WireConnection;85;0;82;0
WireConnection;85;1;84;0
WireConnection;69;0;63;0
WireConnection;44;0;13;0
WireConnection;80;0;44;3
WireConnection;80;1;85;0
WireConnection;45;0;44;0
WireConnection;45;1;44;1
WireConnection;45;2;44;2
WireConnection;47;0;45;0
WireConnection;47;1;70;0
WireConnection;87;0;80;0
WireConnection;87;1;85;0
WireConnection;87;2;86;0
WireConnection;46;0;47;0
WireConnection;46;3;87;0
WireConnection;8;0;7;0
WireConnection;8;1;46;0
WireConnection;1;0;8;0
ASEEND*/
//CHKSM=6F35D71A622907FBA97107C8A951D3329C5C328F