using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

namespace DG_Tweening_Exemple
{
    public class DT_UI_Popup : MonoBehaviour
    {

        private Tween _tweenAnimation;
        [SerializeField] public CanvasGroup _bodyAlphaGroup;
        [SerializeField] private RectTransform _body;
        [SerializeField] private Button _button;

        private Vector2 _targetBodyPosition;
        private Vector2 _startShift;

        private Sequence _sequenceAnimation;


        private void Awake()
        {
            _targetBodyPosition = _body.anchoredPosition;
            _startShift = new Vector2(_targetBodyPosition.x, -Screen.height / 2);

            _bodyAlphaGroup.gameObject.SetActive(false);
            _bodyAlphaGroup.alpha = 0;
        }

        public void Show()
        {
            _bodyAlphaGroup.gameObject.SetActive(true);
            // _bodyAlphaGroup.DOFade(1, 0.5f); // Появление попапа за 0.5 секунды
            // Sequence _sequenceAnimation = DOTween.Sequence();

            KillCurentAnimationIfActive();

            _sequenceAnimation = DOTween.Sequence();
            _sequenceAnimation
                .Append(_bodyAlphaGroup.DOFade(1, 1f).From(0))
                .Join(_body.DOAnchorPos(_targetBodyPosition, 1f).From(_startShift))   // Join присоеденить  - выполнить совместно
                .Append(_button.transform.DOScale(1, 0.5f).From(0).SetEase(Ease.OutBounce)); // Append - добавить в конец



        }

        public void Hide( Action callback)
        {
            // _bodyAlphaGroup.DOFade(0, 0.5f); // Исчезновение попапа за 0.5 секунды
            //Sequence _sequenceAnimation = DOTween.Sequence();
            //_bodyAlphaGroup.gameObject.SetActive(false);

            KillCurentAnimationIfActive();
            _sequenceAnimation = DOTween.Sequence();
            _sequenceAnimation
                .Append(_bodyAlphaGroup.DOFade(0, 1f).From(1))
                .Join(_body.DOAnchorPos(_startShift, 1f).From(_targetBodyPosition))
                .OnComplete(() => callback?.Invoke() );  // как и _bodyAlphaGroup.gameObject.SetActive(false); но из дркгого скрипта



        }

        public void ForceComplitAnimation() => _sequenceAnimation.Complete(true); // прерывание фнимации 

        public bool InAnimation() => _sequenceAnimation != null && _sequenceAnimation.active;

        private void KillCurentAnimationIfActive()
        {

            if (InAnimation())
            {
                _sequenceAnimation.Kill();
            }
        }

        private void OnDestroy()
        {
            KillCurentAnimationIfActive();
        }
    }
}
