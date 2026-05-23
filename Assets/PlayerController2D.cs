using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController2D : MonoBehaviour
{
    private UDPReceiver receptor;
    private GestionJuego gestorJuego; // Caching del gestor de juego
    private int carrilActual = 1;
    public float distanciaCarril = 2.0f;
    private bool saltando = false;

    private Rigidbody2D rb;
    private Animator animator;

    // Variables para el cálculo del tiempo de juego
    private float tiempoInicioPartida;

    [Header("Efectos de Sonido (Opcional)")]
    public AudioSource altavozEfectos;
    public AudioClip sonidoSalto;
    public AudioClip sonidoChoque;

    void Start()
    {
        Time.timeScale = 1f;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Buscamos el receptor UDP en la escena
        receptor = Object.FindFirstObjectByType<UDPReceiver>();

        // Buscamos el gestor de juego en la escena desde el inicio
        gestorJuego = Object.FindFirstObjectByType<GestionJuego>();
        if (gestorJuego == null)
        {
            Debug.LogError(">>> JUGADOR: ¡No se encuentra el script GestionJuego en la escena!");
        }

        // Guardamos el segundo exacto en el que arranca la partida
        tiempoInicioPartida = Time.time;
    }

    void Update()
    {
        // 1. CONTROL POR TECLADO
        if (Keyboard.current.upArrowKey.wasPressedThisFrame) {
            ProcesarComando("EXTENSION");
        }
        if (Keyboard.current.downArrowKey.wasPressedThisFrame) {
            ProcesarComando("FLEXION");
        }
        if (Keyboard.current.spaceKey.wasPressedThisFrame) {
            ProcesarComando("COCONTRACCION");
        }

        // 2. CONTROL POR MATLAB (Recibe números "1", "2", "3")
        if (receptor != null) {
            string comandoUDP = receptor.lastReceivedPacket;
            if (comandoUDP != "") {
                Debug.Log(">>> JUGADOR: Intentando procesar comando: " + comandoUDP);
                ProcesarComando(comandoUDP);
                receptor.lastReceivedPacket = "";
            }
        }
        else {
            Debug.LogWarning("El Player no tiene asignado el receptor UDP");
        }

        // 3. MOVIMIENTO SUAVE ENTRE CARRILES (Optimizado a 20f para evitar retrasos)
        if (!saltando)
        {
            float yBase = (transform.parent != null) ? transform.parent.position.y : 0f;
            float yObjetivo = (carrilActual - 1) * distanciaCarril + yBase;

            Vector3 posDestino = new Vector3(transform.position.x, yObjetivo, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, posDestino, Time.deltaTime * 20f);
        }

        if (animator != null) {
            animator.SetBool("esta_saltando", saltando);
        }
    }

    void ProcesarComando(string cmd)
    {
        switch (cmd)
        {
            case "EXTENSION":
            case "1":
                if (carrilActual < 2) carrilActual++;
                break;

            case "FLEXION":
            case "2":
                if (carrilActual > 0) carrilActual--;
                break;

            case "COCONTRACCION":
            case "3":
                if (!saltando) {
                    StartCoroutine(EfectoSalto());
                }
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Obstaculo"))
        {
            Debug.Log(">>> JUGADOR: Choque detectado con el obstáculo.");

            // Reproducir sonido de impacto si está configurado
            if (altavozEfectos != null && sonidoChoque != null) {
                altavozEfectos.PlayOneShot(sonidoChoque);
            }

            if (gestorJuego != null)
            {
                // Calculamos los segundos exactos que ha durado el usuario esquivando
                float tiempoTranscurrido = Time.time - tiempoInicioPartida;

                // Enviamos el tiempo al gestor para que detenga la sesión y lo pinte en la UI
                gestorJuego.DetenerSesion(tiempoTranscurrido);
            }
            else
            {
                Time.timeScale = 0f;
                Debug.LogError("No se pudo mostrar el Game Over porque gestorJuego es nulo.");
            }
        }
    }

    System.Collections.IEnumerator EfectoSalto()
    {
        saltando = true;

        // Reproducir sonido de salto si está configurado
        if (altavozEfectos != null && sonidoSalto != null) {
            altavozEfectos.PlayOneShot(sonidoSalto);
        }

        float alturaOriginal = transform.position.y;
        float alturaSalto = alturaOriginal + 3.0f;
        float t = 0;
        while(t < 1) {
            t += Time.deltaTime * 3f;
            float nuevaY = Mathf.Lerp(alturaOriginal, alturaSalto, t);
            transform.position = new Vector3(transform.position.x, nuevaY, 0);
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        t = 0;
        while(t < 1) {
            t += Time.deltaTime * 3f;
            float nuevaY = Mathf.Lerp(alturaSalto, alturaOriginal, t);
            transform.position = new Vector3(transform.position.x, nuevaY, 0);
            yield return null;
        }
        saltando = false;
    }
}