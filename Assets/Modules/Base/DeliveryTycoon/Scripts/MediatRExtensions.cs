using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MediatR;
using UnityEngine;
using VContainer;

namespace Modules.Base.DeliveryTycoon.Scripts
{
    public static class MediatRExtensions
    {
        public static void AddMediatR(this IContainerBuilder builder, Assembly assembly)
        {
            // ServiceFactory: MediatR uses this to resolve handlers via DI
            builder.Register<ServiceFactory>(objectResolver =>
            {
                var resolver = objectResolver;
                return type =>
                {
                    // Support IEnumerable<T> (for multiple notification handlers)
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    {
                        var elementType = type.GetGenericArguments()[0];
                        var method = typeof(IObjectResolver).GetMethod("ResolveAll")
                            ?.MakeGenericMethod(elementType);
                        var result = method?.Invoke(resolver, null);

                        // Return empty array if no registrations found
                        return result ?? Array.CreateInstance(elementType, 0);
                    }

                    Debug.Log($"[ServiceFactory] Attempting to resolve: {type.FullName}");
                    return resolver.Resolve(type);
                };
            }, Lifetime.Singleton);

            // IMediator itself
            builder.Register(ctx => 
                new Mediator(ctx.Resolve<ServiceFactory>()), Lifetime.Singleton);
            
            RegisterMediatRHandlers(builder, assembly);
        }

        private static void RegisterMediatRHandlers(IContainerBuilder builder, Assembly assembly)
        {
            var handlerTypes = assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType && (
                        i.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                        i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                        i.GetGenericTypeDefinition() == typeof(INotificationHandler<>)
                    )));

            var handlerTypesArray = handlerTypes as Type[] ?? handlerTypes.ToArray();
            Debug.Log("handlers" + handlerTypesArray.Length);

            foreach (var handlerType in handlerTypesArray)
            {
                var interfaces = handlerType.GetInterfaces()
                    .Where(i => i.IsGenericType && (
                        i.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                        i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                        i.GetGenericTypeDefinition() == typeof(INotificationHandler<>)
                    ));

                foreach (var handlerInterface in interfaces) 
                    builder.Register(handlerType, Lifetime.Transient).As(handlerInterface);
            }
        }
    }
}