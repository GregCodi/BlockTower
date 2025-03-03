using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Zenject;
using BlockTower.Notifications;
using BlockTower.Config;

[RequireComponent(typeof(CanvasGroup))]
public class Cube : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool IsPlaced { get; set; }
    public string CubeId => _cubeId;
    [SerializeField] private float disappearDuration = 0.3f;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 startPosition;
    private GameObject draggedCopy;
    private bool dropSuccess = false;
    private bool heightLimitReached = false;  
    private string _cubeId;

    [Inject] private NotificationService _notificationService;
    [Inject] private Canvas _canvas; 

    [Inject]
    public void Initialize(CubeConfig config, Transform parent)
    {
        _cubeId = config.cubeId;
        transform.position = parent.position;
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    

    private void AnimateDisappear(GameObject target)
    {
        if (target == null) return;
        
        Sequence disappearSequence = DOTween.Sequence();

        RectTransform targetRect = target.GetComponent<RectTransform>();
        CanvasGroup targetCanvasGroup = target.GetComponent<CanvasGroup>();
        
        if (targetRect == null || targetCanvasGroup == null) return;

        disappearSequence.Join(
            targetRect.DOScale(Vector3.zero, disappearDuration)
            .SetEase(Ease.InBack)
        );

        disappearSequence.Join(
            targetCanvasGroup.DOFade(0f, disappearDuration)
            .SetEase(Ease.InQuad)
        );

        disappearSequence.Join(
            targetRect.DORotate(new Vector3(0, 0, 360), disappearDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.InBack)
        );

        disappearSequence.OnComplete(() => {
            Destroy(target);
        });

        disappearSequence.Play();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {     
        startPosition = rectTransform.anchoredPosition;
         
        if (!IsPlaced)
        {
            draggedCopy = Instantiate(gameObject, transform.parent.parent);
            draggedCopy.transform.position = transform.position;
            
            var copyCube = draggedCopy.GetComponent<Cube>();
            if (copyCube != null)
            {
                copyCube.SetCanvas(_canvas);
                copyCube.SetCubeId(_cubeId);
            }
            
            CanvasGroup copyCanvasGroup = draggedCopy.GetComponent<CanvasGroup>();
            if (copyCanvasGroup != null)
            {
                copyCanvasGroup.alpha = 0.6f;
                copyCanvasGroup.blocksRaycasts = false;
            }
        }
        else
        {
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
        }         
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsPlaced)
        {   
            if (draggedCopy != null)
            {
                RectTransform copyRectTransform = draggedCopy.GetComponent<RectTransform>();
                if (copyRectTransform != null)
                {
                    copyRectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
                }
            }
        }
        else
        {
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsPlaced)
        {
            if (!dropSuccess)
            {
                if (draggedCopy != null)
                {
                    AnimateDisappear(draggedCopy);
                    draggedCopy = null;
                }
                
                if (!heightLimitReached && _notificationService != null)
                {
                    _notificationService.NotifyCubeDisappeared();
                }
            }
        }
        else
        {
            if (!dropSuccess)
            {
                rectTransform.anchoredPosition = startPosition;
            }

            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
        
        dropSuccess = false;
        heightLimitReached = false;
    }

    public void OnHeightLimitReached()
    {
        heightLimitReached = true;
    }

    public GameObject GetDraggedCopy()
    {
        return draggedCopy;
    }

    public void SetCanvas(Canvas canvas)
    {
        _canvas = canvas;
    }

    public void OnDropSuccess()
    {
        dropSuccess = true;
    }

    public void SetCubeId(string cubeId)
    {
        _cubeId = cubeId;
    }
}