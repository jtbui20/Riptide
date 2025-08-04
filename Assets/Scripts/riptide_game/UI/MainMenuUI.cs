using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    // ! You can kill this script
    public void StartGame()
    {
        SceneFlowManager.GotoGame();
    }
}