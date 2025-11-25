using UnityEngine;
using UnityEngine.UI;

public class MiniJuegoBase : MonoBehaviour
{
    [Header("Configuración del Minijuego")]
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
            botonCerrar.onClick.AddListener(CerrarMiniJuego);

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

    public virtual void TerminarMinijuego(bool exito)
    {
        minijuegoActivo = false;
        gameObject.SetActive(false);

        OnMinijuegoTerminado?.Invoke(exito);
    }

    public void CerrarMiniJuego()
    {
        TerminarMinijuego(false);
    }

    void Update()
    {
        // Cerrar con ESC
        if (minijuegoActivo && Input.GetKeyDown(KeyCode.Escape))
        {
            CerrarMiniJuego();
        }
    }

}
