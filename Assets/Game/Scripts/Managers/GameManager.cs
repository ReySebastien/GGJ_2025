using UnityEngine;

namespace GGJ
{
    [DefaultExecutionOrder(-1000)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
    }
}
