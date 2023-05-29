using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectRadius : MonoBehaviour
{
    public EnemyBehaviour _enemy;

    void Awake()
    {
        _enemy = GetComponentInParent<EnemyBehaviour>();
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            _enemy._canMove = true;
            _enemy.StartShotCoroutine();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            _enemy._canMove = false;
            _enemy.StopShotCoroutine();
        }
    }
}
