using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using System.Reflection;
using NHibernate.Tool.hbm2ddl;

namespace NHibernateManager
{
    /// <summary>
    /// Abstract base for creating an NHibernate data base connection. 
    /// </summary>
    /// <remarks>
    /// in order to use this connection class:
    /// 1) add NHibernateManager.dll and NHibernate.dll to your project's refrences.
    /// 2) add a refrence to the dll's required by your DataBase connection, 
    ///     for example for SQLite: add refrence to System.Data.SQLite.dll and copy SQLite.Interop.dll to your projects root.
    /// 3) create a "hibernate.cfg.xml" file in your project's root, and configure it as needed by your connection.
    /// 4) create a "*.hbm.xml" file to each class you wish to map, where the name "*" is identical to the class its mapping,
    ///    and map it according to its respective class. 
    /// </remarks>
    public abstract class NHibernateConnection
    {
        private ISessionFactory m_SessionFactory;
        /// <summary>
        /// Open's sessions to the database
        /// </summary>
        /// <remarks>
        /// <see cref="SessionFactory"/> is auto initialized in its getter property, and is only created once on the first request of a session.
        /// </remarks>
        protected ISessionFactory SessionFactory
        {
            get 
            {
                if (m_SessionFactory == null)
                {
                    initializeSessionFactory();
                }

                return m_SessionFactory;
            }
        }
        
        private Configuration m_Configuration;
        
        private Assembly m_LocalAssembly;
        
        /// <summary>
        /// Initiate a new instance of <see cref="NHibernateConnection"/>
        /// </summary>
        /// <param name="i_Assembly">The assembly containing the persistent classes</param>
        protected NHibernateConnection(Assembly i_Assembly)
        {
            if (i_Assembly == null)
            {
                throw new ArgumentNullException("i_Assembly");
            }
            m_LocalAssembly = i_Assembly;
        }
        
        private void initializeSessionFactory()
        {
            m_Configuration = new Configuration();
            m_Configuration.DataBaseIntegration(x => { x.ConnectionString = this.ConnectionString; x.BatchSize = 10; });
            m_Configuration.Configure();
            m_Configuration.AddAssembly(m_LocalAssembly);
            this.GenerateSchema(m_Configuration);
            m_SessionFactory = m_Configuration.BuildSessionFactory();
        }

        protected virtual void GenerateSchema(Configuration i_Configuration)
        {
            new SchemaUpdate(i_Configuration).Execute(false, true);
        }

        /// <summary>
        /// Open's a new session
        /// </summary>
        /// <returns>the newly opened session</returns>
        public ISession OpenSession()
        {
            return this.SessionFactory.OpenSession();
        }

        /// <summary>
        /// The connection string for the database connection.
        /// </summary>
        /// <remarks>
        /// property is defined as abstract, to be implemented in a concrete DAO.
        /// set the connection string as appropriate for you database connection.
        /// </remarks>
        protected abstract string ConnectionString { get; set; }

        /// <summary>
        /// Save or update a persistent object type
        /// </summary>
        /// <typeparam name="PersistentT">The persistent class type to save/update</typeparam>
        /// <param name="i_Session">An open session</param>
        /// <param name="i_ObjectToPersist">An object of type <typeparamref name="PersistentT"/> to save/update </param>
        protected void SaveOrUpdate<PersistentT>(ISession i_Session, PersistentT i_ObjectToPersist)
        {
            using (ITransaction transaction = i_Session.BeginTransaction())
            {
                i_Session.SaveOrUpdate(i_ObjectToPersist);
                transaction.Commit();
            }
        }

        /// <summary>
        /// Load a persistent object by its id
        /// </summary>
        /// <typeparam name="PersistentT">The persistent class type to retrieve</typeparam>
        /// <param name="i_Session">An open session</param>
        /// <param name="i_Id">Id of the requested object</param>
        /// <returns>the persistent object class type with the specified id</returns>
        protected PersistentT Load<PersistentT>(ISession i_Session, Guid i_Id)
        {
            PersistentT obj;
            using (ITransaction trans = i_Session.BeginTransaction())
            {
                obj = i_Session.Get<PersistentT>(i_Id);
                trans.Commit();
            }

            return obj;
        }

        /// <summary>
        /// Delete a persistent object
        /// </summary>
        /// <param name="i_Session">An open session</param>
        /// <param name="i_ObjectToDelete">The persistent object delete</param>
        protected void Delete(ISession i_Session, object i_ObjectToDelete)
        {
            using (ITransaction transaction = i_Session.BeginTransaction())
            {
                i_Session.Delete(i_ObjectToDelete);
                transaction.Commit();
            }
        }

        /// <summary>
        /// Retrieve a collection of persitent objects by a query.
        /// </summary>
        /// <typeparam name="PersistentT">The persistent class type to retrieve</typeparam>
        /// <param name="i_Session">An open session</param>
        /// <param name="i_Query">A selection query string</param>
        /// <returns>A list of the retrieved objects</returns>
        protected IList<PersistentT> CollectionQuery<PersistentT>(ISession i_Session, string i_Query)
        {
            IList<PersistentT> collection = null;
            
            using (ITransaction trans = i_Session.BeginTransaction())
            {
                collection = i_Session.CreateSQLQuery(i_Query).AddEntity(typeof(PersistentT)).List<PersistentT>();
                trans.Commit();
            }


            return collection;
        }

        /// <summary>
        /// Retrieves a value list of a specified property, in a specific persistent class.
        /// </summary>
        /// <typeparam name="PropertyT">Type of property to retrieve</typeparam>
        /// <typeparam name="PersistentT">Type of the persistent class</typeparam>
        /// <param name="i_Session">An open session</param>
        /// <param name="i_Property">Name of the property</param>
        /// <param name="i_DistinctValues">true to retrieve only distinct values, false is default</param>
        /// <returns>A value list of the specified property</returns>
        protected IList<PropertyT> GetPropertyCollection<PropertyT, PersistentT>(ISession i_Session, string i_Property, bool i_DistinctValues = false)
        {
            IList<PropertyT> collection = null;

            using (ITransaction trans = i_Session.BeginTransaction())
            {
                StringBuilder builder = new StringBuilder().Append("SELECT ");
                if (i_DistinctValues == true)
                {
                    builder.Append("DISTINCT ");
                }
                builder.AppendFormat("{0} FROM {1}", i_Property, typeof(PersistentT).Name);

                collection = i_Session.CreateSQLQuery(builder.ToString()).List<PropertyT>();
                trans.Commit();
            }


            return collection;
        }
    }
}
