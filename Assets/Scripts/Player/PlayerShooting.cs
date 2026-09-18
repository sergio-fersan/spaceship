using UnityEngine;

namespace Spaceship
{
    /// <summary>
    /// Tiro da nave. Espaco / Ctrl esquerdo / botao esquerdo do mouse.
    /// Tambem usa tempo nao escalado: durante a camera lenta o jogador
    /// continua atirando na cadencia normal.
    /// </summary>
    public class PlayerShooting : MonoBehaviour
    {
        [Header("Projetil")]
        public Projectile prefabTiro;
        [Tooltip("Ponto de saida do tiro. Se vazio, usa a posicao da nave.")]
        public Transform canoDaArma;

        [Header("Cadencia")]
        [Tooltip("Tiros por segundo.")]
        public float tirosPorSegundo = 5f;
        public bool tiroAutomatico = true;

        [Header("Disparo")]
        public float velocidadeDoTiro = 20f;
        public int danoDoTiro = 1;
        [Tooltip("Quantidade de canos (1 = simples, 2 = duplo, 3 = triplo).")]
        [Range(1, 3)] public int quantidadeDeTiros = 1;
        [Tooltip("Distancia vertical entre tiros quando ha mais de um.")]
        public float espacamento = 0.25f;

        private float proximoTiro;

        private void Update()
        {
            if (GameManager.Instance != null && !GameManager.Instance.EstaJogando) return;

            bool querAtirar = tiroAutomatico
                ? (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftControl) || Input.GetMouseButton(0))
                : (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetMouseButtonDown(0));

            if (!querAtirar) return;
            if (Time.unscaledTime < proximoTiro) return;

            Atirar();
            proximoTiro = Time.unscaledTime + (tirosPorSegundo > 0f ? 1f / tirosPorSegundo : 0.2f);
        }

        private void Atirar()
        {
            if (prefabTiro == null)
            {
                Debug.LogWarning("PlayerShooting: prefab do tiro nao atribuido.", this);
                return;
            }

            Vector3 origem = canoDaArma != null ? canoDaArma.position : transform.position;

            for (int i = 0; i < quantidadeDeTiros; i++)
            {
                float deslocamento = (i - (quantidadeDeTiros - 1) * 0.5f) * espacamento;
                Vector3 pos = origem + new Vector3(0f, deslocamento, 0f);

                Projectile tiro = Instantiate(prefabTiro, pos, Quaternion.identity);
                tiro.equipeDoDono = Equipe.Jogador;
                tiro.direcao = Vector2.right;
                tiro.velocidade = velocidadeDoTiro;
                tiro.dano = danoDoTiro;
                tiro.ignorarTempoLento = true;
            }
        }
    }
}
