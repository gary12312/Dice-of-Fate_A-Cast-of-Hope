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
        // 5 - атака
        // 6 - защита противника + контрудар
        // 7 - процесс атаки
        // 8 - контрудар противника

        // Прогрессия игрока
        public static int numberDiceToDrop = 1;
      //  public static int numberDiceToDropForTutorial = 1;

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

        // Флаги для мышки
        public static bool isPlayerMoveble; // заглушка для Фазы 4 чтобы фигурка не двигалась пока кубики не остановятся
       
        public static bool isPlayerUnitSelect; 
        public static bool isEnemyUnitSelect;
        public static bool isOtherUnitSelect;

        public static bool isBattle; // битва
        public static bool isTurentPlayer; // Ход игрока
        public static bool isTurentEnemy;  // Ход врага



        // Характкристики Юнита (для временного хранения) для UI_Mane
        public static string currentUser; // Player, Enemy, Other
        public static string nameUnit; 
        public static int moveUser;
        public static int attackUser;
        public static int shildUser;
        public static int conterAttackUser;

    }
}