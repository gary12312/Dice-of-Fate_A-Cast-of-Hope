using UnityEngine;

public class CubeController : MonoBehaviour
{
    // Ссылка на компонент Renderer куба, чтобы менять его материал
    private Renderer cubeRenderer;

    // Статическая переменная, которая хранит ссылку на ВЫБРАННЫЙ куб
    // Статическая - значит, она одна на все экземпляры класса (на все кубы)
    public static CubeController SelectedCube { get; private set; }

    void Start()
    {
        // В начале получаем компонент Renderer этого конкретного куба
        cubeRenderer = GetComponent<Renderer>();
    }

    // Этот метод вызывается, когда по кубу кликают мышью
    void OnMouseDown()
    {
        // Выбираем этот куб
        SelectCube();
    }

    // Метод для выбора этого куба
    public void SelectCube()
    {
        // Делаем этот куб выбранным
        SelectedCube = this;
        // Выводим в консоль сообщение с именем куба
        Debug.Log("Куб выбран: " + gameObject.name);

        // (ОПЦИОНАЛЬНО) Можно добавить визуальное выделение, например, изменить цвет на серый
        // Запоминаем исходный материал, чтобы потом можно было его вернуть
        // cubeRenderer.material.color = Color.gray;
    }

    // Метод для применения нового материала к этому кубу
    public void ApplyMaterial(Material newMaterial)
    {
        // Меняем материал рендерера этого куба на переданный
        cubeRenderer.material = newMaterial;
    }
}