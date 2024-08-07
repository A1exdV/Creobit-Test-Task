using Reflex.Attributes;
using SaveLoad;
using SceneLoader;
using UnityEngine;
using UnityEngine.UI;

namespace Clicker_Game.Scripts
{
    public class Clicker : MonoBehaviour
    {
        [SerializeField] private ClickerView clickerView;
        [SerializeField] private Button exitButton;
        
        private ClickerModel _clickerModel;
        private ClickerPresenter _clickerPresenter;
        
        [Inject] private ISaveLoad _saveLoad;
        private void Awake()
        {
            _clickerModel = new ClickerModel(_saveLoad);
            _clickerPresenter = new ClickerPresenter(_clickerModel, clickerView);
            
            exitButton.onClick.AddListener(OnGameQuit);
        }

        private void OnGameQuit()
        {
            _saveLoad.Save();
            SceneLoaderService.LoadMainMenu();
        }
        private void OnApplicationQuit()
        {
            _saveLoad.Save();
        }
    }
}