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
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false)]
    public class ASP_ReactorCategoryAttribute : PropertyAttribute
    {
        public string Category { get; private set; }

        public ASP_ReactorCategoryAttribute(string category)
        {
            Category = category;
        }
    }
}
