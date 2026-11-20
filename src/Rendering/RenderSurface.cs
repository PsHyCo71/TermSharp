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

    public unsafe TextureView* AcquireSurfaceFrame(GpuContext gpuContext)
    {
        SurfaceTexture surfaceTexture = new SurfaceTexture();

        gpuContext.WebGpu.SurfaceGetCurrentTexture(_surface, &surfaceTexture);

        if (surfaceTexture.Status != SurfaceGetCurrentTextureStatus.Success)
            return null;

        return gpuContext.WebGpu.TextureCreateView(surfaceTexture.Texture, null);
    }
}