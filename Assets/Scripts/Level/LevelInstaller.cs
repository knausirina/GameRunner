using UnityEngine;
using Zenject;

namespace Level
{
    [CreateAssetMenu(menuName = "Installers/Level Installer")]
    public class LevelInstaller : ScriptableObjectInstaller
    {
        public GameObject SegmentPrefab;
        public GameObject CointPrefab;
        public GameObject ObstacleSimplePrefab;
        public GameObject ObstacleComplexPrefab;
        public Vector3 Position;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<LevelController>().AsSingle().WithArguments(SegmentPrefab, CointPrefab, ObstacleSimplePrefab, ObstacleComplexPrefab, Position);
        }
    }
}