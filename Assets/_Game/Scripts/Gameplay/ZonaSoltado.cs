using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ZonaSoltado : MonoBehaviour, IDropHandler
{
    [Header("Configuración Zona")]
    public string letraCorrecta; // A, B, C, etc.

    private Image imagenZona;
    private Color colorOriginal;

    [HideInInspector]
    public bool ocupada = false;
    [HideInInspector]
    public ArrastrarPortafolio portafolioActual = null;

    void Start()
    {
        imagenZona = GetComponent<Image>();
        colorOriginal = imagenZona.color;

        // La letra correcta es la última del nombre (ZonaA a A)
        letraCorrecta = gameObject.name.Substring(4);
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject objetoArrastrado = eventData.pointerDrag;
        if (objetoArrastrado == null) return;

        ArrastrarPortafolio portafolio = objetoArrastrado.GetComponent<ArrastrarPortafolio>();
        if (portafolio == null) return;

        // Si ya hay un portafolio aquí, lo devolvemos
        if (ocupada && portafolioActual != null)
        {
            portafolioActual.VolverAPosicionOriginal();
        }

        // Colocar el portafolio en esta zona
        portafolio.transform.SetParent(transform);
        portafolio.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        portafolio.enRepisa = true;

        // Actualizar estado
        ocupada = true;
        portafolioActual = portafolio;

        // Cambiar color de feedback
        imagenZona.color = Color.green;
    }

    public void LiberarZona()
    {
        ocupada = false;
        portafolioActual = null;
        imagenZona.color = colorOriginal;
    }

    public bool TienePortafolioCorrecto()
    {
        return ocupada && portafolioActual != null && portafolioActual.letraPortafolio == letraCorrecta;
    }
}