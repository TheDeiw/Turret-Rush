using UnityEngine;


namespace Core
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject levelPrefab;
        [SerializeField] private int levelLength = 10;
        [SerializeField] private float segmentLength = 20f;

        private void Start()
        {
            GenerateLevel();
        }

        private void GenerateLevel()
        {
            for (int i = 0; i < levelLength; i++)
            {
                Vector3 position = new Vector3(0, 0, i * segmentLength);
                Instantiate(levelPrefab, position, Quaternion.identity);
            }
        }
    }
}

