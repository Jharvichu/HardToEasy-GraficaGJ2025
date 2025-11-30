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
    public float rangoEngrapado = 80f; // Aumentado de 45 para mayor tolerancia

    private int hojasEngrapdasCorrectamente = 0;
    private int hojasEngrapadas = 0;
    private List<HojaEngrapable> hojasActivas = new List<HojaEngrapable>();
    private RectTransform engrapadoraRect;
    private PlayerController playerController;

    void Awake()
    {
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        if (playerController.CurrentState == PlayerState.Awaken) dificultad = DificultadMiniJuego.Dificil;
        if (playerController.CurrentState == PlayerState.Asleep) dificultad = DificultadMiniJuego.Facil;

        playerController.OnPlayerStateChanged += OnPlayerStateChanged;
    }

    void OnDestroy()
    {
        if (playerController != null)
        {
            playerController.OnPlayerStateChanged -= OnPlayerStateChanged;
        }
    }

    void OnPlayerStateChanged(PlayerState nuevoEstado)
    {
        // Actualizar dificultad según el nuevo estado
        dificultad = nuevoEstado == PlayerState.Awaken ? DificultadMiniJuego.Dificil : DificultadMiniJuego.Facil;

        // Calcular nueva velocidad
        float nuevaVelocidad = dificultad == DificultadMiniJuego.Facil ? velocidadFacil : velocidadDificil;

        // Actualizar velocidad de todas las hojas activas
        foreach (HojaEngrapable hoja in hojasActivas)
        {
            if (hoja != null)
            {
                hoja.SetVelocidad(nuevaVelocidad);
            }
        }
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

        hojasEngrapdasCorrectamente = 0;
        hojasEngrapadas = 0;

        if (engrapadoraObject != null)
        {
            engrapadoraRect = engrapadoraObject.GetComponent<RectTransform>();
        }

        CrearHojas();
        ActualizarProgreso(); // Llamar DESPUÉS de CrearHojas para que hojasActivas.Count sea correcto
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
            hojasEngrapdasCorrectamente++;
            hojasEngrapadas++;
            Debug.Log($"Hoja {hoja.name} engrapada correctamente - Progreso: {hojasEngrapdasCorrectamente}/{hojasActivas.Count}");

            AudioManager.Instance.Play("ExitoPerforar");
            hoja.Engrapar(true);
            ActualizarProgreso();
            
            // Activar siguiente hoja
            if (hojasEngrapadas < hojasActivas.Count)
            {
                hojasActivas[hojasEngrapadas].gameObject.SetActive(true);
                Debug.Log($"Siguiente hoja activada: {hojasActivas[hojasEngrapadas].name}");
            }
            else
            {
                // Juego completado
                Debug.Log($"Juego completado correctamente - Hojas engrapadas: {hojasEngrapdasCorrectamente}/{hojasActivas.Count}");
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
                // Juego completado
                Debug.Log($"Juego completado - Hojas engrapadas: {hojasEngrapdasCorrectamente}/{hojasActivas.Count}");
                TerminarMinijuego(false, 0);
            }
        }
    }

    void ActualizarProgreso()
    {
        if (textoPuntos != null)
        {
            textoPuntos.text = $"{hojasEngrapdasCorrectamente}/{hojasActivas.Count}";
        }
    }
}
