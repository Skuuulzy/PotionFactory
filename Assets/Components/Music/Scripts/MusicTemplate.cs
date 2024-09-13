using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Music Template", menuName = "Music/Template")]
public class MusicTemplate : ScriptableObject
{
    [SerializeField] private string _musicName;
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private int _bpm;
    [SerializeField] List<MusicBPMModifier> _musicBPMModifiers = new List<MusicBPMModifier>();
    public string MusicName => _musicName;
    public AudioClip AudioClip => _audioClip;
    public int Bpm => _bpm;

    public List<MusicBPMModifier> MusicBPMModifiers => _musicBPMModifiers;

}

[Serializable]
public struct MusicBPMModifier
{
    public int Time;
    public int BPM;
    public int VoiceVolume;
}
