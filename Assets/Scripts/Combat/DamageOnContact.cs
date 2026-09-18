using UnityEngine;

namespace Spaceship
{
    /// <summary>
    /// Causa dano ao encostar em alguem da equipe oposta
    /// (usado para a colisao corpo a corpo entre nave e inimigo).
    /// </summary>
    public class DamageOnContact : MonoBehaviour
    {
        public Equipe equipe = Equipe.Inimigo;
        public int dano = 1;
        [Tooltip("Destroi este objeto depois de causar o dano.")]
        public bool destruirAposContato = true;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (GameManager.Instance != null && !GameManager.Instance.EstaJogando) return;

            Health alvo = other.GetComponentInParent<Health>();
            if (alvo == null || alvo.equipe == equipe || alvo.Morto) return;

            int vidaAntes = alvo.Vida;
            alvo.LevarDano(dano);

            // So some se realmente machucou (evita sumir contra alvo invulneravel).
            if (destruirAposContato && alvo.Vida != vidaAntes)
                Destroy(gameObject);
        }
    }
}
