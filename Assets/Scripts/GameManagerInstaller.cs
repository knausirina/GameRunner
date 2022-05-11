using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "Installers/GameManager Installer")]
public class GameManagerInstaller : ScriptableObjectInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<GameManager>()
            .AsSingle();
    }
}