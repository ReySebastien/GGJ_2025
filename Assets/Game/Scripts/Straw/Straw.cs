using DG.Tweening;
using UnityEngine;

namespace GGJ
{
    public class Straw : CustomBehaviour
    {
        [Header("---Parameters---")]
        [SerializeField] private float _strawPosY;
        [SerializeField] private SpriteRenderer _typhoonSpriteRenderer;
        [SerializeField] private ParticleSystem _suckParticles;

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
            Sequence sequence = DOTween.Sequence()
            .Append(transform.DOMoveY(_strawPosY, 1.2f).SetEase(Ease.OutBack))
            .AppendInterval(1f)
            .InsertCallback(2.2f, StartSucking)
            .InsertCallback(2.7f, () => Beer.Instance.ReduceBeerVolume(2f))
            .Append(Shake())
            .InsertCallback(4.5f, StopSucking)
            .AppendInterval(1f)
            .Append(transform.DOMoveY(_initialPosition.y, 1.2f).SetEase(Ease.InBack));

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
        }

        private void StopSucking()
        {
            _typhoonSpriteRenderer.DOFade(0f, 0.8f);
            _suckParticles.Stop();
        }
    }
}
