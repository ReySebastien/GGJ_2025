using UnityEngine;

namespace GGJ
{
    public abstract class CustomBehaviour : MonoBehaviour
    {
        private GameManager _gm;
        private InputManager _im;

        protected GameManager _gameManager
        {
            get
            {
                if (_gm == null)
                    _gm = GameManager.Instance;

                return _gm;
            }
        }

        protected InputManager _inputManager
        {
            get
            {
                if (_im == null)
                    _im = _gameManager.GetManager<InputManager>();

                return _im;
            }
        }
    }
}
