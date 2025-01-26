using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GGJ
{
    [DefaultExecutionOrder(-1000)]
    public class GameManager : MonoBehaviour
    {
        [Header("---References---")]
        [SerializeField] private GameObject _winScreen;
        [SerializeField] private GameObject _loseScreen;

        public static GameManager Instance { get; private set; }
        private List<Manager> _managers;



        private void Awake()
        {
            Instance = this;
            _managers = GetComponents<Manager>().ToList();
        }

        public T GetManager<T>() where T : Manager => _managers.FirstOrDefault(m => m is T) as T;

        public void Win()
        {
            OnEnd();
            _winScreen.SetActive(true);

            IWinable[] winables = FindObjectsByType(typeof(MonoBehaviour), FindObjectsSortMode.None).OfType<IWinable>().ToArray();

            foreach (IWinable winable in winables)
                winable?.OnWin();
        }

        public void Lose()
        {
            OnEnd();
            _loseScreen.SetActive(true);
            ILosable[] losables = FindObjectsByType(typeof(MonoBehaviour), FindObjectsSortMode.None).OfType<ILosable>().ToArray();

            foreach (ILosable losable in losables)
                losable?.OnLose();
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
}
