// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Sprite/Fold"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
		_FoldAngle("FoldAngle", Float) = 0
		_FoldDark("FoldDark", Range( 0 , 1)) = 0.7
		[Toggle]_Tearing("Tearing", Float) = 0
		_TearMask("TearMask", 2D) = "white" {}
		[Toggle]_UseGreenMask("UseGreenMask", Float) = 0
		[Toggle]_DEBUG("DEBUG", Float) = 0

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
			uniform float _DEBUG;
			uniform float _Tearing;
			uniform float3 DRAG_POINT;
			uniform float _FoldAngle;
			uniform float _UseGreenMask;
			uniform sampler2D _TearMask;
			uniform float _FoldDark;

			
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
				float3 ase_worldPos = IN.ase_texcoord1.xyz;
				float2 appendResult39 = (float2(( ase_worldPos - DRAG_POINT ).xy));
				float temp_output_29_0 = ( _FoldAngle * ( UNITY_PI / 180.0 ) );
				float2 appendResult33 = (float2(sin( temp_output_29_0 ) , cos( temp_output_29_0 )));
				float2 angle36 = appendResult33;
				float dotResult40 = dot( appendResult39 , angle36 );
				float4 tex2DNode72 = tex2D( _TearMask, texCoord3 );
				float mask74 = (( _UseGreenMask )?( tex2DNode72.g ):( tex2DNode72.r ));
				float lerpResult79 = lerp( step( dotResult40 , 0.0 ) , 0.0 , ( 1.0 - mask74 ));
				clip( (( _DEBUG )?( 1.0 ):( (( _Tearing )?( lerpResult79 ):( 0.0 )) )) - 0.5);
				float4 break44 = tex2D( _MainTex, texCoord3 );
				float3 appendResult45 = (float3(break44.r , break44.g , break44.b));
				float lerpResult54 = lerp( _FoldDark , 1.0 , saturate( pow( -dotResult40 , 0.5 ) ));
				float lerpResult77 = lerp( lerpResult54 , 1.0 , ( 1.0 - mask74 ));
				float shadow69 = lerpResult77;
				float4 appendResult46 = (float4(( appendResult45 * shadow69 ) , break44.a));
				
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
0;118.4;1520;684.6;803.176;134.5249;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;35;-2189.454,518.3019;Inherit;False;1093.426;554.3004;Comment;8;33;34;26;29;31;30;28;36;GetVector;1,1,1,1;0;0
Node;AmplifyShaderEditor.PiNode;30;-2139.454,759.7344;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-2104.91,616.3018;Inherit;False;Property;_FoldAngle;FoldAngle;0;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;31;-1951.453,759.7344;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;180;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-1809.453,625.7344;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;26;-1617.509,568.3019;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-1010.741,-299.8348;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CosOpNode;34;-1624.453,748.7344;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;20;-1340.823,-89.22693;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;22;-1349.42,76.83759;Inherit;False;Global;DRAG_POINT;DRAG_POINT;1;0;Create;True;0;0;0;False;0;False;0,0,0;2.733727,5.574024,1.3;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;72;-742.2351,-686.8627;Inherit;True;Property;_TearMask;TearMask;3;0;Create;True;0;0;0;False;0;False;-1;None;17b78c25296033a44868bd92734def64;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;33;-1447.453,617.7344;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ToggleSwitchNode;73;-381.9703,-663.6083;Inherit;False;Property;_UseGreenMask;UseGreenMask;4;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;38;-1141.679,-10.12662;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;36;-1315.29,614.1981;Inherit;False;angle;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;74;-97.28537,-662.2202;Inherit;False;mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;39;-984.6787,-8.126617;Inherit;False;FLOAT2;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;37;-1011.679,273.8734;Inherit;False;36;angle;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;68;-594.3892,812.2878;Inherit;False;1403.197;524.5459;Comment;9;54;52;41;55;51;69;77;76;81;Apply ShadowMask;1,1,1,1;0;0
Node;AmplifyShaderEditor.DotProductOpNode;40;-796.6787,109.8734;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;78;-844.018,416.673;Inherit;True;74;mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;80;-608.2045,422.2781;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;51;-544.3892,959.0353;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;6;-614.1735,109.8163;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;79;-488.9413,254.442;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;55;-396.8046,1090.807;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;2;-741.7513,-402.6411;Inherit;False;0;0;_MainTex;Shader;False;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;76;157.543,1094.647;Inherit;False;74;mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;41;-276.0056,965.5242;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-137.0852,862.2878;Inherit;False;Property;_FoldDark;FoldDark;1;0;Create;True;0;0;0;False;0;False;0.7;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;71;-331.2661,156.8036;Inherit;False;Property;_Tearing;Tearing;2;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;81;337.6221,1101.948;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;82;-110.5183,320.1092;Inherit;False;Property;_DEBUG;DEBUG;5;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;4;-740.7015,-325.7423;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;54;160.5303,919.7072;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;77;353.9393,935.2053;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClipNode;13;-22.20389,136.1881;Inherit;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;44;271.0914,93.59668;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RegisterLocalVarNode;69;563.0995,966.071;Inherit;False;shadow;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;45;480.3535,-29.054;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;70;462.3626,100.4516;Inherit;False;69;shadow;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;634.8148,24.91425;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;46;806.0751,142.733;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.VertexColorNode;7;1026.11,-11.74754;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;1254.497,119.1779;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;1404.981,119.4208;Float;False;True;-1;2;ASEMaterialInspector;0;6;Sprite/Fold;0f8ba0101102bb14ebf021ddadce9b49;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;False;True;3;1;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;31;0;30;0
WireConnection;29;0;28;0
WireConnection;29;1;31;0
WireConnection;26;0;29;0
WireConnection;34;0;29;0
WireConnection;72;1;3;0
WireConnection;33;0;26;0
WireConnection;33;1;34;0
WireConnection;73;0;72;1
WireConnection;73;1;72;2
WireConnection;38;0;20;0
WireConnection;38;1;22;0
WireConnection;36;0;33;0
WireConnection;74;0;73;0
WireConnection;39;0;38;0
WireConnection;40;0;39;0
WireConnection;40;1;37;0
WireConnection;80;0;78;0
WireConnection;51;0;40;0
WireConnection;6;0;40;0
WireConnection;79;0;6;0
WireConnection;79;2;80;0
WireConnection;55;0;51;0
WireConnection;41;0;55;0
WireConnection;71;1;79;0
WireConnection;81;0;76;0
WireConnection;82;0;71;0
WireConnection;4;0;2;0
WireConnection;4;1;3;0
WireConnection;54;0;52;0
WireConnection;54;2;41;0
WireConnection;77;0;54;0
WireConnection;77;2;81;0
WireConnection;13;0;4;0
WireConnection;13;1;82;0
WireConnection;44;0;13;0
WireConnection;69;0;77;0
WireConnection;45;0;44;0
WireConnection;45;1;44;1
WireConnection;45;2;44;2
WireConnection;47;0;45;0
WireConnection;47;1;70;0
WireConnection;46;0;47;0
WireConnection;46;3;44;3
WireConnection;8;0;7;0
WireConnection;8;1;46;0
WireConnection;1;0;8;0
ASEEND*/
//CHKSM=5C7CE577A193047F14AF6BEDE2C044AA216215EA