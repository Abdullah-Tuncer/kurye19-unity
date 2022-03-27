using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class YakitSistemi : MonoBehaviour
{
    public Slider yakitSlider;      //oyun ekranındaki yakıtı gösteren slider
    public float mevcutYakit;
    public float temelHarcamaMik = 1f;
    float yakitHarcamaMik = 1f;
    bool aracHareketEdiyor;         //araç scriptinden değeri al
    public bool yakitDurumu;
    public float yakitKapasitesi;

    [SerializeField] private Canvas YakitAlCanvas;

    [SerializeField] private GameObject Kontrolcu;


    void Start()
    {
        yakitHarcamaMik = temelHarcamaMik;
        YakitAlCanvas.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        aracHareketEdiyor = Kontrolcu.GetComponent<AracKontrolcu>().aracHareketi;
        if (aracHareketEdiyor && (mevcutYakit > 1))
        {
            if (yakitHarcamaMik > 0)
            {
                yakitHarcamaMik -= Time.deltaTime * 0.6f;
            }
            else
            {
                yakitHarcamaMik = temelHarcamaMik;
                mevcutYakit -= 1f;
            }
            yakitDurumu = true;
        }

        if (mevcutYakit <= 1) yakitDurumu = false;

        float sliderDeger = (mevcutYakit / yakitKapasitesi) * 100;
        yakitSlider.value = sliderDeger;
    }


    /*
        Benzin istasyonlarının önüne GasStation taginde trigger collider yerleştirilir.
        Araç girdiğinde alttaki fonksiyon çalışır
    */

    //benzinliğe giriş
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("GasStation"))
        {
            YakitAlCanvas.enabled = true;
        }
    }

    //benzinlikten çıkış
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("GasStation"))
        {
            YakitAlCanvas.enabled = false;
        }
    }

}
