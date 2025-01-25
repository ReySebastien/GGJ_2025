using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace GGJ
{
    public class PlayerController : CustomBehaviour
    {
        [Header("GameObjects")]
        [SerializeField] private Transform _planeTransform;
        [SerializeField] private Rigidbody2D _playerRb;

        [Header("Speed Settings")]
        [SerializeField] private float _baseSpeed = 5f;
        [SerializeField] private float _verticalSpeedFactor = 2f;

        [Header("Friction Settings")]
        [SerializeField] private float _chaosModifier;

        [Header("Propelling Settings")]
        [SerializeField] private AnimationCurve _accelerationCurve;
        [SerializeField] private float _accelerationDuration = 2f; // Durée de base de l'accélération
        [SerializeField] private float _propellingMaxSpeed = 10f;

        [Header("Stamina Settings")]
        [SerializeField] private float _staminaMax = 10f; // Stamina maximale
        [SerializeField] private float _staminaConsumptionRate = 2f; // Consommation par seconde pendant l'accélération
        [SerializeField] private float _staminaCurrent; // Serialize informatif
        [SerializeField] private float _maxPickableBubble; // défini le pourcentage de régen d'une bulle (= 4 ? 25%/bubble)  

        private float _verticalDirection = 0;
        private Vector3 _previousMouseWorldPosition = Vector3.zero;
        private Camera _mainCamera;

        private float _accelerationTimer = 0f; // Temps d'accélération
        private bool _isAccelerating = false; // Indique si on est en phase d'accélération

        void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Confined;
        }

        void FixedUpdate()
        {
            DepleteStamina();
            Accelerate();
            MovePlayer();
        }

        private void DepleteStamina()
        {
            if (_isAccelerating)
            {
                _staminaCurrent -= _staminaConsumptionRate * Time.deltaTime;
                _staminaCurrent = Mathf.Clamp(_staminaCurrent - (_staminaConsumptionRate * Time.deltaTime), 0f,
                    _staminaMax);
            }
        }
        // Ajoute la stamina au ramassage d'une bulle
        public void RestoreStamina()
        {
            _staminaCurrent = Mathf.Clamp(_staminaCurrent+(_staminaMax/_maxPickableBubble), 0, _staminaMax);
        }

        private void Accelerate()
        {
            // Vérifie si le joueur appuie sur le bouton "Jump" et s'il reste de la stamina
            if (Input.GetButton("Jump") && _staminaCurrent > 0f)
            {
                _isAccelerating = true;
                _accelerationTimer += Time.deltaTime;

                // Ajuste la durée d'accélération en fonction de la stamina restante
                float staminaFactor = _staminaCurrent / _staminaMax; // Facteur de stamina (0 à 1)
                _accelerationTimer = Mathf.Clamp(_accelerationTimer, 0f, _accelerationDuration * staminaFactor);
            }
            else
            {
                _isAccelerating = false;
                _accelerationTimer = 0f; // Réinitialise le timer lorsqu'on relâche "Jump"
            }
        }

        private float GetAccelerationFactor()
        {
            if (_isAccelerating)
            {
                float normalizedTime = _accelerationTimer / _accelerationDuration; // Normalise le temps (0 à 1)
                return _accelerationCurve.Evaluate(normalizedTime) * _propellingMaxSpeed; // Récupère la valeur sur la courbe
            }
            return 1f; // Pas d'accélération (vitesse normale)
        }

        private void MovePlayer()
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            Ray ray = _mainCamera.ScreenPointToRay(mouseScreenPosition);
            Plane xyPlane = new Plane(_planeTransform.forward, _planeTransform.position);

            if (xyPlane.Raycast(ray, out float distance))
            {
                Vector3 mouseWorldPosition = ray.GetPoint(distance);
                float speedMultiplier;
                float adjustedSpeed;

                // Détermine si le curseur a bougé
                if (mouseWorldPosition != _previousMouseWorldPosition)
                {
                    // Calcule la direction verticale
                    _verticalDirection = mouseWorldPosition.y == _previousMouseWorldPosition.y
                        ? 0f
                        : Mathf.Sign(mouseWorldPosition.y - _previousMouseWorldPosition.y);
                    

                    // Ajuste la vitesse en fonction du dernier mouvement vertical
                    speedMultiplier = _verticalDirection < 0 ? 1f / _verticalSpeedFactor : _verticalSpeedFactor;

                    // Ajoute l'accélération à la vitesse ajustée
                    adjustedSpeed = _baseSpeed * speedMultiplier * GetAccelerationFactor();

                    // Mouvement chaotique si le mouvement est vers le bas
                    if (_verticalDirection < 0)
                    {
                        float chaosFactor = Mathf.PerlinNoise(Time.time * 5f, 0f) * 0.5f;
                        mouseWorldPosition.x += Random.Range(-chaosFactor * _chaosModifier, chaosFactor * _chaosModifier);
                    }

                    // Mise à jour de la position cible
                    _previousMouseWorldPosition = mouseWorldPosition;

                    // Déplacement immédiat
                    _playerRb.MovePosition(Vector3.Lerp(_playerRb.position, mouseWorldPosition, Time.fixedDeltaTime * adjustedSpeed));
                }
                else
                {
                    // Lerp vers la dernière position connue
                    speedMultiplier = _verticalDirection < 0 ? 1f / _verticalSpeedFactor : _verticalSpeedFactor;

                    // Ajoute l'accélération à la vitesse ajustée
                    adjustedSpeed = _baseSpeed * speedMultiplier * GetAccelerationFactor();

                    if (_verticalDirection < 0)
                    {
                        float chaosFactor = Mathf.PerlinNoise(Time.time * 5f, 0f) * 0.5f;
                        _previousMouseWorldPosition.x += Random.Range(-chaosFactor * _chaosModifier, chaosFactor * _chaosModifier);
                    }

                    _playerRb.MovePosition(Vector3.Lerp(_playerRb.position, _previousMouseWorldPosition, Time.fixedDeltaTime * adjustedSpeed));
                }
            }
        }
    }
}
