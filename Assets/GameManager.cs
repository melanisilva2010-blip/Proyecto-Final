using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    [Header("Configuración de Personajes")]
    public GameObject playerPrincipal;
    public List<GameObject> listaAliados;

    [Header("Puntos de Teletransporte (Cuarto Batalla)")]

    public Transform[] spawnPointsAliados;
    public Transform[] spawnPointsEnemigos;

    [Header("UI y Cámaras")]
    public GameObject canvasMenuBatalla; 
    public Camera camaraPrincipal;

    private Vector3 posicionMundoCaminando; 
    private GameObject enemigoActualEnBatalla;

    void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        if (canvasMenuBatalla != null) canvasMenuBatalla.SetActive(false);
    }


    public void IniciarBatalla(GameObject enemigoQueAtaco)
    {
        Debug.Log("Iniciando Batalla...");
        enemigoActualEnBatalla = enemigoQueAtaco;

        posicionMundoCaminando = playerPrincipal.transform.position;

        playerPrincipal.GetComponent<PlayerController>().enabled = false;

        enemigoActualEnBatalla.GetComponent<EnemyController>().enabled = false;


        playerPrincipal.transform.position = spawnPointsAliados[0].position;

        for (int i = 0; i < listaAliados.Count; i++)
        {
            listaAliados[i].transform.position = spawnPointsAliados[i + 1].position;
        }

        enemigoActualEnBatalla.transform.position = spawnPointsEnemigos[0].position;

        if (canvasMenuBatalla != null) canvasMenuBatalla.SetActive(true);

    }

    public void TerminarBatalla()
    {
        if (canvasMenuBatalla != null) canvasMenuBatalla.SetActive(false);

        playerPrincipal.transform.position = posicionMundoCaminando;

        playerPrincipal.GetComponent<PlayerController>().enabled = true;

        Destroy(enemigoActualEnBatalla);
    }
}