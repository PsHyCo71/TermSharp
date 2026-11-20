using TermSharp.Rendering;
using TermSharp.Windowing;

namespace TermSharp;
public class Program
{
    public static void Main()
    {
        var gpuContext = GpuContext.CreateAsync().GetAwaiter().GetResult();
        AppWindow window = new AppWindow();
        var renderSurface = new RenderSurface();
        
        window.Loaded += () =>
        {
            renderSurface.CreateSurface(window, gpuContext);
            renderSurface.ConfigureSurface(gpuContext, window.size);
        };
        window.Resized += (_, _) => renderSurface.ConfigureSurface(gpuContext, window.size);
        window.RenderRequested += () => renderSurface.RenderFrame(gpuContext);

        window.Run();
    }
}