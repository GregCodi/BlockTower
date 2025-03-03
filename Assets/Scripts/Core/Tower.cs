using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using DG.Tweening;
using Zenject;
using BlockTower.Notifications;
using BlockTower.SaveSystem;
using BlockTower.Core.PlacementRules;

public class Tower : MonoBehaviour, IDropHandler
{
    [SerializeField] private float fallDuration = 0.5f;
    [SerializeField] private Ease fallEase = Ease.OutBounce;
    [SerializeField] private float jumpHeight = 50f;
    [SerializeField] private float jumpDuration = 0.5f;

    private List<GameObject> placedCubes = new List<GameObject>();
    private Canvas canvas;
    private PlacementValidator _placementValidator;

    [Inject] private NotificationService _notificationService;
    [Inject] private SaveService _saveService;
    
    private void Awake()
    {
        canvas = transform.parent.GetComponentInParent<Canvas>();
        _placementValidator = new PlacementValidator(_notificationService);
        
        // Здесь можно установить правила размещения кубиков
        // Например, можно добавить правило для совпадения цветов
        //_placementValidator.AddRule(new ColorMatchRule());
    }

    private void Start()
    {
        if (_saveService.HasSaveData())
        {
            _saveService.LoadTower(this);
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            _saveService.SaveTower(this);
        }
    }

    private void OnApplicationQuit()
    {
        _saveService.SaveTower(this);
    }

    private bool IsTopCubeVisible()
    {
        if (placedCubes.Count == 0) return true;

        GameObject topCube = null;
        float maxY = float.MinValue;
        
        foreach (GameObject cube in placedCubes)
        {
            Vector2 localPos = cube.GetComponent<RectTransform>().localPosition;
            if (localPos.y > maxY)
            {
                maxY = localPos.y;
                topCube = cube;
            }
        }

        if (topCube != null)
        {
            RectTransform topCubeRect = topCube.GetComponent<RectTransform>();
            Vector3[] corners = new Vector3[4];
            topCubeRect.GetWorldCorners(corners);
            
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[1]);
            bool isVisible = screenPos.y < Screen.height;
            
            if (!isVisible)
            {
                _notificationService.NotifyTowerHeightLimit();
            }
            
            return isVisible;
        }

