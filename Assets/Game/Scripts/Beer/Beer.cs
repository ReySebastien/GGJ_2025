using DG.Tweening;
using UnityEngine;

namespace GGJ
{
    public class Beer : CustomBehaviour
    {
        [Header("---Parameters---")]
        [SerializeField] private float _maxBeerScale;
        [SerializeField] private AnimationCurve _beerVolumeAnimCurve;

        [Header("---References---")]
        [SerializeField] private Transform _rotatePivot;
        [SerializeField] private Transform _lateRotatePivot;
        [SerializeField] private Transform _beer;
        [SerializeField] private Transform _lateBeer;
        public static Beer Instance { get; private set; }
        private float _volume = 1f;


        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {

        }

        void FixedUpdate()
        {
            _rotatePivot.rotation = Quaternion.Slerp(_rotatePivot.rotation, Quaternion.identity, 0.05f);
            _lateRotatePivot.rotation = Quaternion.Slerp(_lateRotatePivot.rotation, Quaternion.identity, 0.05f);
        }

        public Sequence TitlBeer()
        {
            Sequence sequence = DOTween.Sequence()
                .Append(transform.DORotate(new Vector3(0f, 0f, -50f), 5f, RotateMode.Fast).SetEase(Ease.InOutBack))
                .InsertCallback(1f, () => SetBeerVolume(_volume - 0.25f, 5f))
                .AppendInterval(1f)
                .Append(transform.DORotate(new Vector3(0f, 0f, 0f), 4f, RotateMode.Fast).SetEase(Ease.InOutBack));

            return sequence;
        }

        public void ReduceBeerVolume(float duration) => SetBeerVolume(_volume - 0.25f, duration);

        private void SetBeerVolume(float volume, float duration)
        {
            _volume = Mathf.Clamp01(volume);

            _beer.DOKill();
            _beer.DOScaleY(_volume * _maxBeerScale, duration).SetEase(_beerVolumeAnimCurve);

            InvokeCallback(() => BeerCounter.Instance.OnBeerReduced(), duration * 0.5f);

            _lateBeer.DOKill();
            _lateBeer.DOScaleY(_volume * _maxBeerScale, duration + 0.4f).SetEase(_beerVolumeAnimCurve).SetDelay(0.2f);
        }

    }
}
