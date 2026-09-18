using UnityEngine;

namespace Spaceship
{
    /// <summary>
    /// Inimigo padrao: entra pela direita, anda para a esquerda em zigue-zague
    /// e dispara de tempos em tempos. Ao morrer da pontuacao ao jogador.
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class Enemy : MonoBehaviour
    {
        [Header("Movimento")]
        public float velocidade = 3.5f;
        [Tooltip("Amplitude do zigue-zague vertical (0 = linha reta).")]
        public float amplitudeOnda = 1.2f;
        public float frequenciaOnda = 1.5f;

        [Header("Pontuacao")]
        public int pontos = 100;
        [Tooltip("Pontos perdidos se o inimigo escapar pela esquerda.")]
        public int penalidadeAoEscapar = 0;

        [Header("Tiro")]
        public Projectile prefabTiro;
        public bool podeAtirar = true;
        public float intervaloMinimo = 1.4f;
        public float intervaloMaximo = 3.2f;
        public float velocidadeDoTiro = 7f;
        public int danoDoTiro = 1;

        private Health vida;
        private float yInicial;
        private float faseOnda;
        private float proximoTiro;

        private void Awake()
        {
            vida = GetComponent<Health>();
            vida.equipe = Equipe.Inimigo;
            vida.OnMorte += AoMorrer;
        }

        private void OnDestroy()
        {
            if (vida != null) vida.OnMorte -= AoMorrer;
        }

        private void Start()
        {
            yInicial = transform.position.y;
            faseOnda = Random.Range(0f, Mathf.PI * 2f);
            AgendarProximoTiro();
        }

        private void Update()
        {
            if (GameManager.Instance != null && !GameManager.Instance.EstaJogando) return;

            float dt = Time.deltaTime; // afetado pela camera lenta, de proposito

            Vector3 p = transform.position;
            p.x -= velocidade * dt;

            if (amplitudeOnda > 0f)
            {
                faseOnda += frequenciaOnda * dt;
                p.y = yInicial + Mathf.Sin(faseOnda) * amplitudeOnda;
            }

            transform.position = p;

            if (podeAtirar && prefabTiro != null && Time.time >= proximoTiro)
            {
                Atirar();
                AgendarProximoTiro();
            }

            if (p.x < ScreenBounds.Esquerda - 2f)
            {
                if (penalidadeAoEscapar > 0 && GameManager.Instance != null)
                    GameManager.Instance.AdicionarPontos(-penalidadeAoEscapar);
                Destroy(gameObject);
            }
        }

        private void AgendarProximoTiro()
        {
            proximoTiro = Time.time + Random.Range(intervaloMinimo, intervaloMaximo);
        }

        private void Atirar()
        {
            // Nao atira fora da tela.
            if (transform.position.x > ScreenBounds.Direita) return;

            Projectile tiro = Instantiate(prefabTiro, transform.position + Vector3.left * 0.6f, Quaternion.identity);
            tiro.equipeDoDono = Equipe.Inimigo;
            tiro.direcao = Vector2.left;
            tiro.velocidade = velocidadeDoTiro;
            tiro.dano = danoDoTiro;
            tiro.ignorarTempoLento = false;
        }

        private void AoMorrer(Health h)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.AdicionarPontos(pontos);

            Destroy(gameObject);
        }
    }
}
