using DG.Tweening;
using UnityEngine;



namespace DG_Tweening_Exemple
{
    public class DT_Player : MonoBehaviour
    {

        [SerializeField] private Transform _transform;
        [SerializeField] private DI_Target dI_Target;

        [Header("Прыжок")]
        [SerializeField] public float hight = 3f; //  высота
        [SerializeField] public float durationJump = 0.2f; // продолжительность
        [SerializeField] private int numberLoop = -1;

        private Vector3 _position;


        private Tween _tweenAnimation;

        public AnimationCurve jumpCurve = new AnimationCurve(
             new Keyframe(0f, 0f),
             new Keyframe(0.5f, 1f),
             new Keyframe(1f, 0f)); // Кривая для плавности прыжка

        private enum Ease1 // для примера
        {
            Unset,
            Linear,
            InSine,
            OutSine,
            InOutSine,
            InQuad,
            OutQuad,
            InOutQuad,
            InCubic,
            OutCubic,
            InOutCubic,
            InQuart,
            OutQuart,
            InOutQuart,
            InQuint,
            OutQuint,
            InOutQuint,
            InExpo,
            OutExpo,
            InOutExpo,
            InCirc,
            OutCirc,
            InOutCirc,
            InElastic,
            OutElastic,
            InOutElastic,
            InBack,
            OutBack,
            InOutBack,
            InBounce,
            OutBounce,
            InOutBounce,
            Flash,
            InFlash,
            OutFlash,
            InOutFlash,
        }


        public void JampAndDamage()
        {
            Debug.Log("Прыжок ");
            _tweenAnimation = _transform
                .DOMoveY(hight, durationJump)
                .SetLoops(numberLoop, LoopType.Yoyo)
                .SetEase(jumpCurve)
                .Play()
                .OnComplete(() => Damage());


        }
        public void Damage()
        {
            Debug.Log("Damage ");
            _position = _transform.position;
            dI_Target.TargetHit(_position);

        }
    }
}
