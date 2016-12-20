﻿// Copyright (c) 2015-2016, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

namespace Saritasa.Tools.Messages.Commands.PipelineMiddlewares
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Linq;
    using System.Linq.Expressions;
    using Common;
    using Internal;

    /// <summary>
    /// Locate command hanlder.
    /// </summary>
    public class CommandHandlerLocatorMiddleware : IMessagePipelineMiddleware
    {
        /// <inheritdoc />
        public string Id { get; set; } = "CommandHandlerLocator";

        const string HandlerPrefix = "Handle";

        readonly Assembly[] assemblies;

        // TODO: [IK] need to implement caching to improve speed
        IDictionary<Type, Expression<Func<object>>> cache =
            new System.Collections.Concurrent.ConcurrentDictionary<Type, Expression<Func<object>>>(4, 150);

        IList<MethodInfo> commandHandlers;

        HandlerSearchMethod handlerSearchMethod = HandlerSearchMethod.ClassAttribute;

        /// <summary>
        /// What method to use to search command handler class.
        /// </summary>
        public HandlerSearchMethod HandlerSearchMethod
        {
            get
            {
                return handlerSearchMethod;
            }

            set
            {
                if (handlerSearchMethod != value)
                {
                    handlerSearchMethod = value;
                    Init();
                }
            }
        }

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="assemblies">Assemblies to locate.</param>
        public CommandHandlerLocatorMiddleware(params Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Length < 1)
            {
                throw new ArgumentException("Assemblies to search handlers were not specified");
            }
            if (assemblies.Any(a => a == null))
            {
                throw new ArgumentNullException(nameof(assemblies));
            }
            this.assemblies = assemblies;
            Init();
        }

        /// <summary>
        /// Prefills command handlers. We cannot do it in runtime because there can be race conditions
        /// during initialization. Much simple just do that once on application start.
        /// </summary>
        private void Init()
        {
            try
            {

                // precache all types with command handlers
                commandHandlers = assemblies.SelectMany(a => a.GetTypes())
                    .Where(t =>
                        HandlerSearchMethod == HandlerSearchMethod.ClassAttribute
                            ? t.GetTypeInfo().GetCustomAttribute<CommandHandlersAttribute>() != null
                            : t.Name.EndsWith("Handlers"))
                    .SelectMany(t => t.GetTypeInfo().GetMethods())
                    .Where(m => m.Name.StartsWith(HandlerPrefix))
                    .ToArray();


                if (!commandHandlers.Any())
                {
                    var assembliesStr = string.Join(",", assemblies.Select(a => a.FullName));
                    InternalLogger.Warn($"Cannot find command handlers in assemblie(-s) {assembliesStr}",
                        nameof(CommandHandlerLocatorMiddleware));
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                InternalLogger.Error($"Cannot find command handlers in assemblie(-s)" + ex.GetBaseException().Message + Environment.NewLine + ex.GetBaseException().StackTrace,
                    nameof(CommandHandlerLocatorMiddleware));
            }

        }

        /// <inheritdoc />
        public void Handle(Message message)
        {
            var commandMessage = message as CommandMessage;
            if (commandMessage == null)
            {
                throw new NotSupportedException("Message should be CommandMessage type");
            }

            // find handler method
            var cmdtype = commandMessage.Content.GetType();
            if (InternalLogger.IsDebugEnabled)
            {
                InternalLogger.Debug($"Finding command handler for type {cmdtype.Name}", nameof(CommandHandlerLocatorMiddleware));
            }
            var method = commandHandlers
                .FirstOrDefault(m => m.GetParameters().Any(pt => pt.ParameterType == cmdtype));
            if (method == null)
            {
                method = cmdtype.GetTypeInfo().GetMethod(HandlerPrefix);
            }
            if (method == null)
            {
                var assembliesStr = string.Join(",", assemblies.Select(a => a.FullName));
                InternalLogger.Warn($"Cannot find command handler for command {cmdtype.Name} in assemblies {assembliesStr}",
                    nameof(CommandHandlerLocatorMiddleware));
                throw new CommandHandlerNotFoundException(cmdtype.Name);
            }
            if (InternalLogger.IsDebugEnabled)
            {
                InternalLogger.Debug($"Found \"{method}\" for command {cmdtype}", nameof(CommandHandlerLocatorMiddleware));
            }
            commandMessage.HandlerMethod = method;
            commandMessage.HandlerType = method.DeclaringType;
        }
    }
}
