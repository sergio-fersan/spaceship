using UnityEngine;

/// <summary>
/// Cria inimigos fora da borda direita da camera, em alturas aleatorias.
/// O intervalo entre eles diminui com o tempo, aumentando a dificuldade.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Inimigos")]
    public GameObject[] enemyPrefabs;

    [Header("Ritmo")]
    [Tooltip("Intervalo inicial entre inimigos, em segundos.")]
    public float spawnInterval = 1.3f;

    [Tooltip("Intervalo minimo que o jogo pode atingir.")]
    public float minInterval = 0.45f;

    [Tooltip("Quanto o intervalo diminui por segundo de jogo.")]
    public float difficultyRamp = 0.02f;

    [Header("Posicionamento")]
    [Tooltip("Margem vertical para os inimigos nao nascerem colados na borda.")]
    public float verticalMargin = 0.6f;

    [Tooltip("Distancia fora da tela onde o inimigo nasce.")]
    public float spawnDistance = 1.5f;

    private Camera cam;
    private float timer;
    private float currentInterval;

    private void Start()
    {
        cam = Camera.main;
        currentInterval = spawnInterval;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.State != GameState.Playing) return;
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;
        if (cam == null) return;

        // Time.deltaTime: na camera lenta os inimigos tambem nascem mais devagar.
        timer += Time.deltaTime;
        currentInterval = Mathf.Max(minInterval, currentInterval - difficultyRamp * Time.deltaTime);

        if (timer >= currentInterval)
        {
            timer = 0f;
            Spawn();
        }
    }

    private void Spawn()
    {
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;
        Vector3 c = cam.transform.position;

        float y = c.y + Random.Range(-halfHeight + verticalMargin, halfHeight - verticalMargin);
        float x = c.x + halfWidth + spawnDistance;

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Instantiate(prefab, new Vector3(x, y, 0f), Quaternion.identity);
    }
}
