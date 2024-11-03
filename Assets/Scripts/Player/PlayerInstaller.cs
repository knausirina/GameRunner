using UnityEngine;
using Zenject;
using Level;

namespace Player
{
    [CreateAssetMenu(menuName = "Installers/Player Installer")]
    public class PlayerInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ScoreController>().AsSingle();
        }
    }
}