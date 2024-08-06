using Reflex.Core;
using UnityEngine;

namespace Adventure_Game.Scripts
{
    public class AdventureInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton(typeof(PlayerInputSystem), typeof(PlayerInputSystem));
        }
    }
}
