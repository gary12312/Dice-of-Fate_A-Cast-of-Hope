using DiceFate.EventBus;
using DiceFate.Events;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

namespace DiceFate.Dice
{
    public class KegTremble : MonoBehaviour
    {
        [SerializeField] private float powerShake = 0.005f;
        [SerializeField] public float moveSpeed = 30f; //управляется из Mane
        private Vector3 start_position;

        private DiceCube diceCubeValue;

        private Camera mainCamera;
      //  private CanvasGroup _canvasGroup;


        private void Awake()
        {
            Bus<OnMoveToMouseEvent>.OnEvent += HandelMoveToMouse;
            Bus<OnDropEvent>.OnEvent += HandelKegDrop;
        }
        private void OnDestroy()
        {
            Bus<OnMoveToMouseEvent>.OnEvent -= HandelMoveToMouse;
            Bus<OnDropEvent>.OnEvent -= HandelKegDrop;
        }

        void Start()
        {
           
            start_position = transform.position;
           
            mainCamera = Camera.main;       


        }

        private void OnEnable()
        {
          //  _сanvasGroup.DOFade(1, 0.5f).Play(); // Появление за 0.5 секунды
        }

        private void HandelKegDrop(OnDropEvent evt)
        {
            Vector3 directionFromCamera = transform.position - Camera.main.transform.position;
            directionFromCamera.y = 0;
            directionFromCamera.Normalize();

            // Основной поворот - кубик смотрит от камеры
            Quaternion targetRotation = Quaternion.LookRotation(directionFromCamera, Vector3.up);

            // Добавляем наклон 90 градусов по оси X, чтобы кубик лежал
            targetRotation *= Quaternion.Euler(90, 0, 0);

            transform.rotation = targetRotation;

            StartCoroutine(DilayBeforDeletObject());



            //transform.rotation = Quaternion.Euler(90, 0, 0);
            //StartCoroutine(DilayBeforDeletObject());

        }

        private void HandelMoveToMouse(OnMoveToMouseEvent point)
        {
            Vector3 transformMousePoint = new Vector3(point.Point.x, transform.position.y, point.Point.z); ;

            transform.position = Vector3.Lerp(transform.position,
                                              transformMousePoint,
                                              moveSpeed * Time.deltaTime);
        }

        IEnumerator DilayBeforDeletObject()
        {
            yield return new WaitForSeconds(0.1f);

            transform
                 .DOScale(0, 0.1f)                 
                 .Play()
                 .OnComplete(() => DestroyObject());



            //_tweenAnimation = _сanvasGroup
            //    .DOFade(0, 0.5f)
            //    .Play()
            //    .OnComplete(() => DestroyObject());// изчезновение  за 0.5 секунды                  
        }

        private void DestroyObject()
        {
            Destroy(this.gameObject);
        }

    }
}
