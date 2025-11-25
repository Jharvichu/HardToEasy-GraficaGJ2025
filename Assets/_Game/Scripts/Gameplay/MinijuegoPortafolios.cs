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

    public enum DificultadMiniJuego
    {
        Facil,    // A-B-C
        Dificil   // A-B-C-D-E
    }

    void BuscarTodosLosElementos()
    {
        // Buscar zonas automáticamente
        if (repisaObject != null)
        {
            zonasRepisa = repisaObject.GetComponentsInChildren<ZonaSoltado>();
            Debug.Log("Encontradas " + zonasRepisa.Length + " zonas en la repisa");
        }

        // Buscar portafolios automáticamente
        if (areaPortafoliosObject != null)
        {
            List<GameObject> portafoliosList = new List<GameObject>();
            foreach (Transform child in areaPortafoliosObject.transform)
            {
                if (child.name.Contains("Portafolio"))
                {
                    portafoliosList.Add(child.gameObject);
                }
            }
            portafoliosObjects = portafoliosList.ToArray();
            Debug.Log("Encontrados " + portafoliosObjects.Length + " portafolios");
        }
    }

    public override void IniciarMinijuego()
    {
        BuscarTodosLosElementos(); // ← ¡IMPORTANTE!

        base.IniciarMinijuego();
        ConfigurarDificultad();
        MezclarPortafolios();
        ActualizarInstrucciones();
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
        foreach (ZonaSoltado zona in zonasRepisa)
        {
            if (zona.gameObject.activeInHierarchy)
            {
                zona.LiberarZona();
            }
        }

        List<GameObject> portafoliosActivos = new List<GameObject>();
        foreach (GameObject portafolio in portafoliosObjects)
        {
            if (portafolio.activeInHierarchy)
            {
                portafoliosActivos.Add(portafolio);
                portafolio.transform.SetParent(areaPortafoliosObject.transform);

                ArrastrarPortafolio arrastre = portafolio.GetComponent<ArrastrarPortafolio>();
                if (arrastre != null)
                {
                    arrastre.VolverAPosicionOriginal();
                }
            }
        }

        for (int i = 0; i < portafoliosActivos.Count; i++)
        {
            int randomIndex = Random.Range(0, portafoliosActivos.Count);
            Vector2 tempPos = portafoliosActivos[i].GetComponent<RectTransform>().anchoredPosition;
            portafoliosActivos[i].GetComponent<RectTransform>().anchoredPosition =
                portafoliosActivos[randomIndex].GetComponent<RectTransform>().anchoredPosition;
            portafoliosActivos[randomIndex].GetComponent<RectTransform>().anchoredPosition = tempPos;
        }
    }

    public void VerificarOrden()
    {
        if (zonasRepisa == null) return;

        bool ordenCorrectoCompleto = true;

        for (int i = 0; i < ordenCorrecto.Count; i++)
        {
            if (i < zonasRepisa.Length && zonasRepisa[i].gameObject.activeInHierarchy)
            {
                if (!zonasRepisa[i].TienePortafolioCorrecto())
                {
                    ordenCorrectoCompleto = false;
                    break;
                }
            }
        }

        if (ordenCorrectoCompleto)
        {
            textoInstrucciones.text = "¡CORRECTO! Orden alfabético logrado";
            textoInstrucciones.color = Color.green;
            TerminarMinijuego(true);
        }
    }

    void ActualizarInstrucciones()
    {
        if (dificultad == DificultadMiniJuego.Facil)
        {
            textoInstrucciones.text = "Arrastra los portafolios a la repisa en orden A-B-C";
        }
        else
        {
            textoInstrucciones.text = "Arrastra los portafolios a la repisa en orden A-B-C-D-E";
        }
        textoInstrucciones.color = Color.white;
    }
}