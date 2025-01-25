using UnityEngine;
using UnityEngine.Serialization;

namespace GGJ
{
    [DefaultExecutionOrder(10)]
    public class CameraController : CustomBehaviour
    {
        [Header("Target Settings")]
        [SerializeField] private Transform playerTransform; // La position du joueur que la caméra suit.

        [Header("Camera Settings")]
        [SerializeField] private Vector3 offset = new Vector3(0, 5, -10); // Décalage entre le joueur et la caméra.
        [SerializeField] private float followSpeed = 5f; // Vitesse du mouvement de la caméra.

        [Header("Bounds Settings")]
        [SerializeField] private Vector2 minBounds; // Limites minimales (X, Y).
        [SerializeField] private Vector2 maxBounds; // Limites maximales (X, Y).

        void FixedUpdate()
        {
            // Position cible de la caméra (position du joueur + offset)
            Vector3 targetPosition = playerTransform.position + offset;

            // Limiter la position cible aux limites spécifiées
            targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);

            // Interpolation linéaire pour un mouvement fluide
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
        
    }

}
