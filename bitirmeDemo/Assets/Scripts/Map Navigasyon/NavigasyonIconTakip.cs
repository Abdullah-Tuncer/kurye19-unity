using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigasyonIconTakip : MonoBehaviour
{
    private Transform target;
    
    // Update is called once per frame
    private void FixedUpdate()
    {
        GameObject GO_target = GameObject.FindGameObjectWithTag("Arac");
        target = GO_target.transform;
        transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
        transform.rotation = Quaternion.Euler(90, target.eulerAngles.y, 0);
    }

}
