using UnityEngine;
using DiceFate.Events;
using DiceFate.EventBus;

namespace DiceFate.Dice
{
    public class KegTremble : MonoBehaviour
    {
        [SerializeField] private float powerShake = 0.005f;
        private Vector3 start_position;
  
        private void Awake() 
        {
            Bus<OnShakeEvent>.OnEvent += HandelKegShake;
            Bus<OnDropEvent>.OnEvent += HandelKegDrop;
        }
        private void OnDestroy()
        {
            Bus<OnShakeEvent>.OnEvent -= HandelKegShake;
            Bus<OnDropEvent>.OnEvent -= HandelKegDrop;
        }
        
        void Start()
        {
            start_position = transform.position;
        }


        private void HandelKegShake(OnShakeEvent point) => KegShakeAndMoveForMouse(point); 
        private void HandelKegDrop(OnDropEvent evt) => KegDrop();


        private void KegDrop()
        {
            transform.rotation = Quaternion.Euler(90, 0, 0);
        }

        void KegShakeAndMoveForMouse(OnShakeEvent point)
        {
            float x = Random.Range(-powerShake, powerShake);
            float y = Random.Range(-powerShake, powerShake);
            float z = Random.Range(-powerShake, powerShake);
            transform.position += new Vector3(x, y, z) * Time.deltaTime * 500;

            Debug.Log($"Координаты  {point.Point}");

        }

    }
}
