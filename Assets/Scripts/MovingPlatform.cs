using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

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
        StartMove();
    }

    //private void Update()
    //{
    //    var target = isMoveToEnd ? endPosition : startPosition;
    //    var time = isMoveToEnd ? toEndPositionMoveTime : toStartPositionMoveTime; 
    //    var speed = Vector3.Distance(endPosition, startPosition) / time;
    //    var step = speed * Time.deltaTime;
        
    //    transform.position = Vector3.MoveTowards(transform.position, target, step);

    //    if (transform.position == target)
    //    {
    //        isMoveToEnd = !isMoveToEnd;
    //    }
    //}

    private void StartMove()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(startPositionDelay);
        sequence.Append(transform.DOLocalMove(endPosition, toEndPositionMoveTime).SetEase(Ease.Linear));
        sequence.AppendInterval(endPositionDelay);
        sequence.Append(transform.DOLocalMove(startPosition, toStartPositionMoveTime).SetEase(Ease.Linear));
        sequence.SetLoops(-1);
    }
}
