using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(PlatformTriggerHandler))]

public class MovingPlatform : MonoBehaviour
{
    [Header("Move Settings")] 
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 endPosition;

    [Header("Timing Settings")] 
    [SerializeField] private float startPositionDelay;
    [SerializeField] private float endPositionDelay;
    [SerializeField] private float toEndPositionMoveTime;
    [SerializeField] private float toStartPositionMoveTime;

    private void Start()
    {
        StartMove(-1);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(startPosition, 0.5f);
        Gizmos.DrawSphere(endPosition, 0.5f);
        Gizmos.DrawLine(startPosition, endPosition);
    }

    private void StartMove(int loops)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(startPositionDelay);
        sequence.Append(transform.DOLocalMove(endPosition, toEndPositionMoveTime).SetEase(Ease.Linear));
        sequence.AppendInterval(endPositionDelay);
        sequence.Append(transform.DOLocalMove(startPosition, toStartPositionMoveTime).SetEase(Ease.Linear));
        sequence.SetLoops(loops);
    }
}
