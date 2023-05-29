using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public EnemyBehaviour _enemy;

    public NavMeshAgent _agent;

    public Transform _player;

    public LayerMask _whatIsGround, _whatIsPlayer;

    //Patroling
    [Header("Patroling")]
    public Vector3 _walkPoint;
    public bool _walkPointSet;
    public float _walkPointRange;

    [Header("Movemen time")]
    public float _stopTime;
    public float _resetTime;
    private float _timer = 0;

    //Attaking
    [Header("Attacking")]
    public float _timeBetweenAttacks;
    bool _alreadyAttacked;

    //States
    [Header("Ranges")]
    public float _sightRange;
    public float _attackRange;
    public bool _playerInSightRange, _playerInAttackRange;

    private bool _canMove = true;
    public bool _onRefill = false;

    public float patrolRadius = 3f;
    private Vector3 _initialPosition;

    public bool _isBeingRefilled = false;
    public SupportShip _refillShip = null;

    void Awake()
    {
        _player = GameObject.Find("Player").transform;
        _agent = GetComponent<NavMeshAgent>();
        _enemy = GetComponent<EnemyBehaviour>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }
    void Start()
    {
        _initialPosition = transform.position;
    }

    void OnDestroy()
    {
        if (_refillShip != null)
        {
            _refillShip._enemyAI = null;
            if (_refillShip._isDead)
            {
                print("Support ship is dead");
                _refillShip = null;
            }
           // _refillShip._isBusy = false;
        }
    }

    void Update()
    {
        //Check if player in sight attack range
        _playerInSightRange = Physics2D.OverlapCircle(transform.position, _sightRange, _whatIsPlayer);
        _playerInAttackRange = Physics2D.OverlapCircle(transform.position, _attackRange, _whatIsPlayer);

        if (_timer >= _resetTime)
        {
            _timer = 0;
        }

        if (!_playerInSightRange && !_playerInAttackRange && _canMove)
        {
            _timer += Time.deltaTime;
            if (_timer < _stopTime)
            {
                Patroling();
            }
        }

        if (_playerInSightRange && !_playerInAttackRange && _canMove)
        {
            Chasing();
        }

        if (_playerInSightRange && _playerInAttackRange && _canMove)
        {
            Attacking();
        }

        if (_enemy._outOfAmmo)
        {
            _canMove = false;
            _agent.SetDestination(transform.position);
        }
        else
            _canMove = true;
    }

    private void Patroling()
    {
        if (!_walkPointSet)
        {
            int maxAttempts = 5;
            int attempts = 0;

            while (!_walkPointSet && attempts < maxAttempts)
            {
                SearchWalkPoint();
                attempts++;
            }
        }

        if (_walkPointSet)
        {
            Vector3 distanceToWalkPoint = transform.position - _walkPoint;
            Vector3 dirToPoint = _walkPoint - transform.position;
            transform.up = dirToPoint;

            if (distanceToWalkPoint.magnitude < 1f)
            {
                _walkPointSet = false;
            }
            else
            {
                _agent.SetDestination(_walkPoint);
            }
        }
    }

    private void SearchWalkPoint()
    {
        // Calculate random point within patrol radius
        float randomX = Random.Range(-patrolRadius, patrolRadius);
        float randomY = Random.Range(-patrolRadius, patrolRadius);
        Vector3 randomOffset = new Vector3(randomX, randomY, transform.position.z);
        _walkPoint = _initialPosition + randomOffset;

        _walkPointSet = true;
    }


    private void Chasing()
    {
        _agent.SetDestination(_player.position);
        _enemy.RotateToPlayer();
    }
    private void Attacking()
    {
        //Make sure enemy doesn't move
        _agent.SetDestination(transform.position);

        _enemy.RotateToPlayer();

        if (!_alreadyAttacked)
        {
            //Attack code here
            _enemy.Shoot();
            _alreadyAttacked = true;
            Invoke(nameof(ResetAttack), _timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        _alreadyAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _sightRange);      
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(patrolRadius * 2, patrolRadius * 2, 0f));
    }
}
