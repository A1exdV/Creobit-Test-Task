using System;
using System.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace SceneLoader
{
    public static class SceneLoaderService
    {
        public static Observable<float> Progress => _progress;
        private static readonly ReactiveProperty<float> _progress = new();

        private const int LoadingSceneIndex = 1;

        private static void EnableLoadingScene()
        {
            SceneManager.LoadScene(LoadingSceneIndex, LoadSceneMode.Additive);
        }

        public static async void LoadSceneByIndex(int index)
        {
            EnableLoadingScene();
            
            _progress.Value = 0;
            var scene = SceneManager.LoadSceneAsync(index);
            if (scene == null)
            {
                throw new IndexOutOfRangeException(index.ToString());
            }
            scene.allowSceneActivation = false;
            do
            {
                await Task.Yield();
                _progress.Value = scene.progress;
            } while (scene.progress < 0.9f);

            scene.allowSceneActivation = true;
        }

        public static async void LoadSceneByReference(AssetReference reference)
        {
            EnableLoadingScene();
            
            var handle = Addressables.LoadSceneAsync(reference);
            _progress.Value = 0;
            
            do
            {
                await Task.Yield();
                _progress.Value = handle.PercentComplete;
                Debug.Log(handle.PercentComplete);
            } while (!handle.IsDone);
        }
    }
}