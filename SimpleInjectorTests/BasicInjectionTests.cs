﻿using NUnit.Framework;
using SimpleInjector;
using System;

namespace SimpleInjectorTests
{
    [TestFixture]
    public class BasicInjectionTests
    {

        [Test]
        public void ConstructorInjection()
        {
            var c = new Container();
            c.Register<IClient, SomeClient>();
            c.Register<IService, SomeService>();

            // somewhere else
            IClient client = c.GetInstance<IClient>();
        }

        [Test]
        public void OpenGenericsInjection()
        {
            var container = new Container();
            container.Register(typeof(ICommand<>), typeof(CommandImpl<>));

            // but I cannot do:
            // container.Register<ICommand<>, DoSomethingCommand<>>();

            // Resolving:
            container.GetInstance<ICommand<string>>();
        }

        [Test]
        public void Override_DefaultLifeCycle()
        {
            var c = new Container();
            c.Register<IClient, SomeClient>(Lifestyle.Singleton);
            c.Register<IService, SomeService>(Lifestyle.Singleton);

            // somewhere else
            IClient client1 = c.GetInstance<IClient>();
            IClient client2 = c.GetInstance<IClient>();

            Assert.AreSame(client1, client2);
        }

        [Test]
        public void Container_tracks_IDisposables()
        {
            var c = new Container();
            c.Register<DisposableObject>(Lifestyle.Singleton);

            // somewhere else
            var client = c.GetInstance<DisposableObject>();

            c.Dispose();

            Assert.IsTrue(client.IsDisposed);
        }

    }

    public interface IService { }
    public class SomeService : IService { }

    public interface IClient { IService Service { get; } }
    public class SomeClient : IClient
    {
        public IService Service { get; private set; }
        public SomeClient(IService service) { Service = service; }
    }

    public class ClassUsingPropertyInjection
    {
        public IService Service { get; set; }
    }

    interface ICommand<T>
    {
    }

    class CommandImpl<T> : ICommand<T>
    {
        public T Value { get; private set; }
    }

    class DisposableObject : IDisposable
    {
        public bool IsDisposed { get; private set; }
        public void Dispose()
        {
            this.IsDisposed = true;
        }
    }
}
