using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ
{
    public class BeerCounter : CustomBehaviour
    {
        [Header("---References---")]
        [SerializeField] private Image _image;
        [SerializeField] private Sprite[] _sprites;

        public static BeerCounter Instance { get; private set; }
        private int _spriteIndex = 0;

        private void Awake()
        {
            Instance = this;
        }

        public void OnBeerReduced()
        {
            _spriteIndex = Mathf.Clamp(_spriteIndex + 1, 0, 4);
            _image.sprite = _sprites[_spriteIndex];
            transform.DOPunchScale(new Vector3(0.4f, -0.5f, 0f), 0.5f, 10, 1f);
        }
    }
}
