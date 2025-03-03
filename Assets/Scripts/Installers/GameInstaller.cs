using UnityEngine;
using Zenject;
using BlockTower.Config;
using BlockTower.Notifications;
using BlockTower.Factories;
using BlockTower.Localization;
using BlockTower.SaveSystem;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private ScriptableObjectConfigProvider configProvider;
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private Canvas mainCanvas;
    
    public override void InstallBindings()
    {
        Container.Bind<SaveService>().AsSingle();
        
        Container.Bind<ILocalizationProvider>()
            .To<DefaultLocalizationProvider>()
            .AsSingle();
        
        Container.Bind<IConfigProvider>().FromInstance(configProvider).AsSingle();
        Container.Bind<Canvas>().FromInstance(mainCanvas).AsSingle();
        Container.Bind<NotificationService>().AsSingle().NonLazy();

        Container.Bind<Tower>().FromComponentInHierarchy().AsSingle();
        Container.Bind<CubeSpawner>().FromComponentInHierarchy().AsSingle();
        Container.Bind<NotificationUI>().FromComponentInHierarchy().AsSingle();
        
        Container.BindFactory<CubeConfig, Transform, Cube, CubeFactory>()
            .FromComponentInNewPrefab(cubePrefab)
            .AsSingle();

    }
}
