﻿// Copyright (c) 2015-2016, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

namespace Saritasa.Tools.Messages.Commands
{
    using System;

    /// <summary>
    /// Occures when command handler cannot be found.
    /// </summary>
#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_6
    [Serializable]
#endif
    public class CommandHandlerNotFoundException : Exception
    {
        const string DefaultMessage = "Cannot find command handler or it cannot be resolved. Make sure it has default public parameterless " +
            "constructor or registered with your dependency injection container.";

        /// <summary>
        /// .ctor
        /// </summary>
        public CommandHandlerNotFoundException() : base(DefaultMessage)
        {
        }

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="message">Exception message.</param>
        public CommandHandlerNotFoundException(string message) : base(message)
        {
        }

#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_6
        /// <summary>
        /// .ctor for deserialization.
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream,
        /// and provides an additional caller-defined context.</param>
        protected CommandHandlerNotFoundException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
