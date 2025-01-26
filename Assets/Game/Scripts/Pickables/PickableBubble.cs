using UnityEngine;
using UnityEngine.Pool;
using DG.Tweening;

namespace GGJ
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PickableBubble : CustomBehaviour
    {

        [Header("Vitesse verticale des bulles")]
        [SerializeField] private float _minSpeed = 0f;
        [SerializeField] private float _maxSpeed = 2f;

        [Header("Taille minimum et maximum des bulles")]
        [SerializeField] private float _minBubbleScale = 0f;
        [SerializeField] private float _maxBubbleScale = 1f;

        [Header("Largeur min et max des mouvements horizontaux des bulles")]
        [SerializeField] private float _minLateralRange = 0f;
        [SerializeField] private float _maxLateralRange = 0.5f;

        [Header("Vitesse de d�placement horizontales des bulles")]
        [SerializeField] private float _minLateralSpeed = 0f;
        [SerializeField] private float _maxLateralSpeed = 1f;

        [Header("�a tu touches je t'atomise")]
        [SerializeField] private float _bubbleDespawnHeight = 10.0f;

        [Header("---Parameters---")]
        [SerializeField] private bool _multiColor;
        [SerializeField] private Gradient _multiColorGradient;

        private float _currentMoveSpeed = 2.0f;
        private float _currentLateralSpeed = 1f;
        private float _currentLateralRange = 0.5f;
        private float _bubbleScale = 0f;
        private float _timeOffset = 0f;
        private float _timeOffsetV2 = 0f;
        private Vector2 _startPosition;
        private Vector2 _direction = Vector2.zero;
        private Rigidbody2D _bubbleBody;
        private IObjectPool<PickableBubble> _pool;

        private void Awake()
        {
            _bubbleBody = GetComponent<Rigidbody2D>();
        }

        public void Init()
        {
            _timeOffset = Random.Range(0.1f, 0.5f);
            _timeOffsetV2 = Random.Range(0.1f, 0.5f);
            _bubbleScale = Random.Range(_minBubbleScale, _maxBubbleScale);
            _currentMoveSpeed = Random.Range(_minSpeed, _maxSpeed);
            _currentLateralRange = Random.Range(_minLateralRange, _maxLateralRange);
            _currentLateralSpeed = Random.Range(_minLateralSpeed, _maxLateralSpeed);
            _direction = Vector2.up;
            _startPosition = _bubbleBody.position;

            transform.localScale = Vector3.zero;
            transform.DOScale(_bubbleScale, 0.5f)
                .SetEase(Ease.OutBounce);

            if (_multiColor)
            {
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                spriteRenderer.color = _multiColorGradient.Evaluate(Random.Range(0f, 1f));
            }
        }

        public void SetPool(IObjectPool<PickableBubble> pool)
        {
            this._pool = pool;
        }

        private void FixedUpdate()
        {
            Vector2 newPosition = _bubbleBody.position + _direction * _currentMoveSpeed * Time.fixedDeltaTime;

            float mainSin = Mathf.Sin(Time.time * _timeOffset);
            float subSin = Mathf.Sin(Time.time * _timeOffsetV2) * 0.5f;

            float lateralMovement = mainSin * subSin * _currentLateralSpeed * _currentLateralRange;

            _bubbleBody.MovePosition(newPosition + new Vector2(lateralMovement, 0));

            if (transform.position.y > _bubbleDespawnHeight)
            {
                ReturnToPool();
            }
        }

        public void SetDirection(Vector2 direction)
        {
            _direction = direction.normalized;
        }

        private void ReturnToPool()
        {
            transform.DOKill(true);
            _pool.Release(this);
        }

        public void Pop()
        {
            ReturnToPool();
        }
    }
}
