﻿// Copyright (c) 2015-2016, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

namespace Saritasa.Tools.Messages.Common
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Message query parameters. Always filters with AND condition.
    /// </summary>
    public class MessageQuery
    {
        /// <summary>
        /// Message guid to filter.
        /// </summary>
        public Guid? Id { get; private set; }

        /// <summary>
        /// Creation start date to filter.
        /// </summary>
        public DateTime? CreatedStartDate { get; private set; }

        /// <summary>
        /// Creation end date to filter.
        /// </summary>
        public DateTime? CreatedEndDate { get; private set; }

        /// <summary>
        /// Content type to filter.
        /// </summary>
        public string ContentType { get; private set; }

        /// <summary>
        /// Error type to filter.
        /// </summary>
        public string ErrorType { get; private set; }

        /// <summary>
        /// Message status to filter.
        /// </summary>
        public Message.ProcessingStatus? Status { get; private set; }

        /// <summary>
        /// Message type to filter.
        /// </summary>
        public byte? Type { get; private set; }

        /// <summary>
        /// Message execution duration should be above or equal the value.
        /// </summary>
        public int? ExecutionDurationAbove { get; private set; }

        /// <summary>
        /// Message execution duration should be below or equal the value.
        /// </summary>
        public int? ExecutionDurationBelow { get; private set; }

        /// <summary>
        /// Assemblies to load types.
        /// </summary>
        public IList<Assembly> Assemblies { get; } = new List<Assembly>();

        /// <summary>
        /// How many messages to skip.
        /// </summary>
        public int Skip { get; private set; }

        /// <summary>
        /// How many record to return. Default is 1000.
        /// </summary>
        public int Take { get; private set; } = 1000;

        MessageQuery()
        {
        }

        /// <summary>
        /// Default create to retrieve all messages.
        /// </summary>
        /// <returns>Message query.</returns>
        public static MessageQuery Create()
        {
            return new MessageQuery();
        }

        /// <summary>
        /// Filter message since the specified creation date.
        /// </summary>
        /// <param name="startDate">Date since the message has been created.</param>
        /// <returns>Message query.</returns>
        public MessageQuery WithCreatedStartDate(DateTime startDate)
        {
            CreatedStartDate = startDate;
            return this;
        }

        /// <summary>
        /// Filters messages till the specified creation date.
        /// </summary>
        /// <param name="endDate">Date till the message has been created.</param>
        /// <returns>Message query.</returns>
        public MessageQuery WithCreatedEndDate(DateTime endDate)
        {
            CreatedEndDate = endDate;
            return this;
        }

        /// <summary>
        /// Filter only messages with specified content type.
        /// </summary>
        /// <typeparam name="TContent">Type of class for content.</typeparam>
        /// <returns>Message query.</returns>
        public MessageQuery WithContentType<TContent>()
        {
            return WithContentType(typeof(TContent).FullName);
        }

        /// <summary>
        /// Filter only messages with specified content type.
        /// </summary>
        /// <param name="contentType">Class name for type.</param>
        /// <returns>Message query.</returns>
        public MessageQuery WithContentType(string contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                throw new ArgumentException(nameof(contentType));
            }
            ContentType = contentType;
            return this;
        }

        /// <summary>
        /// Filter only messages with specified error type.
        /// </summary>
        /// <typeparam name="TError">Type of class for error.</typeparam>
        /// <returns>Message query.</returns>
        public MessageQuery WithErrorType<TError>()
        {
            return WithErrorType(typeof(TError).FullName);
        }

        /// <summary>
        /// Filter only messages with specified error type.
        /// </summary>
        /// <param name="errorType">Class name for type.</param>
        /// <returns>Message query.</returns>
        public MessageQuery WithErrorType(string errorType)
        {
            if (string.IsNullOrWhiteSpace(errorType))
            {
                throw new ArgumentException(nameof(errorType));
            }
            ErrorType = errorType;
            return this;
        }

        /// <summary>
        /// Filter messages only with specified status.
        /// </summary>
        /// <param name="status">Status.</param>
        /// <returns>Message query.</returns>
        public MessageQuery WithStatus(Message.ProcessingStatus status)
        {
            Status = status;
            return this;
        }

        /// <summary>
        /// Select message by id. There should be only one message with specified id.
        /// </summary>
        /// <param name="id">Guid.</param>
        /// <returns>Message query.</returns>
        public MessageQuery WithId(Guid id)
        {
            Id = id;
            return this;
        }

        /// <summary>
        /// Filter message only with specified type.
        /// </summary>
        /// <param name="type">Message type.</param>
        /// <returns>Message query.</returns>
        public MessageQuery WithType(byte type)
        {
            Type = type;
            return this;
        }

        /// <summary>
        /// Filter messages with execution duration above the limit.
        /// </summary>
        /// <param name="duration">Execution duration in ms.</param>
        /// <returns>Message query.</returns>
        public MessageQuery WithExecutionDurationAbove(int duration)
        {
            if (ExecutionDurationBelow.HasValue && ExecutionDurationBelow.Value > duration)
            {
                throw new ArgumentOutOfRangeException($"{nameof(duration)} must be greater than ${nameof(ExecutionDurationBelow)}");
            }
            ExecutionDurationAbove = duration;
            return this;
        }

        /// <summary>
        /// Filter messages with execution duration below the limit.
        /// </summary>
        /// <param name="duration">Execution duration in ms.</param>
        /// <returns>Message query.</returns>
        public MessageQuery WithExecutionDurationBelow(int duration)
        {
            if (ExecutionDurationAbove.HasValue && ExecutionDurationAbove.Value < duration)
            {
                throw new ArgumentOutOfRangeException($"{nameof(duration)} must be lower than ${nameof(ExecutionDurationAbove)}");
            }
            ExecutionDurationBelow = duration;
            return this;
        }

        /// <summary>
        /// Restict output range.
        /// </summary>
        /// <param name="skip">How many messages to skip.</param>
        /// <param name="take">How many messages to take. Default is 1000.</param>
        /// <returns>Message query.</returns>
        public MessageQuery WithRange(int skip, int take = 1000)
        {
            if (take < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(take));
            }
            if (skip < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(skip));
            }

            Skip = skip;
            Take = take;
            return this;
        }

        /// <summary>
        /// Does the message match criterias of query.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <returns>True if message matches criteries.</returns>
        public bool Match(Message message)
        {
            if (Id.HasValue && message.Id != Id.Value)
            {
                return false;
            }
            if (CreatedStartDate.HasValue && message.CreatedAt < CreatedStartDate.Value)
            {
                return false;
            }
            if (CreatedEndDate.HasValue && message.CreatedAt > CreatedEndDate.Value)
            {
                return false;
            }
            if (!string.IsNullOrEmpty(ContentType) && message.ContentType != ContentType)
            {
                return false;
            }
            if (!string.IsNullOrEmpty(ErrorType) && message.ErrorType != ErrorType)
            {
                return false;
            }
            if (Status.HasValue && message.Status != Status.Value)
            {
                return false;
            }
            if (Type.HasValue && message.Type != Type.Value)
            {
                return false;
            }
            if (ExecutionDurationAbove.HasValue && message.ExecutionDuration < ExecutionDurationAbove.Value)
            {
                return false;
            }
            if (ExecutionDurationBelow.HasValue && message.ExecutionDuration > ExecutionDurationBelow.Value)
            {
                return false;
            }

            return true;
        }
    }
}
