using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Mono.Data.Sqlite;
using System.Data;
using System;

public class YakitAlButon : MonoBehaviour
{
    [SerializeField] private Canvas yakitAlCanvas;
    [SerializeField] private Button onYakitAlButon;
    [SerializeField] private Button yakitFulleButon;
    
    float yakitKapasitesi = 0f;
    float mevcutYakit = 0f;

    [SerializeField] private GameObject player;
    private GameObject kullanilanArac;

    //database
    string conn;
    string sqlQuery;
    IDbConnection dbconn;
    IDataReader reader;
    IDbCommand dbcmd;

    private void Start()
    {
        //database bağlantısı
        conn = "URI=file:" + Application.dataPath + "/Plugins/Data.db";
        dbconn = (IDbConnection)new SqliteConnection(conn);
    }

    private void Update()
    {
        // kullanılan aracı bul
        for (int i = 0; i < player.transform.childCount - 2; i++)
        {
            if (player.transform.GetChild(i).gameObject.activeSelf == true)
            {
                kullanilanArac = player.transform.GetChild(i).gameObject;
            }
        }
        yakitKapasitesi = kullanilanArac.GetComponent<YakitSistemi>().yakitKapasitesi;
        mevcutYakit = kullanilanArac.GetComponent<YakitSistemi>().mevcutYakit;
        ButonlariGuncelle();
    }

    public void ButonlariGuncelle()
    {
        int alinabilenYakit = (int)(yakitKapasitesi - mevcutYakit);
        yakitFulleButon.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Fulle\n(-" + alinabilenYakit + " $)";
    }


    public void YakitAl()
    {
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open();
            // Bakiyeyi al
            int bakiye = 0;
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "SELECT Bakiye FROM Kayit WHERE Kayit_ID=1";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();
            while (reader.Read())
                bakiye = reader.GetInt32(0);
            
            float alinabilenYakit = yakitKapasitesi - mevcutYakit;
            if (bakiye >= alinabilenYakit || bakiye >= 10)
            {
                if (alinabilenYakit > 10)
                {
                    dbcmd = dbconn.CreateCommand();
                    sqlQuery = "UPDATE Kayit SET Bakiye=" + (bakiye - 10) + " WHERE Kayit_ID=1";
                    dbcmd.CommandText = sqlQuery;
                    reader = dbcmd.ExecuteReader();
                    mevcutYakit += 10f;
                }
                else
                {
                    dbcmd = dbconn.CreateCommand();
                    sqlQuery = "UPDATE Kayit SET Bakiye=" + (bakiye - alinabilenYakit) + " WHERE Kayit_ID=1";
                    dbcmd.CommandText = sqlQuery;
                    reader = dbcmd.ExecuteReader();
                    mevcutYakit += alinabilenYakit;
                }
                kullanilanArac.GetComponent<YakitSistemi>().mevcutYakit = mevcutYakit;
            }
            else
                Debug.LogWarning("Yakıt Alacak Para Yok!..");
        }
    }

    public void YakitFulle()
    {
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open();
            // Bakiyeyi al
            int bakiye = 0;
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "SELECT Bakiye FROM Kayit WHERE Kayit_ID=1";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();
            while (reader.Read())
                bakiye = reader.GetInt32(0);

            float alinabilenYakit = yakitKapasitesi - mevcutYakit;
            if (bakiye >= alinabilenYakit)
            {
                dbcmd = dbconn.CreateCommand();
                sqlQuery = "UPDATE Kayit SET Bakiye=" + (bakiye - alinabilenYakit) + " WHERE Kayit_ID=1";
                dbcmd.CommandText = sqlQuery;
                reader = dbcmd.ExecuteReader();
                mevcutYakit += alinabilenYakit;
                kullanilanArac.GetComponent<YakitSistemi>().mevcutYakit = mevcutYakit;
            }
            else
                Debug.LogWarning("Yakıt Alacak Para Yok!..");
        }
    }
}
