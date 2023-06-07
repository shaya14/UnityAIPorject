using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class MoveToObj : Action
{
    private NavMeshAgent _agent;

    public SharedTransform _targetObj;
    public override void OnAwake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }
    public override void OnStart()
    {
        if (isInRange(transform.position, _targetObj.Value.position, 0.1f) == false)
        {
            _agent.SetDestination(_targetObj.Value.position);
            LookAtTarget();
            _agent.isStopped = false;
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (_targetObj.Value == null)
        {
            return TaskStatus.Failure;
        }

        if (_agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            _agent.SetDestination(_targetObj.Value.position);
            LookAtTarget();
            _agent.isStopped = false;
        }

        if (isInRange(transform.position, _targetObj.Value.position, 1))
        {
            _agent.isStopped = true;
            LookAtTarget();
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }

    public static bool isInRange(Vector3 origin, Vector3 target, float range)
    {
        return Vector3.Distance(origin, target) <= range;
    }

    private void LookAtTarget()
    {
        Vector3 targetPos = _targetObj.Value.transform.position;
        Vector3 dirToPlayer = targetPos - transform.position;
        dirToPlayer.z = 0.0f;
        transform.up = dirToPlayer;
    }
}
