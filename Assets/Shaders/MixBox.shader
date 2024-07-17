Shader "Mixbox/Mixbox URP Sample Shader"
{
    Properties
    { 
        [NoScaleOffset] _MixboxLUT ("Mixbox LUT", 2D) = "white" {} // assign "Packages/Mixbox/Textures/MixboxLUT.png"
        
        _Color1 ("Color 1", Color) = (0, 0.129, 0.522, 1) // blue
        _Color2 ("Color 2", Color) = (0.988, 0.827, 0, 1) // yellow
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MixboxLUT);
            SAMPLER(sampler_MixboxLUT);

            #include "Assets/ShaderLibrary/Mixbox.hlsl"

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings { float4 positionHCS : SV_POSITION; float2 uv : TEXCOORD0; };

            CBUFFER_START(UnityPerMaterial)
                half4 _Color1;
                half4 _Color2;
            CBUFFER_END

            float3 MixThree(float3 rgb1, float3 rgb2, float3 rgb3)
            {
                MixboxLatent z1 = MixboxRGBToLatent(rgb1);
                MixboxLatent z2 = MixboxRGBToLatent(rgb2);
                MixboxLatent z3 = MixboxRGBToLatent(rgb3);

                // mix together 30% of rgb1, 60% of rgb2, and 10% of rgb3
                MixboxLatent zMix = 0.3*z1 + 0.6*z2 + 0.1*z3;

                float3 rgbMix = MixboxLatentToRGB(zMix);

                return rgbMix;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return MixboxLerp(_Color1, _Color2, IN.uv.x);
            }


            ENDHLSL
        }
    }
}
