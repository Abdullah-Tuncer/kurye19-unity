using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapZoom : MonoBehaviour
{

    [SerializeField] private Camera mapKamerasi;

    [Header("Yakınlaştırma ve Sürükleme")]
    [SerializeField] private float zoomOrani;
    [SerializeField] private float minKameraBoyutu;
    [SerializeField] private float maxKameraBoyutu;

    public void ZoomIn()
    {
        float yeniBoyut = mapKamerasi.orthographicSize - zoomOrani;
        mapKamerasi.orthographicSize = Mathf.Clamp(yeniBoyut, minKameraBoyutu, maxKameraBoyutu);
    }

    public void ZoomOut()
    {
        float yeniBoyut = mapKamerasi.orthographicSize + zoomOrani;
        mapKamerasi.orthographicSize = Mathf.Clamp(yeniBoyut, minKameraBoyutu, maxKameraBoyutu);
    }
}
