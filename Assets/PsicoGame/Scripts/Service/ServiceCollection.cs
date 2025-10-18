using DearTroll.Services;
using System;
using System.Collections.Generic;

namespace DearTroll.Services
{
    /// <summary>
    /// Builder tipo .NET para registrar servicios de manera declarativa.
    /// </summary>
    public sealed class ServiceCollection
    {
        private readonly List<Action<ServiceProvider>> _registrations = new();

        public ServiceCollection AddInstance<TService>(TService instance) where TService : class
        {
            _registrations.Add(p => p.Register(typeof(TService), instance));
            return this;
        }

        public ServiceCollection AddSingleton<TService>(Func<ServiceProvider, TService> factory)
        where TService : class
        {
            _registrations.Add(p => p.RegisterFactory(typeof(TService), () => factory(p)));
            return this;
        }

        public ServiceCollection AddSingleton<TService, TImpl>()
        where TService : class
        where TImpl : class, TService, new()
        {
            _registrations.Add(p => p.RegisterFactory(typeof(TService), () => new TImpl()));
            return this;
        }

        public ServiceProvider Build()
        {
            var provider = new ServiceProvider();
            foreach (var reg in _registrations) reg(provider);
            return provider;
        }
    }

    /// <summary>
    /// Contenedor simple con caching de singletons y soporte de reemplazo.
    /// </summary>
    public sealed class ServiceProvider : IDisposable
    {
        private readonly Dictionary<Type, object> _singletons = new();
        private readonly Dictionary<Type, Func<object>> _factories = new();

        internal void Register(Type t, object instance) => _singletons[t] = instance;
        internal void RegisterFactory(Type t, Func<object> factory) => _factories[t] = factory;

        public T Get<T>() where T : class
        {
            var t = typeof(T);
            if (_singletons.TryGetValue(t, out var obj)) return (T)obj;
            if (_factories.TryGetValue(t, out var fac))
            {
                var created = (T)fac();
                _singletons[t] = created; // singleton cache
                return created;
            }
            throw new InvalidOperationException($"Service of type {t.Name} is not registered.");
        }

        public bool TryGet<T>(out T service) where T : class
        {
            var t = typeof(T);
            if (_singletons.TryGetValue(t, out var obj)) { service = (T)obj; return true; }
            if (_factories.TryGetValue(t, out var fac))
            {
                var created = (T)fac();
                _singletons[t] = created;
                service = created;
                return true;
            }
            service = null;
            return false;
        }

        public void Dispose()
        {
            foreach (var s in _singletons.Values)
            {
                if (s is ITeardown t) t.Teardown();
                if (s is IDisposable d) d.Dispose();
            }
            _singletons.Clear();
            _factories.Clear();
        }
    }
}
