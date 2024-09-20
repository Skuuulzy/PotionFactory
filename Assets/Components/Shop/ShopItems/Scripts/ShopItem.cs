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
        private int _numberOfItemToSell;

        // ----------------------------------------------------------------------- PUBLIC FIELDS -------------------------------------------------------------------------
        public MachineTemplate MachineTemplate => _machineTemplate;
        public int NumberOfItemToSell => _numberOfItemToSell;

        public ShopItem(MachineTemplate machineTemplate, int numberOfItemToSell = 1)
        {
            _machineTemplate = machineTemplate;
            _numberOfItemToSell = numberOfItemToSell;
        }

        public void DecreaseNumberOfItemToSell()
        {
            _numberOfItemToSell--;
        }
    }
}
