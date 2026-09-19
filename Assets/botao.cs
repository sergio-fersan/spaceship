using UnityEngine;
using UnityEngine.SceneManagement;

public class botao : MonoBehaviour
{
    public void NextScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene("Game");
    }
}
