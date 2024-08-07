using UnityEngine;
using UnityEngine.AddressableAssets;


namespace Scripts
{
    [CreateAssetMenu(fileName = "Game Config", menuName = "GameConfig", order = 0)]
    public class GameConfig : ScriptableObject
    {
        [SerializeField] private string gameName;
        [SerializeField] private AssetReference gameScene;

        public string GameName => gameName;
        public AssetReference GameScene => gameScene;
    }
}