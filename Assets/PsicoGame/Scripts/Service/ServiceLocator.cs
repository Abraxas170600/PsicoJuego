using DearTroll.Services;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static readonly Stack<ServiceProvider> _stack = new();

    public static ServiceProvider Current
    {
        get
        {
            if (_stack.Count == 0)
                throw new InvalidOperationException("ServiceLocator has no active provider. Did you install GlobalServicesInstaller?");
            return _stack.Peek();
        }
    }

    /// <summary>
    /// Pone un provider en la cima (útil en bootstrap o tests). Usa el handle para Pop.
    /// </summary>
    public static ScopeHandle Push(ServiceProvider provider)
    {
        if (provider == null) throw new ArgumentNullException(nameof(provider));
        _stack.Push(provider);
        return new ScopeHandle();
    }

    public static void Pop()
    {
        if (_stack.Count == 0) throw new InvalidOperationException("No provider to pop.");
        var p = _stack.Pop();
        p.Dispose();
    }

    public static T Get<T>() where T : class => Current.Get<T>();
    public static bool TryGet<T>(out T service) where T : class => Current.TryGet(out service);

    /// <summary>
    /// Handle disposable para scopes (using var _ = ServiceLocator.Push(...)).
    /// </summary>
    public readonly struct ScopeHandle : IDisposable
    {
        public void Dispose() => Pop();
    }
}
