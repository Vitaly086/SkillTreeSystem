using System;
using System.Collections.Generic;

namespace Services
{
    public sealed class ServiceLocator : Singleton<ServiceLocator>
    {
        private readonly Dictionary<Type, IService> _services = new();

        public void RegisterSingle<TService>(TService service) where TService : IService
        {
            _services[typeof(TService)] = service;
        }

        public TService GetSingle<TService>() where TService : class, IService
        {
            return _services[typeof(TService)] as TService;
        }
    }
}