Shader "Custom/BlobShader"
{
    Properties
    {    
        _RimColor ("Rim Color", Color) = (0,0.5,0.5,0)
        _MainTex ("Texture", 2D) = "white" {}
        _BaseColor ("Color", Color) = (1.,1.,1.,1.)
        _NoiseTex("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        Pass
        {
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
            
            
        HLSLPROGRAM
        
        #pragma vertex vert
        #pragma fragment frag
        
        // The Core.hlsl file contains definitions of frequently used HLSL
        // macros and functions, and also contains #include references to other
        // HLSL files (for example, Common.hlsl, SpaceTransforms.hlsl, etc.).
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        

        struct Attributes
        {
            float3 positionOS : POSITION;
            
        };

         struct Varyings
        {
            float4 positionHCS  : SV_POSITION;
        }; 
    
        CBUFFER_START(UnityPerMaterial)
            half4 _BaseColor;
        CBUFFER_END;

        float rand(float n ) {
            return frac(sin(n)*43758.5453);
        }
     
        float noise( float3 x ) {
            // The noise function returns a value in the range -1.0f -> 1.0f
            float3 p = floor(x);
            float3 f = frac(x);
         
            f = f*f*(3.0-2.0*f);
            float n = p.x + p.y*57.0 + 113.0*p.z;
         
            return lerp(lerp(lerp( rand(n+0.0), rand(n+1.0),f.x),
                   lerp( rand(n+57.0), rand(n+58.0),f.x),f.y),
                   lerp(lerp( rand(n+113.0), rand(n+114.0),f.x),
                   lerp( rand(n+170.0), rand(n+171.0),f.x),f.y),f.z);
        }

        
        Varyings vert(Attributes IN)
        {
            Varyings OUT;
            OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz * (noise(IN.positionOS.xyz + _Time.x * 10.) +2.)*0.4);
            return OUT;
        }

        

        half4 frag() : SV_Target
        {
            return _BaseColor;
        }
        
        ENDHLSL
        }
        
    }
}
