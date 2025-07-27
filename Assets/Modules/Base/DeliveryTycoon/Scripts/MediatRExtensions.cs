using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VContainer;
using UnityEngine;

namespace Modules.Base.DeliveryTycoon.Scripts
{
    public class VContainerServiceProvider : IServiceProvider
    {
        private readonly IObjectResolver _resolver;

        public VContainerServiceProvider(IObjectResolver resolver) => 
            _resolver = resolver;

        public object GetService(Type serviceType)
        {
            if (serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var elementType = serviceType.GetGenericArguments()[0];
                var method = typeof(IObjectResolver).GetMethod("ResolveAll")?.MakeGenericMethod(elementType);
                var result = method?.Invoke(_resolver, null);
                return result ?? Array.CreateInstance(elementType, 0);
            }

            try
            {
                return _resolver.Resolve(serviceType);
            }
            catch (VContainerException ex)
            {
                Debug.LogError($"VContainer не смог разрешить {serviceType}: {ex.Message}");
                return null;
            }
        }
    }

    public static class MediatRExtensions
    {
        private const string LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxNzg0OTM3NjAwIiwiaWF0IjoiMTc1MzQ0NDcyOCIsImFjY291bnRfaWQiOiIwMTk4NDE3MWUxMTM3MzE2YTFhY2Q5NmQxMGEzMmU0NiIsImN1c3RvbWVyX2lkIjoiY3RtXzAxazEwcTZmdHJocHJobXFqNWJheDNjeW5yIiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.U2KZrYT6aLpeIG615WYnNIesM_o64kdibfNkMkyeuFmFdupuSvGVPMsgpK4-U1I_0T89VbgsZ8bBJ3nLEyXqdTOrTDCauGJubghRs_PFbLjd5MXG6vnmSF8BKkc1-zADIGDdq2KXo7ukxuohtQP1vZOyuyiwlRR9E4LjQVKLP_w9v9-iSOukO5woMOSvo_1yTLMTcb0B5xRfgqM0Fn3B9lp0qO_rubk7kDbGJkWLfsesYyq-r80t3k_hrY-JHt3FgnseEuasj9fT4sfpXLr9OIlvKy62hHCC-av3qPP8e4HAB56LXRDUwHWgIGYWVRVYyBweI1l3N17lVo87QoqKxw";
        private static readonly Dictionary<Assembly, Type[]> HandlerTypesCache = new();

        public static void AddMediatR(this IContainerBuilder builder, params Assembly[] assemblies)
        {
            var config = new MediatRServiceConfiguration { LicenseKey = LicenseKey };

            var loggerFactory = new LoggerFactory();
            builder.RegisterInstance<ILoggerFactory>(loggerFactory);

            builder.RegisterInstance(config);
            
            //MediatR.Licensing.LicenseAccessor
            var mediatRAssembly = typeof(Mediator).Assembly;
            var licenseAccessorType = mediatRAssembly.GetType("MediatR.Licensing.LicenseAccessor")
                ?? throw new InvalidOperationException("Не найден тип LicenseAccessor в MediatR");
            
            var licenseValidatorType = mediatRAssembly.GetType("MediatR.Licensing.LicenseValidator") 
                                       ?? throw new InvalidOperationException("Не найден тип LicenseValidator в MediatR");

            builder.Register(licenseValidatorType, Lifetime.Singleton);

            builder.Register(licenseAccessorType, Lifetime.Singleton);
            
            /*builder.Register(resolver =>
            {
                var ctor = licenseAccessorType.GetConstructor(new[] { typeof(MediatRServiceConfiguration), typeof(ILoggerFactory) })
                    ?? throw new InvalidOperationException("Не найден конструктор LicenseAccessor");
                return ctor.Invoke(new object[] { config, loggerFactory });
            }, Lifetime.Singleton).As(licenseAccessorType);*/

            builder.Register<VContainerServiceProvider>(Lifetime.Singleton).As<IServiceProvider>();

            builder.Register(resolver => 
                    new Mediator(resolver.Resolve<IServiceProvider>()), Lifetime.Singleton)
                .As<IMediator>();

            foreach (var assembly in assemblies) 
                RegisterMediatRHandlers(builder, assembly);
        }

        private static void RegisterMediatRHandlers(IContainerBuilder builder, Assembly assembly)
        {
            if (!HandlerTypesCache.TryGetValue(assembly, out var handlerTypesArray))
            {
                handlerTypesArray = assembly.GetTypes()
                    .Where(t => !t.IsAbstract && !t.IsInterface &&
                                t.GetInterfaces().Any(i => i.IsGenericType &&
                                                           (i.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                                                            i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                                                            i.GetGenericTypeDefinition() == typeof(INotificationHandler<>))))
                    .ToArray();
                HandlerTypesCache[assembly] = handlerTypesArray;
            }

            foreach (var handlerType in handlerTypesArray)
            {
                var interfaces = handlerType.GetInterfaces()
                    .Where(i => i.IsGenericType &&
                                (i.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                                 i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                                 i.GetGenericTypeDefinition() == typeof(INotificationHandler<>)))
                    .ToArray();

                foreach (var handlerInterface in interfaces) 
                    builder.Register(handlerType, Lifetime.Transient).As(handlerInterface);
            }
        }
    }
}