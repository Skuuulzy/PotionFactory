using Components.Tick;
using System.Collections.Generic;
using TelePresent.AudioSyncPro;
using UnityEngine;
using VComponent.Tools.Timer;

public class RhythmCaller : MonoBehaviour
{
    [SerializeField] private float _bpm = 120f; // BPM of the music
    [SerializeField] private MusicTemplate _currentMusicTemplate;
    [SerializeField] private AudioSourcePlus _audioSourcePlus;
    private float _beatInterval;
	public void StartMusic()
    {
        //_audioSourcePlus.audioSource.clip = _currentMusicTemplate.AudioClip;
		_audioSourcePlus.audioSource.Play();
    }

	/// <summary>
	/// Calculates the interval between beats based on the BPM.
	/// </summary>
	private void CalculateBeatInterval()
    {
        _beatInterval = 60f / _bpm;
    }

	public void ChangeMusicBPM(int BPM)
	{

		_bpm = BPM;
		CalculateBeatInterval();
		TickSystem.Instance.ChangeTimeSpeed(_beatInterval);
	}

	public void ChangeCurrentMusic(MusicTemplate currentMusicTemplate)
	{
		_currentMusicTemplate = currentMusicTemplate;
	}

}