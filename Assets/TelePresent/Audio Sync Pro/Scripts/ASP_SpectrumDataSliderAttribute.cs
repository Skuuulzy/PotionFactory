/*******************************************************
Product - Audio Sync Pro
  Publisher - TelePresent Games
              http://TelePresentGames.dk
  Author    - Martin Hansen
  Created   - 2024
  (c) 2024 Martin Hansen. All rights reserved.
/*******************************************************/

using UnityEngine;

namespace TelePresent.AudioSyncPro
{
    public class ASP_SpectrumDataSliderAttribute : PropertyAttribute
    {
        public readonly float minLimit;
        public readonly float maxLimit;
        public readonly string spectrumDataFieldName;
        public readonly float verticalMinLimit;
        public readonly float verticalMaxLimit;

        public ASP_SpectrumDataSliderAttribute(float minLimit, float maxLimit, float verticalMinLimit, float verticalMaxLimit, string spectrumDataFieldName)
        {
            this.minLimit = minLimit;
            this.maxLimit = maxLimit;
            this.verticalMinLimit = verticalMinLimit;
            this.verticalMaxLimit = verticalMaxLimit;
            this.spectrumDataFieldName = spectrumDataFieldName;
        }
    }
}
