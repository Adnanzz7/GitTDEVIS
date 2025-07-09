using System.Collections;
using UnityEngine;

public class BuyerWaveManager : MonoBehaviour
{
    [SerializeField] GameObject buyerPrefab;
    [SerializeField] Transform spawnPoint;
    [SerializeField] NPCProfileSO[] profiles;
    [SerializeField] float timeBeforeFirstBuyer = 5f;
    [SerializeField] float delayBetweenBuyers = 15f;
    [SerializeField] int maxBuyersInWave = 15;
    [SerializeField] float waveDuration = 180f;

    Coroutine waveCo;
    BuyerBehaviour currentBuyer;

    void OnEnable() { SalesStats.Reset(); waveCo = StartCoroutine(SpawnRoutine()); }
    void OnDisable() { StopAllCoroutines(); }

    IEnumerator SpawnRoutine()
    {
        if (buyerPrefab == null || buyerPrefab.scene.IsValid())
        { Debug.LogError("[BuyerWave] buyerPrefab harus prefab asset."); yield break; }

        yield return new WaitForSeconds(timeBeforeFirstBuyer);

        float endTime = Time.time + waveDuration;
        int spawned = 0;

        while ((maxBuyersInWave < 0 || spawned < maxBuyersInWave) && Time.time < endTime)
        {
            GameObject go = Instantiate(buyerPrefab, spawnPoint.position, Quaternion.identity);
            currentBuyer = go.GetComponent<BuyerBehaviour>();
            currentBuyer.Init(profiles[Random.Range(0, profiles.Length)]);

            bool done = false;
            currentBuyer.OnTradeFinished += () => done = true;

            // tunggu transaksi OR timer global habis
            yield return new WaitUntil(() => done || Time.time >= endTime);

            Destroy(go);
            currentBuyer = null;

            if (Time.time >= endTime) break;

            yield return new WaitForSeconds(delayBetweenBuyers);
            spawned++;
        }

        EndWave();
    }

    void EndWave()
    {
        if (currentBuyer) Destroy(currentBuyer.gameObject);

        FindObjectOfType<RecapPanel>()?.ShowRecap();

        StopAllCoroutines();
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
