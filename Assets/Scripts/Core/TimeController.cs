using UnityEngine;

namespace Spaceship
{
    /// <summary>
    /// Habilidade de desacelerar o tempo ("bullet time").
    /// Enquanto a tecla estiver pressionada o Time.timeScale cai e a energia
    /// e consumida; ao soltar (ou zerar a energia) o tempo volta ao normal
    /// e a energia recarrega sozinha.
    ///
    /// O jogador e os tiros do jogador usam tempo NAO escalado
    /// (Time.unscaledDeltaTime), entao continuam rapidos enquanto o mundo
    /// fica lento - e isso que da a sensacao de poder.
    /// </summary>
    public class TimeController : MonoBehaviour
    {
        public static TimeController Instance { get; private set; }

        [Header("Controle")]
        public KeyCode tecla = KeyCode.LeftShift;
        public KeyCode teclaAlternativa = KeyCode.Q;

        [Header("Ajustes")]
        [Range(0.05f, 0.9f)]
        [Tooltip("Velocidade do mundo enquanto a habilidade esta ativa (1 = normal).")]
        public float fatorLento = 0.3f;
        public float energiaMaxima = 100f;
        [Tooltip("Energia consumida por segundo enquanto ativo.")]
        public float consumoPorSegundo = 35f;
        [Tooltip("Energia recuperada por segundo quando desligado.")]
        public float recargaPorSegundo = 18f;
        [Tooltip("Energia minima necessaria para reativar depois de zerar.")]
        public float energiaMinimaParaAtivar = 12f;

        public float Energia { get; private set; }
        public bool Ativo { get; private set; }
        public float Percentual => energiaMaxima <= 0f ? 0f : Energia / energiaMaxima;

        private float fixedDeltaPadrao;
        private bool bloqueadoAteRecarregar;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            fixedDeltaPadrao = Time.fixedDeltaTime;
            Energia = energiaMaxima;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            Time.timeScale = 1f;
            Time.fixedDeltaTime = fixedDeltaPadrao > 0f ? fixedDeltaPadrao : 0.02f;
        }

        private void Update()
        {
            if (GameManager.Instance != null && !GameManager.Instance.EstaJogando)
            {
                Desligar();
                return;
            }

            bool segurando = Input.GetKey(tecla) || Input.GetKey(teclaAlternativa);

            if (bloqueadoAteRecarregar && Energia >= energiaMinimaParaAtivar)
                bloqueadoAteRecarregar = false;

            bool deveAtivar = segurando && !bloqueadoAteRecarregar && Energia > 0f;

            if (deveAtivar)
            {
                Energia -= consumoPorSegundo * Time.unscaledDeltaTime;
                if (Energia <= 0f)
                {
                    Energia = 0f;
                    deveAtivar = false;
                    bloqueadoAteRecarregar = true;
                }
            }
            else
            {
                Energia = Mathf.Min(energiaMaxima, Energia + recargaPorSegundo * Time.unscaledDeltaTime);
            }

            Aplicar(deveAtivar);
        }

        private void Aplicar(bool ativo)
        {
            Ativo = ativo;
            Time.timeScale = ativo ? fatorLento : 1f;
            Time.fixedDeltaTime = fixedDeltaPadrao * Time.timeScale;
        }

        /// <summary>Encerra a habilidade sem mexer no timeScale do fim de jogo.</summary>
        public void Desligar()
        {
            Ativo = false;
            Time.fixedDeltaTime = fixedDeltaPadrao > 0f ? fixedDeltaPadrao : 0.02f;
        }
    }
}
