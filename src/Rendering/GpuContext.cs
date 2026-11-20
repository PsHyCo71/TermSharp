using Silk.NET.WebGPU;

namespace TermSharp.Rendering;

public class GpuContext
{  
    public readonly WebGPU WebGpu;
    public readonly unsafe Instance* Instance;
    private unsafe Adapter* _adapter;
    public unsafe Device* Device;
    public unsafe Queue* Queue;

    public unsafe GpuContext(WebGPU webGpu, Instance* instance, Adapter* adapter, Device* device, Queue* queue)
    {
        this.WebGpu = webGpu;
        this.Instance = instance;
        _adapter = adapter;
        this.Device = device;
        this.Queue = queue;
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
