using UnityEngine;

namespace Spaceship
{
    /// <summary>
    /// Animacao simples quadro a quadro (usada no fogo da turbina da nave).
    /// Evita precisar criar um Animator Controller na mao.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimator : MonoBehaviour
    {
        public Sprite[] quadros;
        public float quadrosPorSegundo = 12f;
        public bool emLoop = true;
        [Tooltip("Se ligado, a animacao ignora a camera lenta.")]
        public bool ignorarTempoLento = true;

        private SpriteRenderer sr;
        private float acumulado;
        private int indice;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (quadros == null || quadros.Length == 0 || quadrosPorSegundo <= 0f) return;

            acumulado += (ignorarTempoLento ? Time.unscaledDeltaTime : Time.deltaTime) * quadrosPorSegundo;

            while (acumulado >= 1f)
            {
                acumulado -= 1f;
                indice++;

                if (indice >= quadros.Length)
                {
                    if (emLoop) indice = 0;
                    else { indice = quadros.Length - 1; enabled = false; }
                }
            }

            if (quadros[indice] != null)
                sr.sprite = quadros[indice];
        }
    }
}
