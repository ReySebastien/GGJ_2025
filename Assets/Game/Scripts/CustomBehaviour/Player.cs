using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;
using UnityEditor;

namespace GGJ
{
    public class Player : CustomBehaviour, IWinable, ILosable
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

        [Header("---References---")]
        [SerializeField] private Collider2D _collider;
        [SerializeField] private SpriteRenderer[] _spriteRenderers;
        [SerializeField] private ParticleSystem _subBubblesParticles;

        private Rigidbody2D _rigidbody2D;
        private Vector2 _moveDirection;
        private Vector2 _gravityDirection = Vector2.up;
        private Vector2 _suckDirection;
        private bool _canMove = true;
        private Vector2 _velocity;
        private bool _isProuting;
        private Tweener _proutingTweener;
        private float _accelerationTimer = 0f; // Temps d'accélération
        private bool _isAccelerating = false; // Indique si on est en phase d'accélération

        private Camera mainCamera;

        #region Init

        void Awake()
        {
            mainCamera = Camera.main;
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _actualSprite.sprite = _spriteList[0];
        }

        #endregion

        #region Movement

        private void Update()
        {
            if (!_canMove)
                return;

            if (Input.GetMouseButtonDown(0) && _staminaCurrent > 0f)
                StartProut();
            else if (Input.GetMouseButtonUp(0))
                StopProut();

            UpdateStaminaHehe();
        }

        void FixedUpdate()
        {
            if (!_canMove)
                return;

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

            if (_moveDirection.magnitude < _deadZone)
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

        private void Accelerate()
        {
            if (Input.GetMouseButton(0) && _staminaCurrent > 0f)
            {
                _isAccelerating = true;
                _actualSprite.sprite = _spriteList[1];
                _accelerationTimer += Time.fixedDeltaTime;

                float staminaFactor = _staminaCurrent / _staminaMax;
                _accelerationTimer = Mathf.Clamp(_accelerationTimer, 0f, _accelerationDuration * staminaFactor);
            }
            else
            {
                if (_gravity != 6f)
                {
                    _actualSprite.sprite = _spriteList[0];
                }
                _isAccelerating = false;
                _accelerationTimer = 0f; // Réinitialise le timer lorsqu'on relâche "Jump"
            }
        }

        private void StartProut()
        {
            if (_isProuting)
                return;

            _isProuting = true;
            _subBubblesParticles.Play();
            float remainingStamina = _staminaCurrent / _staminaMax;
            float durationMax = (1f / _staminaConsumptionRate) * _staminaMax;
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
            _subBubblesParticles.Stop();
            if (_proutingTweener != null && _proutingTweener.IsPlaying())
            {
                _proutingTweener.Kill();
                _proutingTweener = null;
            }
        }

        private void RestoreStamina()
            => _staminaCurrent = Mathf.Clamp(_staminaCurrent + (_staminaMax / _maxPickableBubble), 0, _staminaMax);


        private float GetAccelerationFactor()
        {
            if (_isAccelerating)
            {
                float normalizedTime = _accelerationTimer / _accelerationDuration; // Normalise le temps (0 à 1)
                return _accelerationCurve.Evaluate(normalizedTime) * _propellingMaxSpeed; // Récupère la valeur sur la courbe
            }
            return 1f; // Pas d'accélération (vitesse normale)
        }

        #endregion

        #region Collision & Trigger

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out PickableBubble bubble))
                OnTouchedBubble(bubble);
            else if (collision.TryGetComponent(out DeathZone deathZone))
                PopPlayer();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.TryGetComponent(out Obstacle obstacle))
                PopPlayer();
        }

        private void OnTouchedBubble(PickableBubble bubble)
        {
            if (_isProuting)
                StopProut();

            RefreshSize(1);
            RestoreStamina();
            bubble.Pop();
        }

        #endregion

        #region Stamina

        private void RefreshSize(float value)
        {
            if (value < 0f)
            {
                transform.localScale += Vector3.one * _bubbleProutValue;
                return;
            }

            transform.DOKill(true);
            transform.localScale = new Vector3()
            {
                x = Mathf.Clamp(transform.localScale.x + _bubbleGrowValue, 1f, 1f + _maxPickableBubble * _bubbleGrowValue),
                y = Mathf.Clamp(transform.localScale.y + _bubbleGrowValue, 1f, 1f + _maxPickableBubble * _bubbleGrowValue),
                z = 1f
            };

            transform.DOPunchScale(new Vector3(0.3f, 0.5f, 0f), 1f, 5, 0.5f);
        }

        private void UpdateStaminaHehe()
        {
            // if (_staminaCurrent == 0f)
            //     _gravity = 2f;
            // else if (_staminaCurrent < 5f && _staminaCurrent >= 2.5f)
            //     _gravity = 3f;
            // else if (_staminaCurrent < 7.5f && _staminaCurrent >= 5f)
            //     _gravity = 4f;
            // else if (_staminaCurrent < 10f && _staminaCurrent >= 7.5f)
            //     _gravity = 5f;
            // else if (_staminaCurrent == 10f)
            // {
            //     _gravity = 6f;
            //     _actualSprite.sprite = _spriteList[2];
            // }

            switch (_staminaCurrent)
            {
                case 0f:
                    _gravity = 2f;
                    break;
                case < 5f and >= 2.5f:
                    _gravity = 3f;
                    break;
                case < 7.5f and >= 5f:
                    _gravity = 4f;
                    break;
                case < 10f and >= 7.5f:
                    _gravity = 5f;
                    break;
                case 10f:
                    _gravity = 6f;
                    _actualSprite.sprite = _spriteList[2];
                    break;
            }
        }

        #endregion

        #region Death and End

        public void PopPlayer()
        {
            _collider.enabled = false;
            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

            foreach (SpriteRenderer spriteRenderer in _spriteRenderers)
                spriteRenderer.enabled = false;

            Lose();
        }

        public void OnWin() => _canMove = false;
        public void OnLose() => _canMove = false;

        #endregion
    }


}
