using UnityEngine;
using UnityEngine.EventSystems;

public class HojaEngrapable : MonoBehaviour, IPointerClickHandler
{
    [Header("Referencias")]
    public GameObject zonaEngrapado;
    
    private MinijuegoEngrapadora minijuego;
    private RectTransform rectTransform;
    private float velocidad;
    private bool engrapada = false;
    private bool moviendoArriba = true;
    private RectTransform areaMovimiento;

    public void Inicializar(MinijuegoEngrapadora juego, float vel)
    {
        minijuego = juego;
        velocidad = vel;
        rectTransform = GetComponent<RectTransform>();
        
        // Obtener area de movimiento
        if (transform.parent != null)
        {
            areaMovimiento = transform.parent.GetComponent<RectTransform>();
        }
        
        // Asegurar que zona engrapado existe
        if (zonaEngrapado == null)
        {
            // Buscar hijo con nombre "ZonaEngrapado"
            Transform zona = transform.Find("ZonaEngrapado");
            if (zona != null)
            {
                zonaEngrapado = zona.gameObject;
            }
        }

    }

    void Update()
    {
        if (engrapada || areaMovimiento == null) return;

        // Mover hoja arriba y abajo
        float movimiento = velocidad * Time.deltaTime;

        if (moviendoArriba)
        {
            rectTransform.anchoredPosition += new Vector2(0, movimiento);

            // Verificar limite superior
            if (rectTransform.anchoredPosition.y >= areaMovimiento.rect.height / 2)
            {
                moviendoArriba = false;
            }
        }
        else
        {
            rectTransform.anchoredPosition -= new Vector2(0, movimiento);

            // Verificar limite inferior
            if (rectTransform.anchoredPosition.y <= -areaMovimiento.rect.height / 2)
            {
                moviendoArriba = true;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!engrapada && minijuego != null)
        {
            Debug.Log($"Click detectado en hoja {gameObject.name} - Activa: {gameObject.activeInHierarchy}");
            minijuego.IntentarEngrapar(this);
        }
        else
        {
            Debug.Log($"Click ignorado - Engrapada: {engrapada}, Minijuego: {(minijuego != null ? "OK" : "NULL")}");
        }
    }

    public void Engrapar(bool exitoso)
    {
        engrapada = true;
        
        if (exitoso)
        {
            // Desactivar hoja
            gameObject.SetActive(false);
        }
        else
        {
            engrapada = false;
        }
    }

    public RectTransform GetZonaEngrapado()
    {
        if (zonaEngrapado != null)
        {
            return zonaEngrapado.GetComponent<RectTransform>();
        }
        return null;
    }

    public bool EstaEngrapada()
    {
        return engrapada;
    }

    public void SetVelocidad(float nuevaVelocidad)
    {
        velocidad = nuevaVelocidad;
    }
}
