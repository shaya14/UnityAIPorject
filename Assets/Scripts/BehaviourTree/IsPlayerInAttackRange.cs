using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class IsPlayerInAttackRange : Conditional
{

    public SharedTransform _closestObject;

    public SharedBool _playerInAttackRange;
    public LayerMask _whatIsPlayer;
    public float _attackRange;


    public override void OnStart()
    {

    }

    public override TaskStatus OnUpdate()
    {
        Debug.Log("Player in range: " + _playerInAttackRange.Value);
        _playerInAttackRange.Value = Physics2D.OverlapCircle(transform.position, _attackRange, _whatIsPlayer);
        if (_playerInAttackRange.Value)
        {
            _closestObject.Value = GameObject.FindGameObjectWithTag("Player").transform;
            Debug.Log("Called");
            return TaskStatus.Success;
        }

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

    public static bool isInRange(Vector3 origin, Vector3 target, float range)
    {
        return Vector3.Distance(origin, target) <= range;
    }
}
