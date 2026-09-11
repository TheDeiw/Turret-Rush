using Core;
using Gameplay.Combat;
using UnityEngine;
using Zenject;

namespace Gameplay.Player
{
    public class CarLogic : MonoBehaviour
    {
        [Header("Wave Settings")]
        [SerializeField] private float waveAmplitude = 0.5f;
        [SerializeField] private float waveFrequency = 0.02f;

        private float _startX;
        private float _startZ;
        private bool _isWaving;

        public void StartWaving(float startZ)
        {
            _startZ = startZ;
            _startX = transform.localPosition.x;
            _isWaving = true;
        }

        public void StopWaving()
        {
            _isWaving = false;
        }

        public void ResetPosition()
        {
            _isWaving = false;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public void UpdateWave(float currentZ)
        {
            if (!_isWaving) return;

            float distance = currentZ - _startZ;
            float wave = Mathf.Sin(distance * waveFrequency) * waveAmplitude;

            transform.localPosition = new Vector3(_startX + wave, 0f, 0f);

            float slope = waveAmplitude * waveFrequency * Mathf.Cos(distance * waveFrequency);
            float rotationAngle = Mathf.Atan(slope) * Mathf.Rad2Deg;
            transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
        }
    }
}