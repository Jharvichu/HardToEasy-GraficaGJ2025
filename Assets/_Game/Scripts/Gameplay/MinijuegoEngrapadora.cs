using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class MinijuegoEngrapadora : MiniJuegoBase
{
    [Header("Configuración Engrapadora")]
    public DificultadMiniJuego dificultad = DificultadMiniJuego.Facil;

    [Header("Referencias UI")]
    public Text textoPuntos;
    public GameObject mesaObject;
    public GameObject engrapadoraObject;
    public GameObject areaHojasObject;

    [Header("Configuración Juego")]
    public float velocidadFacil = 100f;
    public float velocidadDificil = 250f;
    public float rangoEngrapado = 45f;

    private int puntos = 0;
    private int hojasEngrapadas = 0;
    private List<HojaEngrapable> hojasActivas = new List<HojaEngrapable>();
    private RectTransform engrapadoraRect;

    void Awake()
    {
        PlayerController player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        if (player.CurrentState == PlayerState.Awaken) dificultad = DificultadMiniJuego.Dificil;
        if (player.CurrentState == PlayerState.Asleep) dificultad = DificultadMiniJuego.Facil;
    }

    void Start()
    {
        // Verificar GraphicRaycaster
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            UnityEngine.UI.GraphicRaycaster raycaster = canvas.GetComponent<UnityEngine.UI.GraphicRaycaster>();
            if (raycaster == null)
            {
                canvas.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }
        }
        
        // Configurar boton cerrar
        if (botonCerrar != null)
        {
            botonCerrar.onClick.RemoveAllListeners();
            botonCerrar.onClick.AddListener(CerrarMiniJuego);
        }
        
        IniciarMinijuego();
    }

    void Update()
    {
        // Detectar click izquierdo en cualquier lugar
        if (minijuegoActivo && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && hojasActivas.Count > 0)
        {
            // Buscar hoja activa actual
            HojaEngrapable hojaActual = null;
            foreach (HojaEngrapable hoja in hojasActivas)
            {
                if (hoja != null && hoja.gameObject.activeInHierarchy && !hoja.EstaEngrapada())
                {
                    hojaActual = hoja;
                    break;
                }
            }
            
            if (hojaActual != null)
            {
                Debug.Log($"Click detectado en pantalla - Intentando engrapar {hojaActual.name}");
                IntentarEngrapar(hojaActual);
            }
        }
    }

    public enum DificultadMiniJuego
    {
        Facil,
        Dificil
    }

    public override void IniciarMinijuego()
    {
        minijuegoActivo = true;
        gameObject.SetActive(true);
        
        puntos = 0;
        hojasEngrapadas = 0;
        ActualizarPuntos();
        
        if (engrapadoraObject != null)
        {
            engrapadoraRect = engrapadoraObject.GetComponent<RectTransform>();
        }
        
        CrearHojas();
    }

    void CrearHojas()
    {
        // Buscar hojas que ya existen en AreaHojas
        if (areaHojasObject == null) return;

        hojasActivas.Clear();
        
        foreach (Transform child in areaHojasObject.transform)
        {
            HojaEngrapable hoja = child.GetComponent<HojaEngrapable>();
            if (hoja != null)
            {
                float velocidad = dificultad == DificultadMiniJuego.Facil ? velocidadFacil : velocidadDificil;
                hoja.Inicializar(this, velocidad);
                hojasActivas.Add(hoja);
                
                // Solo la primera hoja activa
                child.gameObject.SetActive(hojasActivas.Count == 1);
            }
        }
    }

    public void IntentarEngrapar(HojaEngrapable hoja)
    {
        if (engrapadoraRect == null || hoja == null) return;

        // Calcular distancia entre engrapadora y zona de engrapado de la hoja
        RectTransform zonaEngrapado = hoja.GetZonaEngrapado();
        if (zonaEngrapado == null)
        {
            Debug.Log($"Zona de engrapado no encontrada en {hoja.name}");
            return;
        }

        float distancia = Vector3.Distance(engrapadoraRect.position, zonaEngrapado.position);
        Debug.Log($"Distancia al engrapar: {distancia} (rango permitido: {rangoEngrapado})");

        if (distancia <= rangoEngrapado)
        {
            // Engrapado correcto
            puntos += 10;
            hojasEngrapadas++;
            Debug.Log($"Hoja {hoja.name} engrapada correctamente - Puntos: +10 - Total: {puntos}");
            
            hoja.Engrapar(true);
            ActualizarPuntos();
            
            // Activar siguiente hoja
            if (hojasEngrapadas < hojasActivas.Count)
            {
                hojasActivas[hojasEngrapadas].gameObject.SetActive(true);
                Debug.Log($"Siguiente hoja activada: {hojasActivas[hojasEngrapadas].name}");
            }
            else
            {
                // Juego completado
                Debug.Log($"Juego completado correctamente - Puntuacion final: {puntos}");
                TerminarMinijuego(true, 10);
            }
        }
        else
        {
            // Engrapado fallido
            hojasEngrapadas++;
            Debug.Log($"Hoja {hoja.name} engrapada incorrectamente - Demasiado lejos - Oportunidad perdida");
            
            // Desactivar hoja fallida
            hoja.gameObject.SetActive(false);
            
            // Activar siguiente hoja
            if (hojasEngrapadas < hojasActivas.Count)
            {
                hojasActivas[hojasEngrapadas].gameObject.SetActive(true);
                Debug.Log($"Siguiente hoja activada: {hojasActivas[hojasEngrapadas].name}");
            }
            else
            {
                // Juego completado sin puntos
                Debug.Log($"Juego completado - Puntuacion final: {puntos}");
                TerminarMinijuego(false, 0);
            }
        }
    }

    void ActualizarPuntos()
    {
        if (textoPuntos != null)
        {
            textoPuntos.text = puntos.ToString();
            Debug.Log($"Puntos actualizados en UI: {puntos}");
        }
        else
        {
            Debug.LogWarning("textoPuntos es NULL - No se asignó en el Inspector");
        }
    }
}
