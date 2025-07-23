using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MediatR;
using VContainer;

namespace Modules.Base.DeliveryTycoon.Scripts
{
    /// <summary>
    /// Registers MediatR components and handlers in the VContainer.
    /// </summary>
    public static class MediatRExtensions
    {
        private static readonly Dictionary<Assembly, Type[]> HandlerTypesCache = new();

        /// <summary>
        /// Adds MediatR to the container builder, registering components and handlers.
        /// </summary>
        public static void AddMediatR(this IContainerBuilder builder, Assembly assembly)
        {
            // Register ServiceFactory for MediatR dependency resolution
            builder.Register<ServiceFactory>(objectResolver =>
            {
                var resolver = objectResolver;
                return type =>
                {
                    // Handle IEnumerable<T> for multiple notification handlers
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    {
                        var elementType = type.GetGenericArguments()[0];
                        var method = typeof(IObjectResolver).GetMethod("ResolveAll")
                            ?.MakeGenericMethod(elementType);
                        var result = method?.Invoke(resolver, null);

                        // Return empty array if no registrations found
                        return result ?? Array.CreateInstance(elementType, 0);
                    }
                    
                    return resolver.Resolve(type);
                };
            }, Lifetime.Singleton);

            // Register IMediator with VContainer
            builder.Register(ctx => 
                new Mediator(ctx.Resolve<ServiceFactory>()), Lifetime.Singleton);

            RegisterMediatRHandlers(builder, assembly);
        }

        /// <summary>
        /// Scans the provided assembly and registers MediatR handlers with VContainer.
        /// Uses caching to optimize repeated calls.
        /// </summary>
        private static void RegisterMediatRHandlers(IContainerBuilder builder, Assembly assembly)
        {
            // Check cache first to avoid re-scanning
            if (!HandlerTypesCache.TryGetValue(assembly, out var handlerTypesArray))
            {
                // Get all types once and filter in a single pass
                handlerTypesArray = assembly.GetTypes()
                    .Where(t => !t.IsAbstract 
                                && !t.IsInterface 
                                && t.GetInterfaces()
                                    .Any(i => i.IsGenericType 
                                              && (i.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                                                  i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                                                  i.GetGenericTypeDefinition() == typeof(INotificationHandler<>))))
                    .ToArray();

                // Cache the result
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

                // Register each handler as transient
                foreach (var handlerInterface in interfaces) 
                    builder.Register(handlerType, Lifetime.Transient).As(handlerInterface);
            }
        }
    }
}