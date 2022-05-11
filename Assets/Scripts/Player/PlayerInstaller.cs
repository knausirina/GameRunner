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
            var playerData = new PlayerData();
            Container.BindInterfacesTo<ScoreController>()
                .AsSingle()
               .WithArguments(playerData);
        }
    }
}