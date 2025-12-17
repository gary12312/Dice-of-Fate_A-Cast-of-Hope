using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiceFate
{
    public class GameStats
    {
        
        // Прогресс по сценам: 0 = первая сцена, 1 = вторая и т.д.
        public static int sceneProgress;
     
        public static int currentPhasePlayer = 0;
        // 0 - пусто 
        // 1 - выбор юнита
        // 2 - активация кубиков
        // 3 - получение значений кубиков
        // 4 - движение
        // 5 - подготовка к атаке 
        // 6 - защита противника + контрудар
        // 7 - процесс атаки
        // 8 - контрудар противника


        // Значения последних брошенных кубиков игрока на поле
        public static int diceMovement;
        public static int diceAttack;
        public static int diceShield;
        public static int diceCounterattack;

        // Значения последних брошенных кубиков противника на поле
        public static int diceMovementEnemy;
        public static int diceAttackEnemy;
        public static int diceShieldEnemy;
        public static int diceCounterattackEnemy;


    }
}