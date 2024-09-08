using Components.Machines;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Components.Shop.ShopItems
{
    public class ShopItem 
    {
        // ----------------------------------------------------------------------- PRIVATE FIELDS -------------------------------------------------------------------------
        private MachineTemplate _machineTemplate;

        // ----------------------------------------------------------------------- PUBLIC FIELDS -------------------------------------------------------------------------
        public MachineTemplate MachineTemplate => _machineTemplate;

        public ShopItem(MachineTemplate machineTemplate)
        {
            _machineTemplate = machineTemplate;
        }

        public ShopItem(List<MachineTemplate> machineTemplates, List<ShopItem> _shopItemAlreadyInShop)
        {
            float random = UnityEngine.Random.value;
			machineTemplates = machineTemplates.OrderBy(x => Guid.NewGuid()).ToList();
			foreach (var machineTemplate in machineTemplates)
            {
                if(random <= machineTemplate.ShopSpawnProbability && _shopItemAlreadyInShop.Find(x => x.MachineTemplate == machineTemplate) == null)
                {
                    _machineTemplate = machineTemplate;
                    break;
                }
            }
        }
    }
}
