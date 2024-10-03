Shader "Custom/MixboxCollision"
{
    Properties
    {
        [NoScaleOffset] _MixboxLUT ("Mixbox LUT", 2D) = "white" {} // assign "Packages/Mixbox/Textures/MixboxLUT.png"
        
        _Color1 ("Color 1", Color) = (0, 0.129, 0.522, 1) // blue
        _Color2 ("Color 2", Color) = (1, 1, 1, 1) // yellow
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert  // Corrected the vertex function name
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            sampler2D _MixboxLUT;

            #include "Assets/ShaderLibrary/Mixbox.hlsl"

            fixed4 _Color1;
            fixed4 _Color2;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv: TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                bool isColor2White = all(_Color2 == float4(1,1,1,1));  // Component-wise comparison
                i.uv.x = 0.5f;
                fixed4 mixedColor = isColor2White ? MixboxLerp(_Color1, _Color1, i.uv.x) :
                                                    MixboxLerp(_Color1, _Color2, i.uv.x);
  
                return  mixedColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
