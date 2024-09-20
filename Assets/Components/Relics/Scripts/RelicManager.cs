using System;
using VComponent.Tools.Singletons;

namespace Components.Relics
{
    public class RelicManager : Singleton<RelicManager>
    {
        public RelicTemplate SelectedRelic { get; private set; }
        public static Action<RelicTemplate> OnChangeSelectedRelic;


        protected override void Awake()
        {
            base.Awake();

            RelicSelectorView.OnSelected += HandleRelicSetected;
        }

        private void OnDestroy()
        {
            RelicSelectorView.OnSelected -= HandleRelicSetected;
        }

        private void HandleRelicSetected(RelicTemplate relic)
        {
            SelectedRelic = relic;
            OnChangeSelectedRelic?.Invoke(relic);
        }
    }

}
