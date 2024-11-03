using TMPro;
using UniRx;
using UnityEngine;
using Zenject;
using UnityEngine.UI;
using UI;

public class MainInstaller : MonoInstaller
{
    [SerializeField] private StartView _startView;
    [SerializeField] private GameContext _gameContext;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<StartView>().FromInstance(_startView).AsSingle();
        Container.BindInterfacesAndSelfTo<GameContext>().FromInstance(_gameContext).AsSingle();

        Container.BindInterfacesAndSelfTo<GameManager>().AsSingle();
    }
}