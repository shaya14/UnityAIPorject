using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements.Experimental;

public class PoliceShip : MonoBehaviour
{
    public NavMeshAgent _agent;
    public Transform[] _wayPoints;

    private bool _playerInSightRange;
    private bool _playerInStopRange;

    public LayerMask _whatIsPlayer;

    public float _sightRange;
    public float _stopRange;

    private Player _player;
    private EnemyBehaviour _enemy;

    public bool _callEveryOne = false;

    public GameObject _playerHolder;


    public GameObject _blueLight;
    public GameObject _redLight;
    public float _lightSpeed = 0.3f;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _player = FindObjectOfType<Player>().GetComponent<Player>();
        _enemy = GetComponent<EnemyBehaviour>();
    }
    void Start()
    {
        MoveToPoints(0);
        StartCoroutine(PoliceLights());
    }

    // Update is called once per frame
    void Update()
    {
        _playerInSightRange = Physics2D.OverlapCircle(transform.position, _sightRange, _whatIsPlayer);
        _playerInStopRange = Physics2D.OverlapCircle(transform.position + (_player.transform.localScale / 2), _stopRange, _whatIsPlayer);

        if (!_playerInSightRange && !_playerInStopRange)
        {
            Patroling();
            _lightSpeed = 0.3f;
        }
        else if (_playerInSightRange && !_playerInStopRange)
        {
            Chasing();
            _lightSpeed = 0.15f;
        }
        else if (_playerInSightRange && _playerInStopRange)
        {
            _lightSpeed = 0.05f;
            _agent.SetDestination(transform.position);
            _callEveryOne = true;
            _player._canMove = false;
            _playerHolder.SetActive(true);
        }
    }

    void OnDisable()
    {
        _player._canMove = true;
        _player.MoveSpeed = 8;
        _callEveryOne = false;
    }

    public void MoveToPoints(int num)
    {
        _agent.SetDestination(_wayPoints[num].position);
        Vector3 distanceToWalkPoint = transform.position - _wayPoints[num].position;
        Vector3 dirToPoint = _wayPoints[num].position - transform.position;
        transform.up = dirToPoint;
    }

    private IEnumerator PoliceLights()
    {
        while (true)
        {
            _blueLight.SetActive(true);
            _redLight.SetActive(false);
            yield return new WaitForSeconds(_lightSpeed);
            _redLight.SetActive(true);
            _blueLight.SetActive(false);
            yield return new WaitForSeconds(_lightSpeed);
        }
    }

    public void Patroling()
    {
        _agent.speed = 3;
        for (int i = 0; i < _wayPoints.Length; i++)
        {
            if (Vector3.Distance(transform.position, _wayPoints[i].position) < 0.5f)
            {
                MoveToPoints(i + 1);
            }
            // ^1 = _wayPoints.Length - 1 - last element in array
            else if (Vector3.Distance(transform.position, _wayPoints[^1].position) < 0.5f)
            {
                MoveToPoints(0);
            }
        }
    }

    private void Chasing()
    {
        _agent.SetDestination(_player.transform.position);
        _enemy.RotateToPlayer();
        _player.MoveSpeed = 3;
        _agent.speed = 6f;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _stopRange);
    }

}
