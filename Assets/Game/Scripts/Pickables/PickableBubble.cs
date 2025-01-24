using UnityEngine;
using UnityEngine.Pool;
using DG.Tweening;

namespace GGJ
{
    public class PickableBubble : CustomBehaviour
    {
        [SerializeField] private float _ascentSpeed = 2.0f; // Vitesse de montée de la bulle
        [SerializeField] private float _bubbleDespawnHeight = 10.0f;
        private IObjectPool<PickableBubble> _pool;

        public void SetPool(IObjectPool<PickableBubble> pool)
        {
            this._pool = pool;
        }

        private void Update()
        {
            // Déplace la bulle vers le haut à une vitesse constante
            transform.Translate(Vector2.up * _ascentSpeed * Time.deltaTime);

            if (transform.position.y > _bubbleDespawnHeight)
            {
                ReturnToPool();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Logique de fusion avec la bulle du joueur
            if (other.CompareTag("PlayerBubble"))
            {
                // Implémente la logique de fusion ici
                // Par exemple, augmenter la taille de la bulle du joueur et détruire cette bulle
                ReturnToPool();
            }
        }

        private void ReturnToPool()
        {
            // Retourne la bulle au pool
            _pool.Release(this);
        }

        public void Init()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(1f, 0.3f)
                .SetEase(Ease.OutBack);

            GetComponent<SpriteRenderer>().DOColor(new Color(Random.value, Random.value, Random.value), 1f);
        }
    }

}
