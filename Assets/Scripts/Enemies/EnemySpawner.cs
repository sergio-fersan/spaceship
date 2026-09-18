using UnityEngine;

namespace Spaceship
{
    /// <summary>
    /// Gera inimigos na borda direita da tela, ficando mais dificil com o tempo.
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Prefab")]
        public Enemy prefabInimigo;

        [Header("Ritmo")]
        public float intervaloInicial = 1.6f;
        public float intervaloMinimo = 0.45f;
        [Tooltip("Quanto o intervalo diminui por segundo de jogo.")]
        public float aceleracaoDaDificuldade = 0.02f;
        public float atrasoInicial = 1.2f;

        [Header("Variacao dos inimigos")]
        public float velocidadeMinima = 2.5f;
        public float velocidadeMaxima = 5.5f;
        public int vidaMinima = 1;
        public int vidaMaxima = 3;
        [Tooltip("Chance (0 a 1) de o inimigo gerado poder atirar.")]
        [Range(0f, 1f)] public float chanceDeAtirar = 0.55f;
        public float margemVertical = 1.0f;

        [Header("Cores")]
        public Color[] paletaDeCores = new Color[]
        {
            new Color(1.00f, 0.55f, 0.45f),
            new Color(0.65f, 0.90f, 0.70f),
            new Color(0.75f, 0.70f, 1.00f),
            new Color(1.00f, 0.90f, 0.55f)
        };

        private float proximoSpawn;
        private float tempoDecorrido;

        private void Start()
        {
            proximoSpawn = Time.time + atrasoInicial;
        }

        private void Update()
        {
            if (GameManager.Instance != null && !GameManager.Instance.EstaJogando) return;
            if (prefabInimigo == null) return;

            tempoDecorrido += Time.deltaTime;

            if (Time.time < proximoSpawn) return;

            Gerar();

            float intervalo = Mathf.Max(intervaloMinimo, intervaloInicial - tempoDecorrido * aceleracaoDaDificuldade);
            proximoSpawn = Time.time + intervalo;
        }

        private void Gerar()
        {
            float y = Random.Range(ScreenBounds.Baixo + margemVertical, ScreenBounds.Cima - margemVertical);
            Vector3 pos = new Vector3(ScreenBounds.Direita + 1.5f, y, 0f);

            Enemy inimigo = Instantiate(prefabInimigo, pos, Quaternion.identity);

            inimigo.velocidade = Random.Range(velocidadeMinima, velocidadeMaxima);
            inimigo.podeAtirar = Random.value < chanceDeAtirar;
            inimigo.amplitudeOnda = Random.value < 0.6f ? Random.Range(0.5f, 1.6f) : 0f;

            Health vida = inimigo.GetComponent<Health>();
            if (vida != null)
            {
                int hp = Random.Range(vidaMinima, vidaMaxima + 1);
                vida.DefinirVidaMaxima(hp);
                // Inimigos mais resistentes valem mais pontos.
                inimigo.pontos = 60 + hp * 40;
            }

            if (paletaDeCores != null && paletaDeCores.Length > 0)
            {
                Color cor = paletaDeCores[Random.Range(0, paletaDeCores.Length)];
                foreach (SpriteRenderer sr in inimigo.GetComponentsInChildren<SpriteRenderer>())
                    sr.color = cor;
            }
        }
    }
}
