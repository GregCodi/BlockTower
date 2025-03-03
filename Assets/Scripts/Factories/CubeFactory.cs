using UnityEngine;
using UnityEngine.UI;
using Zenject;
using BlockTower.Config;

namespace BlockTower.Factories
{
    public class CubeFactory : PlaceholderFactory<CubeConfig, Transform, Cube>
    {
        public GameObject CreateConfiguredCube(CubeConfig config, Transform parent)
        {
            Cube cube = Create(config, parent);
            GameObject cubeObject = cube.gameObject;
            
            if (cubeObject.transform.parent != parent)
            {
                cubeObject.transform.SetParent(parent, false);
            }
            
            Image cubeImage = cubeObject.GetComponent<Image>();
            if (cubeImage != null)
            {
                if (config.cubeSprite != null)
                {
                    cubeImage.sprite = config.cubeSprite;
                }
                cubeImage.color = config.cubeColor;
            }
            
            cubeObject.name = $"Cube_{config.cubeId}";
            
            return cubeObject;
        }
    }
} 