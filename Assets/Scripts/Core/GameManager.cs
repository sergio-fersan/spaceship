using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Spaceship
{
    public enum GameState { Jogando, Vitoria, Derrota }

    /// <summary>
    /// Cerebro do jogo: guarda a pontuacao, o estado atual e dispara
    /// as condicoes de vitoria e de derrota.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Condicao de vitoria")]
        [Tooltip("Pontuacao necessaria para vencer a fase.")]
        public int pontuacaoParaVencer = 1500;

        [Header("Debug")]
        [SerializeField] private GameState estadoAtual = GameState.Jogando;

        public GameState Estado => estadoAtual;
        public int Pontuacao { get; private set; }
        public bool EstaJogando => estadoAtual == GameState.Jogando;

        /// <summary>Disparado sempre que a pontuacao muda (pontuacao atual).</summary>
        public event Action<int> OnPontuacaoMudou;
        /// <summary>Disparado uma unica vez quando o jogo termina.</summary>
        public event Action<GameState> OnFimDeJogo;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            estadoAtual = GameState.Jogando;
            Time.timeScale = 1f;
        }

        private void Start()
        {
            OnPontuacaoMudou?.Invoke(Pontuacao);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void AdicionarPontos(int pontos)
        {
            if (!EstaJogando || pontos == 0) return;

            Pontuacao = Mathf.Max(0, Pontuacao + pontos);
            OnPontuacaoMudou?.Invoke(Pontuacao);

            if (Pontuacao >= pontuacaoParaVencer)
                Vencer();
        }

        public void Vencer() => Finalizar(GameState.Vitoria);

        public void Perder() => Finalizar(GameState.Derrota);

        private void Finalizar(GameState novoEstado)
        {
            if (!EstaJogando) return;

            estadoAtual = novoEstado;

            if (TimeController.Instance != null)
                TimeController.Instance.Desligar();

            // Congela a acao. A UI continua funcionando porque o uGUI
            // usa tempo nao escalado para os botoes.
            Time.timeScale = 0f;

            OnFimDeJogo?.Invoke(novoEstado);
        }

        public void Reiniciar()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Sair()
        {
            Time.timeScale = 1f;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
