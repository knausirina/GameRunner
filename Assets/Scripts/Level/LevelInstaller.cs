using UnityEngine;
using Zenject;

namespace Level
{
    [CreateAssetMenu(menuName = "Installers/Level Installer")]
    public class LevelInstaller : ScriptableObjectInstaller
    {
        [SerializeField]
        private LevelPools _levelPools;
        [SerializeField]
        private Vector3 _middleLinePosition;

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<LevelPools>().FromInstance(_levelPools);
            Container.BindInterfacesAndSelfTo<LevelController>().AsSingle().WithArguments(_levelPools, _middleLinePosition);
        }
    }
}