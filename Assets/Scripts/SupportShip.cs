using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class SupportShip : MonoBehaviour
{
    public NavMeshAgent _agent;
    public LayerMask _whatIsPlayer, _whatIsEnemyRefill;

    [Header("Enemies")]
    public EnemyAI _enemyAI;
    public EnemyAI[] _enemyAIArray;
    private EnemyBehaviour _enemy;

    [Header("Attacking")]
    public float _timeBetweenAttacks;
    bool _alreadyAttacked;

    private Vector3 _zAxis = new Vector3(0, 0, 1f);

    [Header("Ranges")]
    public float _sightRange;
    public float _attackRange;
    public float _refillRange;

    [Header("Ammo")]
    public Image _ammoImage;
    public float _ammoCounter;
    public float _maxAmmo;

    [Header("Rotate Stats")]
    public float _speed = 0.15f;
    public Transform _target;
    private Vector3 _shipPosition;
    [SerializeField] private float _radius;

    [Header("Points")]
    public RadiusPoint _raduisPoint;
    [SerializeField] private Transform _stationPoint;

    [Header("AI States")]
    public bool _hasAmmoOnHim = true;
    public bool _isBusy = false;
    public bool _isDead = false;
    public bool _playerInSightRange, _playerInAttackRange, _enemyInRefillRange;
    private bool _isNeedRefill;

    private float _timer = 0;
    private float _timerEnd = 1;
    public bool _isRefilling = false;
    private EnemyAI _enemyBeingRefilled;
    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
        _enemy = GetComponent<EnemyBehaviour>();
        _enemyAI = FindObjectOfType<EnemyAI>();
        _enemyAIArray = FindObjectsOfType<EnemyAI>();
        _maxAmmo = _enemyAI.GetComponent<EnemyBehaviour>()._maxAmmo;
        _ammoCounter = _maxAmmo;
    }

    void Start()
    {

    }

    void Update()
    {

        //Check if player in sight attack range
        _playerInSightRange = _raduisPoint._playerInRadius;
        _playerInAttackRange = Physics2D.OverlapCircle(transform.position, _attackRange, _whatIsPlayer);
        _enemyInRefillRange = Physics2D.OverlapCircle(transform.position, _refillRange, _whatIsEnemyRefill);

        if (!_playerInSightRange && !_playerInAttackRange && !_isBusy)
        {
            _speed = 0.15f;
            RotateAround(_zAxis);
        }

        if (_playerInSightRange && !_playerInAttackRange && !_isBusy)
        {
            _speed = 0.3f;
            Chasing();
        }

        if (_playerInSightRange && _playerInAttackRange && !_isBusy)
        {
            Attacking();
        }

        foreach (var enemy in _enemyAIArray)
        {
            if (enemy != null && enemy.GetComponent<EnemyBehaviour>()._outOfAmmo && _hasAmmoOnHim && !_isBusy && enemy._refillShip == null)
            {
                _enemyAI = enemy;
                if (!_isRefilling)
                {
                    _enemyAI._refillShip = this;
                    _enemyAI._refillShip._isRefilling = true;
                }
            }
        }

        if (_enemyAI != null)
        {
            if (_enemyAI.GetComponent<EnemyBehaviour>()._outOfAmmo && _hasAmmoOnHim && _isRefilling)
            {
                _isBusy = true;
                GoToEmptyEnemy();
                if (_enemyInRefillRange)
                {
                    _timer += Time.deltaTime;
                    _agent.SetDestination(transform.position);
                    if (_timer > _timerEnd)
                    {
                        RefillEnemyAmmo();
                        _timer = 0;
                    }

                    if (_playerInAttackRange)
                    {
                        Attacking();
                    }
                }
            }
        }
        else if (_isDead)
        {
            _agent.SetDestination(_stationPoint.position);
            if (_ammoCounter >= _maxAmmo)
            {
                if (Vector3.Distance(transform.position, _stationPoint.transform.position) < 1)
                {
                    _isBusy = false;
                    _hasAmmoOnHim = true;
                    _isRefilling = false;
                    _isDead = false;
                }
            }
        }





        if (!_hasAmmoOnHim)
        {
            if (_ammoCounter >= _maxAmmo)
            {
                _isBusy = false;
                _hasAmmoOnHim = true;
            }
        }

        if (_enemy._currentAmmo <= 0)
        {
            if (_isNeedRefill)
            {
                _agent.enabled = true;
                _isBusy = true;
                _agent.SetDestination(_stationPoint.position);
                _isNeedRefill = false;
            }
        }
        else if (_enemy._currentAmmo >= _enemy._maxAmmo && !_isNeedRefill)
        {
            _isBusy = false;
            _agent.enabled = false;
            _isNeedRefill = true;
        }
    }


    void RotateAround(Vector3 axis)
    {
        _shipPosition = _radius * Vector3.Normalize(this.transform.position - _target.position) + _target.position;
        this.transform.position = _shipPosition;
        transform.RotateAround(_target.position, axis, _speed);
        _agent.enabled = false;
    }

    private void Chasing()
    {
        if (_raduisPoint._playerPos.x > this.transform.position.x)
            RotateAround(_zAxis);
        else if (_raduisPoint._playerPos.x < this.transform.position.x)
            RotateAround(-_zAxis);
        else if (_raduisPoint._playerPos.x > this.transform.position.x && _raduisPoint._playerPos.y < this.transform.position.y)
            RotateAround(_zAxis);
    }

    private void Attacking()
    {
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

    void GoToEmptyEnemy()
    {
        _agent.enabled = true;
        _agent.SetDestination(_enemyAI.transform.position);
        _agent.speed = 6f;
    }

    void RefillEnemyAmmo()
    {
        _enemyAI.GetComponent<EnemyBehaviour>()._currentAmmo++;
        _enemyAI._enemy.UpdateAmmoBar();

        _ammoCounter--;
        _ammoImage.fillAmount = _ammoCounter / _maxAmmo;

        if (_enemyAI.GetComponent<EnemyBehaviour>()._currentAmmo >= _maxAmmo && _hasAmmoOnHim)
        {
            _agent.SetDestination(_stationPoint.position);
            _enemyAI.GetComponent<EnemyBehaviour>()._outOfAmmo = false;
            _enemyAI.GetComponent<EnemyBehaviour>()._currentAmmo = (int)_maxAmmo;
            _ammoCounter = 0;
            _hasAmmoOnHim = false;
            _enemyAI._refillShip = null;
            _isRefilling = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _sightRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _refillRange);
    }
}
