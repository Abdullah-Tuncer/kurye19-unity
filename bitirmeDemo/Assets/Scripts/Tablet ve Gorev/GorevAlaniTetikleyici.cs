using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GorevAlaniTetikleyici : MonoBehaviour
{
    [SerializeField] private GameObject gorev;
    private bool baslangicAlanaGirildi = false;
    private bool bitisAlanaGirildi = false;
    private void FixedUpdate()
    {
        if (baslangicAlanaGirildi)
        {
            gorev.GetComponent<Gorev>().GorevAlmaAlani();
        }
        if (bitisAlanaGirildi)
        {
            gorev.GetComponent<Gorev>().GorevBirakmaAlani();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("GorevBaslangicNoktasi"))
        {
            baslangicAlanaGirildi = true;
        }

        if (other.gameObject.CompareTag("GorevBitisNoktasi"))
        {
            bitisAlanaGirildi = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("GorevBaslangicNoktasi"))
        {
            baslangicAlanaGirildi = false;
            gorev.GetComponent<Gorev>().YukUyari(true);
        }
        if (other.gameObject.CompareTag("GorevBitisNoktasi"))
        {
            bitisAlanaGirildi = false;
            gorev.GetComponent<Gorev>().YukUyari(false);
            gorev.GetComponent<Gorev>().GorevKontrol();
        }
    }
}
