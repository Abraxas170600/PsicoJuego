using DearTroll.Audio;
using UnityEngine;

namespace DearTroll.Services
{
    [DefaultExecutionOrder(-1000)]
    public class GlobalServicesInstaller : MonoBehaviour
    {
        [Header("Audio Settings")] public AudioService.Settings audioSettings;
        [Header("VFX Settings")] public VfxService.Settings vfxSettings;

        private ServiceProvider _provider;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            var services = new ServiceCollection()
            .AddSingleton<IAudioService>(p => new AudioService(audioSettings, gameObject))
            .AddSingleton<IVfxService>(p => new VfxService(vfxSettings, gameObject));

            _provider = services.Build();

            ServiceLocator.Push(_provider);

            foreach (var f in new System.Type[] { typeof(IAudioService), typeof(IVfxService)})
            {
                if (_provider.TryGet(f, out var s) && s is IInitializable init) init.Initialize();
            }
        }

        void OnDestroy()
        {
            // Retira y dispone s�lo si este installer es el tope del stack
            try
            {
                if (ServiceLocator.Current == _provider)
                    ServiceLocator.Pop();
                else
                    _provider?.Dispose();
            }
            catch
            {
                _provider?.Dispose();
            }
        }
    }

    internal static class ServiceProviderExtensions
    {
        public static bool TryGet(this ServiceProvider provider, System.Type t, out object service)
        {
            var method = typeof(ServiceProvider).GetMethod("TryGet")?.MakeGenericMethod(t);
            var args = new object[] { null };
            var ok = (bool)method.Invoke(provider, args);
            service = args[0];
            return ok;
        }
    }
}
