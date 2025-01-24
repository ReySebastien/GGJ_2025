using UnityEngine;
using UnityEngine.Serialization;

namespace GGJ
{
    public class PlayerController : CustomBehaviour
    {
        [SerializeField] private Transform _planeTransform; 
        [SerializeField] private Transform _player;   
        [SerializeField] private float _baseSpeed = 5f;     
        [SerializeField] private float _verticalSpeedFactor = 2f; 
        
        private Vector3 previousMouseWorldPosition; 
        private Camera _mainCamera;

        void Awake()
        {
            _mainCamera = Camera.main;
        }
        void Update()
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            Ray ray = _mainCamera.ScreenPointToRay(mouseScreenPosition);
            Plane xyPlane = new Plane(_planeTransform.forward, _planeTransform.position);

            if (xyPlane.Raycast(ray, out float distance))
            {
                Vector3 mouseWorldPosition = ray.GetPoint(distance);
                float verticalDirection = mouseWorldPosition.y - previousMouseWorldPosition.y;
                float speedMultiplier = verticalDirection > 0 ? _verticalSpeedFactor : 1f / _verticalSpeedFactor;
                float adjustedSpeed = _baseSpeed * speedMultiplier;
                
                // Movement
                _player.position = Vector3.Lerp(_player.position, mouseWorldPosition, Time.deltaTime * adjustedSpeed);
                
                previousMouseWorldPosition = mouseWorldPosition;
            }
            
        }
        
        
    }

}
