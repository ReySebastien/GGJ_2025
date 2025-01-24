using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GGJ
{
    [DefaultExecutionOrder(-1000)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        private List<Manager> _managers;


        private void Awake()
        {
            Instance = this;
            _managers = GetComponents<Manager>().ToList();
        }

        public T GetManager<T>() where T : Manager => _managers.FirstOrDefault(m => m is T) as T;

    }
}
