using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace Spaceship
{
    /// <summary>
    /// Controla o HUD (pontuacao, vida, barra de tempo lento) e
    /// as telas de vitoria e de derrota.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RegistrarCarregamentoDeCena()
        {
            SceneManager.sceneLoaded += AoCarregarCena;
        }

        private static void AoCarregarCena(Scene cena, LoadSceneMode modo)
        {
            string cenaAtual = cena.name;
            if (cenaAtual != "Vitoria" && cenaAtual != "Derrota") return;
            if (FindObjectOfType<UIManager>() == null)
                new GameObject("UIManager - Tela Final").AddComponent<UIManager>();
        }

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

        [Header("Cenas finais")]
        public string cenaVitoria = "Vitoria";
        public string cenaDerrota = "Derrota";

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
            if (SceneManager.GetActiveScene().name == "Vitoria" || SceneManager.GetActiveScene().name == "Derrota")
            {
                CriarTelaFinal();
                return;
            }

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

        private void CriarTelaFinal()
        {
            GameObject canvasObject = new GameObject("Canvas - Tela Final");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();

            if (FindObjectOfType<EventSystem>() == null)
            {
                GameObject eventSystemObject = new GameObject("EventSystem");
                eventSystemObject.AddComponent<EventSystem>();
                eventSystemObject.AddComponent<StandaloneInputModule>();
            }

            Text textoResultado = CriarTexto(canvas.transform);
            RectTransform resultadoRect = textoResultado.rectTransform;
            resultadoRect.anchorMin = new Vector2(0.5f, 0.5f);
            resultadoRect.anchorMax = new Vector2(0.5f, 0.5f);
            resultadoRect.sizeDelta = new Vector2(700f, 140f);
            resultadoRect.anchoredPosition = new Vector2(0f, 80f);
            bool venceu = SceneManager.GetActiveScene().name == "Vitoria";
            int pontuacaoFinal = PlayerPrefs.GetInt("Spaceship.PontuacaoFinal", GameManager.PontuacaoFinal);
            textoResultado.text = venceu
                ? "VITORIA!\nPontuacao final: " + pontuacaoFinal
                : "DERROTA\nPontuacao final: " + pontuacaoFinal;
            textoResultado.alignment = TextAnchor.MiddleCenter;
            textoResultado.color = Color.white;

            GameObject botaoObject = new GameObject("Botao Reiniciar");
            botaoObject.transform.SetParent(canvas.transform, false);
            RectTransform retangulo = botaoObject.AddComponent<RectTransform>();
            retangulo.sizeDelta = new Vector2(220f, 60f);
            retangulo.anchoredPosition = new Vector2(0f, -100f);

            Image fundo = botaoObject.AddComponent<Image>();
            fundo.color = new Color(0.1f, 0.35f, 0.7f, 1f);
            Button botao = botaoObject.AddComponent<Button>();
            botao.onClick.AddListener(ReiniciarPartida);

            Text textoBotao = CriarTexto(botaoObject.transform);
            textoBotao.text = "REINICIAR";
            textoBotao.alignment = TextAnchor.MiddleCenter;
            textoBotao.color = Color.white;
        }

        private Text CriarTexto(Transform pai)
        {
            GameObject textoObject = new GameObject("Texto");
            textoObject.transform.SetParent(pai, false);
            RectTransform retangulo = textoObject.AddComponent<RectTransform>();
            retangulo.anchorMin = Vector2.zero;
            retangulo.anchorMax = Vector2.one;
            retangulo.offsetMin = Vector2.zero;
            retangulo.offsetMax = Vector2.zero;
            Text texto = textoObject.AddComponent<Text>();
            texto.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            texto.fontSize = 28;
            return texto;
        }

        private void ReiniciarPartida()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Game");
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
            string nomeCena = estado == GameState.Vitoria ? cenaVitoria : cenaDerrota;
            if (string.IsNullOrWhiteSpace(nomeCena)) return;

            Time.timeScale = 1f;
            SceneManager.LoadScene(nomeCena);
        }
    }
}
