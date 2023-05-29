using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShipBehaviour : MonoBehaviour
{
    private BehaviourTree tree;
    public GameObject player;

    NavMeshAgent agent;

    public enum ActionState { IDLE, WORKING };
    ActionState state = ActionState.IDLE;

    Node.Status treeStatus = Node.Status.RUNNING;
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        
        tree = new BehaviourTree();
        
        Sequence patrol = new Sequence("Patrol");
        Leaf chase = new Leaf("Chase Player", ChasePlayer);
        Leaf attack = new Leaf("Attack Player", null);

        patrol.AddChild(chase);
        patrol.AddChild(attack);
        tree.AddChild(patrol);

        tree.PrintTree();


    }
    

    public Node.Status ChasePlayer()
    {
        return GoToLocation(player.transform.position);
    }

    Node.Status GoToLocation(Vector3 destination)
    {
        float distanceToTarget = Vector3.Distance(destination, this.transform.position);
        if (state == ActionState.IDLE)
        {
            agent.SetDestination(destination);
            state = ActionState.WORKING;
        }
        else if (Vector3.Distance(agent.pathEndPosition, destination) >= 2)
        {
            state = ActionState.IDLE;
            return Node.Status.FAILURE;
        }
        else if (distanceToTarget < 2)
        {
            state = ActionState.IDLE;
            return Node.Status.SUCCESS;
        }
        return Node.Status.RUNNING;
    }

    void Update()
    {
        if(treeStatus != Node.Status.SUCCESS)
            treeStatus = tree.Process();
    }
}
