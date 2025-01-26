using UnityEngine;

namespace GGJ
{
    public abstract class Endscreen : CustomBehaviour
    {
        protected RectTransform _rectTransform;
        void Awake() => _rectTransform = GetComponent<RectTransform>();
        void Start() => Animation();
        protected abstract void Animation();
    }
}
