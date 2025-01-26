using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace GGJ
{
    

    public class SceneFader : CustomBehaviour
    {
        [SerializeField] private float _fadeTime;
        [SerializeField] private Image _fadeOutUIImage;
        [SerializeField] private bool _isEtien;

        void Start()
        {
            if (_isEtien)
                StartCoroutine(Fade(FadeDirection.Out));
        }
        public enum FadeDirection
        {
            In,
            Out
        }
        

        void SetColorImage(ref float _alpha, FadeDirection _fadeDirection)
        {
            _fadeOutUIImage.color = new Color(_fadeOutUIImage.color.r, _fadeOutUIImage.color.g, _fadeOutUIImage.color.b, _alpha);
            _alpha+=Time.deltaTime*(1/_fadeTime)* (_fadeDirection==FadeDirection.Out?-1:1);
        }

        public IEnumerator Fade(FadeDirection _fadeDirection)
        {
            float _alpha = _fadeDirection == FadeDirection.Out ? 1 : 0;
            float _fadeEndValue= _fadeDirection == FadeDirection.Out ? 0 : 1;
            if (_fadeDirection == FadeDirection.Out)
            {
                while (_alpha >= _fadeEndValue)
                {
                    SetColorImage(ref _alpha, _fadeDirection);
                    yield return null;
                }
                _fadeOutUIImage.enabled = false;
            }
            else
            {
                _fadeOutUIImage.enabled = true;
                while (_alpha<= _fadeEndValue)
                {
                    SetColorImage(ref _alpha, _fadeDirection);
                    yield return null;
                }
            }
        }
        public IEnumerator FadeAndLoadScene(FadeDirection _fadeDirection, int _levelToLoad)
        {
            _fadeOutUIImage.enabled = true;
            yield return Fade(_fadeDirection);
            SceneManager.LoadScene(_levelToLoad);
        }

        public void CallFadeAndLoadScene(int _sceneToLoad)
        {
            StartCoroutine(FadeAndLoadScene(FadeDirection.In, _sceneToLoad));
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}
