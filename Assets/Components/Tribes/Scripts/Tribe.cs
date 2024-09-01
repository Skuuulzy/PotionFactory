using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Tribes 
{ 
    public class Tribe : MonoBehaviour
    {
        // ----------------------------------------------------------------------- PRIVATE FIELDS -------------------------------------------------------------------------
        private readonly TribeTemplate _template;

        // ----------------------------------------------------------------------- PUBLIC FIELDS -------------------------------------------------------------------------
        public TribeTemplate Template => _template;

        public Tribe(TribeTemplate template)
		{
            _template = template;
		}

    }
}

