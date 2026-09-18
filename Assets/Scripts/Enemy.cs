using UnityEngine;

/// <summary>
/// Inimigo do exercicio. Anda para a esquerda (opcionalmente em onda),
/// vale pontos quando destruido e tira uma vida do jogador se encostar nele.
///
/// Usa Time.deltaTime, entao e afetado pela camera lenta - exatamente o que
/// o enunciado pede ("esse efeito deve diminuir a velocidade do fundo e os inimigos").
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 3f;

    [Tooltip("Altura do zigue-zague. Use 0 para andar em linha reta.")]
    public float waveAmplitude = 0f;

    public float waveFrequency = 2f;

    [Header("Combate")]
    public int health = 1;

    [Tooltip("Pontos que este inimigo vale.")]
    public int scoreValue = 10;

    private Camera cam;
    private float baseY;
    private float timeAlive;

    private void Start()
    {
        cam = Camera.main;
        baseY = transform.position.y;
    }

    private void Update()
    {
        timeAlive += Time.deltaTime;

        transform.position += Vector3.left * speed * Time.deltaTime;

        if (waveAmplitude > 0f)
        {
            Vector3 p = transform.position;
            p.y = baseY + Mathf.Sin(timeAlive * waveFrequency) * waveAmplitude;
            transform.position = p;
        }

        // Limpa inimigos que ja passaram da tela.
        if (cam != null)
        {
            float leftEdge = cam.transform.position.x - cam.orthographicSize * cam.aspect;
            if (transform.position.x < leftEdge - 2f)
            {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>REQUISITO 2 (Pontuacao): destruir inimigo soma pontos.</summary>
    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health > 0) return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;
        if (player.IsInvulnerable) return;

        player.TakeHit();
        Destroy(gameObject);
    }
}
