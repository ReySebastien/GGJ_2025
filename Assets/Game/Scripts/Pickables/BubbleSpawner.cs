using UnityEngine;

namespace GGJ
{
    public class BubbleSpawner : CustomBehaviour
    {
        [SerializeField] private BubblePool _bubblePool; //Ref du script Bubble spawn
        [SerializeField] private float _spawnInterval = 1.0f; // Intervalle de temps entre chaque apparition de bulle
        [SerializeField] private Vector2 _spawnAreaSize = new Vector2(5.0f, 1.0f); // Taille de la zone de spawn

        private void Start()
        {
            // Démarre la génération répétée des bulles
            InvokeRepeating(nameof(SpawnBubble), _spawnInterval, _spawnInterval);
        }

        private void SpawnBubble()
        {
            // Obtient une bulle du pool
            PickableBubble bubble = _bubblePool.GetBubble();

            if (bubble == null)
            {
                // Si le pool ne peut pas fournir de nouvelle bulle, on ne fait rien
                return;
            }

            bubble.Init();
            // Détermine une position aléatoire dans la zone de spawn
            Vector2 spawnPosition = new Vector2(
                Random.Range(-_spawnAreaSize.x / 2, _spawnAreaSize.x / 2),
                transform.position.y
            );

            // Positionne la bulle
            bubble.transform.position = spawnPosition;
        }
    }
}
