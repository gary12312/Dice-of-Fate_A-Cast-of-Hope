using DiceFate.EventBus;
using DiceFate.Events;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace DiceFate.Dice
{
    public class KegTremble : MonoBehaviour
    {
        [SerializeField] private float powerShake = 0.005f;
        [SerializeField] public float moveSpeed = 30f; //управляется из Mane
        private Vector3 start_position;

        private DiceCube diceCubeValue;

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

        }

        private void HandelKegDrop(OnDropEvent evt)
        {
            transform.rotation = Quaternion.Euler(90, 0, 0);
            StartCoroutine(DilayBeforDeletObject());
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
            yield return new WaitForSeconds(1f);
            Destroy(this.gameObject);
        }
    }
}
