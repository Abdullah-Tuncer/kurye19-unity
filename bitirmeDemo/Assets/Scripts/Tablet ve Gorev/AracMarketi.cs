using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Mono.Data.Sqlite;
using System.Data;
using System;

public class AracMarketi : MonoBehaviour
{
    public Button[] aracButon;

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
        MarketButonlariGuncelle();
    }


    public void MarketButonlariGuncelle()
    {
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open();
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "SELECT Arac_ID, Sahiplik_Durumu FROM AracBilgileri";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                int aracNo = (reader.GetInt32(0)) - 1;
                bool sahipMi = Convert.ToBoolean(reader.GetInt32(1));

                if (sahipMi)
                    aracButon[aracNo].transform.GetComponent<Button>().interactable = false;
                else
                    aracButon[aracNo].transform.GetComponent<Button>().interactable = true;
            }
        }
    }

    public void SatinAlOnClick(int aracID)
    {
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open();
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "SELECT Bakiye FROM Kayit WHERE Kayit_ID=1";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();
            int bakiye = 0;
            while (reader.Read())
                bakiye = reader.GetInt32(0);

            // Buton üzerinden araç fiyatı alındı
            // OnClick fonksiyonlarında birden fazla parametre kullanılamadığı için böyle bir çözüm yolu kullanıldı.
            int aracUcreti = Convert.ToInt32(aracButon[aracID - 1].transform.GetChild(4).GetChild(1).GetComponent<TextMeshProUGUI>().text);

            if (bakiye >= aracUcreti)
            {
                // Ücreti kes
                dbcmd = dbconn.CreateCommand();
                sqlQuery = "UPDATE Kayit SET Bakiye=" + (bakiye - aracUcreti) + " WHERE Kayit_ID=1";
                dbcmd.CommandText = sqlQuery;
                reader = dbcmd.ExecuteReader();

                // Aracı ekle
                dbcmd = dbconn.CreateCommand();
                sqlQuery = "UPDATE AracBilgileri SET Sahiplik_Durumu=1 WHERE Arac_ID=" + aracID;
                dbcmd.CommandText = sqlQuery;
                reader = dbcmd.ExecuteReader();

                GetComponent<Tablet>().TabletBakiyeGüncelle();
                MarketButonlariGuncelle();
            }
            else
                Debug.LogWarning("Bakiye yetersiz!..");
        }
    }
}
