using UnityEngine;

/// <summary>
/// REQUISITO 1 (Movimentacao) e REQUISITO 3 (Tiros da nave).
///
/// O jogador usa Time.unscaledDeltaTime de proposito: assim ele continua
/// rapido enquanto o resto do mundo esta em camera lenta, que e exatamente
/// a "vantagem" pedida no enunciado.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimentacao")]
    [Tooltip("Velocidade em unidades por segundo.")]
    public float speed = 7f;

    [Header("Tiro")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Tooltip("Tiros por segundo.")]
    public float fireRate = 6f;

    public KeyCode fireKey = KeyCode.Space;

    [Header("Dano")]
    [Tooltip("Tempo de invencibilidade depois de levar um dano.")]
    public float invulnerabilityTime = 1.5f;

    private Camera cam;
    private SpriteRenderer spriteRenderer;
    private Vector2 halfSize;
    private float nextFireTime;
    private float invulnerableUntil;

    /// <summary>Verdadeiro logo depois de tomar dano (piscando).</summary>
    public bool IsInvulnerable
    {
        get { return Time.unscaledTime < invulnerableUntil; }
    }

    private void Start()
    {
        cam = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        halfSize = spriteRenderer.bounds.extents;
    }

    private void Update()
    {
        // Se o jogo acabou (vitoria ou derrota), a nave para.
        if (GameManager.Instance != null && GameManager.Instance.State != GameState.Playing)
        {
            return;
        }

        float dt = Time.unscaledDeltaTime;

        Move(dt);
        HandleShooting();
        UpdateBlink();
    }

    private void Move(float dt)
    {
        // Setas ou WASD (configurado no Input Manager padrao da Unity).
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(h, v, 0f);
        if (direction.sqrMagnitude > 1f) direction.Normalize();

        transform.position += direction * speed * dt;

        // Mantem a nave dentro da area visivel da camera.
        if (cam == null) return;

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;
        Vector3 c = cam.transform.position;

        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, c.x - halfWidth + halfSize.x, c.x + halfWidth - halfSize.x);
        p.y = Mathf.Clamp(p.y, c.y - halfHeight + halfSize.y, c.y + halfHeight - halfSize.y);
        p.z = 0f;
        transform.position = p;
    }

    private void HandleShooting()
    {
        if (projectilePrefab == null) return;

        bool pressed = Input.GetKey(fireKey) || Input.GetMouseButton(0);
        if (!pressed) return;

        // unscaledTime para que a cadencia de tiro nao caia na camera lenta.
        if (Time.unscaledTime < nextFireTime) return;

        nextFireTime = Time.unscaledTime + 1f / Mathf.Max(0.01f, fireRate);

        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
        Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
    }

    /// <summary>Chamado pelo inimigo quando encosta na nave.</summary>
    public void TakeHit()
    {
        if (IsInvulnerable) return;

        invulnerableUntil = Time.unscaledTime + invulnerabilityTime;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoseLife();
        }
    }

    private void UpdateBlink()
    {
        if (spriteRenderer == null) return;

        Color c = spriteRenderer.color;
        if (IsInvulnerable)
        {
            c.a = Mathf.PingPong(Time.unscaledTime * 10f, 1f) > 0.5f ? 0.3f : 1f;
        }
        else
        {
            c.a = 1f;
        }
        spriteRenderer.color = c;
    }
}
