using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MiniJuegoBase : MonoBehaviour
{
    [Header("Configuraci�n del Minijuego")]
    public string nombreMinijuego;
    public bool minijuegoActivo = false;

    // Referencias a los elementos de la ventana
    public Text textoTitulo;
    public Button botonCerrar;
    public GameObject areaJuego;

    // Evento cuando termina el minijuego
    public System.Action<bool> OnMinijuegoTerminado;

    void Start()
    {
        // Configurar botón cerrar
        if (botonCerrar != null)
        {
            botonCerrar.onClick.AddListener(CerrarMiniJuego);
            Debug.Log($"[MiniJuegoBase] Botón cerrar configurado en: {botonCerrar.name}");
        }
        else
        {
            Debug.LogWarning("[MiniJuegoBase] botonCerrar NO está asignado!");
        }

        // Ocultar al inicio
        gameObject.SetActive(true);
    }

    public virtual void IniciarMinijuego()
    {
        gameObject.SetActive(true);
        minijuegoActivo = true;

        if (textoTitulo != null)
            textoTitulo.text = nombreMinijuego;

        Debug.Log("Minijuego iniciado: " + nombreMinijuego);
    }

    public virtual void TerminarMinijuego(bool exito, int score)
    {
        minijuegoActivo = false;
        ScoreManager.Instance.AddScore(score);
        OnMinijuegoTerminado?.Invoke(exito);
        Destroy(gameObject);
    }

    public void CerrarMiniJuego()
    {
        Debug.Log("Juego cerrado");
        TerminarMinijuego(false, 0);
    }

    void Update()
    {
        // Cerrar con ESC usando el nuevo Input System
        if (minijuegoActivo && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("[MiniJuegoBase] ESC presionado, cerrando minijuego...");
            CerrarMiniJuego();
        }
    }

}
