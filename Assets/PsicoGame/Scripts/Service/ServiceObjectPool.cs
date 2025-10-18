using System;
using System.Collections.Generic;
using UnityEngine;

namespace DearTroll.Services
{
    public interface IPool<T>
    {
        T Get();
        void Release(T item);
        int CountInactive { get; }
    }

    /// <summary>
    /// Simple object pool implementation.
    /// </summary>

    public class ServiceObjectPool<T> : IPool<T>
    {
        private readonly Stack<T> _stack = new();
        private readonly Func<T> _factory;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onRelease;
        private readonly int _maxSize;

        public ServiceObjectPool(Func<T> factory, Action<T> onGet = null, Action<T> onRelease = null, int initial = 0, int maxSize = int.MaxValue)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _onGet = onGet; 
            _onRelease = onRelease; 
            _maxSize = Math.Max(1, maxSize);
            for (int i = 0; i < initial; i++) 
                _stack.Push(_factory());
        }

        public T Get()
        {
            var item = _stack.Count > 0 ? _stack.Pop() : _factory();
            _onGet?.Invoke(item);
            return item;
        }

        public void Release(T item)
        {
            _onRelease?.Invoke(item);
            if (_stack.Count < _maxSize) _stack.Push(item);
        }

        public int CountInactive => _stack.Count;
    }
}
