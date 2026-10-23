using Silk.NET.WebGPU;
using Silk.NET.Windowing;
using TermSharp.Windowing;

namespace TermSharp.Rendering;

public class RenderSurface
{  
    public unsafe Surface* CreateSurface(AppWindow window, GpuContext gpuContext)
    {
        Surface* surface = window.WindowSource.CreateWebGPUSurface(gpuContext._webGpu, gpuContext._instance); 
        return surface;
    }
    
}