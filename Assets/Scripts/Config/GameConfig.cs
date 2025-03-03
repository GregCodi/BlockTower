using System.Collections.Generic;

namespace BlockTower.Config
{
    [System.Serializable]
    public class GameConfig
    {
        public int initialCubeCount = 5;
        public List<CubeConfig> availableCubes = new List<CubeConfig>();
    }

} 