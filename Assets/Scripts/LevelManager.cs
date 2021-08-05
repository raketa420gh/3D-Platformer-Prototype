using System.Collections;
using UnityEngine;

public class LevelManager : SingletonMonoBehaviour<LevelManager>
{
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private float restartLevelDelay;

    private int coinsCollected;
    private int coinsRemains;

    private void OnEnable()
    {
        Player.OnDied += PlayerOnDied;
        Coin.OnCreated += CoinOnCreated;
        Coin.OnCollected += CoinOnCollected;
    }

    private void OnDisable()
    {
        Player.OnDied -= PlayerOnDied;
        Coin.OnCreated -= CoinOnCreated;
        Coin.OnCollected -= CoinOnCollected;
    }

    private void RestartLevel()
    {
        sceneLoader.LoadScene(0);
    }

    private void ResetCollectedCoins()
    {
        coinsCollected = 0;
    }

    private IEnumerator RestartLevelFromDelay()
    {
        yield return new WaitForSeconds(restartLevelDelay);
        RestartLevel();
    }

    private void PlayerOnDied()
    {
        StartCoroutine(RestartLevelFromDelay());
    }

    private void CoinOnCollected()
    {
        coinsRemains -= 1;
        coinsCollected += 1;
        
        Debug.Log($"Собрано {coinsCollected} монет");

        if (coinsRemains <= 0)
        {
            Debug.Log($"Всего монет осталось = {coinsRemains}");
            StartCoroutine(RestartLevelFromDelay());
        }
    }

    private void CoinOnCreated()
    {
        coinsRemains += 1;
        
        Debug.Log($"Всего монет осталось = {coinsRemains}");
    }
}
