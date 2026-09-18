using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// REQUISITO 2 (Pontuacao na tela), REQUISITO 5 (Tela de derrota)
/// e REQUISITO 6 (Tela de vitoria).
///
/// Le o estado do GameManager a cada frame e atualiza a interface.
/// </summary>
public class HUDController : MonoBehaviour
{
    [Header("HUD")]
    public Text scoreText;
    public Text livesText;
    public Image slowMotionBar;
    public Text slowMotionLabel;

    [Header("Telas de fim de jogo")]
    public GameObject victoryPanel;
    public GameObject defeatPanel;
    public Text victoryScoreText;
    public Text defeatScoreText;

    [Header("Botoes")]
    public Button victoryRestartButton;
    public Button victoryQuitButton;
    public Button defeatRestartButton;
    public Button defeatQuitButton;

    [Header("Referencias")]
    public TimeController timeController;

    private void Start()
    {
        if (victoryRestartButton != null) victoryRestartButton.onClick.AddListener(Restart);
        if (defeatRestartButton != null) defeatRestartButton.onClick.AddListener(Restart);
        if (victoryQuitButton != null) victoryQuitButton.onClick.AddListener(Quit);
        if (defeatQuitButton != null) defeatQuitButton.onClick.AddListener(Quit);

        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (defeatPanel != null) defeatPanel.SetActive(false);
    }

    private void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return;

        if (scoreText != null)
        {
            scoreText.text = "PONTOS  " + gm.Score + " / " + gm.scoreToWin;
        }

        if (livesText != null)
        {
            livesText.text = "VIDAS  " + new string('*', Mathf.Max(0, gm.Lives));
        }

        if (timeController != null)
        {
            if (slowMotionBar != null)
            {
                slowMotionBar.fillAmount = timeController.EnergyNormalized;
                slowMotionBar.color = timeController.IsSlowMotion
                    ? new Color(0.45f, 0.85f, 1f, 1f)
                    : new Color(0.35f, 0.60f, 0.95f, 1f);
            }

            if (slowMotionLabel != null)
            {
                slowMotionLabel.text = timeController.IsSlowMotion
                    ? "CAMERA LENTA ATIVA"
                    : "SHIFT = CAMERA LENTA";
            }
        }

        bool victory = gm.State == GameState.Victory;
        bool defeat = gm.State == GameState.Defeat;

        if (victoryPanel != null && victoryPanel.activeSelf != victory)
        {
            victoryPanel.SetActive(victory);
        }

        if (defeatPanel != null && defeatPanel.activeSelf != defeat)
        {
            defeatPanel.SetActive(defeat);
        }

        if (victory && victoryScoreText != null)
        {
            victoryScoreText.text = "Pontuacao final: " + gm.Score;
        }

        if (defeat && defeatScoreText != null)
        {
            defeatScoreText.text = "Pontuacao final: " + gm.Score;
        }

        // Atalho de teclado para reiniciar.
        if ((victory || defeat) && Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }

    private void Restart()
    {
        if (GameManager.Instance != null) GameManager.Instance.Restart();
    }

    private void Quit()
    {
        if (GameManager.Instance != null) GameManager.Instance.QuitGame();
    }
}
