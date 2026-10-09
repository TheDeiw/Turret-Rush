using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Audio
{
    public class AudioService : MonoBehaviour
    {
        [Serializable]
        private class Sound
        {
            public SoundId id;
            public AudioClip[] clips;
            [Range(0f, 1f)] public float volume = 1f;
            [Range(0f, 0.5f)] public float pitchVariance = 0.05f;
            [Min(0f)] public float minInterval;
        }

        private const string MusicEnabledKey = "Audio.MusicEnabled";
        private const string SfxEnabledKey = "Audio.SfxEnabled";

        [Header("Music")]
        [SerializeField] private AudioClip musicClip;
        [SerializeField, Range(0f, 1f)] private float musicVolume = 0.5f;

        [Header("Sound Effects")]
        [SerializeField] private Sound[] sounds;
        [SerializeField, Min(1)] private int sfxVoices = 8;

        public event Action OnSettingsChanged;

        private readonly Dictionary<SoundId, Sound> _sounds = new();
        private readonly Dictionary<SoundId, float> _lastPlayTimes = new();

        private AudioSource _musicSource;
        private AudioSource[] _sfxSources;
        private int _nextSfxSource;

        public bool MusicEnabled { get; private set; }
        public bool SfxEnabled { get; private set; }

        private void Awake()
        {
            foreach (var sound in sounds)
            {
                _sounds[sound.id] = sound;
            }

            MusicEnabled = PlayerPrefs.GetInt(MusicEnabledKey, 1) == 1;
            SfxEnabled = PlayerPrefs.GetInt(SfxEnabledKey, 1) == 1;

            CreateSources();
            ApplySettings();

            // Starts silent; GameAudio fades it in to the level of the current game state.
            _musicSource.clip = musicClip;
            _musicSource.volume = 0f;
            if (musicClip)
            {
                _musicSource.Play();
            }
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }

        public void Play(SoundId id)
        {
            if (!_sounds.TryGetValue(id, out var sound) || sound.clips.Length == 0)
            {
                return;
            }

            // Unscaled time, so throttling keeps working while the game is paused.
            var now = Time.unscaledTime;
            if (_lastPlayTimes.TryGetValue(id, out var lastPlayTime) && now - lastPlayTime < sound.minInterval)
            {
                return;
            }
            _lastPlayTimes[id] = now;

            // Each sound gets its own source, so random pitch doesn't affect sounds that are still playing.
            var source = _sfxSources[_nextSfxSource];
            _nextSfxSource = (_nextSfxSource + 1) % _sfxSources.Length;

            source.clip = sound.clips[Random.Range(0, sound.clips.Length)];
            source.volume = sound.volume;
            source.pitch = 1f + Random.Range(-sound.pitchVariance, sound.pitchVariance);
            source.Play();
        }

        /// <param name="level">Multiplier of the base music volume, 0..1.</param>
        public void SetMusicLevel(float level, float duration)
        {
            DOTween.Kill(this);

            _musicSource.DOFade(musicVolume * level, duration)
                .SetUpdate(true)
                .SetTarget(this)
                .SetLink(gameObject);
        }

        public void SetMusicEnabled(bool isEnabled)
        {
            MusicEnabled = isEnabled;
            PlayerPrefs.SetInt(MusicEnabledKey, isEnabled ? 1 : 0);
            ApplySettings();
        }

        public void SetSfxEnabled(bool isEnabled)
        {
            SfxEnabled = isEnabled;
            PlayerPrefs.SetInt(SfxEnabledKey, isEnabled ? 1 : 0);
            ApplySettings();
        }

        private void CreateSources()
        {
            _musicSource = CreateSource("Music");
            _musicSource.loop = true;
            // Music keeps playing (ducked) when the listener is paused.
            _musicSource.ignoreListenerPause = true;

            _sfxSources = new AudioSource[sfxVoices];
            for (var i = 0; i < sfxVoices; i++)
            {
                _sfxSources[i] = CreateSource($"Sfx {i}");
            }
        }

        private AudioSource CreateSource(string sourceName)
        {
            var sourceObject = new GameObject(sourceName);
            sourceObject.transform.SetParent(transform, false);

            var source = sourceObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            return source;
        }

        private void ApplySettings()
        {
            _musicSource.mute = !MusicEnabled;

            foreach (var source in _sfxSources)
            {
                source.mute = !SfxEnabled;
            }

            OnSettingsChanged?.Invoke();
        }
    }
}
