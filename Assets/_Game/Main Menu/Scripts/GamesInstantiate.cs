using Scripts;
using UnityEngine;

namespace Main_Menu.Scripts
{
    public class GamesInstantiate : MonoBehaviour
    {
        [SerializeField] private GameMode prefab;
        [SerializeField] private GameConfig[] gameConfigs;


        private void Awake()
        {
            foreach (var t in gameConfigs)
            {
                var game = Instantiate(prefab, transform);
                game.SetConfig(t);
            }
        }
    }
}
