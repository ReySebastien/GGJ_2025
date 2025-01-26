using System;
using System.Collections;
using UnityEngine;

namespace GGJ
{
    public abstract class CustomBehaviour : MonoBehaviour
    {

        #region refs

        private GameManager _gm;
        private EventManager _em;

        protected GameManager _gameManager
        {
            get
            {
                if (_gm == null)
                    _gm = GameManager.Instance;

                return _gm;
            }
        }

        protected EventManager _eventManager
        {
            get
            {
                if (_em == null)
                    _em = _gameManager.GetManager<EventManager>();

                return _em;
            }
        }

        #endregion

        #region methods

        protected void Log(object message) => Debug.Log(message == null ? "NULL" : message);

        protected virtual void InvokeCallback(Action action, float delay, float repeat = 0f) => StartCoroutine(InvokeCallbackCoroutine(action, delay, repeat));

        protected IEnumerator InvokeCallbackCoroutine(Action action, float delay, float repeat)
        {
            yield return new WaitForSecondsRealtime(delay);
            do
            {
                action?.Invoke();

                if (repeat > 0f)
                    yield return new WaitForSecondsRealtime(repeat);
            }
            while (repeat > 0f);
        }

        protected virtual void Win() => _gameManager.Win();
        protected virtual void Lose() => _gameManager.Lose();

        #endregion
    }
}
