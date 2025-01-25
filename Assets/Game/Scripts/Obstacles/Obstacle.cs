using UnityEngine;

namespace GGJ
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public abstract class Obstacle : CustomBehaviour
    {
        protected Rigidbody2D _rigidbody;
        protected Collider2D _collider;

        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
        }

        #region Trigger

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out MonoBehaviour behaviour))
                return;

            PopPlayer();
        }

        protected virtual void PopPlayer()
        {

        }

        #endregion

    }
}
