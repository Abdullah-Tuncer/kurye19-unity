using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Mono.Data.Sqlite;
using System.Data;
using System;

public class butonKontrol : MonoBehaviour
{
    [SerializeField] private Canvas GarajCanvas;
    [SerializeField] private GameObject AracPanel;
    [SerializeField] private GameObject GarajMenuPanel;
    [SerializeField] private GameObject UyariPanel;

    public GameObject butonPrefab;

    public GameObject[] araclar;
    public bool garajAc;

    //database
    string conn;
    string sqlQuery;
    IDbConnection dbconn;
    IDataReader reader;
    IDbCommand dbcmd;

    void Start()
    {
        //database bağlantısı
        conn = "URI=file:" + Application.dataPath + "/Plugins/Data.db";
        dbconn = (IDbConnection)new SqliteConnection(conn);

        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open();
            //Sorgu
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "SELECT Arac_ID, Yakit, Dayaniklilik FROM AracBilgileri WHERE Kullanilma_Durumu=1";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();

            int kullanilanArac = 0;
            float kullanilanAracYakit = 0;
            float kullanilanAracDayanim = 0;
            while (reader.Read())
            {
                kullanilanArac = reader.GetInt32(0);
                kullanilanAracYakit = reader.GetFloat(1);
                kullanilanAracDayanim = reader.GetFloat(2);
            }

            foreach (GameObject arac in araclar)
                arac.SetActive(false);

            araclar[kullanilanArac - 1].SetActive(true);
            araclar[kullanilanArac - 1].GetComponent<YakitSistemi>().mevcutYakit = kullanilanAracYakit;
            araclar[kullanilanArac - 1].GetComponent<CarpismaSistemi>().dayaniklilik = kullanilanAracDayanim;

            dbcmd = dbconn.CreateCommand();
            sqlQuery = "SELECT X_Konum, Y_Konum, Z_Konum, Y_Acisi FROM Kayit";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                araclar[kullanilanArac - 1].transform.position = new Vector3(reader.GetFloat(0), reader.GetFloat(1), reader.GetFloat(2));
                araclar[kullanilanArac-1].transform.rotation = Quaternion.Euler(0, reader.GetFloat(3), 0);
            }

            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }
    }

    public void MenuyuAc(bool garajDurumu)
    {
        if (garajDurumu)
        {
            GarajCanvas.enabled = true;
            if (YukVar())
            {
                GarajMenuPanel.SetActive(false);
                UyariPanel.SetActive(true);
            }
            else
            {
                GarajMenuPanel.SetActive(true);
                UyariPanel.SetActive(false);
            }
        }
        else
        {
            GarajCanvas.enabled = false;
        }
    }

    public void TamamButon()
    {
        UyariPanel.SetActive(false);
        GarajCanvas.enabled = false;
    }

    public void ButonlariGuncelle()
    {
        // Eski butonları sil
        foreach (Transform child in AracPanel.transform)
            GameObject.Destroy(child.gameObject);

        // Yeni butonları oluştur
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open();
            //Sorgu
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "SELECT Arac_ID, Arac_Adi, Kullanilma_Durumu FROM AracBilgileri WHERE Sahiplik_Durumu=1";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {

                int DB_Arac_ID = reader.GetInt32(0);
                String DB_Arac_Adi = reader.GetString(1);
                bool DB_Kullanilma_Durumu = Convert.ToBoolean(reader.GetInt32(2));

                GameObject buton = (GameObject)Instantiate(butonPrefab);
                buton.transform.SetParent(AracPanel.transform);

                buton.GetComponent<Button>().onClick.AddListener(() => OnClick(DB_Arac_ID));

                buton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = DB_Arac_Adi;
                if (DB_Kullanilma_Durumu)
                    buton.transform.GetComponent<Button>().interactable = false;

            }

            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }
    }

    private bool YukVar()
    {
        int SatirSayisi;
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open();
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "SELECT Gorev_ID FROM GorevListesi WHERE Yuk_Durumu=1";
            dbcmd.CommandText = sqlQuery;
            SatirSayisi = Convert.ToInt32(dbcmd.ExecuteScalar());
        }

        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
        if (SatirSayisi != 0) return true;
        else return false;
    }


    void OnClick(int butonID)
    {
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open();

            // Şuan kullanılan araç
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "SELECT Arac_ID FROM AracBilgileri WHERE Kullanilma_Durumu=1";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();
            int kullanilanArac = 0;
            while (reader.Read())
                kullanilanArac = reader.GetInt32(0);

            // Şuan kullanılan araç kullanılmıyor olarak databasede değiştirildi ve yakıt-dayanıklılık bilgileri güncellendi
            String kullanilanAracMevcutYakit = ((araclar[kullanilanArac - 1].GetComponent<YakitSistemi>().mevcutYakit).ToString()).Replace(",", ".");
            String kullanilanAracMevcutDayanim = ((araclar[kullanilanArac - 1].GetComponent<CarpismaSistemi>().dayaniklilik).ToString()).Replace(",", ".");
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "UPDATE AracBilgileri SET Yakit=" + kullanilanAracMevcutYakit + ", Dayaniklilik=" + kullanilanAracMevcutDayanim + ", Kullanilma_Durumu=0 WHERE Kullanilma_Durumu=1";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();

            // Kullanılcak olan araç kullanılıyor olarak databasede değiştirildi ve yakıt-dayanıklılık bilgileri alındı
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "SELECT Yakit, Dayaniklilik FROM AracBilgileri WHERE Arac_ID=" + butonID;
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();
            float kullanilacakAracMevcutYakit = 0;
            float kullanilacakAracMevcutDayanim = 0;
            while (reader.Read())
            {
                kullanilacakAracMevcutYakit = reader.GetFloat(0);
                kullanilacakAracMevcutDayanim = reader.GetFloat(1);
            }

            dbcmd = dbconn.CreateCommand();
            sqlQuery = "UPDATE AracBilgileri SET Kullanilma_Durumu=1 WHERE Arac_ID=" + butonID;
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();

            araclar[kullanilanArac - 1].SetActive(false);
            araclar[kullanilanArac - 1].transform.Find("navigasyon").transform.GetComponent<Navigasyon>().YoluKaldir();     // Navigasyonda yol çiziliyse kaldır

            araclar[butonID - 1].SetActive(true);
            araclar[butonID - 1].GetComponent<YakitSistemi>().mevcutYakit = kullanilacakAracMevcutYakit;
            araclar[butonID - 1].GetComponent<CarpismaSistemi>().dayaniklilik = kullanilacakAracMevcutDayanim;
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        ButonlariGuncelle();
    }
}
