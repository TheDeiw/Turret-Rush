using UnityEngine;

namespace Gameplay.Enemy
{
    public interface IEnemy
    {
        GameObject GameObject { get; }
        void Activate(Transform target);
        void ResetEnemy(Vector3 spawnPosition, Quaternion spawnRotation);
    }
}