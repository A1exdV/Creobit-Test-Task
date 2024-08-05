using System;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace SceneLoader
{
    [RequireComponent(typeof(Image))]
    public class LoadBar : MonoBehaviour
    {

        private Image _image;
        private IDisposable _disposable;
        private void Awake()
        {
            _image = GetComponent<Image>();
            _image.fillAmount = 0;
            _disposable = SceneLoaderService.Progress.Subscribe(HandleProgress);
        }

        private void HandleProgress(float value)
        {
            _image.fillAmount = value;
        }

        private void OnDestroy()
        {
            _disposable.Dispose();
        }
    }
}
