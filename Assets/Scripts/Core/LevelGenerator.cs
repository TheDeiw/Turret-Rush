using UnityEngine;

namespace Core
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject levelPrefab;
        [SerializeField] private int levelLength = 10;
        private const float SegmentLength = 75f;

        private void Start()
        {
            GenerateLevel();
        }

        private void GenerateLevel()
        {
            for (var i = 0; i < levelLength; i++)
            {
                var position = new Vector3(0, 0, i * SegmentLength);
                Instantiate(levelPrefab, position, Quaternion.identity);
            }
        }
    }
}

