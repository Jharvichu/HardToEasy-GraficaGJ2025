using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArrastrarPortafolio : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 posicionOriginal;
    private Transform parentOriginal;
    private Vector2 tamanoOriginal;
    private Vector3 escalaOriginal;

    [HideInInspector]
    public string letraPortafolio;
    [HideInInspector]
    public bool enRepisa = false;

    void Awake()
    {
        // Inicializar componentes
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // Guardar estado original
        if (rectTransform != null)
        {
            posicionOriginal = rectTransform.anchoredPosition;
            parentOriginal = transform.parent;
            tamanoOriginal = rectTransform.sizeDelta;
            escalaOriginal = rectTransform.localScale;
        }

        // Obtener letra del nombre
        if (!string.IsNullOrEmpty(gameObject.name))
        {
            letraPortafolio = gameObject.name.Substring(gameObject.name.Length - 1);
        }
    }

    void Start()
    {
        // Componente inicializado
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        enRepisa = false;
        canvasGroup.alpha = 0.9f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas != null && rectTransform != null)
        {
            float scaleFactor = canvas.scaleFactor > 0 ? canvas.scaleFactor : 1f;
            rectTransform.anchoredPosition += eventData.delta / scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Detectar zona cercana
        MinijuegoPortafolios minijuego = FindObjectOfType<MinijuegoPortafolios>();
        if (minijuego != null)
        {
            ZonaSoltado zonaDetectada = minijuego.DetectarZonaCercana(rectTransform.position);
            
            if (zonaDetectada != null)
            {
                minijuego.ColocarPortafolioEnZona(this, zonaDetectada);
            }
            else
            {
                VolverAPosicionOriginal();
            }
            
            minijuego.VerificarOrden();
        }
    }

    public void VolverAPosicionOriginal()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        
        if (rectTransform != null && parentOriginal != null)
        {
            transform.SetParent(parentOriginal, true);
            rectTransform.sizeDelta = tamanoOriginal;
            rectTransform.localScale = escalaOriginal;
            rectTransform.anchoredPosition = posicionOriginal;
        }
        enRepisa = false;
    }
    
    // Actualizar posicion original
    public void ActualizarPosicionOriginal()
    {
        if (rectTransform != null)
        {
            posicionOriginal = rectTransform.anchoredPosition;
            parentOriginal = transform.parent;
        }
    }
    
    // Forzar tamano original
    public void ForzarTamanoOriginal()
    {
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = tamanoOriginal;
            rectTransform.localScale = escalaOriginal;
        }
    }
    
    // Obtener Image del portafolio
    public Image GetImage()
    {
        return GetComponent<Image>();
    }
}