using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GGJ
{
    public class RetryButton : CustomBehaviour
    {
        void Start()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce).SetDelay(0.8f).SetUpdate(true);
        }

        public void OnClick()
        {
            DOTween.KillAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
