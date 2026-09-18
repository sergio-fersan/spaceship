using UnityEngine;

/// <summary>
/// REQUISITO 3 (Tiros da nave). O tiro anda sempre para a direita,
/// destroi inimigos e se autodestroi ao sair da tela.
/// </summary>
public class Projectile : MonoBehaviour
{
    public float speed = 14f;
    public int damage = 1;

    [Tooltip("Tempo maximo de vida, como rede de seguranca.")]
    public float lifeTime = 4f;

    [Tooltip("Se verdadeiro, o tiro do jogador ignora a camera lenta.")]
    public bool ignoreSlowMotion = true;

    private Camera cam;
    private float spawnTime;

    private void Start()
    {
        cam = Camera.main;
        spawnTime = Time.unscaledTime;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.State != GameState.Playing)
        {
            return;
        }

        float dt = ignoreSlowMotion ? Time.unscaledDeltaTime : Time.deltaTime;
        transform.position += Vector3.right * speed * dt;

        if (Time.unscaledTime - spawnTime > lifeTime)
        {
            Destroy(gameObject);
            return;
        }

        if (cam != null)
        {
            float rightEdge = cam.transform.position.x + cam.orthographicSize * cam.aspect;
            if (transform.position.x > rightEdge + 1f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null) return;   // ignora qualquer coisa que nao seja inimigo

        enemy.TakeDamage(damage);
        Destroy(gameObject);
    }
}
