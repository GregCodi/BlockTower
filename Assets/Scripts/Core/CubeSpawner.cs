using UnityEngine;
using System.Collections.Generic;
using BlockTower.Config;
using Zenject;
using UniRx;
using BlockTower.Factories;

public class CubeSpawner : MonoBehaviour
{
    [SerializeField] private RectTransform contentContainer;
    
    [Inject] private CubeFactory _cubeFactory;
    [Inject] private IConfigProvider _configProvider;
    
    private GameConfig _currentConfig;
    private List<GameObject> _spawnedCubes = new List<GameObject>();
    private CompositeDisposable _disposables = new CompositeDisposable();

    private void Awake()
    {
        InitializeConfigProvider();
    }

    private void Start()
    {
        SpawnCubes();
    }

    private void InitializeConfigProvider()
    {
        _currentConfig = _configProvider.GetConfig();
    }

    public void SpawnCubes()
    {
        ClearSpawnedCubes();

        if (_currentConfig == null || _currentConfig.availableCubes.Count == 0)
        {
            Debug.LogError("No cube configurations available");
            return;
        }
        
        for (int i = 0; i < _currentConfig.availableCubes.Count; i++)
        {
            CubeConfig cubeConfig = _currentConfig.availableCubes[i];        
            GameObject cube = _cubeFactory.CreateConfiguredCube(cubeConfig, contentContainer);
            
            if (cube != null)
                _spawnedCubes.Add(cube);
            else
                Debug.LogError($"Failed to create cube with config {cubeConfig.cubeId}");
        }
    }

    public void ClearSpawnedCubes()
    {
        foreach (GameObject cube in _spawnedCubes)
        {
            if (cube != null)
            {
                Destroy(cube);
            }
        }
        _spawnedCubes.Clear();
    }

    public void UpdateConfig(IConfigProvider newConfigProvider)
    {
        _configProvider = newConfigProvider;
        _currentConfig = _configProvider.GetConfig();
        SpawnCubes();
    }
    
    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}