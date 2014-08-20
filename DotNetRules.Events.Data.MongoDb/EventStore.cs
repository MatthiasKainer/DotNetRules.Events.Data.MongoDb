namespace DotNetRules.Events.Data.MongoDb
{
    using System.Collections.Generic;
    using System.Linq;

    public class EventStore : ICanStoreAndLoadEvents
    {
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

        public IEnumerable<BaseEvent> GetAllEvents(string applicationName)
        {
            return this.session.Queryable.Where(_ => _.ContextName == applicationName).OrderByDescending(_ => _.Created).ToList().Select(_ => _.Get());
        }

        public IEnumerable<TEvents> GetSpecificEventsFromStore<TEvents>(string applicationName) where TEvents : BaseEvent
        {
            var events = this.session.Queryable.Where(_ => _.ContextName == applicationName
                                                      && _.EventType == typeof(TEvents).AssemblyQualifiedName)
                .OrderByDescending(_ => _.Created);
            return events.ToList().Select(_ => _.Get<TEvents>());
        }

        public IEnumerable<TEvents> GetEventsFromStoreByEntity<TEvents>(string applicationName) where TEvents : BaseEvent
        {
            var events = this.session.Queryable.Where(_ => _.ContextName == applicationName
                                                      && _.EntityContextType == typeof(TEvents).AssemblyQualifiedName)
                .OrderByDescending(_ => _.Created);
            return events.ToList().Select(_ => _.Get<TEvents>());
        }
    }
}