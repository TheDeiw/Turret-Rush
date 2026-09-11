using UnityEngine;

namespace Gameplay.Enemy
{
    public abstract class EnemyBase : MonoBehaviour, IEnemy
    {
        public GameObject GameObject => gameObject;

        public abstract void Activate(Transform target);
        public abstract void ResetEnemy(Vector3 spawnPosition, Quaternion spawnRotation);
    }
}