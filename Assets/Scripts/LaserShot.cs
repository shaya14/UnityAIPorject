using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShot : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    public int _damage = 1;

    private float _lifeTime = 2f;
    private float _timer = 0f;

    Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {

    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= _lifeTime)
        {
            Destroy(gameObject);
            _timer = 0;
        }
    }

    void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        Vector3 movement = transform.position += transform.up * _speed * Time.deltaTime;
        _rb.MovePosition(movement);
    }
}
