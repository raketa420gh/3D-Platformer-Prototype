using System;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public static event Action OnCreated;
    public static event Action OnCollected;
    
    private void Awake()
    {
        OnCreated?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayerInsideTrigger(other))
        {
            Destroy(gameObject);
            OnCollected?.Invoke();
        }
    }
    
    private bool IsPlayerInsideTrigger(Collider collider)
    {
        return collider.gameObject.CompareTag(TagsNames.Player);
    }
}
