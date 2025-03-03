using System;
using UniRx;
using UnityEngine;
using Zenject;
using BlockTower.Localization;

namespace BlockTower.Notifications
{
    public class NotificationService
    {
        public IObservable<NotificationData> OnNotification => _notificationSubject.AsObservable();
        public IObservable<NotificationData> OnCubePlaced => _cubePlacedSubject.AsObservable();
        public IObservable<NotificationData> OnCubeRemoved => _cubeRemovedSubject.AsObservable();
        public IObservable<NotificationData> OnCubeDisappeared => _cubeDisappearedSubject.AsObservable();
        public IObservable<NotificationData> OnTowerHeightLimit => _towerHeightLimitSubject.AsObservable();

        private readonly Subject<NotificationData> _notificationSubject = new Subject<NotificationData>();     
        private readonly Subject<NotificationData> _cubePlacedSubject = new Subject<NotificationData>();
        private readonly Subject<NotificationData> _cubeRemovedSubject = new Subject<NotificationData>();
        private readonly Subject<NotificationData> _cubeDisappearedSubject = new Subject<NotificationData>();
        private readonly Subject<NotificationData> _towerHeightLimitSubject = new Subject<NotificationData>();

        [Inject] private ILocalizationProvider _localization;

        private void SendNotification(NotificationData notification)
        {
            _notificationSubject.OnNext(notification);
            
            switch (notification.Type)
            {
                case NotificationType.CubePlaced:
                    _cubePlacedSubject.OnNext(notification);
                    break;
                case NotificationType.CubeRemoved:
                    _cubeRemovedSubject.OnNext(notification);
                    break;
                case NotificationType.CubeDisappeared:
                    _cubeDisappearedSubject.OnNext(notification);
                    break;
                case NotificationType.TowerHeightLimit:
                    _towerHeightLimitSubject.OnNext(notification);
                    break;
            }
        }

        public void NotifyCubePlaced(GameObject cube, int score = 10)
        {
            string message = _localization.GetString(LocalizationKeys.Notifications.CUBE_PLACED, score);
            SendNotification(new NotificationData(
                NotificationType.CubePlaced,
                message,
                Color.green,
                cube,
                score
            ));
        }

        public void NotifyCubeRemoved(GameObject cube, int score = 5)
        {
            string message = _localization.GetString(LocalizationKeys.Notifications.CUBE_REMOVED, score);
            SendNotification(new NotificationData(
                NotificationType.CubeRemoved,
                message,
                Color.yellow,
                cube,
                score
            ));
        }

        public void NotifyCubeDisappeared(int score = -5)
        {
            string message = _localization.GetString(LocalizationKeys.Notifications.CUBE_DISAPPEARED);
            SendNotification(new NotificationData(
                NotificationType.CubeDisappeared,
                message,
                Color.red,
                null,
                score
            ));
        }

        public void NotifyTowerHeightLimit()
        {
            string message = _localization.GetString(LocalizationKeys.Notifications.TOWER_HEIGHT_LIMIT);
            SendNotification(new NotificationData(
                NotificationType.TowerHeightLimit,
                message,
                Color.blue,
                null,
                0
            ));
        }
    }
} 