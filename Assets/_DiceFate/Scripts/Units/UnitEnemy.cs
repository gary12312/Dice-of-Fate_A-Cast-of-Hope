using DiceFate.EventBus;
using DiceFate.Events;
using System.Collections;
using UnityEngine;


namespace DiceFate.Units
{
    public class UnitEnemy : AbstractUnit
    {

        [SerializeField] private Transform spawnPointShield;
        [SerializeField] private Transform spawnPointCounterattack;

        [SerializeField] private GameObject prefabDiceShield;
        [SerializeField] private GameObject prefabDiceCounterattack;
        [SerializeField] private float prefabScale = 0.2f;
        [SerializeField] private int spawnForce = 500;


        protected override void Start()
        {
            base.Start();  // Сначало Вызвыть базовый метод Start из AbstractUnit
            Debug.Log($"UnitPlayer started with {CurrentHealth}/{MaxHealth} health.");

            ValidateScriptsAndComponents();
        }

        protected override void ProtectedBeforeToAttack()
        {
            base.ProtectedBeforeToAttack();
            StartCoroutine(EmemySpawnDice());
        }

        private IEnumerator EmemySpawnDice()
        {
            yield return new WaitForSeconds(2f);

            // Устанавливаем масштаб префаба
            Vector3 scaleSpawnDice = new Vector3(prefabScale, prefabScale, prefabScale);

            GameObject shieldDice = Instantiate(prefabDiceShield, spawnPointShield.position, spawnPointShield.rotation);
            GameObject counterDice = Instantiate(prefabDiceCounterattack, spawnPointCounterattack.position, spawnPointCounterattack.rotation);
            shieldDice.transform.localScale = scaleSpawnDice;
            counterDice.transform.localScale = scaleSpawnDice;

            // Bus<OnDropEvent>.Raise(new OnDropEvent(spawnForce));
        }



        // ----------------------Дополнительные методы ----------------------
        private void ValidateScriptsAndComponents()
        {
            if (spawnPointShield == null) { Debug.LogWarning("Установить Spawn Point Shield. " + this); }
            if (prefabDiceShield == null) { Debug.LogWarning("Установить Prefab Dice Shield. " + this); }
            if (spawnPointCounterattack == null) { Debug.LogWarning("Установить Spawn Point Counterattack. " + this); }
            if (prefabDiceCounterattack == null) { Debug.LogWarning("Установить Prefab Dice Counterattack. Установить" + this); }
        }

    }
}
