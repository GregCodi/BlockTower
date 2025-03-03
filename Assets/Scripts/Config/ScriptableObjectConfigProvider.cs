using UnityEngine;

namespace BlockTower.Config
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "BlockTower/Game Config")]
    public class ScriptableObjectConfigProvider : ScriptableObject, IConfigProvider
    {
        public GameConfig config;

        public GameConfig GetConfig()
        {
            return config;
        }
    }
} 