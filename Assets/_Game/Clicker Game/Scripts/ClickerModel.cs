using SaveLoad;

namespace Clicker_Game.Scripts
{
    public class ClickerModel
    {
        public int Score => _saveLoad.SaveData.clickerScore;
        
        private readonly ISaveLoad _saveLoad;

        public ClickerModel(ISaveLoad saveLoad)
        {
            _saveLoad = saveLoad;
        }

        public void IncrementScore()
        {
            _saveLoad.SaveData.clickerScore++;
        }
        
    }
}