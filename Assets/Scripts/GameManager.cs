using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject gameOverScreen;
    public KillerDollAI killerDoll;
    private PlayerCam playerCam;
    private bool gameOverShown;

    void Start()
    {
        gameOverScreen.SetActive(false);
        killerDoll = GameObject.FindGameObjectWithTag("TheDoll").GetComponent<KillerDollAI>();
        playerCam = FindFirstObjectByType<PlayerCam>();
    }

    void Update()
    {
        if (killerDoll.isPlayerDead && !gameOverShown)
        {
            ShowGameOver();
        }
    }

    void ShowGameOver()
    {
        gameOverShown = true;
        gameOverScreen.SetActive(true);

        if (playerCam != null)
        {
            playerCam.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Restart()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene("FinalScene");
    }
}
