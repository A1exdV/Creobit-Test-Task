using System;

namespace SaveLoad
{
    [Serializable]
    public class SaveData
    {
        public int clickerScore = 0;
        public float bestTime = 0;
        public bool isFirstPlay = true;
    }
}