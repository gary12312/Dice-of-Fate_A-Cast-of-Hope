using DG.Tweening;
using UnityEngine;


namespace DG_Tweening_Exemple
{
    public class DI_Target : MonoBehaviour
    {
        public float punch = 0.3f; //   удар
        public float duration = 0.3f; // продолжительность
        public int vibrato = 10;
        public float elasticiti = 1; // продолжительность

        private Vector3 currentVector;

        public void TargetHit(Vector3 playerPosition)
        {
            // Вычисляем направление ОТ игрока К цели
            Vector3 directionFromPlayer = transform.position - playerPosition;

            // Нормализуем, чтобы получить чистый вектор направления
            directionFromPlayer.Normalize();

            // Применяем пунч-анимацию в направлении от игрока
            transform.DOPunchPosition(directionFromPlayer * punch, duration, vibrato, elasticiti).Play();
        }
    }
}
