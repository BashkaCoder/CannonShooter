Shader "Flexus Test/Decal Stamp"
{
    Properties
    {
        _PrevTex ("Previous", 2D) = "black" {}
        _StampTex ("Stamp", 2D) = "white" {}
        _StampUV ("Stamp UV", Vector) = (0.5, 0.5, 0, 0)
        _StampUvSize ("Stamp UV Size", Vector) = (0.05, 0.05, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct attributes
            {
                float4 position_os : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_PrevTex);
            SAMPLER(sampler_PrevTex);
            TEXTURE2D(_StampTex);
            SAMPLER(sampler_StampTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _StampUV;
                float4 _StampUvSize;
            CBUFFER_END

            varyings vert(attributes input)
            {
                varyings output;
                output.positionHCS = TransformObjectToHClip(input.position_os.xyz);
                output.uv = input.uv;
                return output;
            }

            half4 frag(varyings input) : SV_Target
            {
                half4 previous = SAMPLE_TEXTURE2D(_PrevTex, sampler_PrevTex, input.uv);
                float2 local = (input.uv - _StampUV.xy) / _StampUvSize.xy + 0.5;

                if (local.x < 0 || local.x > 1 || local.y < 0 || local.y > 1)
                {
                    return previous;
                }

                half4 stamp = SAMPLE_TEXTURE2D(_StampTex, sampler_StampTex, local);
                return lerp(previous, stamp, stamp.a);
            }
            ENDHLSL
        }
    }
}
