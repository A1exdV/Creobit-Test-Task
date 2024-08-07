using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
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
        private const int MainMenuIndex = 0;

        public static void LoadMainMenu()
        {
            LoadSceneByIndex(MainMenuIndex);
        }
        public static async void LoadSceneByIndex(int index)
        {
            await SceneManager.LoadSceneAsync(LoadingSceneIndex, LoadSceneMode.Single);
            await Task.Yield();
            
            var scene = SceneManager.LoadSceneAsync(index);
            _progress.Value = 0;
            
            do
            {
                await Task.Yield();
                _progress.Value = scene.progress;
                Debug.Log($"Scene Progress - {scene.progress}");
            } while (scene.progress < 0.9f);

        }
        
        

        public static async void LoadSceneByReference(AssetReference reference)
        {
            await SceneManager.LoadSceneAsync(LoadingSceneIndex, LoadSceneMode.Single);
            await Task.Yield();
            
            var handle = Addressables.LoadSceneAsync(reference);
            _progress.Value = 0;
            
            do
            {
                await Task.Yield();
                _progress.Value = handle.PercentComplete;
                Debug.Log($"Scene Progress - {handle.PercentComplete}");
            } while (!handle.IsDone);
        }
    }
}