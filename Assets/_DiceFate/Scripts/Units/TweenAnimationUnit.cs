using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;


namespace DiceFate.Units
{
    public class TweenAnimationUnit : MonoBehaviour
    {
        [Header("Прыжок")]
        [SerializeField] public float hight = 3f; //  высота
        [SerializeField] public float durationJump = 0.2f; // продолжительность     
        [SerializeField] private int numberLoop = 1; // -1 бесконечность

        [Header("При получении урона")]
        [SerializeField] public float punch = 0.4f; //   удар
        [SerializeField] public float durationPunch = 0.2f; // продолжительность

        private Transform _transform;
        private Tween _tweenAnimation;

        private Vector3 _position;


        public AnimationCurve jumpCurve = new AnimationCurve(
              new Keyframe(0f, 0f),
              new Keyframe(0.5f, 1f),
              new Keyframe(1f, 0f)); // Кривая для плавности прыжка

        public void JampAnimation()
        {
            //_transform = transform;
            Debug.Log("Прыжок ");
            _tweenAnimation = transform
                .DOMoveY(hight, durationJump)          //      .SetLoops(numberLoop, LoopType.Yoyo)
                .SetEase(jumpCurve)
                .Play()
                .OnComplete(() => Damage());

            //DOTween.Sequence()
            //    .Append(transform.DOMoveY(hight, durationJump).SetEase(jumpCurve))
            //    .Append(transform.DOMoveY(-hight, durationDown))
            //    .Play()
            //    .OnComplete(() => Damage());







        }
        public void Damage()
        {
            Debug.Log("Damage ");
            //_position = _transform.position;
            //dI_Target.TargetHit(_position);

        }

        // Толчок при получении урона
        public void TargetHitAnimation(Vector3 uninHitPosition)
        {
            Vector3 directionFromPlayer = transform.position - uninHitPosition;        // Вычисляем направление ОТ юнита К цели           
            directionFromPlayer.Normalize();                                          // Нормализуем, чтобы получить чистый вектор направления           
            transform.DOPunchPosition(directionFromPlayer * punch, durationPunch).Play();  // Применяем пунч-анимацию в направлении от игрока
        }



    }
}
