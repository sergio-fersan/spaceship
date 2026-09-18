using System;
using UnityEngine;

namespace Spaceship
{
    /// <summary>Lado a que o objeto pertence. Evita depender de Tags.</summary>
    public enum Equipe { Jogador, Inimigo }

    /// <summary>
    /// Vida generica usada pelo jogador e pelos inimigos.
    /// </summary>
    public class Health : MonoBehaviour
    {
        [Header("Identificacao")]
        public Equipe equipe = Equipe.Inimigo;

        [Header("Vida")]
        public int vidaMaxima = 3;

        [Header("Invulnerabilidade")]
        [Tooltip("Tempo (em segundos) sem poder levar dano depois de ser atingido.")]
        public float tempoInvulneravel = 0f;
        [Tooltip("Piscar o sprite enquanto invulneravel.")]
        public bool piscarAoLevarDano = true;

        public int Vida { get; private set; }
        public bool Morto { get; private set; }
        public bool Invulneravel => tempoRestanteInvulneravel > 0f;
        public float PercentualVida => vidaMaxima <= 0 ? 0f : (float)Vida / vidaMaxima;

        public event Action<Health> OnDano;
        public event Action<Health> OnMorte;
        public event Action<Health> OnVidaMudou;

        private float tempoRestanteInvulneravel;
        private SpriteRenderer[] sprites;

        private void Awake()
        {
            Vida = vidaMaxima;
            sprites = GetComponentsInChildren<SpriteRenderer>();
        }

        private void Update()
        {
            if (tempoRestanteInvulneravel > 0f)
            {
                tempoRestanteInvulneravel -= UnityEngine.Time.unscaledDeltaTime;

                if (piscarAoLevarDano && sprites != null)
                {
                    bool visivel = Mathf.Repeat(UnityEngine.Time.unscaledTime, 0.16f) > 0.08f;
                    DefinirVisibilidade(visivel);
                }

                if (tempoRestanteInvulneravel <= 0f)
                    DefinirVisibilidade(true);
            }
        }

        private void DefinirVisibilidade(bool visivel)
        {
            if (sprites == null) return;
            for (int i = 0; i < sprites.Length; i++)
            {
                if (sprites[i] == null) continue;
                Color c = sprites[i].color;
                c.a = visivel ? 1f : 0.25f;
                sprites[i].color = c;
            }
        }

        public void LevarDano(int dano)
        {
            if (Morto || dano <= 0) return;
            if (Invulneravel) return;
            if (GameManager.Instance != null && !GameManager.Instance.EstaJogando) return;

            Vida = Mathf.Max(0, Vida - dano);
            OnDano?.Invoke(this);
            OnVidaMudou?.Invoke(this);

            if (Vida <= 0)
            {
                Morto = true;
                OnMorte?.Invoke(this);
            }
            else if (tempoInvulneravel > 0f)
            {
                tempoRestanteInvulneravel = tempoInvulneravel;
            }
        }

        /// <summary>
        /// Define a vida maxima em tempo de execucao (usado pelo spawner,
        /// porque o Awake ja rodou quando o objeto foi instanciado).
        /// </summary>
        public void DefinirVidaMaxima(int valor, bool curarTotal = true)
        {
            vidaMaxima = Mathf.Max(1, valor);
            Vida = curarTotal ? vidaMaxima : Mathf.Min(Vida, vidaMaxima);
            Morto = Vida <= 0;
            OnVidaMudou?.Invoke(this);
        }

        public void Curar(int quantidade)
        {
            if (Morto || quantidade <= 0) return;
            Vida = Mathf.Min(vidaMaxima, Vida + quantidade);
            OnVidaMudou?.Invoke(this);
        }
    }
}
