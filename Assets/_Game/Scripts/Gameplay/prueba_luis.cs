using UnityEngine;

public class prueba_luis : MonoBehaviour
{
    public float speed = 3f;
    private Vector3 direction = Vector3.left;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        // Mover
        transform.Translate(direction * speed * Time.deltaTime);

        // Convertir posición en coordenadas de pantalla
        Vector3 posScreen = cam.WorldToViewportPoint(transform.position);

        // Si llega a un borde lateral, invertir dirección
        // izquierda || derecha
        if (posScreen.x <= 0f || posScreen.x >= 1f)
        {
            direction.x *= -1;
        }
    }
}
