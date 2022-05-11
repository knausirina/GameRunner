using UnityEngine;
using Zenject;

namespace UI
{
    [CreateAssetMenu(menuName = "Installers/UI Installer")]
    public class UIInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameViewModel>()
                .FromNew()
                .AsCached();
        }
    }
}