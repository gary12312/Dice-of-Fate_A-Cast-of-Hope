using UnityEngine;
using UnityEngine.UI;
using DiceFate.Units;

namespace DiceFate.UI
{
    public class UI_AvatarBoard : MonoBehaviour
    {
        [SerializeField] private Slider sliderHealth;
        [SerializeField] private Image iBoardColor;

        private Color defaultColor;
        private string curentUnitType; // Тип юнита: игрок, враг и т.д.


        private AbstractUnit linkedUnit; // Ссылка на связанный юнит

        private void Awake()
        {
            ValidateScriptsAndObject();
            defaultColor = iBoardColor.color;
        }

        private void Update()
        {
            // В реальном проекте лучше вызывать UpdateSelectionState() только при изменении состояния юнита
            //  UpdateSelectionState();
            //RefreshAvatar();
        }

        // UI_Mane_Battle вызывает Инициализацию аватара с данными 
        public void InitializeWithAbstractUnit(AbstractUnit unit)
        {
            if (unit == null)
            {
                Debug.LogWarning("UI_AvatarBoard: Попытка инициализации с null юнитом");
                return;
            }

            linkedUnit = unit;

            // if (unitNameText != null) { unitNameText.text = unit.name; }// Устанавливаем имя юнита

            curentUnitType = unit.unitType; // Получаем тип юнита 

            SetHealth(unit.CurrentHealth, unit.MaxHealth); // Устанавливаем начальное здоровье            
            AvatarBoardDefault(); // Устанавливаем цвет по умолчанию           
            UpdateSelectionState(); // Обновляем состояние выделения
        }






        // Метод для обновления всех данных аватара
        public void RefreshAvatar()
        {
            UpdateHealthFromUnit();
            UpdateSelectionState();
        }

        // Метод для обновления данных из связанного юнита
        public void UpdateHealthFromUnit()
        {
            if (linkedUnit != null)
            {
                SetHealth(linkedUnit.CurrentHealth, linkedUnit.MaxHealth);
            }
        }

        // Метод для обновления состояния выделения
        public void UpdateSelectionState()
        {
            if (linkedUnit == null)
            {
                AvatarBoardInactiveInBattle();
                return;
            }

            //if (curentUnitType == "Player" && linkedUnit.IsSelected == true)
            //{

                if (linkedUnit.IsSelected)
                {
                    AvatarBoardSelected();
                }
                //else if (linkedUnit.IsSelectedForVisibleUi) // в дальнейшем добавтть
                //{
                //    AvatarBoardInactiveInBattle();
                //}
                else
                {
                    AvatarBoardDefault();
                }
            //}

        }

        public void UpdateHoverState() // Наведене
        {
            if (linkedUnit == null )
            {
                AvatarBoardInactiveInBattle();
                return;
            }

            if (curentUnitType == "Player" && linkedUnit.IsSelected == false)
            {
                if (linkedUnit.IsHover)
                {
                    AvatarBoardPlayer();
                }
                else
                {
                    AvatarBoardDefault();
                }

            }
            else if (curentUnitType == "Enemy")
            {
                if (linkedUnit.IsHover)
                {
                    AvatarBoardEnemy();
                }
                else
                {
                    AvatarBoardDefault();
                }
            }     
        }





        private void SetBoardColor(Color color) => iBoardColor.color = color;
        public void AvatarBoardDefault() => SetBoardColor(defaultColor);
        public void AvatarBoardSelected() => SetBoardColor(Color.blue);
        public void AvatarBoardInactiveInBattle() => SetBoardColor(Color.white);
        public void AvatarBoardDamaged() => SetBoardColor(Color.red);
        public void AvatarBoardHealed() => SetBoardColor(Color.green);
        public void AvatarBoardWarning() => SetBoardColor(Color.yellow);  // Метод для установки предупреждающего цвета
        public void AvatarBoardPlayer() => SetBoardColor(Color.white);  
        public void AvatarBoardEnemy() => SetBoardColor(Color.red);  


        public void SetHealth(int currentHealth, int maxHealth)
        {
            // Проверяем валидность значений
            if (maxHealth <= 0)
            {
                Debug.LogWarning($"UI_AvatarBoard: Некорректное максимальное здоровье: {maxHealth}");
                maxHealth = 1;
            }

            currentHealth = maxHealth - currentHealth; // Инвертируем здоровье для отображения в слайдере
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            sliderHealth.maxValue = maxHealth;
            sliderHealth.value = currentHealth;
            //if (currentHealth >= maxHealth)
            //{
            //    DieUsers();
            //}
        }


        private void DieUsers()
        {
            // Очистка ссылки на юнит при уничтожении аватара
            linkedUnit = null;
            Destroy(gameObject);
        }

        // Метод для получения связанного юнита
        public AbstractUnit GetLinkedUnit() => linkedUnit;

        // Метод для проверки, связан ли аватар с конкретным юнитом
        public bool IsLinkedToUnit(AbstractUnit unit)
        {
            return linkedUnit != null && linkedUnit == unit;
        }
        // Метод для очистки ссылки при уничтожении юнита
        public void ClearLinkedUnit()
        {
            linkedUnit = null;
            AvatarBoardInactiveInBattle();  // неправильно
        }


        // Метод для проверки, является ли связанный юнит игроком
        public bool IsPlayerUnit()
        {
            if (linkedUnit == null) return false;
            return linkedUnit.CompareTag("Player"); // Проверяем по тегу
        }

        // Метод для проверки, является ли связанный юнит врагом
        public bool IsEnemyUnit()
        {
            if (linkedUnit == null) return false;
            return linkedUnit.CompareTag("Enemy"); // Проверяем по тегу
        }



        // Метод для установки видимости аватара --???
        public void SetVisible(bool isVisible)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(isVisible);
            }
        }




        // ------------------------ анимации получения урона и исцеления  ---------------------------------
        public void PlayDamageAnimation()
        {
            // Здесь можно добавить анимацию получения урона
            AvatarBoardDamaged();
            Invoke("ReturnToNormalColor", 0.5f); // Возвращаем цвет через некоторое время
        }
        public void PlayHealAnimation()
        {
            // Здесь можно добавить анимацию исцеления
            AvatarBoardHealed();
            Invoke("ReturnToNormalColor", 0.5f);// Возвращаем цвет через некоторое время
        }

        private void ReturnToNormalColor()
        {
            if (linkedUnit != null)
            {
                UpdateSelectionState();
            }
            else
            {
                AvatarBoardDefault();
            }
        }

        // ------------------------ Проверки ---------------------------------
        private void ValidateScriptsAndObject()
        {
            if (sliderHealth == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на sliderHealth!");
            if (iBoardColor == null)
                Debug.LogError($" для {this.name} Не установлена ссылка на iBoardColor!");
        }





        // ------------------------ Дополнительные методы ??? ---------------------------------
        //[Header("Необязательно")]
        //[SerializeField] private Image avatarIcon; // Опционально: для отображения иконки юнита
        //[SerializeField] private Text unitNameText; // Опционально: для отображения имени

        //// Метод для установки иконки (если передается спрайт)
        //public void SetAvatarIcon(Sprite icon)
        //{
        //    if (avatarIcon != null && icon != null)
        //    {
        //        avatarIcon.sprite = icon;
        //    }
        //}

        //// Метод для установки имени юнита напрямую
        //public void SetUnitName(string name)
        //{
        //    if (unitNameText != null)
        //    {
        //        unitNameText.text = name;
        //    }
        //}

    }
}