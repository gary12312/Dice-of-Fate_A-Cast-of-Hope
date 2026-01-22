using UnityEngine;

namespace DiceFate.UI
{
    public class UI_BlockOnClick : MonoBehaviour
    {
        [SerializeField] private GameObject a_BlockOnClick;
        [SerializeField] private GameObject b_BlockOnClick;
        [SerializeField] private GameObject c_BlockOnClick;



        public void A_BlockOnClicActive() => BlocOnClick(a_BlockOnClick, true);
        public void B_BlockOnClicActive() => BlocOnClick(b_BlockOnClick, true);
        public void C_BlockOnClicActive() => BlocOnClick(c_BlockOnClick, true);

        public void A_BlockOnClicDeactive() => BlocOnClick(a_BlockOnClick, false);
        public void B_BlockOnClicDeactive() => BlocOnClick(b_BlockOnClick, false);
        public void C_BlockOnClicDeactive() => BlocOnClick(c_BlockOnClick, false);


        private void BlocOnClick(GameObject blockImage, bool isActive)
        {
            if (blockImage != null)
            {
                blockImage.SetActive(isActive);
            }
            else
            {
                Debug.LogError($" для {this.name} Не установлена ссылка на Image {blockImage} ");
            }
        }
    }
}
