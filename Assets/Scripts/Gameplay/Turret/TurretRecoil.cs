using UnityEngine;

namespace Gameplay.Turret
{
    public class TurretRecoil : MonoBehaviour
    {
        [Header("Recoil Settings")]
        [SerializeField] private float recoilDistance = 0.2f;
        [SerializeField] private float returnSpeed = 15f;

        private Vector3 _initialLocalPosition;

        private void Awake()
        {
            _initialLocalPosition = transform.localPosition;
        }

        public void PlayRecoil()
        {
            transform.localPosition = _initialLocalPosition - Vector3.forward * recoilDistance;
        }

        private void Update()
        {
            if (transform.localPosition != _initialLocalPosition)
            {
                transform.localPosition = Vector3.Lerp(
                    transform.localPosition, 
                    _initialLocalPosition, 
                    Time.deltaTime * returnSpeed
                );
            }
        }

        public void ResetRecoil()
        {
            transform.localPosition = _initialLocalPosition;
        }
    }
}