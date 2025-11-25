using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MinijuegoPortafolios : MiniJuegoBase
{
    [Header("Configuración Portafolios")]
    public DificultadMiniJuego dificultad = DificultadMiniJuego.Facil;

    [Header("Referencias UI")]
    public Text textoInstrucciones;
    public GameObject repisaObject;
    public GameObject areaPortafoliosObject;

    private List<string> ordenCorrecto = new List<string>();
    private ZonaSoltado[] zonasRepisa;
    private GameObject[] portafoliosObjects;

    void Awake()
    {
        // Componente inicializado
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

    public enum DificultadMiniJuego
    {
        Facil,    // A-B-C
        Dificil   // A-B-C-D-E
    }

    void BuscarTodosLosElementos()
    {
        // Buscar zonas
        if (repisaObject != null)
        {
            zonasRepisa = repisaObject.GetComponentsInChildren<ZonaSoltado>(true);
            
            if (zonasRepisa.Length == 0)
            {
                List<ZonaSoltado> zonasList = new List<ZonaSoltado>();
                
                foreach (Transform child in repisaObject.transform)
                {
                    if (child.name.StartsWith("Zona"))
                    {
                        ZonaSoltado zona = child.GetComponent<ZonaSoltado>();
                        if (zona == null)
                        {
                            zona = child.gameObject.AddComponent<ZonaSoltado>();
                        }
                        
                        Image img = child.GetComponent<Image>();
                        if (img == null)
                        {
                            img = child.gameObject.AddComponent<Image>();
                            img.color = new Color(1, 1, 1, 0);
                        }
                        img.raycastTarget = true;
                        
                        zonasList.Add(zona);
                    }
                }
                
                zonasRepisa = zonasList.ToArray();
            }
        }

        // Buscar portafolios
        if (areaPortafoliosObject != null)
        {
            List<GameObject> portafoliosList = new List<GameObject>();
            foreach (Transform child in areaPortafoliosObject.transform)
            {
                if (child.name.Contains("Portafolio"))
                {
                    portafoliosList.Add(child.gameObject);
                    
                    ArrastrarPortafolio arrastre = child.GetComponent<ArrastrarPortafolio>();
                    if (arrastre == null)
                    {
                        arrastre = child.gameObject.AddComponent<ArrastrarPortafolio>();
                    }
                    
                    Image img = child.GetComponent<Image>();
                    if (img != null)
                    {
                        img.raycastTarget = true;
                    }
                }
            }
            portafoliosObjects = portafoliosList.ToArray();
        }
    }

    public override void IniciarMinijuego()
    {
        BuscarTodosLosElementos();
        ConfigurarDificultad();
        MezclarPortafolios();
        
        minijuegoActivo = true;
        gameObject.SetActive(true);
    }

    void ConfigurarDificultad()
    {
        ordenCorrecto.Clear();

        if (dificultad == DificultadMiniJuego.Facil)
        {
            ordenCorrecto = new List<string> { "A", "B", "C" };

            // Ocultar portafolios D y E
            foreach (GameObject portafolio in portafoliosObjects)
            {
                if (portafolio.name.Contains("D") || portafolio.name.Contains("E"))
                {
                    portafolio.SetActive(false);
                }
                else
                {
                    portafolio.SetActive(true);
                }
            }

            // Ocultar zonas D y E
            foreach (ZonaSoltado zona in zonasRepisa)
            {
                if (zona.letraCorrecta == "D" || zona.letraCorrecta == "E")
                {
                    zona.gameObject.SetActive(false);
                }
                else
                {
                    zona.gameObject.SetActive(true);
                }
            }
        }
        else // Dificil
        {
            ordenCorrecto = new List<string> { "A", "B", "C", "D", "E" };

            foreach (GameObject portafolio in portafoliosObjects)
            {
                portafolio.SetActive(true);
            }

            foreach (ZonaSoltado zona in zonasRepisa)
            {
                zona.gameObject.SetActive(true);
            }
        }
    }

    void MezclarPortafolios()
    {
        // Liberar zonas
        foreach (ZonaSoltado zona in zonasRepisa)
        {
            if (zona != null && zona.gameObject.activeInHierarchy)
            {
                zona.LiberarZona();
            }
        }

        List<GameObject> portafoliosActivos = new List<GameObject>();
        foreach (GameObject portafolio in portafoliosObjects)
        {
            if (portafolio != null && portafolio.activeInHierarchy)
            {
                portafoliosActivos.Add(portafolio);
                
                if (portafolio.transform.parent != areaPortafoliosObject.transform)
                {
                    portafolio.transform.SetParent(areaPortafoliosObject.transform, false);
                }

                ArrastrarPortafolio arrastre = portafolio.GetComponent<ArrastrarPortafolio>();
                if (arrastre != null)
                {
                    if (arrastre.letraPortafolio == null || arrastre.letraPortafolio == "")
                    {
                        arrastre.letraPortafolio = portafolio.name.Substring(portafolio.name.Length - 1);
                    }
                    arrastre.VolverAPosicionOriginal();
                }
            }
        }

        // Mezclar posiciones
        for (int i = 0; i < portafoliosActivos.Count; i++)
        {
            int randomIndex = Random.Range(0, portafoliosActivos.Count);
            Vector2 tempPos = portafoliosActivos[i].GetComponent<RectTransform>().anchoredPosition;
            portafoliosActivos[i].GetComponent<RectTransform>().anchoredPosition =
                portafoliosActivos[randomIndex].GetComponent<RectTransform>().anchoredPosition;
            portafoliosActivos[randomIndex].GetComponent<RectTransform>().anchoredPosition = tempPos;
        }
        
        // Actualizar posiciones originales
        foreach (GameObject portafolio in portafoliosActivos)
        {
            ArrastrarPortafolio arrastre = portafolio.GetComponent<ArrastrarPortafolio>();
            if (arrastre != null)
            {
                arrastre.ActualizarPosicionOriginal();
            }
        }
    }

    public void VerificarOrden()
    {
        if (zonasRepisa == null) return;

        bool ordenCorrectoCompleto = true;
        int zonasCorrectas = 0;

        for (int i = 0; i < ordenCorrecto.Count; i++)
        {
            if (i < zonasRepisa.Length && zonasRepisa[i].gameObject.activeInHierarchy)
            {
                bool correcto = zonasRepisa[i].TienePortafolioCorrecto();
                
                if (correcto)
                {
                    zonasCorrectas++;
                }
                else
                {
                    ordenCorrectoCompleto = false;
                }
            }
        }

        if (ordenCorrectoCompleto && zonasCorrectas == ordenCorrecto.Count)
        {
            Debug.Log("Juego completado correctamente");
            TerminarMinijuego(true);
        }
    }


    
    // Detectar zona mas cercana
    public ZonaSoltado DetectarZonaCercana(Vector3 posicionMundo)
    {
        if (zonasRepisa == null || zonasRepisa.Length == 0) return null;
        
        float distanciaMinima = 400f;
        ZonaSoltado zonaMasCercana = null;
        
        foreach (ZonaSoltado zona in zonasRepisa)
        {
            if (zona == null || !zona.gameObject.activeInHierarchy) continue;
            
            RectTransform zonaRect = zona.GetComponent<RectTransform>();
            if (zonaRect == null) continue;
            
            float distancia = Vector3.Distance(zonaRect.position, posicionMundo);
            
            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                zonaMasCercana = zona;
            }
        }
        
        return zonaMasCercana;
    }
    
    // Colocar portafolio en zona
    public void ColocarPortafolioEnZona(ArrastrarPortafolio portafolio, ZonaSoltado zona)
    {
        if (portafolio == null || zona == null) return;
        
        // Liberar zona anterior
        foreach (ZonaSoltado z in zonasRepisa)
        {
            if (z != null && z.portafolioActual == portafolio && z != zona)
            {
                z.LiberarZona();
            }
        }
        
        // Devolver portafolio anterior si existe
        if (zona.ocupada && zona.portafolioActual != null && zona.portafolioActual != portafolio)
        {
            zona.portafolioActual.VolverAPosicionOriginal();
        }
        
        // Colocar portafolio
        RectTransform portafolioRect = portafolio.GetComponent<RectTransform>();
        portafolio.transform.SetParent(zona.transform, true);
        portafolioRect.anchoredPosition = Vector2.zero;
        portafolio.enRepisa = true;
        
        zona.ocupada = true;
        zona.portafolioActual = portafolio;
        
        // Log de posicion
        if (portafolio.letraPortafolio == zona.letraCorrecta)
        {
            Debug.Log($"Portafolio {portafolio.name} colocado en posicion correcta");
        }
        else
        {
            Debug.Log($"Portafolio {portafolio.name} colocado en posicion incorrecta");
        }
    }
}