using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.Diagnostics;

public class isPlayerInSight : Conditional
{
    public SharedFloat _range;

    public string _objectTag;
    public SharedTransform _closestObject;

    public override void OnStart()
    {

    }

    public override TaskStatus OnUpdate()
    {
        Transform closestObject = FindClosestObj(transform, _range.Value, _objectTag);
        if (closestObject)
        {
            _closestObject.Value = closestObject;
            return TaskStatus.Success;
        }
        else
            return TaskStatus.Failure;

    }

    public static Transform FindClosestObj(Transform origin, float range, string tag)
    {
        Collider[] collidersInRange = Physics.OverlapSphere(origin.position, range);
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
