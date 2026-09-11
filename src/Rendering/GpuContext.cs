using Silk.NET.WebGPU;

namespace TermSharp;

public class GpuContext
{
    private WebGPU _webGpu;
    private unsafe Instance* _instance;
    private unsafe Adapter* _adapter;
    private unsafe Device* _device;
    private unsafe Queue* _queue;

    public unsafe GpuContext(WebGPU webGpu, Instance* instance, Adapter* adapter, Device* device, Queue* queue)
    {
        _webGpu = webGpu;
        _instance = instance;
        _adapter = adapter;
        _device = device;
        _queue = queue;
    }

    private static unsafe Instance* CreateInstance(WebGPU webGpu)
    {
        InstanceDescriptor descriptor = new InstanceDescriptor();
        return webGpu.CreateInstance(&descriptor);
    }

    public static async Task<GpuContext> CreateAsync()
    {
        WebGPU webGpu = WebGPU.GetApi();

        nint instanceHandle;
        unsafe
        {
            Instance* instance = CreateInstance(webGpu);
            instanceHandle = (nint)instance;
        }

        var adapterTcs = new TaskCompletionSource<nint>();
        unsafe
        {
            Instance* instance = (Instance*)instanceHandle;
            webGpu.InstanceRequestAdapter(
                instance,
                null,
                new PfnRequestAdapterCallback(
                    (status, adapterPtr, message, userdata) => adapterTcs.SetResult((nint)adapterPtr)
                ),
                null
            );
        }
        nint adapterHandle = await adapterTcs.Task;

        var deviceTcs = new TaskCompletionSource<nint>();
        unsafe
        {
            Adapter* adapter = (Adapter*)adapterHandle;
            DeviceDescriptor descriptor = new DeviceDescriptor();
            webGpu.AdapterRequestDevice(
                adapter,
                ref descriptor,
                new PfnRequestDeviceCallback(
                    (status, devicePtr, message, userdata) => deviceTcs.SetResult((nint)devicePtr)
                ),
                null
            );
        }
        nint deviceHandle = await deviceTcs.Task;

        nint queueHandle;
        unsafe
        {
            Device* device = (Device*)deviceHandle;
            Queue* queue = webGpu.DeviceGetQueue(device);
            queueHandle = (nint)queue;
        }

        unsafe
        {
            return new GpuContext(
                webGpu,
                (Instance*)instanceHandle,
                (Adapter*)adapterHandle,
                (Device*)deviceHandle,
                (Queue*)queueHandle
            );
        }
    }
}