using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mono.Data.Sqlite;
using System.Data;
using System;

public class ServisMenu : MonoBehaviour
{
    public GameObject mevcutArac;

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

    public void TamirButon()
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
            if (bakiye >= 150)
            {
                float mevcutDayanim = mevcutArac.GetComponent<CarpismaSistemi>().dayaniklilik;
                if (mevcutDayanim != 100f)
                {
                    dbcmd = dbconn.CreateCommand();
                    sqlQuery = "UPDATE Kayit SET Bakiye=" + (bakiye - 150) + " WHERE Kayit_ID=1";
                    dbcmd.CommandText = sqlQuery;
                    reader = dbcmd.ExecuteReader();
                    mevcutArac.GetComponent<CarpismaSistemi>().dayaniklilik = 100f;
                }
            }
            else
                Debug.LogWarning("Yakıt Alacak Para Yok!..");
        }
    }

    public void TamirYakitButon()
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

            if (bakiye >= 200)
            {
                float yakitKapasitesi = mevcutArac.GetComponent<YakitSistemi>().yakitKapasitesi;
                float mevcutYakit = mevcutArac.GetComponent<YakitSistemi>().mevcutYakit;
                float alinabilenYakitMiktari = yakitKapasitesi - mevcutYakit;

                dbcmd = dbconn.CreateCommand();
                sqlQuery = "UPDATE Kayit SET Bakiye=" + (bakiye - 200) + " WHERE Kayit_ID=1";
                dbcmd.CommandText = sqlQuery;
                reader = dbcmd.ExecuteReader();

                mevcutArac.GetComponent<CarpismaSistemi>().dayaniklilik = 100f;
                if (alinabilenYakitMiktari > 25)
                    mevcutArac.GetComponent<YakitSistemi>().mevcutYakit += 25f;
                else
                    mevcutArac.GetComponent<YakitSistemi>().mevcutYakit += alinabilenYakitMiktari;
            }
            else
                Debug.LogWarning("Yakıt Alacak Para Yok!..");
        }
    }
}
