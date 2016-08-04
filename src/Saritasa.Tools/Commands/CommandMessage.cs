﻿// Copyright (c) 2015-2016, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

namespace Saritasa.Tools.Commands
{
    using Messages;
    using System;
    using System.Reflection;

    /// <summary>
    /// Command execution context.
    /// </summary>
    public class CommandMessage : Message
    {
        /// <summary>
        /// Command handler.
        /// </summary>
        public Type HandlerType { get; set; }

        /// <summary>
        /// Command handler method to execute.
        /// </summary>
        protected internal MethodInfo HandlerMethod { get; set; }

        /// <summary>
        /// Information about the exception source.
        /// </summary>
        protected internal System.Runtime.ExceptionServices.ExceptionDispatchInfo ErrorDispatchInfo { get; set; }

        /// <inheritdoc />
        public override string ErrorMessage
        {
            get
            {
                return Error != null ? Error.Message : string.Empty;
            }
        }

        /// <inheritdoc />
        public override string ErrorType
        {
            get
            {
                return Error != null ? Error.GetType().FullName : string.Empty;
            }
        }

        /// <inheritdoc />
        public override string ContentType
        {
            get
            {
                return Content.GetType().FullName;
            }
        }

        /// <summary>
        /// .ctor
        /// </summary>
        public CommandMessage()
        {
            Type = Message.MessageTypeCommand;
        }

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="command">Command message.</param>
        public CommandMessage(object command) : base(command, Message.MessageTypeCommand)
        {
        }
    }
}