using Audio;
using UnityEngine;
using Zenject;

namespace Gameplay.Player
{
    public class CarEngineAudio : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerController playerController;
        [SerializeField] private AudioSource engineSource;

        [Header("Engine Settings")]
        [SerializeField] private float idlePitch = 0.7f;
        [SerializeField] private float maxPitch = 1.2f;
        [SerializeField, Range(0f, 1f)] private float idleVolume = 0.15f;
        [SerializeField, Range(0f, 1f)] private float maxVolume = 0.4f;

        private AudioService _audioService;

        [Inject]
        public void Construct(AudioService audioService)
        {
            _audioService = audioService;
        }

        private void Start()
        {
            _audioService.OnSettingsChanged += ApplySettings;
            ApplySettings();

            engineSource.loop = true;
            engineSource.Play();
        }

        private void Update()
        {
            var speed = playerController.SpeedNormalized;
            engineSource.pitch = Mathf.Lerp(idlePitch, maxPitch, speed);
            engineSource.volume = Mathf.Lerp(idleVolume, maxVolume, speed);
        }

        private void OnDestroy()
        {
            if (_audioService)
            {
                _audioService.OnSettingsChanged -= ApplySettings;
            }
        }

        private void ApplySettings()
        {
            engineSource.mute = !_audioService.SfxEnabled;
        }
    }
}
