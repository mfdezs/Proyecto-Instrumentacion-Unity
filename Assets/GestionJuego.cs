using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Requerido para controlar el componente TextMeshPro del tiempo

public class GestionJuego : MonoBehaviour
{
    [Header("Paneles de la Interfaz")]
    public GameObject panelGameOver;
    public GameObject menuInicio;

    [Header("UI de Puntuación")]
    public TextMeshProUGUI textoTiempoFinal; // Casilla para arrastrar tu objeto "TextoTiempo"

    [Header("Elementos del Juego")]
    public GameObject jugador;
    public GameObject generadorObstaculos;

    void Start()
    {
        // Al empezar la aplicación nos aseguramos de que solo se vea el menú de inicio
        if (menuInicio != null) menuInicio.SetActive(true);
        if (panelGameOver != null) panelGameOver.SetActive(false);

        // El generador de obstáculos empieza apagado hasta que termine la calibración de MATLAB
        if (generadorObstaculos != null) generadorObstaculos.SetActive(false);
    }

    // Esta función la llamará el botón START del menú inicial
    public void IniciarJuego()
    {
        if (menuInicio != null) menuInicio.SetActive(false);

        // NOTA: El jugador se mantiene activo para que sea visible mientras MATLAB calibra.
        // El generador de obstáculos se encenderá automáticamente cuando UDPReceiver detecte el comando "READY".
    }

    // Esta es la función que recibe el tiempo calculado por el jugador al chocar
    public void DetenerSesion(float tiempoTotal)
    {
        Debug.Log("¡DetenerSesion() ejecutado por colisión!");

        // Congelamos el motor de físicas y movimientos del juego
        Time.timeScale = 0f;

        // Modificamos el "New Text" por el tiempo real formateado con dos decimales
        if (textoTiempoFinal != null)
        {
            textoTiempoFinal.text = "Tiempo de juego: " + tiempoTotal.ToString("F2") + " segundos";
        }
        else
        {
            Debug.LogWarning("¡Atención!: No has asignado el objeto TextoTiempo en el script de GestionJuego.");
        }

        // Activamos el panel final de Game Over con el botón Reintentar
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(true);
        }
    }

    // Esta es la función CORREGIDA para reiniciar DIRECTAMENTE sin volver al menú de inicio
    public void ReiniciarPartidaManual()
    {
        Debug.Log("Reiniciando partida directamente de forma limpia...");

        // 1. Descongelamos el tiempo del juego para que todo se vuelva a mover
        Time.timeScale = 1f;

        // 2. Ocultamos el panel de Game Over
        if (panelGameOver != null) panelGameOver.SetActive(false);

        // 3. Nos aseguramos de que el menú de inicio siga APAGADO
        if (menuInicio != null) menuInicio.SetActive(false);

        // 4. Buscamos y destruimos todos los obstáculos viejos que quedaran flotando en la pantalla
        GameObject[] obstaculosViejos = GameObject.FindGameObjectsWithTag("Obstaculo");
        foreach (GameObject obstaculo in obstaculosViejos)
        {
            Destroy(obstaculo);
        }

        // 5. SOLUCIÓN AL ERROR ROJO: Reseteamos la velocidad estática a su valor inicial suave
        ObstaculoMovil.velocidadActual = 5f;

        // 6. Volvemos a encender el generador para que vuelvan a salir troncos nuevos de inmediato
        if (generadorObstaculos != null)
        {
            generadorObstaculos.SetActive(true);
        }

        // 7. Forzamos un reinicio rápido en el script del Player para actualizar el cronómetro interno
        PlayerController2D playerScript = Object.FindFirstObjectByType<PlayerController2D>();
        if (playerScript != null)
        {
            playerScript.enabled = false;
            playerScript.enabled = true;
        }
    }
}