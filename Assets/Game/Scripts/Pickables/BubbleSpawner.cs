using UnityEngine;

namespace GGJ
{
    public class BubbleSpawner : CustomBehaviour
    {
        [SerializeField] private BubblePool _bubblePool;
        [Header("D�lais entre chaque spawn de bulle")]
        [SerializeField] private float _spawnInterval = 1.0f;
        [Header("Taille de la zone de spawn des bulles")]
        [SerializeField] private float _spawnAreaSize = 5.0f;

        private void Start()
        {
            InvokeRepeating(nameof(SpawnBubble), _spawnInterval, _spawnInterval);
        }

        private void SpawnBubble()
        {
            PickableBubble bubble = _bubblePool.GetBubble();

            if (bubble == null)
                return;

            Vector2 spawnPosition = new Vector2(
                Random.Range(-_spawnAreaSize / 2, _spawnAreaSize / 2),
                transform.position.y
            );

            bubble.transform.position = spawnPosition;
            bubble.Init();
        }
    }
}
