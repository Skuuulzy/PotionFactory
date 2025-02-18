using System;
using VComponent.Tools.Singletons;

namespace Components.Consumable
{
	public class ConsumableManager : Singleton<ConsumableManager>
	{
		public ConsumableTemplate SelectedConsumable { get; private set; }
		public static Action<ConsumableTemplate> OnChangeSelectedConsumable;


		protected override void Awake()
		{
			base.Awake();
			ConsumableSelectorView.OnSelected += HandleConsumableSetected;
		}

		private void OnDestroy()
		{
			ConsumableSelectorView.OnSelected -= HandleConsumableSetected;
		}

		private void HandleConsumableSetected(ConsumableTemplate relic)
		{
			SelectedConsumable = relic;
			OnChangeSelectedConsumable?.Invoke(relic);
		}
	}

}
