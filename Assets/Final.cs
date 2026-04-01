using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Final : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(DesvanecerPantalla());
            SceneManager.LoadScene(2);
        }
    }
    IEnumerator DesvanecerPantalla()
    {
        canvas.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        canvas.GetComponent<Image>().color = Color.clear;
        yield return new WaitForSeconds(0.6f);
    }
}

