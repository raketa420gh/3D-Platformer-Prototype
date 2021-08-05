using System;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    public static event Action OnGotPlayer;
    
    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayerInsideTrigger(other))
        {
            OnGotPlayer?.Invoke();
        }
    }

    private bool IsPlayerInsideTrigger(Collider other)
    {
        return other.gameObject.CompareTag(TagsNames.Player);
    }
}