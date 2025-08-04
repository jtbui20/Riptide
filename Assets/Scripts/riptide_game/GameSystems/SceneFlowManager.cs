using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFlowManager : MonoBehaviour
{
    public static void GotoMenu()
    {
        SceneManager.LoadScene(0);
    }

    public static void GotoGame()
    {
        SceneManager.LoadScene(1);
    }
}
