using DiceFate.Units;
using System.Collections.Generic;
using UnityEngine;

namespace DiceFate.UI
{
    public class UI_Mane_Battle : MonoBehaviour
    {
        [SerializeField] private GameObject panelPlayers;
        [SerializeField] private GameObject panelEnemy;
        [SerializeField] private float scaleAvatars = 0.5f;

        private List<GameObject> unitsPlayer = new List<GameObject>();
        private List<GameObject> unitsEnemy = new List<GameObject>();

        // Список созданных аватаров для отслеживания
        private List<GameObject> playerAvatars = new List<GameObject>();
        private List<GameObject> enemyAvatars = new List<GameObject>();

        private bool isBattle = false;

        private void Start()
        {
            FindAndSetupUnits();
        }

        public void StartBattleUI()
        {
            isBattle = true;
            // При начале битвы можно обновить UI
            RefreshUnitsUI();
        }

        private void FindAndSetupUnits()
        {
            // Находим все объекты с тегом Player
            GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
            unitsPlayer.AddRange(playerObjects);

            // Находим все объекты с тегом Enemy
            GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
            unitsEnemy.AddRange(enemyObjects);

            // Создаем аватары для игроков
            CreatePlayerAvatars();

            // Создаем аватары для врагов
            CreateEnemyAvatars();
        }

        private void CreatePlayerAvatars()
        {
            // Очищаем старые аватары, если есть
            ClearAvatars(panelPlayers.transform, playerAvatars);

            // Проходим по всем найденным игрокам
            foreach (GameObject player in unitsPlayer)
            {
                // Получаем компонент UnitPlayer
                UnitPlayer unitPlayer = player.GetComponent<UnitPlayer>();

                if (unitPlayer != null)
                {
                    // Получаем префаб аватара из UnitPlayer
                    GameObject avatarPrefab = unitPlayer.avatarPrefab;

                    if (avatarPrefab != null)
                    {
                        // Создаем префаб аватара
                        GameObject avatar = Instantiate(avatarPrefab, panelPlayers.transform);

                        // Устанавливаем масштаб префаба
                        avatar.transform.localScale = new Vector3(scaleAvatars, scaleAvatars, scaleAvatars);

                        // Настраиваем аватар
                        UI_AvatarBoard avatarBoard = avatar.GetComponent<UI_AvatarBoard>();
                        if (avatarBoard != null)
                        {
                            avatarBoard.InitializeWithAbstractUnit(unitPlayer);               
                        }

                        playerAvatars.Add(avatar);
                    }
                    else
                    {
                        Debug.LogWarning($"avatarPrefab не найден у объекта Player: {player.name}");
                    }
                }
                else
                {
                    Debug.LogWarning($"UnitPlayer не найден у объекта с тегом Player: {player.name}");
                }
            }

            // Обновляем Layout Group
            UpdateLayoutGroup(panelPlayers);
        }

        private void CreateEnemyAvatars()
        {
            // Очищаем старые аватары, если есть
            ClearAvatars(panelEnemy.transform, enemyAvatars);

            // Проходим по всем найденным врагам
            foreach (GameObject enemy in unitsEnemy)
            {
                // Получаем компонент UnitEnemy
                UnitEnemy unitEnemy = enemy.GetComponent<UnitEnemy>();

                if (unitEnemy != null)
                {
                    // Получаем префаб аватара из UnitEnemy
                    GameObject avatarPrefab = unitEnemy.avatarPrefab;


                    if (avatarPrefab != null)
                    {
                        // Создаем префаб аватара
                        GameObject avatar = Instantiate(avatarPrefab, panelEnemy.transform);

                        // Устанавливаем масштаб префаба
                        avatar.transform.localScale = new Vector3(scaleAvatars, scaleAvatars, scaleAvatars);

                        // Настраиваем аватар
                        UI_AvatarBoard avatarBoard = avatar.GetComponent<UI_AvatarBoard>();
                        if (avatarBoard != null)
                        {
                            avatarBoard.InitializeWithAbstractUnit(unitEnemy);
                        }

                        enemyAvatars.Add(avatar);
                    }
                    else
                    {
                        Debug.LogWarning($"avatarPrefab не найден у объекта Enemy: {enemy.name}");
                    }
                }
                else
                {
                    // Если не найден UnitEnemy, пробуем найти AbstractUnit
                    AbstractUnit abstractUnit = enemy.GetComponent<AbstractUnit>();
                    if (abstractUnit != null)
                    {
                        GameObject avatarPrefab = abstractUnit.avatarPrefab;

                        if (avatarPrefab != null)
                        {
                            GameObject avatar = Instantiate(avatarPrefab, panelEnemy.transform);

                            UI_AvatarBoard avatarBoard = avatar.GetComponent<UI_AvatarBoard>();
                            if (avatarBoard != null)
                            {
                                avatarBoard.InitializeWithAbstractUnit(abstractUnit);
                            }

                            enemyAvatars.Add(avatar);
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"UnitEnemy или AbstractUnit не найден у объекта с тегом Enemy: {enemy.name}");
                    }
                }
            }

            // Обновляем Layout Group
            UpdateLayoutGroup(panelEnemy);
        }

        private void ClearAvatars(Transform parent, List<GameObject> avatars)
        {
            foreach (var avatar in avatars)
            {
                if (avatar != null)
                    Destroy(avatar);
            }
            avatars.Clear();
        }

