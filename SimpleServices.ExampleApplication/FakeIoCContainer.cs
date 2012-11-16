using System;
using System.Collections.Generic;

namespace SimpleServices.ExampleApplication
{
    public class FakeIoCContainer : IIocContainer
    {
        public T GetType<T>()
        {
            return (T)Activator.CreateInstance(typeof(T));
        }

        public IEnumerable<T> GetAll<T>()
        {
            return new List<T> {(T)Activator.CreateInstance(typeof(T))};
        }

        public object Get(Type t)
        {
            return Activator.CreateInstance(t);
        }

        public IEnumerable<object> GetAll(Type t)
        {
            return new List<object> { Activator.CreateInstance(t) };
        }
    }
}