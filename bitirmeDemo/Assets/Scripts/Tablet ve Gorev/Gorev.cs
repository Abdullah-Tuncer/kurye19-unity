using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mono.Data.Sqlite;
using System.Data;
using System;
public class Gorev : MonoBehaviour
{
    [SerializeField] private GameObject tablet;
    [SerializeField] private GameObject baslangicNoktasi;
    [SerializeField] private GameObject bitisNoktasi;
    private bool yukDurumu = false;
    private int gorevUcreti = 0;

    [Header("Uyarılar")]
    [SerializeField] private GameObject uYukAl;
    [SerializeField] private GameObject uYukBırak;
    [SerializeField] private GameObject uYukYok;
    [SerializeField] private GameObject uAracUygunDegil;

    [Header("Yük Göstergesi")]
    [SerializeField] private GameObject imgYukVar;
    [SerializeField] private GameObject imgYukYok;


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

        baslangicNoktasi.SetActive(false);
        bitisNoktasi.SetActive(false);

        imgYukYok.SetActive(false);
        imgYukVar.SetActive(false);

        uYukAl.SetActive(false);
        uYukBırak.SetActive(false);
        uYukYok.SetActive(false);
        uAracUygunDegil.SetActive(false);

        GorevKontrol();
    }

   
    public void GorevKontrol()
    {
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open();
            //Sorgu
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "SELECT Baslangic_Kapi_NO, Bitis_Kapi_NO, Ucret, Yuk_Durumu FROM GorevListesi WHERE Secilme_Durumu=1";
            dbcmd.CommandText = sqlQuery;

            //Görev var mı kontrol etmek için sayaç
            int SatirSayisi = Convert.ToInt32(dbcmd.ExecuteScalar());

            //sorguyu çalıştır
            reader = dbcmd.ExecuteReader();

            // Seçili görev varsa
            if (SatirSayisi != 0)
            {
                float baslangicNoktasi = 0;
                float bitisNoktasi = 0;

                while (reader.Read())
                {
                    float db_Baslangic = reader.GetFloat(0);
                    baslangicNoktasi = db_Baslangic;

                    float db_Bitis = reader.GetFloat(1);
                    bitisNoktasi = db_Bitis;

                    int db_Ucret = reader.GetInt32(2);
                    gorevUcreti = db_Ucret;

                    int db_YukDurum = reader.GetInt32(3);
                    yukDurumu = Convert.ToBoolean(db_YukDurum);

                }
                BaslangicBitisNoktasiniAyarla(baslangicNoktasi, bitisNoktasi);
                if (yukDurumu)
                {
                    imgYukVar.SetActive(true);
                    imgYukYok.SetActive(false);
                }
                else
                {
                    imgYukYok.SetActive(true);
                    imgYukVar.SetActive(false);
                }
            }
            else
            {
                baslangicNoktasi.SetActive(false);
                bitisNoktasi.SetActive(false);
                imgYukYok.SetActive(false);
                imgYukVar.SetActive(false);
                uYukBırak.SetActive(false);
                uYukYok.SetActive(false);
            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }
    }

    private void BaslangicBitisNoktasiniAyarla(float baslangic, float bitis)
    {
        baslangicNoktasi.SetActive(true);
        bitisNoktasi.SetActive(true);

        // adres alımı ve konumlandırma
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open();
            //Sorgu
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "SELECT X_Konumu, Y_Konumu, Z_Konumu FROM Adresler WHERE Kapi_NO=" + baslangic;
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                float db_X = reader.GetFloat(0);
                float db_Y = reader.GetFloat(1);
                float db_Z = reader.GetFloat(2);

                baslangicNoktasi.transform.position = new Vector3(db_X, db_Y, db_Z);
            }

            dbcmd = dbconn.CreateCommand();
            sqlQuery = "SELECT X_Konumu, Y_Konumu, Z_Konumu FROM Adresler WHERE Kapi_NO=" + bitis;
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                float db_X = reader.GetFloat(0);
                float db_Y = reader.GetFloat(1);
                float db_Z = reader.GetFloat(2);

                bitisNoktasi.transform.position = new Vector3(db_X, db_Y, db_Z);
            }
        }
    }

    public void GorevAlmaAlani()
    {
        bool aracUygunMu = AracUygunMu();
        if (aracUygunMu)
            uYukAl.SetActive(true);
        else
            uAracUygunDegil.SetActive(true);
        if (Input.GetKeyUp(KeyCode.E))
        {
            using (dbconn = new SqliteConnection(conn))
            {
                dbconn.Open();

                if (!yukDurumu && aracUygunMu)
                {
                    dbcmd = dbconn.CreateCommand();
                    sqlQuery = "UPDATE GorevListesi SET Yuk_Durumu=1 WHERE Secilme_Durumu=1";
                    dbcmd.CommandText = sqlQuery;
                    reader = dbcmd.ExecuteReader();
                    yukDurumu = true;
                    imgYukVar.SetActive(true);
                    imgYukYok.SetActive(false);
                }
                else if (yukDurumu)
                {
                    dbcmd = dbconn.CreateCommand();
                    sqlQuery = "UPDATE GorevListesi SET Yuk_Durumu=0 WHERE Secilme_Durumu=1";
                    dbcmd.CommandText = sqlQuery;
                    reader = dbcmd.ExecuteReader();
                    yukDurumu = false;
                    imgYukVar.SetActive(false);
                    imgYukYok.SetActive(true);
                }
            }
        }
    }

    public bool AracUygunMu()
    {
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open();

            // Kullanılan aracın indexi+1
            int kullanilanAracID = (GameObject.FindGameObjectWithTag("Arac").transform.GetSiblingIndex()) + 1;

            // Arac gorev türü
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "SELECT Arac_Gorev_Turu FROM AracBilgileri WHERE Arac_ID=" + kullanilanAracID;
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();
            string aracGorevTuru = null;
            while (reader.Read())
                aracGorevTuru = reader.GetString(0);

            // Görevin görev türü
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "SELECT Gorev_Turu FROM GorevListesi WHERE Secilme_Durumu=1";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();
            int gorevTuru = 0;
            while (reader.Read())
                gorevTuru = reader.GetInt32(0);

            int deger = aracGorevTuru.IndexOf(gorevTuru.ToString());

            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            if (deger == -1)
                return false;
            else
                return true;
        }
    }

    public void GorevBirakmaAlani()
    {
        if (yukDurumu)
        {
            uYukBırak.SetActive(true);
            uYukYok.SetActive(false);
        }
        else
        {
            uYukBırak.SetActive(false);
            uYukYok.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            using (dbconn = new SqliteConnection(conn))
            {
                dbconn.Open();
                
                if (yukDurumu)
                {
                    // Butonları güncellemek için görev türü ve bakiyeyi güncellemek için ücret bilgisi alınıyor
                    dbcmd = dbconn.CreateCommand();
                    sqlQuery = "SELECT Gorev_Turu, Ucret FROM GorevListesi WHERE Secilme_Durumu=1";
                    dbcmd.CommandText = sqlQuery;
                    reader = dbcmd.ExecuteReader();
                    int gorevTuru = 0;
                    int ucret = 0;
                    while (reader.Read())
                    {
                        gorevTuru = reader.GetInt32(0);
                        ucret = reader.GetInt32(1);
                    }

                    // Mevcut bakiye alınıyor
                    dbcmd = dbconn.CreateCommand();
                    sqlQuery = "SELECT Bakiye FROM Kayit WHERE Kayit_ID=1";
                    dbcmd.CommandText = sqlQuery;
                    reader = dbcmd.ExecuteReader();
                    int mevcutBakiye = 0;
                    while (reader.Read())
                        mevcutBakiye = reader.GetInt32(0);

                    // Bakiye güncelleniyor
                    dbcmd = dbconn.CreateCommand();
                    sqlQuery = "UPDATE Kayit SET Bakiye=" + (mevcutBakiye + ucret) + " WHERE Kayit_ID=1";
                    dbcmd.CommandText = sqlQuery;
                    reader = dbcmd.ExecuteReader();

                    // Görev teslim ediliyor
                    dbcmd = dbconn.CreateCommand();
                    sqlQuery = "UPDATE GorevListesi SET Secilme_Durumu=0, Yuk_Durumu=0, Tamamlanma_Durumu=1 WHERE Secilme_Durumu=1";
                    dbcmd.CommandText = sqlQuery;
                    reader = dbcmd.ExecuteReader();
                    yukDurumu = false;

                    tablet.GetComponent<Tablet>().GetGorevleriGetir(gorevTuru);
                }
                else
                {
                    Debug.LogWarning(yukDurumu+" Yükü almadın");

                }

            }
        }
    }

    public void YukUyari(bool AlBırak)
    {
        // AlBırak true -> Başlangıçtan çıkış false -> Bitişten çıkış
        if (AlBırak)
        {
            uYukAl.SetActive(false);
            uAracUygunDegil.SetActive(false);
        }
        else
        {
            uYukYok.SetActive(false);
            uYukBırak.SetActive(false);
        }
    }
}
