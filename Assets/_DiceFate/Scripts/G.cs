using System.Collections;
using UnityEngine;


namespace DiceFate
{
    public class G : MonoBehaviour
    {
       public static bool isGamePaused = false;
       public static bool isCanLeftClick = true;


        private bool isw = false;

        private void Update()
        {
            isw = isCanLeftClick;
        }


        public void PauseGame()
        {
            isGamePaused = true;
            Time.timeScale = 0f;
        }



        public void LeftClick(bool isCan)
        {
            if (isCan) isCanLeftClick = true;            else isCanLeftClick = false;  
        }




        public IEnumerator WaitAndAction(float waitTime, System.Action action)
        {
            yield return new WaitForSeconds(waitTime);
            action?.Invoke();
        }


    }
}
