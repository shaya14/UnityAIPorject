using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.GraphView.GraphView;


public class MoveToPos : Action
{
    private NavMeshAgent _agent;
    public SharedVector3 _targetPos;
    public override void OnAwake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }
    public override void OnStart()
    {
        if (isInRange(transform.position, _targetPos.Value, 0.1f) == false)
        {
            _agent.SetDestination(_targetPos.Value);
            _agent.isStopped = false;
            
            // LookAtTarget 
            Vector3 targetPos = _targetPos.Value;
            Vector3 dirToPlayer = targetPos - transform.position;
            dirToPlayer.z = 0.0f;
            transform.up = dirToPlayer;
        }
    }

    // Update is called once per frame
    public override TaskStatus OnUpdate()
    {
        if (isInRange(transform.position, _targetPos.Value, 0.1f))
        {
            _agent.isStopped = true;
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }

    public static bool isInRange(Vector3 origin, Vector3 target, float range)
    {
        return Vector3.Distance(origin, target) <= range;
    }
}
