using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArrastrarPortafolio : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 posicionOriginal;

    [HideInInspector]
    public string letraPortafolio;
    [HideInInspector]
    public bool enRepisa = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        posicionOriginal = rectTransform.anchoredPosition;

        // Obtener la letra del nombre (último carácter)
        letraPortafolio = gameObject.name.Substring(gameObject.name.Length - 1);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Si no está en repisa, volver a posición original
        if (!enRepisa)
        {
            rectTransform.anchoredPosition = posicionOriginal;
        }

        // Notificar al minijuego que soltamos un portafolio
        MinijuegoPortafolios minijuego = FindObjectOfType<MinijuegoPortafolios>();
        if (minijuego != null)
        {
            minijuego.VerificarOrden();
        }
    }

    public void VolverAPosicionOriginal()
    {
        rectTransform.anchoredPosition = posicionOriginal;
        enRepisa = false;
    }
}