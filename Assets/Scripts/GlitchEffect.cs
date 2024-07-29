using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlitchEffectURP : ScriptableRendererFeature
{
    class GlitchRenderPass : ScriptableRenderPass
    {
        public Material glitchMaterial = null;
        public RenderTargetIdentifier source;
        private RenderTargetHandle tempTexture;
        

        public GlitchRenderPass(Material material)
        {
            this.glitchMaterial = material;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;
            cmd.GetTemporaryRT(tempTexture.id, descriptor, FilterMode.Bilinear);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("Glitch Effect");

            Blit(cmd, source, tempTexture.Identifier(), glitchMaterial);
            Blit(cmd, tempTexture.Identifier(), source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tempTexture.id);
        }
    }

    [System.Serializable]
    public class GlitchSettings
    {
        public Material glitchMaterial = null;
        [Range(0, 1)]
        public float glitchIntensity = 0.5f;
        [Range(0, 1)]
        public float colorIntensity = 0.5f;
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
        if (settings.glitchMaterial == null)
        {
            Debug.LogWarningFormat("Missing Glitch Material. {0} pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
            return;
        }
        glitchRenderPass.source = renderer.cameraColorTarget;
        renderer.EnqueuePass(glitchRenderPass);
    }
}
