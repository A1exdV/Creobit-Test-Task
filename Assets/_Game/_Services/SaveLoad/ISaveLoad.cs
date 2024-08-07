namespace SaveLoad
{
    public interface ISaveLoad
    {
        public SaveData SaveData { get; set; }

        public void Load();

        public void Save();
    }
}