using UnityEngine;

public class MovimientoSuelo : MonoBehaviour
{
    public float velocidadSuelo = 0.5f;
    private Material materialSuelo;
    private Vector2 offsetActual = Vector2.zero;

    void Start()
    {
        materialSuelo=GetComponent<SpriteRenderer>().material;
    }

    void Update()
    {
        offsetActual.x += velocidadSuelo * Time.deltaTime;
        materialSuelo.mainTextureOffset = offsetActual;
    }
}