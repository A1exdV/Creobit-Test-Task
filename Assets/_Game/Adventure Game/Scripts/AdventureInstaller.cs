using Reflex.Core;
using UnityEngine;

namespace Adventure_Game.Scripts
{
    public class AdventureInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private Timer timerPrefab;
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton(typeof(PlayerInputSystem), typeof(PlayerInputSystem));
            var timer = Instantiate(timerPrefab);
            containerBuilder.AddSingleton(timer, typeof(Timer));
        }
    }
}
