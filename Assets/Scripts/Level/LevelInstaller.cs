using UnityEngine;
using Zenject;
using Pool;

namespace Level
{
    [CreateAssetMenu(menuName = "Installers/Level Installer")]
    public class LevelInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private Pools _pools;
        [SerializeField] private Vector3 _middleLinePosition;

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<Pools>().FromInstance(_pools);
            Container.BindInterfacesAndSelfTo<LevelController>().AsSingle().WithArguments(_pools, _middleLinePosition);
        }
    }
}