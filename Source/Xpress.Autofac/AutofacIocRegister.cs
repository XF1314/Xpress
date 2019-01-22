using Autofac;
using Autofac.Builder;
using System;
using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Autofac.Extras.DynamicProxy;
using Autofac.Core.Registration;
using Autofac.Core;
using Xpress.Core.DependencyInjection;
using Xpress.Core;
using Xpress.Core.EventBus.Local;

namespace Xpress.Autofac
{
    /// <inheritdoc />
    public class AutofacIocRegister : IocRegisterBase
    {
        private readonly ContainerBuilder _builder;

        /// <inheritdoc />
        public AutofacIocRegister()
        {
            _builder = new ContainerBuilder();
        }

        /// <inheritdoc />
        public override IServiceProvider GetServiceProvider(IServiceCollection services)
        {
            IContainer container = null;
            _builder.Populate(services);
            _builder.Register(c => container).SingleInstance();
            container = _builder.Build();

            return new AutofacServiceProvider(container);
        }

        /// <inheritdoc />
        public override void Register<TService>(TService implementationInstance)
        {
            _builder.Register(c => implementationInstance).AddLifeStyle(DependencyLifeStyle.Singleton);
        }

        /// <inheritdoc />
        public override void Register<TService>(Func<IIocResolver, TService> implementationFactory, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
        {
            _builder.Register(context => implementationFactory.Invoke(context.Resolve<IIocResolver>()))
                    .AddLifeStyle(lifeStyle);
        }

        /// <inheritdoc />
        public override void Register(Type serviceType, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
        {
            if (serviceType.IsGenericType)
                _builder.RegisterGeneric(serviceType).AddLifeStyle(lifeStyle);
            else
            {
                _builder.RegisterType(serviceType).AddLifeStyle(lifeStyle);
            }
        }

        /// <inheritdoc />
        public override void Register(Type serviceType, Type implementationType, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
        {
            if (implementationType.IsGenericType)
            {
                _builder.RegisterGeneric(implementationType).As(serviceType)
                    .AddLifeStyle(lifeStyle);
            }
            else
            {
                _builder.RegisterType(implementationType).As(serviceType)
                    .AddLifeStyle(lifeStyle);
            }
        }

        /// <inheritdoc />
        public override void RegisterInterceptor<TInterceptor>(Func<TypeInfo, bool> filterCondition)
        {
            _builder.RegisterType<TInterceptor>().AddLifeStyle(DependencyLifeStyle.Transient);
            _builder.RegisterBuildCallback(container =>
            {
                foreach (var registration in container.ComponentRegistry.Registrations)
                {
                    var implType = registration.Activator.LimitType;
                    if (filterCondition(implType.GetTypeInfo()))
                    {
                        var types = registration.Services.OfType<IServiceWithType>()
                            .Select(s => s.ServiceType).ToList();

                        IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> registrationBuilder;
                        if (types.Any(t => t.IsClass))
                            registrationBuilder = RegistrationBuilder.ForType(implType).EnableClassInterceptors();
                        else
                        {
                            registrationBuilder = RegistrationBuilder.ForType(implType).AsImplementedInterfaces().EnableInterfaceInterceptors();
                            ((ComponentRegistration)registration).Activator = registrationBuilder.ActivatorData.Activator;
                        }

                        registrationBuilder.InterceptedBy(typeof(TInterceptor));
                        foreach (var keyValuePair in registrationBuilder.RegistrationData.Metadata)
                        {
                            registration.Metadata[keyValuePair.Key] = keyValuePair.Value;
                        }
                        foreach (var preparingEventHandler in registrationBuilder.RegistrationData.PreparingHandlers)
                        {
                            registration.Preparing += preparingEventHandler;
                        }
                        foreach (var activatingEventHandler in registrationBuilder.RegistrationData.ActivatingHandlers)
                        {
                            registration.Activating += activatingEventHandler;
                        }
                        foreach (var activatedEventHandler in registrationBuilder.RegistrationData.ActivatedHandlers)
                        {
                            registration.Activated += activatedEventHandler;
                        }
                    }
                }
            });
        }

        /// <inheritdoc />
        public override void RegisterAssemblyByBasicInterface(ServicesBuilderOptions servicesBuilderOptions, Assembly assembly)
        {
            var assemblyTypes = assembly.GetTypes();
            assemblyTypes
                .Where(type =>  type.IsClass  && !type.IsAbstract 
                && type.GetInterfaces().Any(i => i.GetTypeInfo() == typeof(ILocalEventHandler))).ToList()
                .ForEach(x =>
                {
                    if (!servicesBuilderOptions.LocalEventBusOptions.Handlers.Contains(x))
                    {
                        servicesBuilderOptions.LocalEventBusOptions.Handlers.Add(x);
                    }
                });

            var transientTypes = assemblyTypes.Where(type =>
                type.GetInterfaces().Any(i => i.GetTypeInfo() == typeof(ITransientDependency)) &&
                !type.IsAbstract &&
                !type.GetTypeInfo().IsGenericTypeDefinition)
                .ToArray();
            foreach (var transientType in transientTypes)
            {
                _builder.RegisterType(transientType).AsImplementedInterfaces().AddLifeStyle(DependencyLifeStyle.Transient);
                _builder.RegisterType(transientType).AddLifeStyle(DependencyLifeStyle.Transient);
            }

            var scopedTypes = assemblyTypes.Where(type =>
                type.GetInterfaces().Any(i => i.GetTypeInfo() == typeof(IScopedDependency)) &&
                !type.IsAbstract &&
                !type.GetTypeInfo().IsGenericTypeDefinition)
                .ToArray();
            foreach (var scopedType in scopedTypes)
            {
                _builder.RegisterType(scopedType).AsImplementedInterfaces().AddLifeStyle(DependencyLifeStyle.Scoped);
                _builder.RegisterType(scopedType).AddLifeStyle(DependencyLifeStyle.Scoped);
            }

            var singletonTypes = assemblyTypes.Where(type =>
                type.GetInterfaces().Any(i => i.GetTypeInfo() == typeof(ISingletonDependency)) &&
                !type.IsAbstract &&
                !type.GetTypeInfo().IsGenericTypeDefinition)
                .ToArray();
            foreach (var singletonType in singletonTypes)
            {
                _builder.RegisterType(singletonType).AsImplementedInterfaces().AddLifeStyle(DependencyLifeStyle.Singleton);
                _builder.RegisterType(singletonType).AddLifeStyle(DependencyLifeStyle.Singleton);
            }
        }
    }
}