        return true;
    }

    private bool IsPositionValid(PointerEventData eventData, out Vector2 validPosition)
    {
        validPosition = Vector2.zero;
        
        if (!IsTopCubeVisible())
        {
            _notificationService.NotifyTowerHeightLimit();
            var draggedCube = eventData.pointerDrag.GetComponent<Cube>();

            if (draggedCube != null)
            {
                draggedCube.OnHeightLimitReached();
            }
            return false;
        }

        RectTransform towerRect = transform as RectTransform;
        
        if (placedCubes.Count == 0)
        {
            Vector2 screenPoint = eventData.position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(towerRect, screenPoint, eventData.pressEventCamera, out validPosition);
            return true;
        }

        Vector2 dropLocalPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(towerRect, eventData.position, eventData.pressEventCamera, out dropLocalPoint);

        GameObject topCube = null;
        float maxY = float.MinValue;
        GameObject touchedCube = null;
        
        foreach (GameObject cube in placedCubes)
        {
            RectTransform cubeRect = cube.GetComponent<RectTransform>();
            Vector2 cubePos = cubeRect.localPosition;
            float cubeHeight = cubeRect.rect.height;
            float cubeWidth = cubeRect.rect.width;

            bool isTouching = Mathf.Abs(dropLocalPoint.x - cubePos.x) < cubeWidth &&
                             Mathf.Abs(dropLocalPoint.y - cubePos.y) < cubeHeight;

            if (isTouching)
            {
                touchedCube = cube;
            }

            if (cubePos.y > maxY)
            {
                maxY = cubePos.y;
                topCube = cube;
            }
        }

        if (touchedCube != null && topCube != null)
        {
            // Проверяем правила размещения
            if (!_placementValidator.ValidatePlacement(eventData.pointerDrag, touchedCube))
            {
                return false;
            }

            RectTransform topCubeRect = topCube.GetComponent<RectTransform>();
            float cubeHeight = topCubeRect.rect.height;
            float cubeWidth = topCubeRect.rect.width;
            
            float maxOffset = cubeWidth * 0.5f;
            float randomOffset = Random.Range(-maxOffset, maxOffset);
            
            validPosition = topCubeRect.localPosition;
            validPosition.y += cubeHeight;
            validPosition.x += randomOffset;

            return true;
        }

        return false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Cube draggedCube = eventData.pointerDrag.GetComponent<Cube>();
        
        if (draggedCube != null && !draggedCube.IsPlaced)
        {
            Vector2 validPosition;
            if (!IsPositionValid(eventData, out validPosition))
            {
                Debug.Log("Invalid position - canceling drop");
                return;
            }

            GameObject cubeCopy = draggedCube.GetDraggedCopy();
            
            if (cubeCopy != null)
            {
                cubeCopy.transform.SetParent(transform);
                RectTransform copyRect = cubeCopy.GetComponent<RectTransform>();
                CanvasGroup canvasGroup = cubeCopy.GetComponent<CanvasGroup>();
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;

                if (placedCubes.Count == 0)
                {
                    copyRect.localPosition = validPosition;
                    
                    _notificationService.NotifyCubePlaced(cubeCopy, 20);
                    _saveService.SaveTower(this);
                }
                else
                {
                    Vector2 startPos = validPosition;
                    startPos.y -= jumpHeight;
                    copyRect.localPosition = startPos;
                    
                    Sequence jumpSequence = DOTween.Sequence();
                    
                    jumpSequence.Append(
                        copyRect.DOLocalMoveY(validPosition.y + jumpHeight * 0.3f, jumpDuration * 0.5f)
                        .SetEase(Ease.OutQuad)
                    );
                    
                    jumpSequence.Append(
                        copyRect.DOLocalMoveY(validPosition.y, jumpDuration * 0.5f)
                        .SetEase(Ease.OutBounce)
                    );
                    
                    copyRect.DOLocalMoveX(validPosition.x, jumpDuration)
                        .SetEase(Ease.OutQuad);
                    
                    jumpSequence.OnComplete(() => {
                        _notificationService.NotifyCubePlaced(cubeCopy);
                        _saveService.SaveTower(this);
                    });
                }
                
                placedCubes.Add(cubeCopy);
                draggedCube.OnDropSuccess();
                
                Cube cubeCopyComponent = cubeCopy.GetComponent<Cube>();
                cubeCopyComponent.IsPlaced = true;
            }
        }
    }

    private void ShiftCubesDown(int removedIndex)
    {
        float cubeHeight = 0f;
        
        if (placedCubes.Count > 0)
        {
            cubeHeight = placedCubes[0].GetComponent<RectTransform>().rect.height;
        }

        Sequence fallSequence = DOTween.Sequence();

        for (int i = removedIndex; i < placedCubes.Count; i++)
        {
            RectTransform cubeRect = placedCubes[i].GetComponent<RectTransform>();
            Vector2 startPos = cubeRect.localPosition;
            Vector2 targetPos = new Vector2(startPos.x, startPos.y - cubeHeight);

            fallSequence.Join(
                cubeRect.DOLocalMove(targetPos, fallDuration)
                    .SetEase(fallEase)
            );
        }

        fallSequence.Play();
    }

    public void RemoveCube(GameObject cube)
    {
        int removedIndex = placedCubes.IndexOf(cube);
        
        if (removedIndex != -1)
        {
            placedCubes.Remove(cube);
            ShiftCubesDown(removedIndex);        
            _notificationService.NotifyCubeRemoved(cube);
            _saveService.SaveTower(this);
        }
    }

    public List<GameObject> GetPlacedCubes()
    {
        return placedCubes;
    }

    public void AddLoadedCube(GameObject cube)
    {
        if (!placedCubes.Contains(cube))
        {
            placedCubes.Add(cube);
        }
    }

    public void ClearTower()
    {
        foreach (var cube in placedCubes.ToArray())
        {
            if (cube != null)
            {
                Destroy(cube);
            }
        }
        placedCubes.Clear();
    }

    public void AddPlacementRule(IPlacementRule rule)
    {
        _placementValidator.AddRule(rule);
    }

    public void ClearPlacementRules()
    {
        _placementValidator.ClearRules();
    }
}