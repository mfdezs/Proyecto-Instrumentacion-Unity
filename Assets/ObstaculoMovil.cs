using UnityEngine;

public class ObstaculoMovil : MonoBehaviour
{
    // Al ser 'static', compartimos esta variable con GestionJuego y GeneradorObstaculos
    public static float velocidadActual = 5f;

    [Header("Configuración de Dificultad")]
    public float velocidadBase = 5f;
    public float aceleracionPorSegundo = 0.1f;
    public float velocidadMaxima = 12f;

    void Start()
    {
        // Si la escena se acaba de reiniciar de forma manual, aseguramos la velocidad base
        if (Time.timeSinceLevelLoad < 1f)
        {
            velocidadActual = velocidadBase;
        }
    }

    void Update()
    {
        // 1. AUMENTAR LA VELOCIDAD PROGRESIVAMENTE CON EL TIEMPO
        if (velocidadActual < velocidadMaxima)
        {
            velocidadActual += aceleracionPorSegundo * Time.deltaTime;
        }

        // 2. MOVER EL OBSTÁCULO HACIA LA IZQUIERDA
        transform.Translate(Vector3.left * velocidadActual * Time.deltaTime);

        // 3. OPTIMIZACIÓN: Destruir el tronco si ya salió de la pantalla
        if (transform.position.x < -12f)
        {
            Destroy(gameObject);
        }
    }
}
