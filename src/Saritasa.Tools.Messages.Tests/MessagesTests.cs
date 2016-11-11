﻿// Copyright (c) 2015-2016, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

namespace Saritasa.Tools.Messages.Tests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Xunit;
    using Common;
    using Commands;
    using Commands.PipelineMiddlewares;

    public class MessagesTests
    {
        #region Test middlewares

        class TestMiddleware1 : IMessagePipelineMiddleware
        {
            public string Id => "Test1";

            public void Handle(Message message)
            {
            }
        }

        class TestMiddleware2 : IMessagePipelineMiddleware
        {
            public string Id => "Test2";

            public void Handle(Message message)
            {
            }
        }

        class TestMiddleware3 : IMessagePipelineMiddleware
        {
            public string Id => "Test3";

            public void Handle(Message message)
            {
            }
        }

        #endregion

        [Fact]
        public void Messages_pipeline_insert_after_should_increase_length()
        {
            var cp = new CommandPipeline();
            cp.AppendMiddlewares(new CommandHandlerLocatorMiddleware(Assembly.GetAssembly(typeof(MessagesTests))));
            Assert.Equal(1, cp.GetMiddlewares().Count());

            cp.InsertMiddlewareAfter(new TestMiddleware1());
            Assert.Equal("Test1", cp.GetMiddlewares().ElementAt(1).Id);
            cp.InsertMiddlewareAfter(new TestMiddleware3());

            cp.InsertMiddlewareAfter(new TestMiddleware2(), "Test1");
            Assert.Equal("Test2", cp.GetMiddlewares().ElementAt(2).Id);
            Assert.Equal("Test3", cp.GetMiddlewares().ElementAt(3).Id);
            Assert.Equal(4, cp.GetMiddlewares().Count());
        }

        [Fact]
        public void Messages_pipeline_insert_before_should_increase_length()
        {
            var cp = new CommandPipeline();
            cp.AppendMiddlewares(new CommandHandlerLocatorMiddleware(Assembly.GetAssembly(typeof(MessagesTests))));
            Assert.Equal(1, cp.GetMiddlewares().Count());

            cp.InsertMiddlewareBefore(new TestMiddleware1());
            Assert.Equal("Test1", cp.GetMiddlewares().ElementAt(0).Id);
            cp.InsertMiddlewareAfter(new TestMiddleware2());

            // test1, locator, test3, test2
            cp.InsertMiddlewareBefore(new TestMiddleware3(), "Test2");
            Assert.Equal("Test3", cp.GetMiddlewares().ElementAt(2).Id);
            Assert.Equal("Test2", cp.GetMiddlewares().ElementAt(3).Id);
            Assert.Equal(4, cp.GetMiddlewares().Count());
        }

        [Fact]
        public void Inserting_middlewares_with_duplicated_ids_should_generate_exception()
        {
            var cp = new CommandPipeline();
            cp.AppendMiddlewares(new CommandHandlerLocatorMiddleware(Assembly.GetAssembly(typeof(MessagesTests))));
            cp.InsertMiddlewareBefore(new TestMiddleware1());

            Assert.Throws<ArgumentException>(() => { cp.InsertMiddlewareBefore(new TestMiddleware1()); });
        }
    }
}
