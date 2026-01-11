using UnityEngine;
using DG.Tweening;

namespace DG_Tweening_Exemple
{

    public class DT_BaseAnimationExemple : MonoBehaviour
    {

        private Tween _tweenAnimation;

        [SerializeField] private Transform _transform;


        void Update()
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Move();

               // Rotate();

            }


            if (Input.GetKeyDown(KeyCode.Q))
            {
                _tweenAnimation.Kill(true); // Остановка анимации или 
                                        // _tweenAnimation.Complete(); // Завершение анимации

                // и только потом уничтожение объекта
                Destroy(_transform.gameObject);
            }
        }

        private void Move()
        {
            _tweenAnimation = _transform.
                DOMoveX(5, 2).
                SetLoops(-1, LoopType.Yoyo);    // 4 количество циклов анимации если -1  то бесконечное количество повторов, Yoyo - движение туда и обратно
                                                // LoopType характеризует тип повторения анимации

        }

        private void Rotate()
        {
            _tweenAnimation = _transform.
                DORotate(new Vector3(0, 360, 0), 1.5f, RotateMode.FastBeyond360); //.SetLoops(-1, LoopType.Yoyo);
            // RotateMode.FastBeyond360 - вращение с возможностью превышения 360 градусов
        }



    }
}
