using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject _player;
    public float offset;
    public float offsetSmoothing;
    private Vector3 _playerPosition;

    void Start()
    {

    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        _playerPosition = transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y, transform.position.z);
        if (_player.transform.localScale.x > 0f)
        {
            _playerPosition = new Vector3(_playerPosition.x + offset, _playerPosition.y, _playerPosition.z);
        }
        else
        {
            _playerPosition = new Vector3(_playerPosition.x - offset, _playerPosition.y, _playerPosition.z);
        }

        transform.position = Vector3.Lerp(transform.position, _playerPosition, offsetSmoothing * Time.deltaTime);

    }
}
