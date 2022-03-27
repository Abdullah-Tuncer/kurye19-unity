using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AracKontrolcu : MonoBehaviour
{


    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizantalInput;          //yatay girdi
    private float verticalInput;            //dikey girdi
    private float currentSteerAngle;
    private float currentbreakForce;
    private bool isBreaking;                //fren

    public bool aracHareketi;
    [SerializeField] private GameObject yakit;
    private bool yakitDurumu;

    [SerializeField] private GameObject dayaniklilik;
    private bool hasarDurumu;

    [SerializeField] private float motorForce;          //motor gücü
    [SerializeField] private float breakForce;          //fren gücü
    [SerializeField] private float maxSteerAngle;       //teker dönüş açısı
    public float topSpeed;            //yapabileceği max hız

    [Header("Ağırlık Merlezi")]
    public GameObject centerOfMass;
    public Rigidbody rigidBody;

    //teker colliderları
    [Header("Teker Colliderları")]
    [SerializeField] private WheelCollider flc;
    [SerializeField] private WheelCollider frc;
    [SerializeField] private WheelCollider blc;
    [SerializeField] private WheelCollider brc;

    //teker modelleri
    [Header("Teker Modelleri")]
    [SerializeField] private Transform flt;
    [SerializeField] private Transform frt;
    [SerializeField] private Transform blt;
    [SerializeField] private Transform brt;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.centerOfMass = centerOfMass.transform.localPosition;
    }

    private void FixedUpdate()
    {
        yakitDurumu = yakit.GetComponent<YakitSistemi>().yakitDurumu;
        hasarDurumu = dayaniklilik.GetComponent<CarpismaSistemi>().hasarDurumu;
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, topSpeed);
    }

    //klavye girdileri
    private void GetInput()
    {
        horizantalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    //gaz verme
    private void HandleMotor()
    {
        // Yakıt durumuna göre gazlama
        if (yakitDurumu && hasarDurumu)
        {
            flc.motorTorque = verticalInput * motorForce;
            frc.motorTorque = verticalInput * motorForce;
        }
        else
        {
            flc.motorTorque = 0;
            frc.motorTorque = 0;
        }

        if (verticalInput == 0) aracHareketi = false;
        else aracHareketi = true;

        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();

    }

    //frenle
    private void ApplyBreaking()
    {
        flc.brakeTorque = currentbreakForce;
        frc.brakeTorque = currentbreakForce;
        blc.brakeTorque = currentbreakForce;
        brc.brakeTorque = currentbreakForce;

    }

    //direksiyon yönü
    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizantalInput;
        flc.steerAngle = currentSteerAngle;
        frc.steerAngle = currentSteerAngle;
    }

    //teker konumları
    private void UpdateWheels()
    {
        UpdateSingleWheel(flc, flt);
        UpdateSingleWheel(frc, frt);
        UpdateSingleWheel(brc, brt);
        UpdateSingleWheel(blc, blt);
    }

    //tek tekerin konumu
    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        //wheelTransform.position = pos;
    }

}
