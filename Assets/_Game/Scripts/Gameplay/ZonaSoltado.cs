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

    void Awake()
    {
        // Componente inicializado
    }

    void Start()
    {
        imagenZona = GetComponent<Image>();
        if (imagenZona != null)
        {
            colorOriginal = imagenZona.color;
            imagenZona.raycastTarget = true;
        }

        // Extraer letra del nombre
        if (gameObject.name.Length >= 5 && gameObject.name.StartsWith("Zona"))
        {
            letraCorrecta = gameObject.name.Substring(4);
        }
        else
        {
            letraCorrecta = gameObject.name.Substring(gameObject.name.Length - 1);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject objetoArrastrado = eventData.pointerDrag;
        if (objetoArrastrado == null) return;

        ArrastrarPortafolio portafolio = objetoArrastrado.GetComponent<ArrastrarPortafolio>();
        if (portafolio == null) return;

        // Liberar portafolio anterior
        if (ocupada && portafolioActual != null && portafolioActual != portafolio)
        {
            portafolioActual.VolverAPosicionOriginal();
        }

        // Colocar portafolio en zona
        RectTransform portafolioRect = portafolio.GetComponent<RectTransform>();
        portafolio.transform.SetParent(transform, true);
        portafolioRect.anchoredPosition = Vector2.zero;
        portafolio.enRepisa = true;

        ocupada = true;
        portafolioActual = portafolio;
    }

    public void LiberarZona()
    {
        if (portafolioActual != null)
        {
            portafolioActual.enRepisa = false;
        }
        
        ocupada = false;
        portafolioActual = null;
    }

    public bool TienePortafolioCorrecto()
    {
        return ocupada && portafolioActual != null && portafolioActual.letraPortafolio == letraCorrecta;
    }
}