using Player;
using UnityEngine;
using Zenject;
using Player;

[CreateAssetMenu(menuName = "Installers/GameManager Installer")]
public class GameManagerInstaller : ScriptableObjectInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<GameManager>()
            .AsSingle();
    }
}