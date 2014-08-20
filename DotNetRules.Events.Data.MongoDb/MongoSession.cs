namespace DotNetRules.Events.Data.MongoDb
{
    using System;
    using System.Configuration;
    using System.Linq;

    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    class MongoSession<T>
    {
        private readonly MongoDatabase database;

        public MongoSession()
            : this(
                ConfigurationManager.ConnectionStrings["DotNetRules.Events.Data.MongoDb"] != null
                    ? ConfigurationManager.ConnectionStrings["DotNetRules.Events.Data.MongoDb"].ConnectionString
                    : string.Empty)
        {
        }

        public MongoSession(string connectionString)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString", "The connection string must not be null. Please add a connection string named DotNetRules.Events.Data.MongoDb");
            }

            var url = new MongoUrl(connectionString);
            var client = new MongoClient(url);
            this.database = client.GetServer().GetDatabase(url.DatabaseName);
            this.EnsureCollection();
        }

        private void EnsureCollection()
        {
            if (!this.database.CollectionExists(typeof(T).Name)) this.database.CreateCollection(typeof(T).Name);
        }

        public IQueryable<T> Queryable
        {
            get
            {
                return this.database.GetCollection<T>(typeof(T).Name).AsQueryable<T>();
            }
        }

        public void Add(T item)
        {
            this.database.GetCollection<T>(typeof(T).Name).Insert(item, SafeMode.True);
        }

        public void Update(T item)
        {
            this.database.GetCollection(typeof(T).Name).Save(item, SafeMode.True);
        }

        public void Update(IMongoQuery query, IMongoUpdate documentToUpdate)
        {
            this.database.GetCollection<T>(typeof(T).Name).Update(query, documentToUpdate);
        }

        public void Delete<TType>(IMongoQuery query)
        {
            this.database.GetCollection<TType>(typeof(TType).Name).Remove(query);
        }

        public void Save(T item)
        {
            this.database.GetCollection<T>(typeof(T).Name).Save(item, SafeMode.True);
        }
    }
}
