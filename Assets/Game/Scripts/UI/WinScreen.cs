using DG.Tweening;
using UnityEngine;

namespace GGJ
{
    public class WinScreen : Endscreen
    {
        protected override void Animation()
        {
            _rectTransform.localScale = Vector3.zero;
            Sequence sequence = DOTween.Sequence()
                .Append(_rectTransform.DOScale(Vector3.one * 1.7f, 0.15f).SetEase(Ease.OutSine))
                .Append(_rectTransform.DOScale(Vector3.one * 1.8f, 0.3f).SetEase(Ease.InOutSine))
                .AppendInterval(0.2f)
                .Append(_rectTransform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack))
                .SetUpdate(true);
        }
    }
}
