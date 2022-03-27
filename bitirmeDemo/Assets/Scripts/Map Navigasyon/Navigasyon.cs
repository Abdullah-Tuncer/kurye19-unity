using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LineRenderer))]

public class Navigasyon : MonoBehaviour
{
    private NavMeshAgent myNavMeshAgent;
    private LineRenderer myLineRenderer;

    public Camera aracKamerasi;
    public Camera mapKamerasi;
    public GameObject navigasyon;
    public GameObject arac;
    private bool yolCizili;          //yol çizili olup olmadığı kontrolü
    private bool mapAcik;            //map açıldığında işaret atılabilir

    [SerializeField] private GameObject clickMarkerPrefab;
    [SerializeField] private Transform visualObjectsParent;

    [SerializeField] private GameObject[] mainUI;

    private Vector3 merkeziSurukle;

    // Start is called before the first frame update
    private void Start()
    {
        myNavMeshAgent = GetComponent<NavMeshAgent>();
        myLineRenderer = GetComponent<LineRenderer>();

        myLineRenderer.startWidth = 0.15f;
        myLineRenderer.endWidth = 0.15f;
        myLineRenderer.positionCount = 0;

        yolCizili = false;
        mapAcik = false;
        mapKamerasi.enabled = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            mapAcik = !mapAcik;
            GameObject GO_target = GameObject.FindGameObjectWithTag("Arac");
            mapKamerasi.transform.position = new Vector3(GO_target.transform.position.x, mapKamerasi.transform.position.y, GO_target.transform.position.z);
        }
        KameraDegis();
        navigasyon.transform.position = arac.transform.position;        //yoldan çıkınca bile yolu gösterir

        if (mapAcik) PanCamera();
         

        if (Input.GetMouseButtonDown(1) && mapAcik && !yolCizili)       //sağ tıklanan yeri işaretle
        {
            GidilecekKonum();
            myLineRenderer.enabled = true;
            yolCizili = true;
        }

        else if (Input.GetMouseButtonDown(1) && mapAcik && yolCizili)
        {
            YoluKaldir();
        }
        else if (myNavMeshAgent.hasPath)
        {
            YoluCiz();
        }
    }

    private void KameraDegis()
    {
        if (mapAcik)
        {
            foreach (GameObject ui in mainUI)
                ui.SetActive(false);
            mapKamerasi.enabled = true;
            aracKamerasi.enabled = false;
        }
        else
        {
            foreach (GameObject ui in mainUI)
                ui.SetActive(true);
            mapKamerasi.enabled = false;
            aracKamerasi.enabled = true;
        }
    }

    private void GidilecekKonum()
    {
        Ray ray = mapKamerasi.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hasHit = Physics.Raycast(ray, out hit);
        if (hasHit)
        {
            WaypointKoy(hit.point);
        }
    }

    private void WaypointKoy(Vector3 target)     //gidilecek hedef
    {
        clickMarkerPrefab.transform.SetParent(visualObjectsParent);
        clickMarkerPrefab.SetActive(true);
        clickMarkerPrefab.transform.position = target;
        myNavMeshAgent.SetDestination(target);

    }

    private void YoluCiz()         //yolu çizer
    {
        myLineRenderer.positionCount = myNavMeshAgent.path.corners.Length;
        myLineRenderer.SetPosition(0, transform.position);

        if (myNavMeshAgent.path.corners.Length < 2)
        {
            return;
        }

        for (int i = 1; i < myNavMeshAgent.path.corners.Length; i++)
        {
            Vector3 pointPosition = new Vector3(myNavMeshAgent.path.corners[i].x, myNavMeshAgent.path.corners[i].y, myNavMeshAgent.path.corners[i].z);
            myLineRenderer.SetPosition(i, pointPosition);
        }
    }

    public void YoluKaldir()
    {
        clickMarkerPrefab.transform.SetParent(transform);
        clickMarkerPrefab.SetActive(false);
        myLineRenderer.enabled = false;
        yolCizili = false;
    }

    private void PanCamera()
    {
        //sürükleme başladığında farenin konumunu dünya alanında kaydet (ilk kez tıklandığında)

        if (Input.GetMouseButtonDown(0))
            merkeziSurukle = mapKamerasi.ScreenToWorldPoint(Input.mousePosition);

        //Hala basılı tutuluyorsa sürükleme orijini ile yeni konum arasındaki mesafeyi hesapla

        if (Input.GetMouseButton(0))
        {
            Vector3 difference = merkeziSurukle - mapKamerasi.ScreenToWorldPoint(Input.mousePosition);

            print("origin " + merkeziSurukle + " newPosition " + mapKamerasi.ScreenToWorldPoint(Input.mousePosition) + " =difference " + difference);

            //kamerayı bu mesafe kadar hareket ettir

            mapKamerasi.transform.position += difference;
        }
    }
}
