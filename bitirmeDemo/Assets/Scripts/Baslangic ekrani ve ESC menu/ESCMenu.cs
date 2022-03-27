using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

using Mono.Data.Sqlite;
using System.Data;
using System;

public class ESCMenu : MonoBehaviour
{
    [SerializeField] private Canvas escMenuCanvas;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject anaPanel;
    [SerializeField] private GameObject ayarPanel;
    [SerializeField] private GameObject[] grafikButonlari;
    private bool escMenuAcik;

    //database
    string conn;
    string sqlQuery;
    IDbConnection dbconn;
    IDataReader reader;
    IDbCommand dbcmd;

    void Start()
    {
        escMenuAcik = false;
        escMenuCanvas.enabled = false;
        anaPanel.SetActive(true);
        ayarPanel.SetActive(false);

        //database bağlantısı
        conn = "URI=file:" + Application.dataPath + "/Plugins/Data.db";
        dbconn = (IDbConnection)new SqliteConnection(conn);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) escMenuAcik = !escMenuAcik;
        MenuAcKapa();
    }

    public void MenuAcKapa()
    {
        if(escMenuAcik)
            escMenuCanvas.enabled = true;
        else
            escMenuCanvas.enabled = false;
    }

    public void DevamEt()
    {
        escMenuAcik = false;
    }

    public void AyarButon()
    {
        anaPanel.SetActive(false);
        ayarPanel.SetActive(true);
        ButonlariGuncelle();
    }

    public void ButonlariGuncelle()
    {
        int grafikLevel = QualitySettings.GetQualityLevel();
        foreach (GameObject buton in grafikButonlari)
            buton.GetComponent<Button>().interactable = true;
        grafikButonlari[grafikLevel].GetComponent<Button>().interactable = false;
    }

    public void GrafikDusuk()
    {
        PlayerPrefs.SetInt("Grafic_Level", 0);
        QualitySettings.SetQualityLevel(0);
        ButonlariGuncelle();
    }

    public void GrafikOrta()
    {
        PlayerPrefs.SetInt("Grafic_Level", 1);
        QualitySettings.SetQualityLevel(1);
        ButonlariGuncelle();
    }
    public void GrafikYuksek()
    {
        PlayerPrefs.SetInt("Grafic_Level", 2);
        QualitySettings.SetQualityLevel(2);
        ButonlariGuncelle();
    }

    public void AyarOnayla()
    {
        anaPanel.SetActive(true);
        ayarPanel.SetActive(false);
    }

    public void KaydetAnamenu()
    {
        // kullanılan aracı bul
        GameObject kullanilanArac = null;
        for (int i = 0; i < player.transform.childCount - 1; i++)
        {
            if (player.transform.GetChild(i).gameObject.activeSelf == true)
            {
                kullanilanArac = player.transform.GetChild(i).gameObject;
            }
        }

        Vector3 konum = kullanilanArac.GetComponent<Rigidbody>().worldCenterOfMass;
        String kullanilanAracMevcutYakit = kullanilanArac.GetComponent<YakitSistemi>().mevcutYakit.ToString().Replace(",", ".");
        String kullanilanAracMevcutDayanim = kullanilanArac.GetComponent<CarpismaSistemi>().dayaniklilik.ToString().Replace(",", ".");

        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open();
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "UPDATE AracBilgileri SET Yakit=" + kullanilanAracMevcutYakit + ", Dayaniklilik=" + kullanilanAracMevcutDayanim + " WHERE Kullanilma_Durumu=1";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();

            dbcmd = dbconn.CreateCommand();
            sqlQuery = "UPDATE Kayit SET X_Konum=" + konum.x.ToString().Replace(",", ".") + ", Y_Konum=" + konum.y.ToString().Replace(",", ".") + ", Z_Konum=" + konum.z.ToString().Replace(",", ".") + ", Y_Acisi=" + kullanilanArac.transform.eulerAngles.y.ToString().Replace(",", ".") + " WHERE Kayit_ID=1";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();

        }
        SceneManager.LoadScene(0);
    }

    public void KaydetCik()
    {
        // kullanılan aracı bul
        GameObject kullanilanArac = null;
        for (int i = 0; i < player.transform.childCount - 1; i++)
        {
            if (player.transform.GetChild(i).gameObject.activeSelf == true)
            {
                kullanilanArac = player.transform.GetChild(i).gameObject;
            }
        }

        Vector3 konum = kullanilanArac.GetComponent<Rigidbody>().worldCenterOfMass;
        String kullanilanAracMevcutYakit = kullanilanArac.GetComponent<YakitSistemi>().mevcutYakit.ToString().Replace(",", ".");
        String kullanilanAracMevcutDayanim = kullanilanArac.GetComponent<CarpismaSistemi>().dayaniklilik.ToString().Replace(",", ".");

        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open();
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "UPDATE AracBilgileri SET Yakit=" + kullanilanAracMevcutYakit + ", Dayaniklilik=" + kullanilanAracMevcutDayanim + " WHERE Kullanilma_Durumu=1";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();

            dbcmd = dbconn.CreateCommand();
            sqlQuery = "UPDATE Kayit SET X_Konum=" + konum.x.ToString().Replace(",", ".") + ", Y_Konum=" + konum.y.ToString().Replace(",", ".") + ", Z_Konum=" + konum.z.ToString().Replace(",", ".") + ", Y_Acisi=" + kullanilanArac.transform.eulerAngles.y.ToString().Replace(",", ".") + " WHERE Kayit_ID=1";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();

        }
        Application.Quit();
    }
}
