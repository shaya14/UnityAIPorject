using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public bool _canMove = true;
    public bool _isDead = false;
    public float _hp = 100f;
    
    [SerializeField] float _moveSpeed = 2;
    [SerializeField] private Transform _shotPos;
    [SerializeField] private GameObject _laserShot;
    [SerializeField] LaserShot _laser;
    [SerializeField] private Image _hpBar;
    
    private float _maxYPos = 47.5f;
    private float _maxXPos = 42f;
    
    Rigidbody2D _rb;

    public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ShotLaser();
        }
    }

    void FixedUpdate()
    {
        if (_canMove)
        {
            Movement();
        }    
        else if (!_canMove)
        {
            RotateToMouse();
        }
    }

    void Movement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10;
        Vector3 dirToMouse = (mousePos - transform.position).normalized;
        dirToMouse.z = 0f;

        Vector3 movement = transform.position + direction * _moveSpeed * Time.deltaTime;
        _rb.MovePosition(movement);

        transform.up = dirToMouse.normalized;

        float boundY = Mathf.Clamp(transform.position.y, -_maxYPos, _maxYPos);
        float boundX = Mathf.Clamp(transform.position.x, -_maxXPos, _maxXPos);
        transform.position = new Vector3(boundX, boundY, transform.position.z);
    }

    void RotateToMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10;
        Vector3 dirToMouse = (mousePos - transform.position).normalized;
        dirToMouse.z = 0f;
        transform.up = dirToMouse.normalized;
    }

    void ShotLaser()
    {
        GameObject newLaser = Instantiate(_laserShot, _shotPos.position, transform.rotation);
    }

    void ReceiveDamage(float damage)
    {
        _hp -= damage;
        _hpBar.fillAmount = _hp / 100f;
        if (_hp <= 0)
        {
            Destroy(gameObject);
            _isDead = true;
            GameManager._Instance.GameOver();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Rock")
        {
            Destroy(other.gameObject);
            GameManager._Instance.EnableAlpha(0);
        }
        if (other.gameObject.tag == "Wood")
        {
            Destroy(other.gameObject);
            GameManager._Instance.EnableAlpha(1);
        }
        if (other.gameObject.tag == "Gold")
        {
            Destroy(other.gameObject);
            GameManager._Instance.EnableAlpha(2);
        }
        if (other.gameObject.tag == "EnemyLaser")
        {
            ReceiveDamage(_laser._damage);
            Destroy(other.gameObject);
        }
    }
}
