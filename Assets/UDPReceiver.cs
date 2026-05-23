using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class UDPReceiver : MonoBehaviour
{
    [Header("Configuración de Red")]
    public string matlabIP = "127.0.0.1";
    public int portRecepción = 5005;
    public int portEnvío = 5006;

    [Header("Referencias UI (Calibración)")]
    public Slider barraProgreso;
    public GameObject panelCalibracion;
    public TextMeshProUGUI textoInstruccion;
    public GameObject menuInicio;

    [Header("Objetos del Juego")]
    public GameObject player;
    public GameObject generadorObstaculos;

    [Header("Datos Recibidos")]
    public string lastReceivedPacket = "";

    private bool midiendo = false;
    private float tiempoActual = 0f;
    Thread receiveThread;
    UdpClient client;

    void Start()
    {
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

        // if (barraProgreso != null) barraProgreso.gameObject.SetActive(false);
    }

    void Update()
    {
        // CHIVATO DE RED: Solo se activa si llega algo
        if (lastReceivedPacket != "") {
            Debug.Log(">>> RED: Ha llegado el paquete: [" + lastReceivedPacket + "]");
        }

        if (lastReceivedPacket != "")
        {
            switch (lastReceivedPacket)
            {
                case "MEDICION_BICEPS":
                    ActivarBarra("¡CONTRAE FUERTE EL BÍCEPS!");
                    lastReceivedPacket = "";
                    break;
                case "MEDICION_TRICEPS":
                    ActivarBarra("¡CONTRAE FUERTE EL TRÍCEPS!");
                    lastReceivedPacket = "";
                    break;
                case "MEDICION_OFF":
                    DesactivarBarra();
                    lastReceivedPacket = "";
                    break;
                case "READY":
                    EmpezarJuego();
                    lastReceivedPacket = "";
                    break;
            }
        }

        if (midiendo)
        {
            tiempoActual += Time.deltaTime;
            barraProgreso.value = tiempoActual;
        }
    }

    void ActivarBarra(string mensaje)
    {
        midiendo = true;
        tiempoActual = 0f;
        if(panelCalibracion != null) panelCalibracion.SetActive(true);
        if(barraProgreso != null) barraProgreso.gameObject.SetActive(true);
        if(textoInstruccion != null) {
            textoInstruccion.gameObject.SetActive(true);
            textoInstruccion.text = mensaje;
        }
    }

    void DesactivarBarra()
    {
        if (barraProgreso != null)
            {
                barraProgreso.value = barraProgreso.maxValue;
            }

        midiendo = false;
        if(panelCalibracion != null) panelCalibracion.SetActive(false);
        if(barraProgreso != null) barraProgreso.gameObject.SetActive(false);
        if(textoInstruccion != null) textoInstruccion.text = "¡Bien hecho! Relaja el brazo...";
    }

    void EmpezarJuego()
    {
        Debug.Log("Calibración finalizada. Iniciando juego...");
        if(player != null) player.SetActive(true);
        if(generadorObstaculos != null) generadorObstaculos.SetActive(true);
        if(menuInicio != null) menuInicio.SetActive(false);
    }

    public void SendStartToMatlab()
    {
        try {
            UdpClient sender = new UdpClient();
            byte[] data = Encoding.UTF8.GetBytes("START");
            sender.Send(data, data.Length, matlabIP, portEnvío);
            sender.Close();
            Debug.Log("Mensaje 'START' enviado a MATLAB");
        } catch (System.Exception e) {
            Debug.LogError("Error al enviar UDP: " + e.Message);
        }
    }

    private void ReceiveData()
    {
        try {
            client = new UdpClient(portRecepción);
            while (true)
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);
                lastReceivedPacket = Encoding.UTF8.GetString(data).Trim().ToUpper();
            }
        } catch (System.Exception e) {
            Debug.LogWarning("Socket cerrado o error: " + e.Message);
        }
    }

    void OnApplicationQuit()
    {
        if (receiveThread != null) receiveThread.Abort();
        if (client != null) client.Close();
    }
}
