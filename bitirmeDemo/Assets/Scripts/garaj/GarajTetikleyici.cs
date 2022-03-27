using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarajTetikleyici : MonoBehaviour
{

    [SerializeField] private GameObject tetikleyici;

    // Start is called before the first frame update
    void Start()
    {
        tetikleyici.GetComponent<butonKontrol>().MenuyuAc(false);
    }

    // Garaja giriş
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Garaj"))
        {
            tetikleyici.GetComponent<butonKontrol>().MenuyuAc(true);
            tetikleyici.GetComponent<butonKontrol>().ButonlariGuncelle();
        }
    }

    // garajdan çıkış
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Garaj"))
        {
            tetikleyici.GetComponent<butonKontrol>().MenuyuAc(false);
        }
    }
}
