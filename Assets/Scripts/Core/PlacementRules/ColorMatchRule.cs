using UnityEngine;
using UnityEngine.UI;

namespace BlockTower.Core.PlacementRules
{
    public class ColorMatchRule : IPlacementRule
    {
        public bool CanPlace(GameObject droppedCube, GameObject targetCube)
        {
            if (droppedCube == null || targetCube == null) return true;

            var droppedImage = droppedCube.GetComponent<Image>();
            var targetImage = targetCube.GetComponent<Image>();

            if (droppedImage == null || targetImage == null) return true;
            if (droppedImage.sprite == null || targetImage.sprite == null) return true;

            return droppedImage.sprite.name == targetImage.sprite.name;
        }

        public string GetFailureMessage()
        {
            return "Кубик можно поставить только на кубик такого же типа!";
        }
    }
} 