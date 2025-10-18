using UnityEngine;

namespace DearTroll.Services
{
    /// <summary>
    /// Marcador para servicios del sistema. No obliga a nada.
    /// </summary>
    public interface IService { }

    /// <summary>
    /// Inicialización síncrona opcional.
    /// </summary>
    public interface IInitializable
    {
        void Initialize();
    }

    /// <summary>
    /// Inicialización asíncrona opcional (carga Addressables, etc.).
    /// </summary>
    public interface IAsyncInitializable
    {
        System.Threading.Tasks.Task InitializeAsync();
    }

    /// <summary>
    /// Teardown/limpieza de recursos.
    /// </summary>
    public interface ITeardown
    {
        void Teardown();
    }
}
