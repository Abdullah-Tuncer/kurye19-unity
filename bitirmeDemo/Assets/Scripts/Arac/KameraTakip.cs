using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KameraTakip : MonoBehaviour
{
    [SerializeField] private Vector3 offset;            //kamera konumu
    [SerializeField] private float translateSpeed;      // kamera takip hızı  
    [SerializeField] private float rotationSpeed;       //kamera Donus Hızı
    [SerializeField] private float cameraBody;
    private Transform target;
    

    private void FixedUpdate()
    {
        GameObject GO_target = GameObject.FindGameObjectWithTag("Arac");
        target = GO_target.transform;
        HandleTranslation();
        HandleRotation();
    }

    private void HandleTranslation()
    {
        var targetPosition = target.TransformPoint(offset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, translateSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, 3f, transform.position.z);
    }
    private void HandleRotation()
    {
        var direction = target.position - transform.position;
        var rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }
}
