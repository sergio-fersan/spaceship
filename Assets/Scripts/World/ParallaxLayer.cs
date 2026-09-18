using System.Collections.Generic;
using UnityEngine;

namespace Spaceship
{
    /// <summary>
    /// Camada de fundo com rolagem infinita (parallax).
    /// Cria automaticamente copias suficientes do sprite para cobrir a tela
    /// e reposiciona cada uma quando ela sai pela esquerda.
    ///
    /// Coloque este script em um GameObject VAZIO e informe o sprite.
    /// </summary>
    public class ParallaxLayer : MonoBehaviour
    {
        [Header("Visual")]
        public Sprite sprite;
        public Color cor = Color.white;
        [Tooltip("Ordem na camada. Use valores negativos para ficar atras de tudo.")]
        public int ordemNaCamada = -100;

        [Header("Rolagem")]
        [Tooltip("Velocidade em unidades por segundo (positivo = anda para a esquerda).")]
        public float velocidade = 1f;
        [Tooltip("Se ligado, a rolagem ignora a camera lenta.")]
        public bool ignorarTempoLento = false;

        [Header("Encaixe")]
        [Tooltip("Estica a camada para cobrir exatamente a altura da camera.")]
        public bool ajustarNaAltura = true;
        public float escalaManual = 1f;

        private readonly List<Transform> pedacos = new List<Transform>();
        private float larguraPedaco;

        private void Start()
        {
            if (sprite == null)
            {
                Debug.LogWarning("ParallaxLayer sem sprite.", this);
                enabled = false;
                return;
            }

            float escala = escalaManual;
            if (ajustarNaAltura && sprite.bounds.size.y > 0f)
                escala = ScreenBounds.Altura / sprite.bounds.size.y;

            larguraPedaco = sprite.bounds.size.x * escala;
            if (larguraPedaco <= 0.01f) larguraPedaco = 1f;

            int quantidade = Mathf.CeilToInt(ScreenBounds.Largura / larguraPedaco) + 2;

            for (int i = 0; i < quantidade; i++)
            {
                GameObject go = new GameObject("Pedaco_" + i);
                go.transform.SetParent(transform, false);
                go.transform.localScale = new Vector3(escala, escala, 1f);
                go.transform.position = new Vector3(
                    ScreenBounds.Esquerda - larguraPedaco * 0.5f + larguraPedaco * i,
                    transform.position.y,
                    transform.position.z);

                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;
                sr.color = cor;
                sr.sortingOrder = ordemNaCamada;

                pedacos.Add(go.transform);
            }
        }

        private void Update()
        {
            if (pedacos.Count == 0) return;

            float dt = ignorarTempoLento ? Time.unscaledDeltaTime : Time.deltaTime;
            float limite = ScreenBounds.Esquerda - larguraPedaco * 0.5f;
            float total = larguraPedaco * pedacos.Count;

            for (int i = 0; i < pedacos.Count; i++)
            {
                Transform t = pedacos[i];
                Vector3 p = t.position;
                p.x -= velocidade * dt;

                if (p.x < limite)
                    p.x += total;

                t.position = p;
            }
        }
    }
}
