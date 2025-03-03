using UnityEngine;

namespace BlockTower.Notifications
{
    public class NotificationData
    {
        public NotificationType Type { get; private set; }
        public string Message { get; private set; }
        public Color Color { get; private set; }
        public GameObject RelatedObject { get; private set; }
        public int Score { get; private set; }

        public NotificationData(NotificationType type, string message, Color color, GameObject relatedObject = null, int score = 0)
        {
            Type = type;
            Message = message;
            Color = color;
            RelatedObject = relatedObject;
            Score = score;
        }
    }
} 