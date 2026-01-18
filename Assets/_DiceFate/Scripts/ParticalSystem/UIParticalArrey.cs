using UnityEngine;

public class UIParticalArrey : MonoBehaviour
{
    [SerializeField] GameObject[] uiParticalArrey;



    private void Awake()
    {
        if (uiParticalArrey.Length == 0) return;

        for (int i = 0; i < uiParticalArrey.Length; i++)
        {
            uiParticalArrey[i].SetActive(false);
        }
    }

    public void ActivePartical(int index, RectTransform rectTransform)
    {
        if (uiParticalArrey.Length == 0) return;
        if (index < 0 || index >= uiParticalArrey.Length) return;
        uiParticalArrey[index].SetActive(false);

        uiParticalArrey[index].transform.position = rectTransform.position;
        uiParticalArrey[index].SetActive(true);
    }





}
