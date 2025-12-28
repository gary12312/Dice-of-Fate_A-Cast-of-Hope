using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiceFate
{
    public class GameStats
    {
        
        // ѕрогресс по сценам: 0 = перва€ сцена, 1 = втора€ и т.д.
        public static int sceneProgress;
     
        public static int currentPhasePlayer = 0;
        // 0 - пусто 
        // 1 - выбор юнита
        // 2 - активаци€ кубиков
        // 3 - получение значений кубиков
        // 4 - движение
        // 5 - атака
        // 6 - защита противника + контрудар
        // 7 - процесс атаки
        // 8 - контрудар противника


        // «начени€ последних брошенных кубиков игрока на поле
        public static int diceMovement;
        public static int diceAttack;
        public static int diceShield;
        public static int diceCounterattack;

        // «начени€ последних брошенных кубиков противника на поле
        public static int diceMovementEnemy;
        public static int diceAttackEnemy;
        public static int diceShieldEnemy;
        public static int diceCounterattackEnemy;

        // ‘лаги дл€ мышки
        public static bool isPlayerMoveble; // заглушка дл€ ‘азы 4 чтобы фигурка не двигалась пока кубики не останов€тс€

    }
}