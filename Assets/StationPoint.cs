using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationPoint : MonoBehaviour
{
    public string _tagName;
    public SupportShip _supportShip;
    public float _timer;
    float _timeAdd = 1;
    void Start()
    {

    }

    void Update()
    {
        _timer += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == _tagName)
        {
            _timer = 0;
        }
        
    }
    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == _tagName)
        {
            // Check if the current ammo to give is less the max
            if (_supportShip._ammoCounter < _supportShip._maxAmmo)
            {
                for (int i = 0; i < _supportShip._maxAmmo; i++)
                {
                    if (_timer > _timeAdd)
                    {
                        _supportShip._ammoCounter++;
                        _supportShip._ammoImage.fillAmount = _supportShip._ammoCounter / _supportShip._maxAmmo;
                        _timer = 0;
                    }
                }
            }

            // Check if the current shooting ammo is less then max and refill it
            if (_supportShip.GetComponent<EnemyBehaviour>()._currentAmmo < _supportShip.GetComponent<EnemyBehaviour>()._maxAmmo)
            {
                for (int i = 0; i < _supportShip._maxAmmo; i++)
                {
                    if (_timer > _timeAdd)
                    {
                        _supportShip.GetComponent<EnemyBehaviour>()._currentAmmo++;
                        _supportShip.GetComponent<EnemyBehaviour>().UpdateAmmoBar();
                        _timer = 0;
                    }
                }
            }
        }
    }
}
