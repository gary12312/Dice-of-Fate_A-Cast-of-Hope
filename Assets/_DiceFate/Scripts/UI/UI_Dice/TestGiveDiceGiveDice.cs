using UnityEngine;
using System.Collections.Generic;


// Тестовый скрипт для проверки кубиков на поле и вывода информации в консоль
public class TestGiveDiceGiveDice : MonoBehaviour
{
    [Header("Ссылка на поле")]
    public UiDropTargetField dropTargetField; // Ссылка на поле с кубиками

    [Header("Настройки")]
    [Tooltip("Интервал проверки поля в секундах")]
    public float checkInterval = 1f; // Интервал проверки в секундах

    private float timeSinceLastCheck = 0f; // Время с последней проверки

    void Start()
    {
        // Проверяем что ссылка на поле установлена
        if (dropTargetField == null)
        {
            Debug.LogError("Не установлена ссылка на DropTargetField!");
        }

        // Выполняем первую проверку сразу при старте
        CheckFieldAndLogDice();
    }

    void Update()
    {
        // Обновляем таймер
        timeSinceLastCheck += Time.deltaTime;

        // Если прошло достаточно времени - выполняем проверку
        if (timeSinceLastCheck >= checkInterval)
        {
            CheckFieldAndLogDice();
            timeSinceLastCheck = 0f; // Сбрасываем таймер
        }

        // Пример: можно также проверять при нажатии клавиши
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckFieldAndLogDice();
        }
    }

    // Метод для проверки поля и вывода информации о кубиках
    public void CheckFieldAndLogDice()
    {
        if (dropTargetField == null)
        {
            Debug.LogError("Ссылка на DropTargetField не установлена!");
            return;
        }

        // Получаем имена кубиков на поле (используем GetDiceName() который работает с DiceType)
        List<string> diceNames = dropTargetField.GetDiceNamesOnField();

        // Формируем строку с результатом
        if (diceNames.Count > 0)
        {
            // Объединяем имена кубиков через запятую
            string result = "На поле кубики: " + string.Join(", ", diceNames);
            Debug.Log(result);
        }
        else
        {
            Debug.Log("На поле нет кубиков");
        }
    }

    // НОВЫЙ МЕТОД: Проверка поля с детальной информацией о типах кубиков
    public void CheckFieldWithDetails()
    {
        if (dropTargetField == null)
        {
            Debug.LogError("Ссылка на DropTargetField не установлена!");
            return;
        }

        // Получаем типы кубиков на поле
        List<UiDragAndDropDice.DiceType> diceTypes = dropTargetField.GetDiceTypesOnField();

        if (diceTypes.Count > 0)
        {
            // Подсчитываем количество каждого типа
            int movementCount = dropTargetField.GetDiceTypeCount(UiDragAndDropDice.DiceType.Movement);
            int attackCount = dropTargetField.GetDiceTypeCount(UiDragAndDropDice.DiceType.Attack);
            int shieldCount = dropTargetField.GetDiceTypeCount(UiDragAndDropDice.DiceType.Shield);
            int counterattackCount = dropTargetField.GetDiceTypeCount(UiDragAndDropDice.DiceType.Counterattack);

            // Формируем детальный отчет
            string details = $"Детальная информация о кубиках на поле:\n" +
                           $"Всего кубиков: {diceTypes.Count}\n" +
                           $"Movement: {movementCount}\n" +
                           $"Attack: {attackCount}\n" +
                           $"Shield: {shieldCount}\n" +
                           $"Counterattack: {counterattackCount}";

            Debug.Log(details);
        }
        else
        {
            Debug.Log("На поле нет кубиков");
        }
    }

    // Метод для ручной проверки поля (можно вызывать из UI кнопки)
    public void ManualCheck()
    {
        CheckFieldAndLogDice();
    }

    // Метод для ручной проверки с деталями
    public void ManualDetailedCheck()
    {
        CheckFieldWithDetails();
    }

    // Метод для получения количества кубиков на поле
    public int GetDiceCountOnField()
    {
        if (dropTargetField != null)
        {
            return dropTargetField.GetDiceOnField().Count;
        }
        return 0;
    }

    // Метод для проверки наличия конкретного типа кубика на поле (обновлен для DiceType)
    public bool HasDiceTypeOnField(UiDragAndDropDice.DiceType diceType)
    {
        if (dropTargetField != null)
        {
            return dropTargetField.HasDiceType(diceType);
        }
        return false;
    }

    // Метод для получения количества кубиков определенного типа на поле
    public int GetDiceTypeCountOnField(UiDragAndDropDice.DiceType diceType)
    {
        if (dropTargetField != null)
        {
            return dropTargetField.GetDiceTypeCount(diceType);
        }
        return 0;
    }
}