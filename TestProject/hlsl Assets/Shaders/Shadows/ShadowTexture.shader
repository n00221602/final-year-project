Shader "ShadowTextureShader"
{
    Properties
    {
        _ShadowTex("Shadow Texture", 2D) = "white" {}
        _ShadowColor("Shadow Color", Color) = (0, 0, 0, 1)
        _Tiling("Tiling", Vector) = (1, 1, 0, 0)
        _Threshold("Shadow Threshold", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        
        Pass
        {
            Name "Shadow Texture Application"
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag
            #pragma multi_compile_fragment _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            TEXTURE2D(_ShadowTex);
            SAMPLER(sampler_ShadowTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _ShadowColor;
                float2 _Tiling;
                float _Threshold;
            CBUFFER_END

            half4 frag(Varyings input) : SV_Target
            {
                // Sample the camera color
                half4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, input.texcoord);
                
                // Get depth and reconstruct world position
                float depth = SampleSceneDepth(input.texcoord);
                float3 worldPos = ComputeWorldSpacePosition(input.texcoord, depth, UNITY_MATRIX_I_VP);
                
                // Calculate shadow attenuation
                float4 shadowCoord = TransformWorldToShadowCoord(worldPos);
                Light mainLight = GetMainLight(shadowCoord);
                float shadowAttenuation = mainLight.shadowAttenuation;
                
                // If in shadow (attenuation below threshold), apply texture
                if (shadowAttenuation < _Threshold)
                {
                    // Sample shadow texture with tiling
                    float2 shadowUV = input.texcoord * _Tiling;
                    half4 shadowTexColor = SAMPLE_TEXTURE2D(_ShadowTex, sampler_ShadowTex, shadowUV);
                    
                    // Blend shadow texture with shadow color
                    half3 shadowEffect = shadowTexColor.rgb * _ShadowColor.rgb;
                    
                    // Apply shadow effect based on shadow strength
                    float shadowStrength = 1.0 - shadowAttenuation;
                    color.rgb = lerp(color.rgb, color.rgb * shadowEffect, shadowStrength);
                }
                
                return color;
            }
            ENDHLSL
        }
    }
}