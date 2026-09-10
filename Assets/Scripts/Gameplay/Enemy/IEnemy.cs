using UnityEngine;

namespace Gameplay.Enemy
{
    public interface IEnemy
    {
        void Activate(Transform target);
    }
}