using Silk.NET.Maths;
using Silk.NET.WebGPU;
using TermSharp.Windowing;

namespace TermSharp.Rendering;

public class RenderSurface
{
    private unsafe Surface* _surface;

    public unsafe void CreateSurface(AppWindow window, GpuContext gpuContext)
    {
        Surface* surface = window.WindowSource.CreateWebGPUSurface(gpuContext.WebGpu, gpuContext.Instance);
        _surface = surface;
    }

    public unsafe void ConfigureSurface(GpuContext gpuContext, Vector2D<int> size)
    {
        if (_surface is null)
            return;
        if (size.X == 0 || size.Y == 0)
            return;

        SurfaceConfiguration config = new SurfaceConfiguration
        (
            device: gpuContext.Device,
            format: TextureFormat.Bgra8Unorm,
            usage: TextureUsage.RenderAttachment,
            alphaMode: CompositeAlphaMode.Auto,
            width: (uint)size.X,
            height: (uint)size.Y,
            presentMode: PresentMode.Fifo
        );

        gpuContext.WebGpu.SurfaceConfigure(_surface, ref config);
    }

    public unsafe void RenderFrame(GpuContext gpuContext)
    {
        WebGPU webGpu = gpuContext.WebGpu;

        SurfaceTexture surfaceTexture = new SurfaceTexture();

        webGpu.SurfaceGetCurrentTexture(_surface, &surfaceTexture);

        if (surfaceTexture.Status != SurfaceGetCurrentTextureStatus.Success)
            return;

        TextureView* textureView = webGpu.TextureCreateView(surfaceTexture.Texture, null);
        CommandEncoder* commandEncoder = webGpu.DeviceCreateCommandEncoder(gpuContext.Device, null);

        RenderPassColorAttachment colorAttachment = new RenderPassColorAttachment
        (
            view: textureView,
            loadOp: LoadOp.Clear,
            storeOp: StoreOp.Store,
            clearValue: new Color(0.10, 0.10, 0.10, 1.0)
        );

        RenderPassDescriptor passDescriptor = new RenderPassDescriptor
        (
            colorAttachmentCount: 1,
            colorAttachments: &colorAttachment
        );

        RenderPassEncoder* renderPassEncoder = webGpu.CommandEncoderBeginRenderPass(commandEncoder, &passDescriptor);
        webGpu.RenderPassEncoderEnd(renderPassEncoder);
        CommandBuffer* commandBuffer = webGpu.CommandEncoderFinish(commandEncoder, null);
        webGpu.QueueSubmit(gpuContext.Queue, 1, &commandBuffer);
        
        webGpu.SurfacePresent(_surface);
        webGpu.CommandBufferRelease(commandBuffer);
        webGpu.RenderPassEncoderRelease(renderPassEncoder);
        webGpu.CommandEncoderRelease(commandEncoder);
        webGpu.TextureViewRelease(textureView);
        webGpu.TextureRelease(surfaceTexture.Texture);
    }
}