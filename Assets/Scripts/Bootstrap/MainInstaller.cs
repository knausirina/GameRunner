using TMPro;
using UniRx;
using UnityEngine;
using Zenject;
using UnityEngine.UI;
using UI;
using Player;

public class MainInstaller : MonoInstaller
{
    [SerializeField] private StartView _startView;
    [SerializeField] private GameContext _gameContext;
    [SerializeField] private GameObject _playerControllerGameObject;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<StartView>().FromInstance(_startView).AsSingle();
        Container.BindInterfacesAndSelfTo<GameContext>().FromInstance(_gameContext).AsSingle();

        var playerController = (IPlayerController)_playerControllerGameObject.GetComponent(typeof(IPlayerController));
        Container.BindInterfacesAndSelfTo<IPlayerController>().FromInstance(playerController).AsSingle();

        Container.BindInterfacesAndSelfTo<GameManager>().AsSingle();
        Container.BindInterfacesTo<ScoreController>().AsSingle();
    }
}