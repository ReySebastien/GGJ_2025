using UnityEngine;
using DG.Tweening;
using System.Collections;
using System;
using Unity.VisualScripting;
using System.Collections.Generic;


namespace GGJ
{
    public class Player : CustomBehaviour
    {
        [SerializeField] public float baseSpeed = 5f;
        [SerializeField] private float _deadZone = 5f;
        [SerializeField] private LayerMask _camRayLayer;
        [SerializeField] private float _gravity;
        [SerializeField] private float _bubbleGrowValue;
        [SerializeField] private float _bubbleProutValue;
        [SerializeField] private List<Sprite> _spriteList;
        [SerializeField] private SpriteRenderer _actualSprite;

        [Header("Stamina Settings")]
        [SerializeField] private float _staminaMax = 10f; // Stamina maximale
        [SerializeField] private float _staminaConsumptionRate = 2f; // Consommation par seconde pendant l'accélération
        [SerializeField] private float _staminaCurrent; // Serialize informatif
        [SerializeField] private float _maxPickableBubble; // défini le pourcentage de régen d'une bulle (= 4 ? 25%/bubble) 
        
        [Header("Propelling Settings")]
        [SerializeField] private AnimationCurve _accelerationCurve;
        [SerializeField] private float _accelerationDuration = 2f; // Durée de base de l'accélération
        [SerializeField] private float _propellingMaxSpeed = 10f;
        
        private Rigidbody2D _rigidbody2D;
        // public Transform objectToMove;   // GameObject qui doit suivre la souris
        private Vector2 _moveDirection;
        private Vector2 _gravityDirection = Vector2.up;
        private Vector2 _suckDirection;
        private Vector2 _velocity;
        private bool _isProuting;
        private Tweener _proutingTweener;
        
        private float _accelerationTimer = 0f; // Temps d'accélération
        private bool _isAccelerating = false; // Indique si on est en phase d'accélération
        

        private Camera mainCamera;

        void Awake()
        {
            mainCamera = Camera.main;
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _actualSprite.sprite = _spriteList[0];

        }
        
        void FixedUpdate()
        {
            DepleteStamina();
            Accelerate();
            Vector3 mouseScreenPosition = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mouseScreenPosition);

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _camRayLayer))
            {
                _velocity = _gravityDirection * _gravity;
                _rigidbody2D.linearVelocity = _velocity;
                return;
            }

            Vector3 mouseWorldPosition = hit.point;
            mouseWorldPosition.z = 0;
            _moveDirection = mouseWorldPosition - transform.position;
            
            if(_moveDirection.magnitude < _deadZone)
            {
                _velocity = _gravityDirection * _gravity;
                _rigidbody2D.linearVelocity = _velocity;
                return;
            }
            
            _moveDirection.Normalize();
            _velocity = _moveDirection * (baseSpeed * GetAccelerationFactor());
            _velocity += _gravityDirection * _gravity;
            _rigidbody2D.linearVelocity = _velocity;
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

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && _staminaCurrent > 0f)
                StartProut();
            else if (Input.GetMouseButtonUp(0))
                StopProut();

            UpdateStaminaHehe();


        }

        private void StartProut()
        {
            if (_isProuting)
                return;

            _isProuting = true;
            float remainingStamina = _staminaCurrent / _staminaMax;
            float durationMax = (1f/_staminaConsumptionRate) * _staminaMax;
            float duration = Mathf.Lerp(0f, durationMax, remainingStamina);
            _proutingTweener = transform.DOScale(1f, duration)
                .OnUpdate(() => _staminaCurrent = Mathf.Clamp(_staminaCurrent - _staminaConsumptionRate * Time.deltaTime, 0f, _staminaMax))
                .OnComplete(() => StopProut());
        }

        private void StopProut()
        {
            if (!_isProuting)
                return;

            _isProuting = false;
            if( _proutingTweener != null && _proutingTweener.IsPlaying())
            {
                _proutingTweener.Kill();
                _proutingTweener = null;
            }
        }

        private void Accelerate()
        {
            // Vérifie si le joueur appuie sur le bouton "Jump" et s'il reste de la stamina
            if (Input.GetMouseButton(0) && _staminaCurrent > 0f) 
            {
                _isAccelerating = true;
                _actualSprite.sprite = _spriteList[1];
                _accelerationTimer += Time.fixedDeltaTime;

                // Ajuste la durée d'accélération en fonction de la stamina restante
                float staminaFactor = _staminaCurrent / _staminaMax; // Facteur de stamina (0 à 1)
                _accelerationTimer = Mathf.Clamp(_accelerationTimer, 0f, _accelerationDuration * staminaFactor);
            }
            else
            {
                if(_gravity != 6f)
                {
                    _actualSprite.sprite = _spriteList[0];
                }
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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.TryGetComponent(out PickableBubble bubble)) 
                return;

            if (_isProuting)
                StopProut();

            RefreshSize(1);
            RestoreStamina();
            bubble.Pop();
        }


        private void RefreshSize(float value)
        {
            if (value < 0f)
            {
                transform.localScale += Vector3.one * _bubbleProutValue;
                
            }
            else
            {
                transform.DOKill(true);
                transform.localScale = new Vector3()
                {
                    x = Mathf.Clamp(transform.localScale.x + _bubbleGrowValue, 1f, 1f + _maxPickableBubble * _bubbleGrowValue ),
                    y = Mathf.Clamp(transform.localScale.y + _bubbleGrowValue, 1f, 1f + _maxPickableBubble * _bubbleGrowValue ),
                    z = 1f
                };
                transform.DOPunchScale(new Vector3(0.3f, 0.5f, 0f), 1f, 5, 0.5f);
            }

        }

        private void UpdateStaminaHehe()
        {
            if (_staminaCurrent == 0f)
            {
                _gravity = 2f;
            }

            else if (_staminaCurrent < 5f && _staminaCurrent >= 2.5f)
            {
                _gravity = 3f;
            }

            else if( _staminaCurrent < 7.5f &&  _staminaCurrent >=5f)
            {
                _gravity = 4f;
            }

            else if( _staminaCurrent < 10f && _staminaCurrent >= 7.5f)
            {
                _gravity = 5f;
            }

            else if(_staminaCurrent == 10f)
            {
                _gravity = 6f;
                _actualSprite.sprite = _spriteList[2];
            }
        }

        public void PopPlayer()
        {

        }
    }

    
}
