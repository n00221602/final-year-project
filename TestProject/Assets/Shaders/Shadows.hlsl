void SampleScreenSpaceShadow_float(float2 uv, out float shadow)
{
    #ifdef SHADERGRAPH_PREVIEW
        shadow = 1.0; // No shadow in preview
    #else
        shadow = SAMPLE_TEXTURE2D(_ScreenSpaceShadowmapTexture,
                                   sampler_ScreenSpaceShadowmapTexture,
                                   uv).r;
    #endif
}