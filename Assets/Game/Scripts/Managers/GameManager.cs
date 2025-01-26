using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

namespace GGJ
{
    [DefaultExecutionOrder(-1000)]
    public class GameManager : MonoBehaviour
    {
        [Header("---References---")]
        [SerializeField] private GameObject _winScreen;
        [SerializeField] private GameObject _loseScreen;
        [SerializeField] private GameObject _retryButton;
        [SerializeField] private Image _backgroundEndscreen;
        [SerializeField] private AudioSource _barSfx;

        public static GameManager Instance { get; private set; }
        private List<Manager> _managers;
        private GameState _gameState = GameState.Playing;


        private void Awake()
        {
            Instance = this;
            _managers = GetComponents<Manager>().ToList();
        }

        void Start()
        {
            SetTimeScale(1f);
            _barSfx.Play();
            _barSfx.DOFade(0.3f, 1f);
        }

        public T GetManager<T>() where T : Manager => _managers.FirstOrDefault(m => m is T) as T;

        public void Win()
        {
            if (_gameState != GameState.Playing)
                return;

            OnEnd();
            IWinable[] winables = FindObjectsByType(typeof(MonoBehaviour), FindObjectsSortMode.None).OfType<IWinable>().ToArray();
            foreach (IWinable winable in winables)
                winable?.OnWin();

            _barSfx.DOFade(0, 1f);

            InvokeCallback(() =>
            {
                _backgroundEndscreen.DOFade(0.7f, 1.2f).SetUpdate(true);
                _winScreen.SetActive(true);
                _retryButton.SetActive(true);
            }, 1f);
        }

        public void Lose()
        {
            if (_gameState != GameState.Playing)
                return;

            OnEnd();
            ILosable[] losables = FindObjectsByType(typeof(MonoBehaviour), FindObjectsSortMode.None).OfType<ILosable>().ToArray();
            foreach (ILosable losable in losables)
                losable?.OnLose();

            _barSfx.DOFade(0, 1f);

            InvokeCallback(() =>
            {
                _backgroundEndscreen.DOFade(0.7f, 1.2f).SetUpdate(true);
                _loseScreen.SetActive(true);
                _retryButton.SetActive(true);
            }, 1f);
        }

        private void OnEnd()
        {
            SetTimeScale(0.25f);
        }

        private void SetTimeScale(float timeScale)
        {
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }

        #region Misc

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

        #endregion

        void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.R))
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);

            if (Input.GetKeyDown(KeyCode.M))
                SetTimeScale(3f);
            else if (Input.GetKeyUp(KeyCode.M))
                SetTimeScale(1f);
#endif
        }

    }

    internal enum GameState { Playing, Win, Lose }
}
