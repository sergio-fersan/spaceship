using UnityEngine;

/// <summary>
/// Animacao quadro a quadro bem simples, sem precisar do Animator.
/// Os arquivos Ship01..Ship04 sao os 4 quadros do fogo do motor da nave.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimator : MonoBehaviour
{
    [Tooltip("Quadros da animacao, em ordem.")]
    public Sprite[] frames;

    [Tooltip("Quantos quadros por segundo.")]
    public float framesPerSecond = 12f;

    [Tooltip("Se verdadeiro, a animacao ignora a camera lenta.")]
    public bool useUnscaledTime = true;

    private SpriteRenderer spriteRenderer;
    private float timer;
    private int index;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (frames == null || frames.Length <= 1 || framesPerSecond <= 0f) return;

        timer += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        if (timer >= 1f / framesPerSecond)
        {
            timer = 0f;
            index = (index + 1) % frames.Length;
            spriteRenderer.sprite = frames[index];
        }
    }
}
