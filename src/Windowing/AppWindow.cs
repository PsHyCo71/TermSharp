using Silk.NET.Core.Contexts;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.Input;

namespace TermSharp.Windowing;

public class AppWindow
{
    private readonly IWindow _window;
    private INativeWindow? _native;
    private IInputContext? _input;

    public INativeWindowSource WindowSource => _window;

    public INativeWindow NativeHandle => _native ?? throw new InvalidOperationException("The window has not been initiliazed yet.");

    public event Action? CloseRequest;
    public event Action<int, int>? Resized;
    public event Action<Key>? KeyDown;
    public event Action<Key>? KeyUp;

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
        _window.Resize += OnResize;
        _window.Closing += OnClosing;
        _window.Run();
    }

    private void OnLoad()
    {
        _native = _window.Native;
        _input = _window.CreateInput();
        _input.Keyboards[0].KeyDown += (keyboard, key, scancode) => KeyDown?.Invoke(key);
        _input.Keyboards[0].KeyUp += (keyboard, key, scancode) => KeyUp?.Invoke(key);
    }
    private void OnUpdate(double _) { }
    private void OnRender(double _) { }
    private void OnResize(Vector2D<int> size)
    {
        Resized?.Invoke(size.X, size.Y);
    }
    private void OnClosing()
    {
        CloseRequest?.Invoke();
    }
}
