using LiveSplit.Model;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.SpyroTheDragonBonkCounter.UI.Components
{
    class SpyroTheDragonBonkCounterFactory : IComponentFactory
    {
        public string ComponentName => "Spyro the Dragon Bonk Counter";

        public string Description => "Counts the number of times you bonk while speedrunning Spyro the Dragon";

        public ComponentCategory Category => ComponentCategory.Information;

        public string UpdateName => ComponentName;

        public string XMLURL => "";

        public string UpdateURL => "";

        public Version Version => Version.Parse("1.0.1");

        public IComponent Create(LiveSplitState state)
        {
            return new SpyroTheDragonBonkCounterComponent(state);
        }
    }
}
