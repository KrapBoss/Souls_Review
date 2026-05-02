using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlitchEffectURP : ScriptableRendererFeature
{
    class GlitchRenderPass : ScriptableRenderPass
    {
        public Material glitchMaterial = null;

        // RTHandle을 사용하여 텍스처를 관리합니다.
        private RTHandle tempTexture;

        public GlitchRenderPass(Material material)
        {
            this.glitchMaterial = material;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // 카메라의 설명자(Descriptor)를 가져옵니다.
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;

            // RTHandle를 재할당(ReAllocate)합니다. 기존에 할당되어 있다면 내부적으로 재사용합니다.
            RenderingUtils.ReAllocateIfNeeded(ref tempTexture, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_TempGlitchTexture");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (glitchMaterial == null) return;

            CommandBuffer cmd = CommandBufferPool.Get("Glitch Effect");

            // 현재 카메라의 컬러 타겟을 가져옵니다.
            RTHandle source = renderingData.cameraData.renderer.cameraColorTargetHandle;

            // Blit 연산: Source -> Temp -> Source
            // 최신 URP에서는 Blitter 클래스를 사용하는 것을 권장하지만, 간단하게는 아래 방식을 유지할 수 있습니다.
            Blit(cmd, source, tempTexture, glitchMaterial);
            Blit(cmd, tempTexture, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // OnCameraCleanup에서는 보통 아무것도 하지 않아도 되지만, 
            // Dispose에서 메모리를 해제하는 것이 중요합니다.
        }

        public void Dispose()
        {
            tempTexture?.Release();
        }
    }

    [System.Serializable]
    public class GlitchSettings
    {
        public Material glitchMaterial = null;
        [Range(0, 1)] public float glitchIntensity = 0.5f;
        [Range(0, 1)] public float colorIntensity = 0.5f;
    }

    public GlitchSettings settings = new GlitchSettings();
    GlitchRenderPass glitchRenderPass;

    public override void Create()
    {
        glitchRenderPass = new GlitchRenderPass(settings.glitchMaterial)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.glitchMaterial == null) return;
        renderer.EnqueuePass(glitchRenderPass);
    }

    protected override void Dispose(bool disposing)
    {
        glitchRenderPass?.Dispose();
    }
}