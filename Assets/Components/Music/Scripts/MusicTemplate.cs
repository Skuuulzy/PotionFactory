using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Music Template", menuName = "Music/Template")]
public class MusicTemplate : ScriptableObject
{
    [SerializeField] private string _musicName;
    [SerializeField] private AudioClip _audioClip;
    
    public string MusicName => _musicName;
    public AudioClip AudioClip => _audioClip;

}


