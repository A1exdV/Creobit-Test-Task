using Reflex.Attributes;
using SaveLoad;
using UnityEngine;

namespace Clicker_Game.Scripts
{
    public class Clicker : MonoBehaviour
    {
        [SerializeField] private ClickerView clickerView;
        
        private ClickerModel _clickerModel;
        private ClickerPresenter _clickerPresenter;
        
        [Inject] private ISaveLoad _saveLoad;
        private void Start()
        {
            _clickerModel = new ClickerModel(_saveLoad);
            _clickerPresenter = new ClickerPresenter(_clickerModel, clickerView);
        }

        private void OnApplicationQuit()
        {
            _saveLoad.Save();
        }

        private void OnDestroy()
        {
            _saveLoad.Save();
        }
    }
}