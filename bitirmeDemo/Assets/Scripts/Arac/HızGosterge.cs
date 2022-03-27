using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HızGosterge : MonoBehaviour
{
    public TextMeshProUGUI hizEkrani;
    public float hiz = 0f;

    // Update is called once per frame
    void Update()
    {
        hiz = GetComponent<Rigidbody>().velocity.magnitude;
        hizEkrani.text = ((int)(hiz*10)).ToString();
    }
}
