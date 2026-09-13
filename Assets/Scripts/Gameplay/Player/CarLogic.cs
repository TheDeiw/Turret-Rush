using System;
using UnityEngine;

namespace Gameplay.Player
{
    public class CarLogic : MonoBehaviour
    {
        [Header("Wave Settings")]
        [SerializeField] private float waveAmplitude = 0.5f;
        [SerializeField] private float waveFrequency = 0.02f;

        [Header("Wheels Settings")]
        [SerializeField] private Transform[] wheels;
        [SerializeField] private float wheelRadius = 0.35f;

        [Header("VFX")]
        [SerializeField] private ParticleSystem[] dustParticles;

        public event Action OnFinishReached;

        private float _startX;
        private float _startZ;
        private bool _isWaving;

        public void StartWaving(float startZ)
        {
            _startZ = startZ;
            _startX = transform.localPosition.x;
            _isWaving = true;

            if (dustParticles != null)
            {
                foreach (var particle in dustParticles)
                {
                    particle.Play();
                }
            }
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

            foreach (var particle in dustParticles)
            {
                particle.Clear();
                particle.Stop();
            }
        }

        public void UpdateWave(float currentZ)
        {
            if (!_isWaving) return;

            var distance = currentZ - _startZ;
            var wave = Mathf.Sin(distance * waveFrequency) * waveAmplitude;

            transform.localPosition = new Vector3(_startX + wave, 0f, 0f);

            var slope = waveAmplitude * waveFrequency * Mathf.Cos(distance * waveFrequency);
            var rotationAngle = Mathf.Atan(slope) * Mathf.Rad2Deg;
            transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);

            RotateWheels(distance);
        }

        private void RotateWheels(float deltaDistance)
        {
            if (wheels == null || wheels.Length == 0) return;

            var angle = (deltaDistance / wheelRadius) * Mathf.Rad2Deg;

            foreach (var wheel in wheels)
            {
                if (wheel)
                {
                    wheel.Rotate(Vector3.right, angle, Space.Self);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Finish"))
            {
                OnFinishReached?.Invoke();
            }
        }
    }
}