using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayGame() {
       SceneManager.LoadSceneAsync(1);
    }
}
