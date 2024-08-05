using Reflex.Core;
using SaveLoad;
using UnityEngine;
using SceneLoader;

namespace Reflex
{
    public class ProjectInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton(typeof(DesktopSaveLoad), typeof(ISaveLoad));
            containerBuilder.AddSingleton(typeof(SceneLoaderService), typeof(SceneLoaderService));
            Debug.Log("Project bindings - done!");
        }
    }
}
