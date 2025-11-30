using UnityEngine;
using UnityEngine.UI;

public class SleepEffectController : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;

    [Header("Configuración de Transición")]
    [Tooltip("Qué tan rápido entra y sale el efecto (Fade In/Out)")]
    [SerializeField] private float _transitionSpeed = 2.0f; 

    [Header("Configuración de Sueño (Pulsación)")]
    [SerializeField] private float _pulseSpeed = 2.5f; 
    [SerializeField, Range(0.1f, 0.3f)] private float _pulseMagnitude = 0.15f; 

    [Header("Rotación (Opcional)")]
    [SerializeField] private float _rotationSpeed = 10f;

    private const float BASE_ALPHA = 0.5f; 
    private Image _sleepImage;

    void Start()
    {
        _sleepImage = GetComponent<Image>();
        SetAlpha(0f);
    }

    void Update()
    {
        float targetAlpha = 0f;

        if (_playerController.CurrentState == PlayerState.Asleep)
        {
            // Fórmula: Centro + (Seno * Amplitud)
            targetAlpha = BASE_ALPHA + Mathf.Sin(Time.time * _pulseSpeed) * _pulseMagnitude;
            transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime);
        }
        else
        {
            targetAlpha = 0f;
        }

        Color currentColor = _sleepImage.color;

        // Mathf.Lerp hace la magia:
        // Si target es 0.5 y current es 0, subirá rápido al principio y suave al final.
        // Si target cambia frame a frame (la onda), Lerp lo seguirá suavemente.
        float newAlpha = Mathf.Lerp(currentColor.a, targetAlpha, Time.deltaTime * _transitionSpeed);

        SetAlpha(newAlpha);
    }

    private void SetAlpha(float alphaVal)
    {
        if (_sleepImage != null)
        {
            Color c = _sleepImage.color;
            c.a = alphaVal;
            _sleepImage.color = c;
        }
    }
}