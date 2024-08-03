using Reflex.Core;
using SaveLoad;
using UnityEngine;

namespace Reflex
{
    public class ProjectInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton(typeof(DesktopSaveLoad), typeof(ISaveLoad));
            Debug.Log("Project bindings - done!");
        }
    }
}
