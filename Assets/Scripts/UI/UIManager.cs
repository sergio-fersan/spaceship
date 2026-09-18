using UnityEngine;
using UnityEngine.UI;

namespace Spaceship
{
    /// <summary>
    /// Controla o HUD (pontuacao, vida, barra de tempo lento) e
    /// as telas de vitoria e de derrota.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("HUD")]
        public Text textoPontuacao;
        public Text textoVida;
        public Text textoDica;
        public Image barraTempoLentoFundo;
        public Image barraTempoLentoPreenchimento;

        [Header("Telas finais")]
        public GameObject painelVitoria;
        public GameObject painelDerrota;
        public Text textoResultadoVitoria;
        public Text textoResultadoDerrota;

        [Header("Botoes")]
        public Button botaoReiniciarVitoria;
        public Button botaoSairVitoria;
        public Button botaoReiniciarDerrota;
        public Button botaoSairDerrota;

        [Header("Referencias")]
        public Health vidaDoJogador;

        [Header("Cores da barra")]
        public Color corBarraNormal = new Color(0.35f, 0.85f, 1f);
        public Color corBarraAtiva = new Color(1f, 0.85f, 0.3f);
        public Color corBarraVazia = new Color(0.9f, 0.35f, 0.35f);

        private void Start()
        {
            if (painelVitoria != null) painelVitoria.SetActive(false);
            if (painelDerrota != null) painelDerrota.SetActive(false);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPontuacaoMudou += AtualizarPontuacao;
                GameManager.Instance.OnFimDeJogo += MostrarTelaFinal;
                AtualizarPontuacao(GameManager.Instance.Pontuacao);
            }

            if (vidaDoJogador != null)
            {
                vidaDoJogador.OnVidaMudou += AtualizarVida;
                vidaDoJogador.OnMorte += AtualizarVida;
                AtualizarVida(vidaDoJogador);
            }

            LigarBotao(botaoReiniciarVitoria, true);
            LigarBotao(botaoReiniciarDerrota, true);
            LigarBotao(botaoSairVitoria, false);
            LigarBotao(botaoSairDerrota, false);

            if (textoDica != null)
                textoDica.text = "WASD / setas: mover    |    ESPACO: atirar    |    SHIFT: camera lenta";
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPontuacaoMudou -= AtualizarPontuacao;
                GameManager.Instance.OnFimDeJogo -= MostrarTelaFinal;
            }

            if (vidaDoJogador != null)
            {
                vidaDoJogador.OnVidaMudou -= AtualizarVida;
                vidaDoJogador.OnMorte -= AtualizarVida;
            }
        }

        private void LigarBotao(Button botao, bool reiniciar)
        {
            if (botao == null) return;
            botao.onClick.RemoveAllListeners();

            if (reiniciar)
                botao.onClick.AddListener(() => { if (GameManager.Instance != null) GameManager.Instance.Reiniciar(); });
            else
                botao.onClick.AddListener(() => { if (GameManager.Instance != null) GameManager.Instance.Sair(); });
        }

        private void Update()
        {
            AtualizarBarraTempo();

            // Atalho: R reinicia depois do fim de jogo.
            if (GameManager.Instance != null && !GameManager.Instance.EstaJogando && Input.GetKeyDown(KeyCode.R))
                GameManager.Instance.Reiniciar();
        }

        private void AtualizarBarraTempo()
        {
            if (barraTempoLentoPreenchimento == null) return;

            TimeController tc = TimeController.Instance;
            float p = tc != null ? tc.Percentual : 1f;

            barraTempoLentoPreenchimento.fillAmount = Mathf.Clamp01(p);

            if (tc != null && tc.Ativo) barraTempoLentoPreenchimento.color = corBarraAtiva;
            else if (p < 0.15f) barraTempoLentoPreenchimento.color = corBarraVazia;
            else barraTempoLentoPreenchimento.color = corBarraNormal;
        }

        private void AtualizarPontuacao(int pontuacao)
        {
            if (textoPontuacao == null) return;

            int meta = GameManager.Instance != null ? GameManager.Instance.pontuacaoParaVencer : 0;
            textoPontuacao.text = meta > 0
                ? string.Format("PONTOS: {0} / {1}", pontuacao, meta)
                : string.Format("PONTOS: {0}", pontuacao);
        }

        private void AtualizarVida(Health h)
        {
            if (textoVida == null || h == null) return;

            string coracoes = "";
            for (int i = 0; i < h.vidaMaxima; i++)
                coracoes += i < h.Vida ? "[#]" : "[ ]";

            textoVida.text = "NAVE: " + coracoes;
        }

        private void MostrarTelaFinal(GameState estado)
        {
            int pontos = GameManager.Instance != null ? GameManager.Instance.Pontuacao : 0;

            if (estado == GameState.Vitoria)
            {
                if (painelVitoria != null) painelVitoria.SetActive(true);
                if (textoResultadoVitoria != null)
                    textoResultadoVitoria.text = string.Format("Pontuacao final: {0}\nSetor limpo com sucesso!", pontos);
            }
            else
            {
                if (painelDerrota != null) painelDerrota.SetActive(true);
                if (textoResultadoDerrota != null)
                    textoResultadoDerrota.text = string.Format("Pontuacao final: {0}\nSua nave foi destruida.", pontos);
            }
        }
    }
}
