using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace NHibernateTester
{
    public interface ITestDAO
    {
        ISession OpenSession();
        T GetById<T>(ISession i_Session, Guid i_Id);
        IList<T> GetAll<T>(ISession i_Session);
        IList<ValueT> GetAllValues<ValueT, PersistedT>(ISession i_Session, string i_Property, bool i_DistinctValues);
        void SaveOrUpdate<T>(ISession i_Session, T obj);
        void Delete(ISession i_Session, object obj);
    }
}
