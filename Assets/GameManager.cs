using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    // Hacemos un "Singleton" para acceder fácil desde otros scripts
    public static GameManager Instance { get; private set; }

    [Header("Configuración de Personajes")]
    public GameObject playerPrincipal; // Arrastra tu Personaje aquí
    // Aquí luego arrastrarás los GameObjects de tus otros 2 aliados
    public List<GameObject> listaAliados;

    [Header("Puntos de Teletransporte (Cuarto Batalla)")]
    // Arrastra los GameObjects vacíos que creaste en el Paso 1
    public Transform[] spawnPointsAliados;
    public Transform[] spawnPointsEnemigos;

    [Header("UI y Cámaras")]
    public GameObject canvasMenuBatalla; // Tu menú con botones (Atacar, etc.)
    public Camera camaraPrincipal;

    private Vector3 posicionMundoCaminando; // Aquí guardamos dónde estaba el Player
    private GameObject enemigoActualEnBatalla;

    void Awake()
    {
        // Lógica del Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        // Asegurarnos de que el menú de batalla empiece apagado
        if (canvasMenuBatalla != null) canvasMenuBatalla.SetActive(false);
    }

    // --- ESTA ES LA FUNCIÓN CLAVE ---
    // El enemigo llamará a esto cuando toque al jugador
    public void IniciarBatalla(GameObject enemigoQueAtaco)
    {
        Debug.Log("Iniciando Batalla...");
        enemigoActualEnBatalla = enemigoQueAtaco;

        // 1. Guardar posición actual del mapa
        posicionMundoCaminando = playerPrincipal.transform.position;

        // 2. Desactivar movimiento del Player (para que no siga caminando)
        playerPrincipal.GetComponent<PlayerController>().enabled = false;

        // 3. Desactivar el movimiento e IA del enemigo que nos tocó
        enemigoActualEnBatalla.GetComponent<EnemyController>().enabled = false;

        // 4. Teletransportar al Grupo (Player + 2 aliados) al cuarto de batalla
        // Teletransportamos al principal
        playerPrincipal.transform.position = spawnPointsAliados[0].position;

        // Teletransportamos aliados (si tienes)
        for (int i = 0; i < listaAliados.Count; i++)
        {
            // Ojo: spawnPointsAliados[i+1] porque el 0 es el player principal
            listaAliados[i].transform.position = spawnPointsAliados[i + 1].position;
        }

        // 5. Teletransportar al enemigo al cuarto de batalla
        enemigoActualEnBatalla.transform.position = spawnPointsEnemigos[0].position;

        // 6. Mover Cámara al cuarto de batalla
        // (Asumimos que la cámara es hija del player o tiene un script de seguimiento,
        //  sino, tendremos que mover su transform manualmente aquí).

        // 7. Activar Interfaz de Combate
        if (canvasMenuBatalla != null) canvasMenuBatalla.SetActive(true);

        // [Aquí activarías tu script de 'BattleSystem' por turnos]
    }

    // Función para llamar cuando ganas la pelea
    public void TerminarBatalla()
    {
        // 1. Apagar Menú
        if (canvasMenuBatalla != null) canvasMenuBatalla.SetActive(false);

        // 2. Volver al Player a su posición original en el mapa
        playerPrincipal.transform.position = posicionMundoCaminando;

        // 3. Reactivar movimiento
        playerPrincipal.GetComponent<PlayerController>().enabled = true;

        // 4. Destruir al enemigo derrotado del mapa
        Destroy(enemigoActualEnBatalla);
    }
}