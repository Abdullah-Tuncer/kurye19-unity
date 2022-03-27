using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorSesi : MonoBehaviour
{
    AudioSource audioSource;

    [SerializeField] public float minimumPitch = 0.3f;
    [SerializeField] public float maximumPitch = 1f;

    private float maxHiz = 0f;
    private float mevcutHiz = 0f;
    private float hesaplananPitch = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = minimumPitch;
        maxHiz = GetComponent<AracKontrolcu>().topSpeed;
    }

    void Update()
    {
        mevcutHiz= GetComponent<Rigidbody>().velocity.magnitude;
        hesaplananPitch = (mevcutHiz / maxHiz) * maximumPitch;
        if (hesaplananPitch <= minimumPitch)
            audioSource.pitch = minimumPitch;
        else
            audioSource.pitch = hesaplananPitch;
    }
}
