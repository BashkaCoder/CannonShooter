Shader "Flexus Test/Decal Surface"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.94, 0.9, 0.8, 1)
        _DecalTex ("Decal Texture", 2D) = "black" {}
        _DecalAxis ("Decal Axis", Vector) = (0, 0, 1, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 decalUv : TEXCOORD0;
                float decalMask : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _DecalAxis;
            CBUFFER_END

            TEXTURE2D(_DecalTex);
            SAMPLER(sampler_DecalTex);

            Varyings vert(attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);

                float3 axis = abs(_DecalAxis.xyz);
                float3 absNormal = abs(normalize(input.normalOS));
                output.decalMask = dot(absNormal, axis) >= 0.75 ? 1.0 : 0.0;

                if (axis.y >= axis.x && axis.y >= axis.z)
                {
                    output.decalUv = input.positionOS.xz + 0.5;
                }
                else if (axis.x >= axis.z)
                {
                    output.decalUv = input.positionOS.zy + 0.5;
                }
                else
                {
                    output.decalUv = input.positionOS.xy + 0.5;
                }

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 decal = SAMPLE_TEXTURE2D(_DecalTex, sampler_DecalTex, input.decalUv);
                decal.a *= input.decalMask;
                half3 color = lerp(_BaseColor.rgb, decal.rgb, decal.a);
                return half4(color, 1);
            }
            ENDHLSL
        }
    }
}
