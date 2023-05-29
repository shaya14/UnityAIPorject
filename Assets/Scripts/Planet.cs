using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Planet : MonoBehaviour
{
    [SerializeField] private float _maxHp;
    [SerializeField] Image _hpBar;
    [SerializeField] LaserShot _laserShot;
    [SerializeField] private GameObject _collectAble;
    [SerializeField] private float _rotateSpeed;
    
    private Transform _plantTrans;
    private Vector3 _origPos;
    private RectTransform _hpBarTrans;
    private Vector3 _hpOrigTrans;

    private float _curHp;
    private float _fillHp = 0;
    private float _shakeRange = 0.05f;
    private float _shakeTime = 0.5f;
    void Awake()
    {
        _curHp = _maxHp;
        _plantTrans = this.transform;
        _origPos = _plantTrans.position;
        _hpBarTrans = _hpBar.GetComponent<RectTransform>();
        _hpOrigTrans = _hpBarTrans.position;
    }
    void Start()
    {
        
    }

    void Update()
    {
        RotatePlanet(this.transform);
        RotatePlanet(_hpBar.transform);
    }

    void ReciveDamage()
    {
        _curHp -= _laserShot._damage;
        _fillHp += _laserShot._damage;
        _hpBar.fillAmount = _fillHp / _maxHp;
        StartCoroutine(GetHit());
        if (_curHp <= 0)
        {
            Destroy(gameObject);
            Destroy(_hpBar.gameObject);
            InstantiateCollectAble();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "LaserShot")
        {
            ReciveDamage();
            Destroy(other.gameObject);
        }
    }

    IEnumerator GetHit()
    {
        float timer = 0;
        while (timer < _shakeTime)
        {
            // Shake planet object
            Vector3 randomPos = _origPos + Random.insideUnitSphere * _shakeRange;
            randomPos.z = _origPos.z;
            _plantTrans.position = randomPos;

            // Shake hp bar
            randomPos.z = _hpOrigTrans.z;
            _hpBarTrans.position = randomPos;

            timer += Time.deltaTime;
            yield return null;
        }
        _plantTrans.position = _origPos;
        _hpBarTrans.position = _hpOrigTrans;
    }

    void InstantiateCollectAble()
    {
        Instantiate(_collectAble, transform.position, Quaternion.identity);
    }

    void RotatePlanet(Transform transform)
    {
        transform.Rotate(0, 0, 1 * Time.deltaTime * _rotateSpeed);
    }
}
