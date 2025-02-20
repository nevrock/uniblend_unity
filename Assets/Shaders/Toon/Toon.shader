Shader "Custom/LambertLightingWithShadows"
{
    Properties
    {
        _Color ("Base Color", Color) = (1,1,1,1)
        _ShadingLevels ("Shading Levels", Range(1,5)) = 3
        _AmbientColor ("Ambient Color", Color) = (0.1, 0.1, 0.1, 1)
        _SteppingPower ("Stepping Power", Range(0.1, 5.0)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            half4 _Color;
            half _ShadingLevels;
            half4 _AmbientColor;
            half _SteppingPower;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv: TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS  : SV_POSITION;
                float2 uv: TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float4 shadowCoord : TEXCOORD3;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.shadowCoord = TransformWorldToShadowCoord(OUT.positionWS);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Get main directional light
                Light mainLight = GetMainLight(IN.shadowCoord);
                half shadowFactor = mainLight.shadowAttenuation;
                
                // Calculate dot product for toon shading
                half NdotL = dot(normalize(IN.normalWS), normalize(mainLight.direction));
                NdotL = max(0, NdotL) * shadowFactor;
                
                // Apply non-linear stepping function
                NdotL = pow(NdotL, _SteppingPower);
                half toonShade = round(NdotL * _ShadingLevels) / _ShadingLevels;

                // Base lighting
                half3 lighting = mainLight.color * toonShade;

                // Additional lights contribution
                #ifdef _ADDITIONAL_LIGHTS
                int additionalLightCount = GetAdditionalLightsCount();
                for (int i = 0; i < additionalLightCount; i++)
                {
                    Light additionalLight = GetAdditionalLight(i, IN.positionWS);
                    half3 lightDir = normalize(additionalLight.direction);
                    half NdotLAdd = dot(normalize(IN.normalWS), lightDir);
                    NdotLAdd = max(0, NdotLAdd);
                    NdotLAdd = pow(NdotLAdd, _SteppingPower);
                    half toonShadeAdd = round(NdotLAdd * _ShadingLevels) / _ShadingLevels;
                    lighting += additionalLight.color * toonShadeAdd * additionalLight.distanceAttenuation;
                }
                #endif

                // Apply ambient color
                lighting = max(lighting, _AmbientColor.rgb);

                // Calculate normalized screen space UV
                float2 normalizedScreenSpaceUV = IN.positionCS.xy / IN.positionCS.w * 0.5 + 0.5;

                half indirectAmbientOcclusion = 1.0;
                half directAmbientOcclusion = 1.0;
                AmbientOcclusionFactor aoFactor = GetScreenSpaceAmbientOcclusion(normalizedScreenSpaceUV);
                indirectAmbientOcclusion = aoFactor.indirectAmbientOcclusion;
                directAmbientOcclusion = aoFactor.directAmbientOcclusion;

                lighting *= indirectAmbientOcclusion;
                lighting *= directAmbientOcclusion;

                return half4(lighting * _Color.rgb, _Color.a);
            }

            ENDHLSL
        }

        // Shadow caster pass
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            HLSLPROGRAM

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings ShadowPassVertex(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformWorldToHClip(TransformObjectToWorld(IN.positionOS.xyz));
                return OUT;
            }

            half4 ShadowPassFragment(Varyings IN) : SV_Target
            {
                return 0;
            }

            ENDHLSL
        }
    }
}
