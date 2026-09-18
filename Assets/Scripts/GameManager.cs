using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Estados possiveis da partida.</summary>
public enum GameState
{
    Playing,
    Victory,
    Defeat
}

/// <summary>
/// Cerebro da partida: guarda a pontuacao, as vidas e decide quando o
/// jogador venceu ou perdeu. Qualquer script acessa por GameManager.Instance.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Regras da partida")]
    [Tooltip("Pontuacao necessaria para vencer o jogo.")]
    public int scoreToWin = 500;

    [Tooltip("Quantidade de vidas com que o jogador comeca.")]
    public int startingLives = 3;

    public GameState State { get; private set; }
    public int Score { get; private set; }
    public int Lives { get; private set; }

    private const float DefaultFixedDelta = 0.02f;

    private void Awake()
    {
        // Singleton simples: garante que exista apenas um GameManager na cena.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Score = 0;
        Lives = Mathf.Max(1, startingLives);
        State = GameState.Playing;

        // Se a partida anterior terminou congelada, devolve o tempo ao normal.
        Time.timeScale = 1f;
        Time.fixedDeltaTime = DefaultFixedDelta;
    }

    /// <summary>Soma pontos e verifica a condicao de vitoria.</summary>
    public void AddScore(int amount)
    {
        if (State != GameState.Playing) return;

        Score += amount;

        if (Score >= scoreToWin)
        {
            EndGame(GameState.Victory);
        }
    }

    /// <summary>Remove uma vida e verifica a condicao de derrota.</summary>
    public void LoseLife(int amount = 1)
    {
        if (State != GameState.Playing) return;

        Lives = Mathf.Max(0, Lives - amount);

        if (Lives <= 0)
        {
            EndGame(GameState.Defeat);
        }
    }

    private void EndGame(GameState newState)
    {
        State = newState;

        // Congela o jogo. A UI continua funcionando porque o Canvas
        // nao depende do Time.timeScale.
        Time.timeScale = 0f;
    }

    /// <summary>Recarrega a cena atual para jogar novamente.</summary>
    public void Restart()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = DefaultFixedDelta;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>Sai do jogo (ou para o Play Mode dentro do editor).</summary>
    public void QuitGame()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
