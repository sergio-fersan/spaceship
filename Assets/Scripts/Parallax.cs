using UnityEngine;

/// <summary>
/// Parallax Scrolling (slides 8 a 11 da aula).
/// Cada camada anda para a esquerda em uma velocidade diferente. Quando um
/// pedaco sai completamente pela esquerda da camera, ele volta para o fim da
/// fila, criando um fundo infinito.
///
/// Versao do slide:
///     transform.position += Vector3.left * Time.deltaTime * parallaxEffect;
///     if (transform.position.x < -lenght) transform.position = new Vector3(lenght, y, z);
///
/// Aqui a mesma ideia foi generalizada para funcionar com qualquer numero de
/// pedacos e com qualquer proporcao de tela (16:9, 4:3 etc.), somando o
/// deslocamento em vez de fixar a posicao (evita as "frestas" entre imagens).
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Parallax : MonoBehaviour
{
    [Tooltip("Velocidade da camada. Valores menores parecem mais distantes.")]
    [Range(0f, 10f)]
    public float parallaxEffect = 1f;

    [Tooltip("Quantos pedacos iguais existem nesta camada.")]
    public int totalTiles = 4;

    private float length;   // largura do sprite em unidades do mundo
    private Camera cam;

    private void Start()
    {
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        cam = Camera.main;
    }

    private void Update()
    {
        // Time.deltaTime e afetado pelo Time.timeScale, entao o fundo
        // automaticamente desacelera quando a camera lenta e ativada.
        transform.position += Vector3.left * parallaxEffect * Time.deltaTime;

        if (cam == null) return;

        float leftEdge = cam.transform.position.x - cam.orthographicSize * cam.aspect;

        // O pivo do sprite fica no centro, por isso o "+ length / 2".
        if (transform.position.x + length * 0.5f < leftEdge)
        {
            transform.position += Vector3.right * (length * totalTiles);
        }
    }
}
