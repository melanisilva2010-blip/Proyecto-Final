using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float velodidad = 5f;
    public float fuerzaSalto = 5f;
    public float longitudRaycast = 0.1f;
    public LayerMask capaSuelo;


    public bool esInmune = true;
    public float vida = 100f;
    public float fuerzaRebote = 10f;
    public float radioDeAtaque = 1.5f;
    public int danoAtaque = 10;
    public float tiempoDeAtaque = 0.5f;

    [Header("UI y Puntuación")]
    public int monedas = 0;
    public TextMeshProUGUI textoMonedas;

    private bool enSuelo;
    private Rigidbody2D rb;

    private bool atacando;
    private bool recibiendoDano;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (recibiendoDano) return;

        float velocidadX = Input.GetAxis("Horizontal") * Time.deltaTime * velodidad;
        Vector3 posicion = transform.position;
        transform.position = new Vector3(velocidadX + posicion.x, posicion.y, posicion.z);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, capaSuelo);
        enSuelo = hit.collider != null;

        if (enSuelo && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
        }

        if (Input.GetMouseButtonDown(0) && !atacando)
        {
            Atacando();
        }
    }

    private void Atacando()
    {
        atacando = true;
        Debug.Log("PlayerController: ¡Ataque en área!");

        Collider2D[] objetosGolpeados = Physics2D.OverlapCircleAll(transform.position, radioDeAtaque);

        foreach (Collider2D objeto in objetosGolpeados)
        {
            EnemyController enemigo = objeto.GetComponent<EnemyController>();

            if (enemigo != null)
            {
                enemigo.TomarDano(transform.position, danoAtaque);
                Debug.Log("PlayerController: ¡Le di a un enemigo!");
            }
        }

        Invoke("NoAtaca", tiempoDeAtaque);
    }

    public void NoAtaca()
    {
        atacando = false;
    }

    public void RecibeDano(Vector2 posicionAtacante, int cantidad)
    {
        if (recibiendoDano) return;

        if (!esInmune)
        {
            vida -= cantidad;
            Debug.Log("PlayerController: Auch! Vida restante: " + vida);
        }

        recibiendoDano = true;

        Vector2 direccionRebote = ((Vector2)transform.position - posicionAtacante).normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(direccionRebote * fuerzaRebote, ForceMode2D.Impulse);

        Invoke("RecuperarControl", 0.3f);
    }

    private void RecuperarControl()
    {
        recibiendoDano = false;
        rb.velocity = Vector2.zero;
    }

    public void SumarMonedas(int cantidad)
    {
        monedas += cantidad;

        if (textoMonedas != null)
        {
            textoMonedas.text = "Monedas: " + monedas.ToString();
        }

        Debug.Log("PlayerController: Recibí monedas. Total: " + monedas);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioDeAtaque);
    }
}