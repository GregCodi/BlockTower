using UnityEngine;
using System.Collections.Generic;
using BlockTower.Notifications;

namespace BlockTower.Core.PlacementRules
{
    public class PlacementValidator
    {
        private readonly List<IPlacementRule> _rules = new List<IPlacementRule>();
        private readonly NotificationService _notificationService;

        public PlacementValidator(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void AddRule(IPlacementRule rule)
        {
            _rules.Add(rule);
        }

        public void ClearRules()
        {
            _rules.Clear();
        }

        public bool ValidatePlacement(GameObject droppedCube, GameObject targetCube)
        {
            foreach (var rule in _rules)
            {
                if (!rule.CanPlace(droppedCube, targetCube))
                {
                    if (_notificationService != null)
                    {
                        _notificationService.NotifyCubeDisappeared();
                    }
                    return false;
                }
            }
            return true;
        }
    }
} 