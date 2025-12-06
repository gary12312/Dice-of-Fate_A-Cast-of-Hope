using UnityEngine;

namespace DiceFate.Maine
{
    public static class PhaseNumber 
    {
        public static int currentPhase = 0;

        // 0 - пусто 
        // 1 - выбор юнита
        // 2 - активация кубиков
        // 3 - получение значений кубиков
        // 4 - движение
        // 5 - подготовка к атаке 
        // 6 - защита противника + контрудар
        // 7 - процесс атаки
        // 8 - контрудар противника
    }
}