using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RainbowMage.OverlayPlugin;

namespace PostNamazu.OverlayHoster
{
    public class Program : IOverlayAddonV2
    {
        public Action<string, string> PostNamazuDelegate = null;
        
        public void Init()
        {
            var container = Registry.GetContainer();
            var registry = container.Resolve<Registry>();
            registry.StartEventSource(new EventSource(container, PostNamazuDelegate));
        }
    }
}
