using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Zenject;
using BlockTower.Notifications;
using BlockTower.Config;

[RequireComponent(typeof(CanvasGroup))]
public class Cube : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private float disappearDuration = 0.3f;

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Vector2 _startPosition;
    private GameObject _draggedCopy;
    private bool _dropSuccess;
    private bool _heightLimitReached;
    private string _cubeId;

    [Inject] private NotificationService _notificationService;
    [Inject] private Canvas _canvas;

    public bool IsPlaced { get; set; }
    public string CubeId => _cubeId; 

    [Inject]
    public void Initialize(CubeConfig config, Transform parent)
    {
        _cubeId = config.cubeId;
        transform.position = parent.position;
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
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
        _startPosition = _rectTransform.anchoredPosition;
         
        if (!IsPlaced)
        {
            _draggedCopy = Instantiate(gameObject, transform.parent.parent);
            _draggedCopy.transform.position = transform.position;
            
            var copyCube = _draggedCopy.GetComponent<Cube>();
            if (copyCube != null)
            {
                copyCube.SetCanvas(_canvas);
                copyCube.SetCubeId(_cubeId);
            }
            
            CanvasGroup copyCanvasGroup = _draggedCopy.GetComponent<CanvasGroup>();
            if (copyCanvasGroup != null)
            {
                copyCanvasGroup.alpha = 0.6f;
                copyCanvasGroup.blocksRaycasts = false;
            }
        }
        else
        {
            _canvasGroup.alpha = 0.6f;
            _canvasGroup.blocksRaycasts = false;
        }         
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsPlaced)
        {   
            if (_draggedCopy != null)
            {
                RectTransform copyRectTransform = _draggedCopy.GetComponent<RectTransform>();
                if (copyRectTransform != null)
                {
                    copyRectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
                }
            }
        }
        else
        {
            if (_rectTransform != null)
            {
                _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsPlaced)
        {
            if (!_dropSuccess)
            {
                if (_draggedCopy != null)
                {
                    AnimateDisappear(_draggedCopy);
                    _draggedCopy = null;
                }
                
                if (!_heightLimitReached && _notificationService != null)
                {
                    _notificationService.NotifyCubeDisappeared();
                }
            }
        }
        else
        {
            if (!_dropSuccess)
            {
                _rectTransform.anchoredPosition = _startPosition;
            }

            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
        }
        
        _dropSuccess = false;
        _heightLimitReached = false;
    }

    public void OnHeightLimitReached()
    {
        _heightLimitReached = true;
    }

    public GameObject GetDraggedCopy()
    {
        return _draggedCopy;
    }

    public void SetCanvas(Canvas canvas)
    {
        _canvas = canvas;
    }

    public void OnDropSuccess()
    {
        _dropSuccess = true;
    }

    public void SetCubeId(string cubeId)
    {
        _cubeId = cubeId;
    }
}