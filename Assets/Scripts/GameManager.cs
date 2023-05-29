using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager _Instance;
    public Image[] _image;
    public int _winState;
    public GameObject _winScreen;
    public GameObject _loseScreen;


    private Player _player;
    private EnemyBehaviour _enemy;
     void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }

        _player = FindObjectOfType<Player>();
        _enemy = FindObjectOfType<EnemyBehaviour>();
    }
    void Start()
    {
 
    }
    
    void Update()
    {
        if (_winState == 3)
        {
            _player._canMove = false;
            ShowWinScreen();
        }
    }

    public void EnableAlpha(int newImage)
    {
        for (int i = 0; i < _image.Length; i++)
        {
            if (newImage == i)
            {
                _image[i].color = new Color(255, 255, 255, 255);
                _winState++;
            }
            else if (_winState == 3)
            {
                Destroy(_image[0]);
                Destroy(_image[1]);
                Destroy(_image[2]);
            }
                
        }
    }

    public void GameOver()
    {
        _loseScreen.SetActive(true);
        _enemy._canMove = false;
        _enemy.StopShotCoroutine();
    }

    public void Restart()
    {
        SceneManager.LoadScene("Game");
    }

    void ShowWinScreen()
    {
        _winScreen.SetActive(true);
    }
}
