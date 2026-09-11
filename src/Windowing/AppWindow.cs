using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace TermSharp;
public class AppWindow
{   
    private readonly IWindow _window;
    public AppWindow()
    {
        Window.PrioritizeGlfw();
        WindowOptions windowOptions = WindowOptions.Default with 
        { 
            Size = new Vector2D<int>(720, 450),
            Title = "TermSharp"
        };
        _window = Window.Create(windowOptions);
        
    }

    public void Run()
    {
        _window.Load += OnLoad;
        _window.Update += OnUpdate;
        _window.Render += OnRender;
        _window.Closing += OnClosing;
        _window.Run();
    }

    private void OnLoad() {}
    private void OnUpdate(double _) {}
    private void OnRender(double _) {}
    private void OnClosing() {}
}