﻿// Copyright (c) 2015-2016, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

namespace Saritasa.Tools.Messages.Common.PipelineMiddlewares
{
    using System;

    /// <summary>
    /// Saves the command execution context to repository.
    /// </summary>
    public class RepositoryMiddleware : IMessagePipelineMiddleware
    {
        /// <inheritdoc />
        public string Id { get; set; }

        readonly IMessageRepository repository;

        readonly RepositoryMessagesFilter filter;

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="repository">Repository implementation.</param>
        public RepositoryMiddleware(IMessageRepository repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }
            this.repository = repository;
            Id = repository.GetType().Name;
        }

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="repository">Repository implementation.</param>
        /// <param name="filter">Filter incoming messages.</param>
        public RepositoryMiddleware(IMessageRepository repository, RepositoryMessagesFilter filter) : this(repository)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }
            this.filter = filter;
        }

        /// <inheritdoc />
        public void Handle(Message message)
        {
            if (filter != null && !filter.IsMatch(message))
            {
                return;
            }
            repository.Add(message);
        }
    }
}
