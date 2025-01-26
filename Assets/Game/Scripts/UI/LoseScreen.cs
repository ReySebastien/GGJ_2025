using UnityEngine;
using DG.Tweening;

namespace GGJ
{
    public class LoseScreen : Endscreen
    {
        protected override void Animation()
        {
            Sequence sequence = DOTween.Sequence()
                .Append(_rectTransform.DOAnchorPosY(0f, 0.35f).SetEase(Ease.OutSine))
                .Append(_rectTransform.DOScale(new Vector3(1.5f, 0.7f, 1f), 0.3f).SetEase(Ease.OutSine))
                .AppendCallback(() => _rectTransform.localScale = Vector3.one)
                .Append(_rectTransform.DOPunchScale(new Vector3(-0.4f, 1.4f, 1f), 0.4f))
                .AppendInterval(0.2f)
                .SetUpdate(true);
        }
    }
}
