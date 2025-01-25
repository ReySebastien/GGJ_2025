using UnityEngine;

namespace GGJ
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class Obstacle : CustomBehaviour
    {
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
