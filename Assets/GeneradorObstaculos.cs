using UnityEngine;

public class GeneradorObstaculos : MonoBehaviour
{
    public GameObject obstaculoPrefab; // Arrastra aquí tu cuadrado rojo (el Prefab)
    public float tiempoEntreObstaculos = 2.0f;
    public float distanciaCarril = 2.0f;
    private float cronometro = 0;

    void Update()
    {
        cronometro += Time.deltaTime;

        if (cronometro >= tiempoEntreObstaculos)
        {
            Generar();
            cronometro = 0;
        }
    }

    void Generar()
    {
        // Usamos un array para definir EXACTAMENTE las 3 alturas posibles
        // Si tu distanciaCarril es 2.0, las alturas son -2, 0 y 2.
        float[] alturasPosibles = { -distanciaCarril, 0f, distanciaCarril };

        // Elegimos un índice al azar entre 0, 1 y 2
        int indiceAzar = Random.Range(0, 3);
        float yPos = alturasPosibles[indiceAzar]+transform.parent.position.y;

        // Crear el obstáculo en la posición X:12 y la Y elegida
        Vector3 posicionAparicion = new Vector3(12f, yPos, 0f);
        Instantiate(obstaculoPrefab, posicionAparicion, Quaternion.identity);
    }
}
