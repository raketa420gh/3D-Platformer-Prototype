using System;
using UnityEngine;

public class ButtonAscensiveObstacle : MonoBehaviour
{
    [SerializeField] private AscensiveObstacle obstacle;

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayerInsideTrigger(other))
        {
            obstacle.StartMove(1);
        }
    }
    
    private bool IsPlayerInsideTrigger(Collider other)
    {
        return other.gameObject.CompareTag(TagsNames.Player);
    }
}
