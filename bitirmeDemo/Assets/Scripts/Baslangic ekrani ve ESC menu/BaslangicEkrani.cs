using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

using Mono.Data.Sqlite;
using System.Data;
using System;

public class BaslangicEkrani : MonoBehaviour
{
    [SerializeField] private GameObject devamEtButon;
    [SerializeField] private GameObject kayitliOyunUyari;
    [SerializeField] private GameObject anaPanel;
    [SerializeField] private GameObject ayarPanel;
    [SerializeField] private GameObject[] grafikButonlari;


    //database
    string conn;
    string sqlQuery;
    IDbConnection dbconn;
    IDataReader reader;
    IDbCommand dbcmd;

    void Start()
    {
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Grafic_Level"));
        //database bağlantısı
        conn = "URI=file:" + Application.dataPath + "/Plugins/Data.db";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        DevamEtButonGuncelle();
        anaPanel.SetActive(true);
        ayarPanel.SetActive(false);
        kayitliOyunUyari.SetActive(false);
    }

    private void DevamEtButonGuncelle()
    {
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open();
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "SELECT Kayit_Durumu FROM Kayit";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();
            bool kayitVarMi = false;
            while (reader.Read())
                kayitVarMi = Convert.ToBoolean(reader.GetInt32(0));

            devamEtButon.SetActive(kayitVarMi);
        }
    }

    public void DevamEtButon()
    {
        SceneManager.LoadScene(1);
    }

    public void YeniOyunButon()
    {
        

        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open();
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "SELECT Kayit_Durumu FROM Kayit";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();
            bool kayitVarMi = false;
            while (reader.Read())
                kayitVarMi = Convert.ToBoolean(reader.GetInt32(0));

            if (kayitVarMi)
                kayitliOyunUyari.SetActive(true);
            else
                YeniOyunOlustur();
        }
    }

    public void YeniOyunOlustur()
    {
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open();

            // Kayit
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "UPDATE Kayit SET Bakiye=1000, X_Konum=-37.0, Y_Konum=0.5, Z_Konum=5.0, Y_Acisi=0.0, Kayit_Durumu=1 WHERE Kayit_ID=1";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();

            // GörevListesi
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "DELETE FROM GorevListesi";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();

            //AracBilgileri
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "UPDATE AracBilgileri SET Sahiplik_Durumu=0, Kullanilma_Durumu=0 WHERE Sahiplik_Durumu=1";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();

            string[] yakitDegerleri = { "90.0", "150.0", "90.0", "150.0", "125.0", "160.0", "150.0", "175.0", "150.0", "175.0", "175.0", "250.0", "200.0", "300.0", "200.0" };
            for (int i = 1; i < 16; i++)
            {
                dbcmd = dbconn.CreateCommand();
                sqlQuery = "UPDATE AracBilgileri SET Yakit=" + yakitDegerleri[i - 1] + ", Dayaniklilik=100.0 WHERE Arac_ID=" + i;
                dbcmd.CommandText = sqlQuery;
                reader = dbcmd.ExecuteReader();
            }

            dbcmd = dbconn.CreateCommand();
            sqlQuery = "UPDATE AracBilgileri SET Sahiplik_Durumu=1, Kullanilma_Durumu=1 WHERE Arac_ID=1";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();

            SceneManager.LoadScene(1);
        }
    }

    public void KayitliOyunUyariHayirButon()
    {
        kayitliOyunUyari.SetActive(false);
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

    public void CikisButon()
    {
        Application.Quit();
    }
}
