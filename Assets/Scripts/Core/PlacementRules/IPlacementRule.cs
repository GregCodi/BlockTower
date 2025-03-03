using UnityEngine;

namespace BlockTower.Core.PlacementRules
{
    public interface IPlacementRule
    {
        bool CanPlace(GameObject droppedCube, GameObject targetCube);
        string GetFailureMessage();
    }
} 