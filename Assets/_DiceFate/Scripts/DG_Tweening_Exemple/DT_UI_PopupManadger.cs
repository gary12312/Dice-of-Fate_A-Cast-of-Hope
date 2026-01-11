using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

namespace DG_Tweening_Exemple
{
    public class DT_UI_PopupManadger : MonoBehaviour
    {
        [SerializeField] private DT_UI_Popup _dT_UI_Popup;
        [SerializeField] private DT_Player _dT_Player;


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                _dT_UI_Popup.Show();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                _dT_UI_Popup.Hide(() => _dT_UI_Popup._bodyAlphaGroup.gameObject.SetActive(false));
            }


            if (Input.GetKeyDown(KeyCode.D))  // принудительное прерывание аимации
            {
                if (_dT_UI_Popup.InAnimation())
                {
                    _dT_UI_Popup.ForceComplitAnimation();
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {              
                _dT_Player.JampAndDamage();               
            }


        }





    }
}
