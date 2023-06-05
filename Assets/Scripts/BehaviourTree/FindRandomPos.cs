using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class FindRandomPos : Action
{
    Vector3 _walkPoint;
    Vector3 _initialPosition;
    public float patrolRadius = 3f;

    public SharedVector3 _rndPos;
    
    public override TaskStatus OnUpdate()
    {
        _rndPos.Value = SearchWalkPoint();
        if (_rndPos.Value == Vector3.negativeInfinity)
            return TaskStatus.Failure;

        return TaskStatus.Success;
    }

    private Vector3 SearchWalkPoint()
    {
        int rndPosCounter = 0;
        while (rndPosCounter < 100)
        {
            // Calculate random point within patrol radius
            float randomX = Random.Range(-patrolRadius, patrolRadius);
            float randomY = Random.Range(-patrolRadius, patrolRadius);
            Vector3 randomOffset = new Vector3(randomX, randomY, transform.position.z);
            _walkPoint = _initialPosition + randomOffset;
            return _walkPoint;
        }
        return Vector3.negativeInfinity;

    }
}
