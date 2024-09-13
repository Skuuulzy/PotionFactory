using Components.Tick;
using System.Collections.Generic;
using UnityEngine;
using VComponent.Tools.Timer;

public class RhythmCaller : MonoBehaviour
{
    [SerializeField] private float _bpm = 120f; // BPM of the music
    [SerializeField] private MusicTemplate _currentMusicTemplate;
    [SerializeField] private AudioSource _audioSource;
    private float _beatInterval;
    private CountdownTimer _countdownTimer;
    private int _currentMusicBPMModifierIndex;
	public void StartMusic()
    {
        _audioSource.clip = _currentMusicTemplate.AudioClip;
		_bpm = _currentMusicTemplate.MusicBPMModifiers[0].BPM;
        _audioSource.volume = (float)_currentMusicTemplate.MusicBPMModifiers[0].VoiceVolume / 100;
        _currentMusicBPMModifierIndex = 0;
		_countdownTimer = new CountdownTimer(_currentMusicTemplate.MusicBPMModifiers[_currentMusicBPMModifierIndex + 1].Time);
        _countdownTimer.OnTimerStop += ChangeMusicBPM;
		_countdownTimer.Start();
		_audioSource.Play();
		CalculateBeatInterval();
        TickSystem.Instance.ChangeTimeSpeed(_beatInterval);
    }


	private void Update()
	{
		HandleVolumeLerp();
		if (_countdownTimer != null)
		{
			_countdownTimer.Tick(Time.deltaTime);
		}

	}

	/// <summary>
	/// Handles the volume interpolation based on the specified time interval.
	/// </summary>
	private void HandleVolumeLerp()
	{
		if(_currentMusicTemplate == null)
		{
			return;
		}

		if(_currentMusicBPMModifierIndex + 1 >= _currentMusicTemplate.MusicBPMModifiers.Count) 
		{
			return;
		}

		float currentTime = _audioSource.time;
		int startTime = _currentMusicTemplate.MusicBPMModifiers[_currentMusicBPMModifierIndex].Time;
		float startVolume = (float)_currentMusicTemplate.MusicBPMModifiers[_currentMusicBPMModifierIndex].VoiceVolume / 100;
		int endTime = _currentMusicTemplate.MusicBPMModifiers[_currentMusicBPMModifierIndex + 1].Time + 1;
		float endVolume = (float)(_currentMusicTemplate.MusicBPMModifiers[_currentMusicBPMModifierIndex + 1].VoiceVolume ) / 100;

		if (currentTime >= startTime && currentTime <= endTime)
		{

			float t = (currentTime - startTime) / (endTime - startTime);
			_audioSource.volume = Mathf.Lerp(startVolume , endVolume, t);
		}
		else if (currentTime > endTime)
		{

			_audioSource.volume = endVolume; // Ensure volume is set to final value
		}
	}

		/// <summary>
		/// Calculates the interval between beats based on the BPM.
		/// </summary>
		private void CalculateBeatInterval()
    {
        _beatInterval = 60f / _bpm;
    }

	private void ChangeMusicBPM()
	{
		_currentMusicBPMModifierIndex++;

		if (_currentMusicBPMModifierIndex >= _currentMusicTemplate.MusicBPMModifiers.Count)
		{
			return;
		}

		_bpm = _currentMusicTemplate.MusicBPMModifiers[_currentMusicBPMModifierIndex].BPM;
		//_audioSource.volume = (float)_currentMusicTemplate.MusicBPMModifiers[_currentMusicBPMModifierIndex - 1].VoiceVolume / 100;
		_countdownTimer = new CountdownTimer(_currentMusicTemplate.MusicBPMModifiers[_currentMusicBPMModifierIndex].Time);
		_countdownTimer.OnTimerStop += ChangeMusicBPM;
		_countdownTimer.Start();
		CalculateBeatInterval();
		TickSystem.Instance.ChangeTimeSpeed(_beatInterval);
	}

	public void ChangeCurrentMusic(MusicTemplate currentMusicTemplate)
	{
		_currentMusicTemplate = currentMusicTemplate;

	}

}