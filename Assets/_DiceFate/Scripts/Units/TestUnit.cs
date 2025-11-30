using UnityEngine;

namespace DiceFate.Units
{
    public class TestUnit : AbstractUnit
    {

        protected override void Start()
        {
            base.Start();  // Сначало Вызвыть базовый метод Start из AbstractUnit
            Debug.Log($"TestUnit started with {CurrentHealth}/{MaxHealth} health.");
        }

    }
}
