using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] GameObject _enemyBullet;
    [SerializeField] LaserShot _laser;
    [SerializeField] float _shotIntervals = 5f;
    [SerializeField] public int _hp = 100;
    [SerializeField] Image _hpBar;
    [SerializeField] Image _AmmoBar;
    [SerializeField] private Transform _shotPos;

    [Header("Ammo & Fuel")]
    public int _currentAmmo;
    [SerializeField] float _fuel = 100;
    public float _maxAmmo = 10f;
    public bool _outOfAmmo = false;

    NavMeshAgent _agent; 

    public bool _canMove = false;

    private EnemyAI _enemyAI;
    private SupportShip[] _supportShip;
    private Transform _player;

    void Start()
    {
        _player = FindObjectOfType<Player>().transform;
        _enemyAI = GetComponent<EnemyAI>();
        _supportShip = FindObjectsOfType<SupportShip>();
        _currentAmmo = (int)_maxAmmo;
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void FixedUpdate()
    {
        if (_canMove)
        {
            RotateToPlayer();
        }
    }
    public void RotateToPlayer()
    {
        Vector3 playerPos = _player.transform.position;
        Vector3 dirToPlayer = playerPos - transform.position;
        dirToPlayer.z = 0.0f;
        transform.up = dirToPlayer;
    }

    public void Shoot()
    {
        if (_currentAmmo > 0)
        {
            Instantiate(_enemyBullet, _shotPos.position, transform.rotation);
            _currentAmmo--;
        }
        else if (_currentAmmo <= 0)
            _outOfAmmo = true;

        _AmmoBar.fillAmount = _currentAmmo / _maxAmmo;
    }

    IEnumerator ShootTimer()
    {
        while (true)
        {
            Shoot();
            yield return new WaitForSeconds(_shotIntervals);
        }
    }
    void ReceiveDamage(int damage)
    {
        _hp -= damage;
        _hpBar.fillAmount = _hp / 100f;
        if (_hp <= 0)
        {
            Destroy(gameObject);
            foreach (var support in _supportShip)
            {
                if (support._isRefilling)
                    support._isDead = true;
            }
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "LaserShot")
        {
            Destroy(other.gameObject);
            ReceiveDamage(_laser._damage);
        }
    }

    public void UpdateAmmoBar()
    {
        _AmmoBar.fillAmount = _currentAmmo / _maxAmmo;
    }

    public void StopShotCoroutine()
    {
        StopCoroutine("ShootTimer");
    }
    public void StartShotCoroutine()
    {
        StartCoroutine("ShootTimer");
    }

}
