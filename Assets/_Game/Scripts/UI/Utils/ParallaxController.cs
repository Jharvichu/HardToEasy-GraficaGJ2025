using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ParallaxLayer
{
    public GameObject background;
    public float speed;
}

public class ParallaxController : MonoBehaviour
{
    [SerializeField] public List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>();
    private float lengthLayer;

    void Start()
    {
        lengthLayer = parallaxLayers[0].background.GetComponentInChildren<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        foreach (ParallaxLayer pl in parallaxLayers)
        {
            if (pl.background == null) continue;

            Vector3 pos = pl.background.transform.position;
            pos.x += pl.speed * Time.deltaTime;
            pl.background.transform.position = pos;

            if(lengthLayer < pos.x)
            {
                pos.x = 0;
                pl.background.transform.position = pos;
            }
        }
    }
}
