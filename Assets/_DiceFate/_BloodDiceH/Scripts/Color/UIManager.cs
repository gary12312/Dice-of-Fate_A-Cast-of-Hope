using UnityEngine;
using UnityEngine.UI; // Не забудь эту библиотеку для работы с UI элементами

public class UIManager : MonoBehaviour
{
    // Ссылки на материалы, которые мы перетащим из Инспектора
    [Header("Материалы")]
    public Material redMaterial;
    public Material blueMaterial;
    public Material greenMaterial;

    // Ссылки на компоненты Image (или Button) в UI
    [Header("UI Images")]
    public Image redImage;
    public Image blueImage;
    public Image greenImage;

    void Start()
    {
        // Подписываем методы-обработчики на событие "клик" для каждой картинки
        // Когда произойдет клик, вызовется соответствующий метод

        // Вариант 1: Если используете Images и хотите кликать по ним
        redImage.GetComponent<Button>().onClick.AddListener(OnRedImageClicked);
        blueImage.GetComponent<Button>().onClick.AddListener(OnBlueImageClicked);
        greenImage.GetComponent<Button>().onClick.AddListener(OnGreenImageClicked);

        // Вариант 2: Если изначально используете Button, замените Image на Button в полях выше
        // и просто подпишитесь на их событие onClick.
        // redButton.onClick.AddListener(OnRedImageClicked);
        // и т.д.
    }

    // Метод, который проверяет, выбран ли куб, и применяет материал
    private void ApplyMaterialToSelectedCube(Material material)
    {
        // Проверяем, есть ли выбранный куб (SelectedCube не равен null)
        if (CubeController.SelectedCube != null)
        {
            // Вызываем у выбранного куба метод ApplyMaterial и передаем нужный материал
            CubeController.SelectedCube.ApplyMaterial(material);
            Debug.Log("Применен материал: " + material.name);
        }
        else
        {
            // Если куб не выбран, сообщаем об этом
            Debug.LogWarning("Сначала выбери куб!");
        }
    }

    // Эти методы вызываются при клике на соответствующие изображения/кнопки
    public void OnRedImageClicked()
    {
        ApplyMaterialToSelectedCube(redMaterial);
    }

    public void OnBlueImageClicked()
    {
        ApplyMaterialToSelectedCube(blueMaterial);
    }

    public void OnGreenImageClicked()
    {
        ApplyMaterialToSelectedCube(greenMaterial);
    }
}