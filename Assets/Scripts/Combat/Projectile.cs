using UnityEngine;

namespace Spaceship
{
    /// <summary>
    /// Tiro. Anda em linha reta, causa dano em quem for da equipe oposta
    /// e se destroi ao sair da tela ou ao acabar o tempo de vida.
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        [Header("Dono")]
        public Equipe equipeDoDono = Equipe.Jogador;

        [Header("Movimento")]
        public Vector2 direcao = Vector2.right;
        public float velocidade = 18f;
        [Tooltip("Se ligado, o tiro ignora a camera lenta (usado nos tiros do jogador).")]
        public bool ignorarTempoLento = false;

        [Header("Dano")]
        public int dano = 1;
        public float tempoDeVida = 4f;

        private float nascimento;

        private void OnEnable()
        {
            nascimento = Time.unscaledTime;
        }

        private void Update()
        {
            float dt = ignorarTempoLento ? Time.unscaledDeltaTime : Time.deltaTime;

            if (GameManager.Instance != null && !GameManager.Instance.EstaJogando)
                return;

            transform.Translate(direcao.normalized * velocidade * dt, Space.World);

            if (Time.unscaledTime - nascimento > tempoDeVida || ForaDaTela())
                Destroy(gameObject);
        }

        private bool ForaDaTela()
        {
            float margem = 2f;
            Vector3 p = transform.position;
            return p.x < ScreenBounds.Esquerda - margem || p.x > ScreenBounds.Direita + margem
                || p.y < ScreenBounds.Baixo - margem || p.y > ScreenBounds.Cima + margem;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Health alvo = other.GetComponentInParent<Health>();
            if (alvo == null) return;
            if (alvo.equipe == equipeDoDono) return;
            if (alvo.Morto) return;

            alvo.LevarDano(dano);
            Destroy(gameObject);
        }
    }
}
