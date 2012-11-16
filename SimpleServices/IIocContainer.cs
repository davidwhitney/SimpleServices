using System;
using System.Collections.Generic;

namespace SimpleServices
{
	public interface IIocContainer
	{
	    T GetType<T>();
        IEnumerable<T> GetAll<T>();
	    object Get(Type t);
	    IEnumerable<object> GetAll(Type t);
	}
}
