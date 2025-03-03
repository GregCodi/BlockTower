using UnityEngine;
using System.IO;
using Zenject;
using BlockTower.Config;
using BlockTower.Factories;

namespace BlockTower.SaveSystem
{
    public class SaveService
    {
        private const string SAVE_FILE_NAME = "tower_save.json";
        private string SavePath => Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

        [Inject] private IConfigProvider _configProvider;
        [Inject] private CubeFactory _cubeFactory;

        public void SaveTower(Tower tower)
        {
            var saveData = new TowerSaveData();
            var placedCubes = tower.GetPlacedCubes();
            
            foreach (var cube in placedCubes)
            {
                if (cube != null)
                {
                    RectTransform rectTransform = cube.GetComponent<RectTransform>();
                    var cubeComponent = cube.GetComponent<Cube>();
                    
                    if (rectTransform != null && cubeComponent != null)
                    {
                        var cubeData = new CubeData(
                            rectTransform.localPosition,
                            cubeComponent.CubeId
                        );
                        saveData.placedCubes.Add(cubeData);
                    }
                }
            }

            string json = JsonUtility.ToJson(saveData);
            File.WriteAllText(SavePath, json);
        }

        public void LoadTower(Tower tower)
        {
            if (!File.Exists(SavePath))
            {
                return;
            }

            string json = File.ReadAllText(SavePath);
            var saveData = JsonUtility.FromJson<TowerSaveData>(json);

            tower.ClearTower();

            var gameConfig = _configProvider.GetConfig();

            foreach (var cubeData in saveData.placedCubes)
            {
                CubeConfig matchingConfig = null;
                foreach (var config in gameConfig.availableCubes)
                {
                    if (config.cubeId == cubeData.configId)
                    {
                        matchingConfig = config;
                        break;
                    }
                }

                if (matchingConfig != null)
                {
                    var cube = _cubeFactory.CreateConfiguredCube(matchingConfig, tower.transform);
                    if (cube != null)
                    {
                        var rectTransform = cube.GetComponent<RectTransform>();
                        if (rectTransform != null)
                        {
                            rectTransform.localPosition = cubeData.position;
                            
                            var cubeComponent = cube.GetComponent<Cube>();
                            if (cubeComponent != null)
                            {
                                cubeComponent.IsPlaced = true;
                                tower.AddLoadedCube(cube.gameObject);
                            }
                        }
                    }
                }
            }
        }

        public bool HasSaveData()
        {
            return File.Exists(SavePath);
        }
    }
} 