Shader "Liquid2d/Unlit_LiquidParticle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)          	    
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"  
            "RenderType" = "Transparent"            
        } 
        LOD 100

        Pass
        {
            Cull back
            ZWrite Off
			//Blend SrcAlpha OneMinusSrcAlpha
            Blend One OneMinusSrcAlpha
            //Blend One One
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag            
            #pragma multi_compile_instancing       

            #include "UnityCG.cginc"

            float4 _MainTex_ST;	
            sampler2D _MainTex;       
            sampler2D _CameraDepthTexture;

            UNITY_INSTANCING_BUFFER_START(Props)            
                UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)                
            UNITY_INSTANCING_BUFFER_END(Props)       
     
            struct v2f
            {
                float2 uv : TEXCOORD0;                
                float4 vertex : SV_POSITION;     
                float4 screenPos : TEXCOORD1;       
                float viewSpaceZ : TEXCOORD2;         
                UNITY_VERTEX_INPUT_INSTANCE_ID													
            };
            
            v2f vert (appdata_base v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);              
                o.vertex = UnityObjectToClipPos(v.vertex);                                                
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);        
                o.screenPos = ComputeScreenPos(o.vertex);     
                
                float3 worldSpaceDir = WorldSpaceViewDir(v.vertex);
                o.viewSpaceZ = mul(UNITY_MATRIX_V, float4(worldSpaceDir, 0.0)).z;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);      
                float depth = UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, i.screenPos));
                depth = LinearEyeDepth(depth);
                clip(depth-i.viewSpaceZ);

                fixed4 col = tex2D(_MainTex, i.uv) * UNITY_ACCESS_INSTANCED_PROP(Props, _Color);                
                col.rgb*=col.a;
                return col;
            }
            ENDCG
        }
    }
}
