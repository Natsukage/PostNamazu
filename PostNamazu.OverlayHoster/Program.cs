using System;
using System.Linq;
using RainbowMage.OverlayPlugin;

namespace PostNamazu.OverlayHoster
{
    public class Program : IOverlayAddonV2
    {
        public Action<string, string> PostNamazuDelegate = null;
        private EventSource eventSource;

        
        public void Init(Action<string, string> action)
        {
            PostNamazuDelegate = action;
            Init();
        }
        public void Init()
        {
            var container = Registry.GetContainer();
            var registry = container.Resolve<Registry>();
            eventSource = (EventSource)registry.EventSources.FirstOrDefault(p => p.Name == "鲶鱼精邮差");
            if (eventSource==null)
            {
                eventSource = new EventSource(container);
                registry.StartEventSource(eventSource);
            }
            eventSource.PostNamazuDelegate = PostNamazuDelegate;
        }

        public void DeInit()
        {
            eventSource.PostNamazuDelegate = null;
        }
    }
}
