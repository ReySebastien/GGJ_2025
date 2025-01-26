using UnityEngine;

namespace GGJ
{
    public class DDOL : CustomBehaviour
    {
        [Header("---References---")]
        [SerializeField] private AudioSource _bgm;

        public static DDOL Instance { get; private set; }



        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                transform.SetParent(null);
                _bgm.Play();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
