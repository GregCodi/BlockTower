using UnityEngine;
using TMPro;
using UniRx;
using DG.Tweening;
using Zenject;
using BlockTower.Notifications;

public class NotificationUI : MonoBehaviour
{
    [SerializeField] private GameObject notificationObject;
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private CanvasGroup notificationCanvasGroup;
    [SerializeField] private float notificationDuration = 2f;
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    
    [Inject] private NotificationService _notificationService;
    
    private Sequence _currentAnimation;
    private CompositeDisposable _disposables = new CompositeDisposable();
    private bool _isAnimating = false;
    
    private void Awake()
    {
        if (notificationObject != null)
        {        
            if (notificationCanvasGroup != null)
            {
                notificationCanvasGroup.alpha = 0f;
            }
        }
        else
        {
            Debug.LogError("Notification object is not assigned in NotificationUI!");
        }
    }
    
    private void Start()
    { 
        _notificationService.OnNotification
            .Subscribe(ShowNotification)
            .AddTo(_disposables);
    }
    
    private void ShowNotification(NotificationData notification)
    {
        if (_isAnimating && _currentAnimation != null)
        {
            _currentAnimation.Kill();
        }
        
        notificationText.text = notification.Message;
         
        _isAnimating = true;
        _currentAnimation = DOTween.Sequence();
        
        _currentAnimation.Append(notificationCanvasGroup.DOFade(1f, fadeInDuration));
        _currentAnimation.AppendInterval(notificationDuration);
        _currentAnimation.Append(notificationCanvasGroup.DOFade(0f, fadeOutDuration));
        
        _currentAnimation.OnComplete(() => {
            _isAnimating = false;
        });
                    
        _currentAnimation.Play();
    }
    
    private void OnDestroy()
    {
        _disposables.Dispose();
        
        if (_currentAnimation != null)
        {
            _currentAnimation.Kill();
        }
    }
} 