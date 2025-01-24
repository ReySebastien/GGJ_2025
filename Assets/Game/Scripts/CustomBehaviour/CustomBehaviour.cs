using UnityEngine;

namespace GGJ
{
    public abstract class CustomBehaviour : MonoBehaviour
    {
        private GameManager _gm;

        protected GameManager _gameManager
        {
            get
            {
                if (_gm == null)
                    _gm = GameManager.Instance;

                return _gm;
            }
        }
    }
}
