using UnityEngine;


namespace DiceFate.Units
{
    public class UnitPlayer : AbstractUnit
    {


        protected override void Start()
        {
            base.Start();  // Сначало Вызвыть базовый метод Start из AbstractUnit
            Debug.Log($"UnitPlayer started with {CurrentHealth}/{MaxHealth} health.");
        }

    }

}