        private void UpdateLayoutGroup(GameObject panel)
        {
            // Обновляем Layout Group для правильного отображения
            var layoutGroup = panel.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>();
            if (layoutGroup != null)
            {
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(panel.GetComponent<RectTransform>());
            }
        }

        // Метод для обновления UI при изменении состава юнитов
        public void RefreshUnitsUI()
        {
            // Очищаем списки юнитов
            unitsPlayer.Clear();
            unitsEnemy.Clear();

            // Повторно находим юниты
            FindAndSetupUnits();
        }

        // Метод для обновления здоровья на аватарах
        public void UpdateHealthUI()
        {
            // Обновляем аватары игроков
            for (int i = 0; i < Mathf.Min(playerAvatars.Count, unitsPlayer.Count); i++)
            {
                if (playerAvatars[i] != null && unitsPlayer[i] != null)
                {
                    UnitPlayer unitPlayer = unitsPlayer[i].GetComponent<UnitPlayer>();
                    if (unitPlayer != null)
                    {
                        UI_AvatarBoard avatarBoard = playerAvatars[i].GetComponent<UI_AvatarBoard>();
                        if (avatarBoard != null)
                        {
                            avatarBoard.SetHealth(unitPlayer.CurrentHealth, unitPlayer.MaxHealth);
                        }
                    }
                }
            }

            // Обновляем аватары врагов
            for (int i = 0; i < Mathf.Min(enemyAvatars.Count, unitsEnemy.Count); i++)
            {
                if (enemyAvatars[i] != null && unitsEnemy[i] != null)
                {
                    AbstractUnit unit = unitsEnemy[i].GetComponent<AbstractUnit>();
                    if (unit != null)
                    {
                        UI_AvatarBoard avatarBoard = enemyAvatars[i].GetComponent<UI_AvatarBoard>();
                        if (avatarBoard != null)
                        {
                            avatarBoard.SetHealth(unit.CurrentHealth, unit.MaxHealth);
                        }
                    }
                }
            }
        }

        // Метод для обновления состояния выделения всех аватаров
        public void UpdateAllAvatarSelectionStates()
        {
            foreach (var avatar in playerAvatars)
            {
                if (avatar != null)
                {
                    UI_AvatarBoard avatarBoard = avatar.GetComponent<UI_AvatarBoard>();
                    if (avatarBoard != null)
                    {
                        avatarBoard.UpdateSelectionState();
                    }
                }
            }

            foreach (var avatar in enemyAvatars)
            {
                if (avatar != null)
                {
                    UI_AvatarBoard avatarBoard = avatar.GetComponent<UI_AvatarBoard>();
                    if (avatarBoard != null)
                    {
                        avatarBoard.UpdateSelectionState();
                    }
                }
            }
        }

        // Метод для нахождения аватара по юниту
        public UI_AvatarBoard GetAvatarByUnit(AbstractUnit unit)
        {
            if (unit == null) return null;

            // Проверяем аватары игроков
            foreach (var avatar in playerAvatars)
            {
                if (avatar != null)
                {
                    UI_AvatarBoard avatarBoard = avatar.GetComponent<UI_AvatarBoard>();
                    if (avatarBoard != null && avatarBoard.IsLinkedToUnit(unit))
                    {
                        return avatarBoard;
                    }
                }
            }

            // Проверяем аватары врагов
            foreach (var avatar in enemyAvatars)
            {
                if (avatar != null)
                {
                    UI_AvatarBoard avatarBoard = avatar.GetComponent<UI_AvatarBoard>();
                    if (avatarBoard != null && avatarBoard.IsLinkedToUnit(unit))
                    {
                        return avatarBoard;
                    }
                }
            }

            return null;
        }

        // Метод для обновления конкретного аватара
        public void UpdateAvatarForUnit(AbstractUnit unit)
        {
            UI_AvatarBoard avatarBoard = GetAvatarByUnit(unit);
            if (avatarBoard != null)
            {
                avatarBoard.UpdateHealthFromUnit();
                avatarBoard.UpdateSelectionState();
            }
        }

        // Метод для обновления всех аватаров
        public void UpdateAllAvatars()
        {
            // Обновляем здоровье
            UpdateHealthUI();

            // Обновляем состояния выделения
            UpdateAllAvatarSelectionStates();
        }

        // Метод для очистки всех аватаров
        public void ClearAllAvatars()
        {
            ClearAvatars(panelPlayers.transform, playerAvatars);
            ClearAvatars(panelEnemy.transform, enemyAvatars);
        }

        // Дополнительные вспомогательные методы
        public List<GameObject> GetPlayerUnits() => unitsPlayer;
        public List<GameObject> GetEnemyUnits() => unitsEnemy;

        public int GetPlayerCount() => unitsPlayer.Count;
        public int GetEnemyCount() => unitsEnemy.Count;

        // Метод для получения конкретного аватара по индексу
        public GameObject GetPlayerAvatar(int index)
        {
            if (index >= 0 && index < playerAvatars.Count)
                return playerAvatars[index];
            return null;
        }

        public GameObject GetEnemyAvatar(int index)
        {
            if (index >= 0 && index < enemyAvatars.Count)
                return enemyAvatars[index];
            return null;
        }

        // Метод для проверки, есть ли активная битва
        public bool IsBattleActive() => isBattle;
    }
}