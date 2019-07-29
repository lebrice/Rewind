using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Static instance of GameManager which allows it to be accessed by any other script.
    public static GameManager Instance { get; private set; }
    void Awake()
    {
        // setup and ensure singleton.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitGame();
        }
    }

    void InitGame()
    {
    }

   
}
