using System.Collections;
using UnityEngine;

public class LevelManager : SingletonMonoBehaviour<LevelManager>
{
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private float restartLevelDelay;
    [SerializeField] private Teleport finishLevelTeleport;

    private int allCoins;
    private int collectedCoins;

    private void OnEnable()
    {
        SceneLoader.OnSceneLoaded += SceneLoaderOnSceneLoaded;
        Player.OnDied += PlayerOnDied;
        Coin.OnCreated += CoinOnCreated;
        Coin.OnCollected += CoinOnCollected;
    }

    private void OnDisable()
    {
        SceneLoader.OnSceneLoaded -= SceneLoaderOnSceneLoaded;
        Player.OnDied -= PlayerOnDied;
        Coin.OnCreated -= CoinOnCreated;
        Coin.OnCollected -= CoinOnCollected;
    }

    private void CalculateAllCoins()
    {
        allCoins ++;
        
        Debug.Log($"Всего монет осталось = {allCoins}");
    }

    private void CollectCoin()
    {
        allCoins--;
        collectedCoins ++;
        
        Debug.Log($"Вы подобрали монету. Всего {collectedCoins}");

        if (allCoins <= 0)
        {
            Debug.Log($"Всего монет осталось = {allCoins}");
            StartCoroutine(RestartLevelFromDelay());
        }
    }

    private void InstantiateTeleport()
    {
        
    }

    private IEnumerator RestartLevelFromDelay()
    {
        yield return new WaitForSeconds(restartLevelDelay);
        sceneLoader.ReloadScene();
    }

    private void PlayerOnDied()
    {
        StartCoroutine(RestartLevelFromDelay());
    }

    private void CoinOnCollected()
    {
        CollectCoin();
    }

    private void CoinOnCreated()
    {
        CalculateAllCoins();
    }

    private void SceneLoaderOnSceneLoaded()
    {
        allCoins = 0;
        collectedCoins = 0;
    }
}
