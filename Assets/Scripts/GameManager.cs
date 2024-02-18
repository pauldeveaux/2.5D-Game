using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject retryUI;
    void Start()
    {
        //retryUI = GameObject.Find("RetryUI");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGameOver()
    {
        retryUI.SetActive(true);
    }

    public void Retry()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
