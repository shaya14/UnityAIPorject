using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiusPoint : MonoBehaviour
{
    public bool _playerInRadius;
    public LayerMask _whatIsPlayer;
    public float _sightRange;

    public Vector3 _playerPos;

    public SupportShip _supportShip;

    public float _timer = 0;

    public float _maxTime = 1;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Collider2D[] colliderArray = Physics2D.OverlapCircleAll(transform.position, _sightRange, _whatIsPlayer);
        foreach (Collider2D col in colliderArray)
        {
            if (col.TryGetComponent<Player>(out Player player))
            {
                _playerPos = col.transform.position;
                _playerInRadius = col;
            }
        }

        _playerInRadius = Physics2D.OverlapCircle(transform.position, _sightRange, _whatIsPlayer);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _sightRange);
    }
}
