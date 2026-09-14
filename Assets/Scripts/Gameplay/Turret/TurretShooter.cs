using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;
using Core;

namespace Gameplay.Turret
{
    public class TurretShooter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private TurretRecoil turretRecoil;
        [SerializeField] private TurretController turretController;

        [Header("Shooting Settings")]
        [SerializeField] private float fireRate = 0.15f;
        private float _fireTimer;

        private readonly List<Bullet> _activeBullets = new();

        private IObjectPool<Bullet> _bulletPool;
        private bool _isShooting;
        private GameManager _gameManager;

        [Inject]
        public void Construct(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        private void Awake()
        {
            _bulletPool = new ObjectPool<Bullet>(
                createFunc: () =>
                {
                    var b = Instantiate(bulletPrefab);
                    b.Init(_bulletPool);
                    return b;
                },
                actionOnGet: b =>
                {
                    b.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);
                    b.gameObject.SetActive(true);
                    _activeBullets.Add(b);
                },
                actionOnRelease: b =>
                {
                    b.gameObject.SetActive(false);
                    _activeBullets.Remove(b);
                },
                actionOnDestroy: b => Destroy(b.gameObject),
                collectionCheck: false,
                defaultCapacity: 30,
                maxSize: 60
            );
        }

        private void Start()
        {
            _gameManager.OnGameStarted += HandleGameStarted;
            _gameManager.OnGameWon += HandleGameWon;
            _gameManager.OnGameLost += HandleGameLost;
            _gameManager.OnGameRestarted += HandleGameRestart;
        }

        private void OnDestroy()
        {
            _gameManager.OnGameStarted -= HandleGameStarted;
            _gameManager.OnGameWon -= HandleGameWon;
            _gameManager.OnGameLost -= HandleGameLost;
            _gameManager.OnGameRestarted -= HandleGameRestart;
        }

        private void Update()
        {
            if (!_isShooting) return;

            _fireTimer += Time.deltaTime;
            if (_fireTimer >= fireRate)
            {
                _fireTimer = 0f;
                _bulletPool.Get();

                if (turretRecoil)
                {
                    turretRecoil.PlayRecoil();
                }
            }
        }

        private void HandleGameStarted()
        {
            _isShooting = true;
        }

        private void HandleGameWon()
        {
            _isShooting = false;
        }

        private void HandleGameLost()
        {
            _isShooting = false;
        }

        private void HandleGameRestart()
        {
            _fireTimer = 0f;

            foreach (var bullet in _activeBullets.ToArray())
            {
                bullet.ReturnToPool();
            }

            if (turretRecoil)
            {
                turretRecoil.ResetRecoil();
            }

            if (turretController)
            {
                turretController.ResetRotation();
            }
        }
    }
}