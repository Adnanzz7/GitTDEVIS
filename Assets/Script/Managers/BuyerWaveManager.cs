using System.Collections;
using UnityEngine;

public class BuyerWaveManager : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] GameObject buyerPrefab;
    [SerializeField] Transform spawnPoint;
    [SerializeField] NPCProfileSO[] profiles;
    [SerializeField] float delayBetweenBuyers = 15f;

    [Header("Limit")]
    [SerializeField] int maxBuyersInWave = 20;

    [Header("Recap Panel")]
    [SerializeField] RecapPanel recapPanel;

    int spawned;
    BuyerBehaviour currentBuyer;

    void OnEnable()
    {
        SalesStats.Reset();
        StartCoroutine(SpawnRoutine());
    }
    void OnDisable() => StopAllCoroutines();

    IEnumerator SpawnRoutine()
    {
        if (buyerPrefab == null || buyerPrefab.scene.IsValid())
        { Debug.LogError("[BuyerWave] buyerPrefab harus prefab asset."); yield break; }

        while (maxBuyersInWave < 0 || spawned < maxBuyersInWave)
        {
            SpawnBuyer();

            bool done = false;
            BuyerBehaviour buyerRef = currentBuyer;
            buyerRef.OnTradeFinished += () => done = true;

            yield return new WaitUntil(() => done);

            if (buyerRef != null)
                Destroy(buyerRef.gameObject);

            currentBuyer = null;
            spawned++;

            if (maxBuyersInWave >= 0 && spawned >= maxBuyersInWave) break;

            yield return new WaitForSeconds(delayBetweenBuyers);
        }

        EndWave();
    }

    void SpawnBuyer()
    {
        GameObject go = Instantiate(buyerPrefab, spawnPoint.position, Quaternion.identity);
        currentBuyer = go.GetComponent<BuyerBehaviour>();
        currentBuyer.Init(profiles[Random.Range(0, profiles.Length)]);
    }

    void EndWave()
    {
        if (currentBuyer) Destroy(currentBuyer.gameObject);

        recapPanel.ShowRecap();

        DayManager.I.StartNight();
    }

    public void StopWaves()
    {
        StopAllCoroutines();
        if (currentBuyer)
        {
            Destroy(currentBuyer.gameObject);
            currentBuyer = null;
        }
    }
}