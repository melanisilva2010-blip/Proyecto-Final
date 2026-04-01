using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float detectionRange = 5f;
    public float speed = 200f;

    [Header("Configuración de Combate")]
    public int vidaMax = 30;
    public int danoAlJugador = 10;
    public float fuerzarebote = 5f;
    [SerializeField] private int vidaActual;

    [Header("Referencias")]
    public Transform player;

    private Rigidbody2D rb;
    private bool recibiendoDano;
    private bool estaMuerto = false;
    private Vector2 direccionMovimiento;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        vidaActual = vidaMax;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (player == null || estaMuerto) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange && !recibiendoDano)
        {
            direccionMovimiento = (player.position - transform.position).normalized;
        }
        else
        {
            direccionMovimiento = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        if (estaMuerto || recibiendoDano) return;

        rb.velocity = direccionMovimiento * speed * Time.fixedDeltaTime;
    }

    public void TomarDano(Vector2 posAtaque, int cantidad)
    {
        if (recibiendoDano || estaMuerto) return;

        Debug.Log("Enemigo: Recibí daño.");
        vidaActual -= cantidad;
        recibiendoDano = true;

        Vector2 rebote = ((Vector2)transform.position - posAtaque).normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(rebote * fuerzarebote, ForceMode2D.Impulse);

        StartCoroutine(DesactivaDano());

        if (vidaActual <= 0) Morir();
    }

    IEnumerator DesactivaDano()
    {
        yield return new WaitForSeconds(0.4f);
        recibiendoDano = false;
    }

    void Morir()
    {
        estaMuerto = true;

        if (player != null)
        {
            PlayerController p = player.GetComponent<PlayerController>();
            if (p != null)
            {
                p.SumarMonedas(10);
            }
        }

        GetComponent<Collider2D>().enabled = false;
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        Debug.Log("Enemigo: Me morí.");
        Destroy(gameObject, 1.0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!estaMuerto && collision.CompareTag("Espada"))
        {
            TomarDano(collision.transform.position, 10);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (estaMuerto) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController p = collision.gameObject.GetComponent<PlayerController>();
            if (p != null)
            {
                p.RecibeDano(transform.position, danoAlJugador);
            }
        }
    }
}
