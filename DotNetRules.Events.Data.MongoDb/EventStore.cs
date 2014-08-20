namespace DotNetRules.Events.Data.MongoDb
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    public class EventStore : ICanStoreAndLoadEvents
    {
        private readonly string application = ConfigurationManager.AppSettings["application.name"];

        private readonly MongoSession<Event> session;

        public EventStore()
        {
            this.session = new MongoSession<Event>();
            Executor.RegisterObject(this);
        }

        public void AddToStore<T>(T eventItem) where T : BaseEvent
        {
            var @event = new Event 
                         { 
                             Author = eventItem.User, 
                             ContextName = eventItem.ContextName 
                         };
            @event.Set(eventItem);
            this.session.Add(@event);
        }

        public IEnumerable<BaseEvent> GetAllEvents()
        {
            return this.session.Queryable.Where(_ => _.ContextName == this.application).OrderByDescending(_ => _.Created).ToList().Select(_ => _.Get());
        }

        public IEnumerable<TEvents> GetSpecificEventsFromStore<TEvents>() where TEvents : BaseEvent
        {
            var events = this.session.Queryable.Where(_ => _.ContextName == this.application
                                                      && _.EventType == typeof(TEvents).AssemblyQualifiedName)
                .OrderByDescending(_ => _.Created);
            return events.ToList().Select(_ => _.Get<TEvents>());
        }

        public IEnumerable<TEvents> GetEventsFromStoreByEntity<TEvents>() where TEvents : BaseEvent
        {
            var events = this.session.Queryable.Where(_ => _.ContextName == this.application
                                                      && _.EntityContextType == typeof(TEvents).AssemblyQualifiedName)
                .OrderByDescending(_ => _.Created);
            return events.ToList().Select(_ => _.Get<TEvents>());
        }
    }
}