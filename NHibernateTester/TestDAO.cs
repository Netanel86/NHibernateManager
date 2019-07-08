using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernateManager;
using NHibernate;
using System.Reflection;
using NHibernate.Tool.hbm2ddl;

namespace NHibernateTester
{
    public class TestDAO : NHibernateConnection, ITestDAO
    {
        public TestDAO() 
            :base(Assembly.GetExecutingAssembly())
        {
            this.ConnectionString = "Data Source=testdb.db;Version=3";
        }

        public T GetById<T>(ISession i_Session, Guid i_Id)
        {
            return base.Load<T>(i_Session, i_Id);
        }

        public IList<T> GetAll<T>(ISession i_Session)
        {
            return CollectionQuery<T>(i_Session, String.Format("SELECT * FROM {0}", typeof(T).Name));
        }

        public IList<ValueT> GetAllValues<ValueT, PersistedT>(ISession i_Session, string i_Property, bool i_DistinctValues = false)
        {
            return GetPropertyCollection<ValueT, PersistedT>(i_Session, i_Property, i_DistinctValues);
        }

        public new void SaveOrUpdate<T>(ISession i_Session, T obj)
        {
            base.SaveOrUpdate<T>(i_Session, obj);
        }

        public new void Delete(ISession i_Session, object obj)
        {
            base.Delete(i_Session, obj);
        }

        protected override string ConnectionString
        {
            get;
            set;
        }
    }
}
