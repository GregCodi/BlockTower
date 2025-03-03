using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlockTower.SaveSystem
{
    [Serializable]
    public class CubeData
    {
        public Vector2 position;
        public string configId;

        public CubeData(Vector2 position, string configId)
        {
            this.position = position;
            this.configId = configId;
        }
    }

    [Serializable]
    public class TowerSaveData
    {
        public List<CubeData> placedCubes = new List<CubeData>();
    }
} 