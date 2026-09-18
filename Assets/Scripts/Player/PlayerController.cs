using UnityEngine;

namespace Spaceship
{
    /// <summary>
    /// Movimentacao da nave do jogador (WASD ou setas).
    /// Usa tempo NAO escalado para continuar agil durante a camera lenta.
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movimento")]
        public float velocidade = 8f;
        [Tooltip("Suavizacao da aceleracao (0 = resposta instantanea).")]
        [Range(0f, 0.4f)] public float suavizacao = 0.08f;
        [Tooltip("Margem em unidades para nao encostar na borda da tela.")]
        public float margemDaTela = 0.6f;

        [Header("Visual")]
        [Tooltip("Inclinacao do sprite ao subir/descer (graus).")]
        public float inclinacaoMaxima = 12f;

        private Health vida;
        private Vector2 velocidadeAtual;
        private Vector2 suavizacaoRef;

        private void Awake()
        {
            vida = GetComponent<Health>();
            vida.equipe = Equipe.Jogador;
            vida.OnMorte += AoMorrer;
        }

        private void OnDestroy()
        {
            if (vida != null) vida.OnMorte -= AoMorrer;
        }

        private void Update()
        {
            if (GameManager.Instance != null && !GameManager.Instance.EstaJogando) return;
            if (vida != null && vida.Morto) return;

            float dt = Time.unscaledDeltaTime;

            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            Vector2 alvo = new Vector2(x, y);
            if (alvo.sqrMagnitude > 1f) alvo.Normalize();
            alvo *= velocidade;

            if (suavizacao > 0.001f)
                velocidadeAtual = Vector2.SmoothDamp(velocidadeAtual, alvo, ref suavizacaoRef, suavizacao, Mathf.Infinity, dt);
            else
                velocidadeAtual = alvo;

            transform.position += (Vector3)(velocidadeAtual * dt);
            Limitar();
            Inclinar(y, dt);
        }

        private void Limitar()
        {
            Vector3 p = transform.position;
            p.x = Mathf.Clamp(p.x, ScreenBounds.Esquerda + margemDaTela, ScreenBounds.Direita - margemDaTela);
            p.y = Mathf.Clamp(p.y, ScreenBounds.Baixo + margemDaTela, ScreenBounds.Cima - margemDaTela);
            p.z = 0f;
            transform.position = p;
        }

        private void Inclinar(float entradaVertical, float dt)
        {
            if (inclinacaoMaxima <= 0f) return;
            float alvo = -entradaVertical * inclinacaoMaxima;
            float atual = Mathf.LerpAngle(transform.eulerAngles.z, alvo, 10f * dt);
            transform.rotation = Quaternion.Euler(0f, 0f, atual);
        }

        private void AoMorrer(Health h)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.Perder();
        }
    }
}
