Shader "Liquid2d/IE_Metaballs_Mix" {
	Properties {    
	    _MainTex ("Texture", 2D) = "white" { }    
		_MetaBallTex ("Texture", 2D) = "black" { }    
		_Cutoff ("Cutoff",Range(0,1)) = 0.2
        _AlphaCont ("AlphaCont",Range(0,1)) = 1
	}
/// <summary>
/// Multiple metaball shader.
/// 
/// Separates each particle by color and overrides it with the one specified.
/// Notice the texture that passes through this shader only looks at particles, and has a black background.
/// The core element for the color merging is the floor function, try tweaking it to achive the desired result.
///
/// Visit: www.codeartist.mx for more stuff. Thanks for checking out this example.
/// Credit: Rodrigo Fernandez Diaz
/// Contact: q_layer@hotmail.com
/// </summary>
	SubShader {
		//Tags {"Queue" = "Transparent" }
	    Pass {
	    	//Blend SrcAlpha OneMinusSrcAlpha    
			ZTest Off 
			Cull Off
			ZWrite Off
			Blend Off 
			Fog { Mode off }
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag	
			#include "UnityCG.cginc"	
			
			sampler2D _MainTex;	
			sampler2D _MetaBallTex;
			float4 _MainTex_ST;		

			fixed _Cutoff;
			fixed _AlphaCont;

			struct v2f {
			    float4  pos : SV_POSITION;
			    float2  uv : TEXCOORD0;
			};	

			v2f vert (appdata_base v){
			    v2f o;
			    o.pos = UnityObjectToClipPos (v.vertex);
			    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
			    return o;
			}	
		
			
			half4 frag (v2f i) : COLOR
			{		
				half4 metaballTex = tex2D (_MetaBallTex, i.uv); 
				half4 texcol= tex2D (_MainTex, i.uv); 			
				half4 metaBallColor = metaballTex;				
				fixed a = metaBallColor.a;											
				a = a * step(_Cutoff, a) * _AlphaCont;
				half3 finalRgb = texcol.rgb * (1 - a) + a * metaBallColor.rgb;									
				return half4(finalRgb, 1);			    
			}
			ENDCG

	    }
	}
	Fallback "VertexLit"
} 