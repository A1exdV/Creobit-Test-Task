using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Reflex.Attributes;
using Reflex.Extensions;
using SceneLoader;
using Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Main_Menu.Scripts
{
    public class GameMode : MonoBehaviour
    {
        [SerializeField] private Button load;
        [SerializeField] private Button play;
        [SerializeField] private Button release;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI messageText;

        private GameConfig _config;
        
        
        private AsyncOperationHandle _handle;
        
        public void SetConfig(GameConfig config)
        {
            if(_config) return;
            
            _config = config;
            nameText.text = _config.GameName;
 
            CheckIfSceneCached();
        }

        private void Awake()
        {
            ButtonsInteraction(ButtonsState.Loading);
            load.onClick.AddListener(OnLoad);
            play.onClick.AddListener(OnPlay);
            release.onClick.AddListener(OnRelease);
        }

        private void CheckIfSceneCached()
        {
            ButtonsInteraction(ButtonsState.Loading);
            messageText.text = "Searching...";
            
            var handle  = Addressables.GetDownloadSizeAsync(_config.GameScene);
            
            handle.Completed += operationHandle =>
            {
                if (operationHandle.Result == 0 & handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"{_config.GameName} cache found.");
                    _handle = Addressables.DownloadDependenciesAsync(_config.GameScene);
                    ButtonsInteraction(ButtonsState.Loaded);
                }
                else
                {
                    Debug.Log($"{_config.GameName} cache not found.");
                    ButtonsInteraction(ButtonsState.Unloaded);
                }
            };
        }

        private void OnLoad()
        {
            Debug.Log($"{_config.GameName} trying to download.");
            DownloadDependencies();
        }
        
        private void OnPlay()
        {
            Debug.Log($"{_config.GameName} starting game.");
            SceneLoaderService.LoadSceneByReference(_config.GameScene);
        }
        
        private void OnRelease()
        {
            if (_handle.IsValid())
            {
                Addressables.Release(_handle);
                Addressables.ClearDependencyCacheAsync(_config.GameScene);
            }
            Debug.Log($"{_config.GameName} cache released.");
            ButtonsInteraction(ButtonsState.Unloaded);
        }

        private void DownloadDependencies()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                StartCoroutine(OutputError("Network Error"));
                return;
            }
            Debug.Log($"To Download - {Addressables.GetDownloadSizeAsync(_config.GameScene).Result}");
            
            var downloadDependencies = Addressables.DownloadDependenciesAsync(_config.GameScene);
            downloadDependencies.Completed += OnDependenciesDownloaded;
            
            StartCoroutine(Progress(downloadDependencies));
        }

        private void OnDependenciesDownloaded(AsyncOperationHandle handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"{_config.GameName} downloaded.");
                _handle = handle;
                ButtonsInteraction(ButtonsState.Loaded);
            }
            else
            {
                StartCoroutine(OutputError("Error on download"));
            }
        }

        private IEnumerator OutputError(string errorText)
        {
            messageText.enabled = true;
            messageText.text = errorText;
            yield return new WaitForSeconds(5);
            ButtonsInteraction(ButtonsState.Unloaded);
        }
        
        private IEnumerator Progress(AsyncOperationHandle handle)
        {
            float progress = 0;
            messageText.enabled = true;
            while (!handle.IsDone)
            {
                progress = handle.PercentComplete * 100;
                messageText.text = $"{progress:0.##}%";
                yield return null;
            }
            messageText.enabled = false;
        }
        
        private IEnumerator Progress(AsyncOperationHandle<SceneInstance> handle)
        {
            float progress = 0;
            messageText.enabled = true;
            while (!handle.IsDone)
            {
                progress = handle.PercentComplete * 100;
                messageText.text = $"{progress:0.##}%";
                yield return null;
            }
            messageText.enabled = false;
        }
        
        private void ButtonsInteraction(ButtonsState state)
        {
            switch (state)
            {
                case ButtonsState.Loaded:
                    play.interactable = true;
                    release.interactable = true;
                    load.interactable = false;
                    messageText.enabled = false;
                    break;
                case ButtonsState.Unloaded:
                    play.interactable = false;
                    release.interactable = false;
                    load.interactable = true;
                    messageText.enabled = false;
                    break;
                case ButtonsState.Loading:
                    play.interactable = false;
                    release.interactable = false;
                    load.interactable = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }

    internal enum ButtonsState
    {
        Loaded,
        Unloaded,
        Loading
    }
}
