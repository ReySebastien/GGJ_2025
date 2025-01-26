using DG.Tweening;
using UnityEngine;

namespace GGJ
{
    public class Straw : CustomBehaviour
    {
        [Header("---Parameters---")]
        [SerializeField] private float _strawPosY;
        [SerializeField] private float _penetrationDuration = 1.2f;
        [SerializeField] private float _waitBeforeSuckDuration = 2f;
        [SerializeField] private float _suckDuration = 3f;

        [Header("---References---")]
        [SerializeField] private SpriteRenderer _typhoonSpriteRenderer;
        [SerializeField] private ParticleSystem _suckParticles;
        [SerializeField] private SpriteRenderer _strawSpriteRenderer;
        [SerializeField] private Collider2D _typhoonCollider;
        [SerializeField] private AudioSource _suckAudio;
        [SerializeField] private AudioSource _splashAudio;

        public static Straw Instance { get; private set; }
        private Vector3 _initialPosition;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            _initialPosition = transform.position;
        }

        public Sequence Show()
        {
            _strawSpriteRenderer.enabled = true;

            Sequence sequence = DOTween.Sequence()
            .Append(transform.DOMoveY(_strawPosY, _penetrationDuration).SetEase(Ease.OutBack))
            .AppendInterval(1f)
            .InsertCallback(_waitBeforeSuckDuration * 0.6f, () => PlayAudioRandomized(_splashAudio))
            .InsertCallback(_waitBeforeSuckDuration, StartSucking)
            .InsertCallback(_waitBeforeSuckDuration + _suckDuration * 0.4f, () => Beer.Instance.ReduceBeerVolume(2f))
            .Append(Shake())
            .InsertCallback(_waitBeforeSuckDuration + _suckDuration, StopSucking)
            .AppendInterval(1f)
            .Append(transform.DOMoveY(_initialPosition.y, _penetrationDuration).SetEase(Ease.InBack))
            .AppendCallback(() => _strawSpriteRenderer.enabled = false);

            return sequence;
        }

        private Sequence Shake()
        {
            Sequence sequence = DOTween.Sequence()
            .Append(transform.DOShakePosition(3f, new Vector3(0.05f, 0.05f, 0f), 20, 90, false, true, ShakeRandomnessMode.Full));

            return sequence;
        }

        private void StartSucking()
        {
            _typhoonSpriteRenderer.DOFade(0.7f, 1.4f);
            _suckParticles.Play();
            _typhoonCollider.enabled = true;

            _suckAudio.volume = 0f;
            _suckAudio.Play();
            _suckAudio.DOFade(0.75f, 1f);
        }

        private void StopSucking()
        {
            _typhoonSpriteRenderer.DOFade(0f, 0.8f);
            _suckParticles.Stop();
            _typhoonCollider.enabled = false;

            _suckAudio.DOFade(0, 1f)
                .OnComplete(() => _suckAudio.Stop());
        }
    }
}
