using UnityEngine;

public enum GameState
{
    PAUSE,
    PLAYING,
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public GameState gameState { get; private set; }

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;
        gameState = GameState.PLAYING;
        GameDataManager.InitEntityData();
        GameDataManager.input.Enable();
    }

    private void OnDestroy()
    {
        
    }
}