using UnityEngine;

/// <summary>
/// REQUISITO 4 (Capacidade de desacelerar o tempo).
///
/// Enquanto o jogador segura SHIFT (ou botao direito do mouse) o
/// Time.timeScale cai. Como o fundo (Parallax), os inimigos e o spawner usam
/// Time.deltaTime, todos desaceleram juntos. O jogador e os tiros usam
/// Time.unscaledDeltaTime e continuam rapidos - essa e a vantagem.
///
/// O efeito gasta energia, que se recarrega sozinha, para nao virar um
/// botao de "facil infinito".
/// </summary>
public class TimeController : MonoBehaviour
{
    [Header("Controles")]
    public KeyCode slowKey = KeyCode.LeftShift;

    [Header("Efeito")]
    [Tooltip("0.35 = o mundo anda a 35% da velocidade normal.")]
    [Range(0.05f, 1f)]
    public float slowFactor = 0.35f;

    [Header("Energia")]
    public float maxEnergy = 100f;
    public float drainPerSecond = 35f;
    public float rechargePerSecond = 15f;

    [Tooltip("Energia minima para conseguir ativar o efeito de novo.")]
    public float minEnergyToActivate = 15f;

    public float Energy { get; private set; }
    public bool IsSlowMotion { get; private set; }

    public float EnergyNormalized
    {
        get { return maxEnergy <= 0f ? 0f : Mathf.Clamp01(Energy / maxEnergy); }
    }

    private const float DefaultFixedDelta = 0.02f;

    private void Awake()
    {
        Energy = maxEnergy;
    }

    private void Update()
    {
        // Durante a tela de vitoria/derrota o GameManager congela o jogo
        // (timeScale = 0). Nao sobrescrevemos isso aqui.
        if (GameManager.Instance != null && GameManager.Instance.State != GameState.Playing)
        {
            IsSlowMotion = false;
            return;
        }

        float dt = Time.unscaledDeltaTime;
        bool wants = Input.GetKey(slowKey) || Input.GetKey(KeyCode.RightShift) || Input.GetMouseButton(1);

        if (IsSlowMotion)
        {
            if (!wants || Energy <= 0f) IsSlowMotion = false;
        }
        else
        {
            if (wants && Energy >= minEnergyToActivate) IsSlowMotion = true;
        }

        if (IsSlowMotion)
        {
            Energy = Mathf.Max(0f, Energy - drainPerSecond * dt);
        }
        else
        {
            Energy = Mathf.Min(maxEnergy, Energy + rechargePerSecond * dt);
        }

        Time.timeScale = IsSlowMotion ? slowFactor : 1f;
        Time.fixedDeltaTime = DefaultFixedDelta * Time.timeScale;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = DefaultFixedDelta;
    }
}
