using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarpismaSistemi : MonoBehaviour
{
    public float dayaniklilik = 100f;
    public float hasarOrani = 0.4f;
    public bool hasarDurumu;
    public Slider dayanimSlider;      //oyun ekranındaki dayanımı gösteren slider
    [SerializeField] private Canvas servisMenuCanvas;

    private void Start()
    {
        servisMenuCanvas.enabled = false;
    }

    private void Update()
    {
        dayanimSlider.value = dayaniklilik;
        if (dayaniklilik > 0) hasarDurumu = true;
        else hasarDurumu = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        float deger = collision.relativeVelocity.magnitude;
        dayaniklilik = dayaniklilik - (deger * hasarOrani);
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Servis"))
        {
            servisMenuCanvas.enabled = true;
            servisMenuCanvas.GetComponent<ServisMenu>().mevcutArac = this.gameObject;

        }
    }

    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Servis"))
        {
            servisMenuCanvas.enabled = false;
        }
    }

}
