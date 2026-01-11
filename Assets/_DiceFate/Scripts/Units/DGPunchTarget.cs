using UnityEngine;
using DG.Tweening;

namespace DiceFate.Units
{
    public class DGPunchTarget : MonoBehaviour
    {
        public float punch = 0.4f; //   удар
        public float duration = 0.2f; // продолжительность

        public void TargetHit(Vector3 playerPosition)
        {           
            Vector3 directionFromPlayer = transform.position - playerPosition;        // Вычисляем направление ОТ игрока К цели           
            directionFromPlayer.Normalize();                                          // Нормализуем, чтобы получить чистый вектор направления           
            transform.DOPunchPosition(directionFromPlayer * punch, duration).Play();  // Применяем пунч-анимацию в направлении от игрока
        }

    }
}
