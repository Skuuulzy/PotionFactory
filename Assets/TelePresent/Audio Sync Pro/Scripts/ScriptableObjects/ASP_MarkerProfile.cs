/*******************************************************
Product - Audio Sync Pro
  Publisher - TelePresent Games
              http://TelePresentGames.dk
  Author    - Martin Hansen
  Created   - 2024
  (c) 2024 Martin Hansen. All rights reserved.
/*******************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TelePresent.AudioSyncPro
{

    [System.Serializable]
    [CreateAssetMenu(fileName = "ASP_MarkerProfile", menuName = "Audio Sync Pro/Marker Profile")]
    public class MarkerProfile : ScriptableObject
    {
        public AudioClip audioClip;
        public List<ASP_Marker> markerList = new List<ASP_Marker>();
    }

}