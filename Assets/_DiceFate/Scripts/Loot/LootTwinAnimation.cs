using DG.Tweening;
using UnityEngine;
using DG_Tweening_Exemple;
using System.Collections;

namespace DiceFate.Loot
{
    public class LootTwinAnimation : MonoBehaviour
    {
        [SerializeField] public int coinsToSpawn = 5;
        [SerializeField] private DT_Coins _coinsPrefab;
        [SerializeField] private GameObject lootBox;


    

        public void OpenLootBox()
        {
            StartCoroutine(ProcessOpenLootBox());
        }

        private IEnumerator ProcessOpenLootBox()
        {           
            Vector3 coinsSpawnerPoint = transform.position;

            yield return Open();
            Destroy(lootBox.gameObject);

            Debug.Log("Старт спавна монеток");

            YieldInstruction[] coinsSpawmAnimation = new YieldInstruction[coinsToSpawn];
            DT_Coins[] coins = new DT_Coins[coinsToSpawn];

            for (int i = 0; i < coinsToSpawn; i++)
            {
                if (i != 0)
                    yield return new WaitForSeconds(0.01f);

                coins[i] = Instantiate(_coinsPrefab, coinsSpawnerPoint, Quaternion.Euler(0, Random.Range(0, 360), 0));

                coinsSpawmAnimation[i] = AnimateSpawnFor(coins[i]);
            }

            foreach (YieldInstruction spawmAnimation in coinsSpawmAnimation)
                yield return spawmAnimation;


            Debug.Log("Спавн кончился!");

            yield return new WaitForSeconds(0.25f);

            foreach (DT_Coins coin in coins)
                StartCoroutine(PickupCoinProcess(coin));


        }

        // Анимировать Сундук
        public YieldInstruction Open() 
        {
            return lootBox.transform
                .DOScale(0, 0.5f)
                .SetEase(Ease.InBack)
                .Play()
                .WaitForCompletion();
        }





        private YieldInstruction AnimateSpawnFor(DT_Coins coins)
        {
            Vector2 randomeOffcet = Random.insideUnitSphere;
            Vector3 offset = new Vector3(randomeOffcet.x, 0, randomeOffcet.y);
            Vector3 jumpPosition = coins.transform.position + offset * 1.5f;

            return coins.transform
                .DOJump(jumpPosition, 2, 1, 0.7f)
                .SetEase(Ease.OutBounce)
                .Play()
                .WaitForCompletion();
        }

        private IEnumerator PickupCoinProcess(DT_Coins coin)
        {
            yield return coin.Pickup();

            Debug.Log("Начислили монетку после анимации");

            Destroy(coin.gameObject);

        }
    }
}
