using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using BlockTower.Notifications;

public class Trash : MonoBehaviour, IDropHandler
{
    private Image _trashImage;
    private RectTransform _rectTransform;
    private Canvas _canvas;
    [Inject] private NotificationService _notificationService;

    private void Awake()
    {
        _trashImage = GetComponent<Image>();
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
    }

    private bool IsInsideEllipse(Vector2 screenPoint, Camera camera)
    {
        float width = _rectTransform.rect.width;
        float height = _rectTransform.rect.height;

        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, screenPoint, camera, out localPoint))
        {
            return false;
        }

        float normalizedX = (localPoint.x) / (width * 0.5f);
        float normalizedY = (localPoint.y) / (height * 0.5f);

        float distance = (normalizedX * normalizedX + normalizedY * normalizedY);

        return distance <= 1f;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Camera camera = _canvas != null && _canvas.renderMode == RenderMode.ScreenSpaceCamera ? 
                       _canvas.worldCamera : null;

        if (!IsInsideEllipse(eventData.position, camera))
        {
            return;
        }

        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject != null)
        {
            Cube cube = droppedObject.GetComponent<Cube>();
            if (cube != null && cube.IsPlaced)
            {
                Tower tower = cube.GetComponentInParent<Tower>();
                if (tower != null)
                {
                    tower.RemoveCube(droppedObject);
                }
                
                _notificationService.NotifyCubeRemoved(droppedObject, 5);
                
                Destroy(droppedObject);
            }
        }
    }
} 