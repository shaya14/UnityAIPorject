using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class IsPlayerInAttackRange : Conditional
{
    public SharedFloat _range;

    public string _objectTag;
    public SharedTransform _closestObject;

    private bool _playerInAttackRange;
    public LayerMask _whatIsPlayer;
    public float _attackRange;

    public override void OnStart()
    {

    }

    public override TaskStatus OnUpdate()
    {
        //Transform closestObject = FindClosestObj(transform, _range.Value, _objectTag);
        _playerInAttackRange = Physics2D.OverlapCircle(transform.position, _attackRange, _whatIsPlayer);
        if (_playerInAttackRange)
        {
            _closestObject.Value = GameObject.FindGameObjectWithTag("Player").transform;
            Debug.Log("Called");
            return TaskStatus.Success;
        }
        else if (!_playerInAttackRange)
            return TaskStatus.Failure;
        return TaskStatus.Failure;

    }

    public Transform FindClosestObj(Transform origin, float range, string tag)
    {
        Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(origin.position, range);
        List<Transform> objList = new List<Transform>();
        for (int i = 0; i < collidersInRange.Length; i++)
        {
            Transform obj = collidersInRange[i].transform;
            if (obj.tag.Equals(tag) && obj.transform != origin.transform)
                objList.Add(obj);
        }

        objList.Sort((a, b) =>
            {
                float distA = Vector3.SqrMagnitude(origin.position - a.transform.position);
                float distB = Vector3.SqrMagnitude(origin.position - b.transform.position);
                if (distA < distB)
                    return -1;
                else if (distA == distB)
                    return 0;
                else return 1;
            }
        );

        if (objList.Count == 0)
            return null;
        else
            return objList[0];
    }
}
