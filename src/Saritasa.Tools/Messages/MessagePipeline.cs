﻿// Copyright (c) 2015-2016, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

namespace Saritasa.Tools.Messages
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Messages processing pipeline.
    /// </summary>
    public class MessagePipeline : IMessagePipeline
    {
        /// <summary>
        /// Middlewares list.
        /// </summary>
        protected IList<IMessagePipelineMiddleware> Middlewares { get; set; } = new List<IMessagePipelineMiddleware>();

        #region IMessagePipeline

        /// <inheritdoc />
        public virtual byte[] MessageTypes
        {
            get { return new byte[] { }; }
        }

        /// <inheritdoc />
        public void AddMiddlewares(params IMessagePipelineMiddleware[] middlewares)
        {
            foreach (var middleware in middlewares)
            {
                Middlewares.Add(middleware);
            }
        }

        /// <inheritdoc />
        public IEnumerable<IMessagePipelineMiddleware> GetMiddlewares()
        {
            return Middlewares;
        }

        /// <inheritdoc />
        public virtual void ProcessRaw(Message message)
        {
        }

        #endregion
    }
}
