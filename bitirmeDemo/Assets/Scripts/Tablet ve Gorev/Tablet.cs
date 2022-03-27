using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Mono.Data.Sqlite;
using System.Data;
using System;
public class Tablet : MonoBehaviour
{
    [Header("Tablet Canvası")]
    [SerializeField] private Canvas TabletCanvas;

    [Header("Tablet Panelleri")]
    [SerializeField] private GameObject AnasayfaPanel;
    [SerializeField] private GameObject LokantaPanel;
    [SerializeField] private GameObject MarketPanel;
    [SerializeField] private GameObject NakliyePanel;
    [SerializeField] private GameObject YolcuPanel;
    [SerializeField] private GameObject AracMarketiPanel;

    [Header("Lokanta Görev Butonları")]
    [SerializeField] private Button LokantaButon1;
    [SerializeField] private Button LokantaButon2;
    [SerializeField] private Button LokantaButon3;
    [SerializeField] private Button LokantaButon4;
    [SerializeField] private Button LokantaButon5;
    [SerializeField] private Button LokantaButon6;

    private int LokantaGorevTuru = 1;
    private int LokantaMaxUcret = 51;
    private int LokantaMinUcret = 25;
    private int LokantaBaslangicAdres = 1;             //Lokanta adres id
    private int LokantaYukAgirligi = 1;

    [Header("Market Görev Butonları")]
    [SerializeField] private Button MarketButon1;
    [SerializeField] private Button MarketButon2;
    [SerializeField] private Button MarketButon3;
    [SerializeField] private Button MarketButon4;
    [SerializeField] private Button MarketButon5;
    [SerializeField] private Button MarketButon6;

    private int MarketGorevTuru = 2;
    private int MarketMaxUcret = 101;
    private int MarketMinUcret = 50;
    private int MarketBaslangicAdres = 2;             //Market adres id
    private int MarketYukAgirligi = 2;

    [Header("Nakliye Görev Butonları")]
    [SerializeField] private Button NakliyeButon1;
    [SerializeField] private Button NakliyeButon2;
    [SerializeField] private Button NakliyeButon3;
    [SerializeField] private Button NakliyeButon4;
    [SerializeField] private Button NakliyeButon5;
    [SerializeField] private Button NakliyeButon6;

    private int NakliyeGorevTuru = 3;
    private int NakliyeMaxUcret = 201;
    private int NakliyeMinUcret = 100;
    private int NakliyeBaslangicAdres = 3;             //Nakliye adres id
    private int NakliyeYukAgirligi = 3;

    [Header("Nakliye Görev Butonları")]
    [SerializeField] private Button YolcuButon1;
    [SerializeField] private Button YolcuButon2;
    [SerializeField] private Button YolcuButon3;
    [SerializeField] private Button YolcuButon4;
    [SerializeField] private Button YolcuButon5;
    [SerializeField] private Button YolcuButon6;

    private int YolcuGorevTuru = 4;
    private int YolcuMaxUcret = 76;
    private int YolcuMinUcret = 25;
    private int YolcuBaslangicAdres = -1;             //Yolcu özel adres id
    private int YolcuYukAgirligi = 4;

    public TextMeshProUGUI bakiyeText;

    [SerializeField] private Scrollbar MarketScroll;

    private int AdresSayisi = 102;                //Databasede bulunan toplam database sayısı

    private bool tabletAcik;

    [SerializeField] private GameObject GorevYukUyariPanel;

    public GameObject gorev;

    [SerializeField] private GameObject player;

    //database
    string conn;
    string sqlQuery;
    IDbConnection dbconn;
    IDataReader reader;
    IDbCommand dbcmd;

    // Start is called before the first frame update
    void Start()
    {
        tabletAcik = false;
        TabletCanvas.enabled = false;
        AnasayfaPanel.SetActive(true);
        LokantaPanel.SetActive(false);
        MarketPanel.SetActive(false);
        NakliyePanel.SetActive(false);
        YolcuPanel.SetActive(false);
        AracMarketiPanel.SetActive(false);
        GorevYukUyariPanel.SetActive(false);

        //database bağlantısı
        conn = "URI=file:" + Application.dataPath + "/Plugins/Data.db";
        dbconn = (IDbConnection)new SqliteConnection(conn);

        TabletBakiyeGüncelle();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) tabletAcik = !tabletAcik;
        TabletAcKapa();
    }

    private void TabletAcKapa()
    {
        if (tabletAcik)
        {
            TabletCanvas.enabled = true;
            TabletBakiyeGüncelle();
        }
        else TabletCanvas.enabled = false;
    }

    public void LokantaButon()
    {
        AnasayfaPanel.SetActive(false);
        LokantaPanel.SetActive(true);
        GorevleriGetir(LokantaGorevTuru);
    }

    public void MarketButon()
    {
        AnasayfaPanel.SetActive(false);
        MarketPanel.SetActive(true);
        GorevleriGetir(MarketGorevTuru);
    }

    public void NakliyeButon()
    {
        AnasayfaPanel.SetActive(false);
        NakliyePanel.SetActive(true);
        GorevleriGetir(NakliyeGorevTuru);
    }

    public void YolcuButon()
    {
        AnasayfaPanel.SetActive(false);
        YolcuPanel.SetActive(true);
        GorevleriGetir(YolcuGorevTuru);
    }

    public void AracMarketiButon()
    {
        AnasayfaPanel.SetActive(false);
        AracMarketiPanel.SetActive(true);
        MarketScroll.value = 1;
        GetComponent<AracMarketi>().MarketButonlariGuncelle();
    }

    public void GeriButon()
    {
        AnasayfaPanel.SetActive(true);
        LokantaPanel.SetActive(false);
        MarketPanel.SetActive(false);
        NakliyePanel.SetActive(false);
        YolcuPanel.SetActive(false);
        AracMarketiPanel.SetActive(false);
    }

    public void KurtariciButon()
    {
        using (dbconn = new SqliteConnection(conn))
        {
            // Mevcut bakiye alınıyor
            dbconn.Open();
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "SELECT Bakiye FROM Kayit WHERE Kayit_ID=1";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();
            int mevcutBakiye = 0;
            while (reader.Read())
                mevcutBakiye = reader.GetInt32(0);

            if (mevcutBakiye >= 50)
            {
                // Bakiye güncelleniyor
                dbcmd = dbconn.CreateCommand();
                sqlQuery = "UPDATE Kayit SET Bakiye=" + (mevcutBakiye - 50) + " WHERE Kayit_ID=1";
                dbcmd.CommandText = sqlQuery;
                reader = dbcmd.ExecuteReader();
                TabletBakiyeGüncelle();

                // kullanılan aracı bul
                GameObject kullanilanArac = null;
                for (int i = 0; i < player.transform.childCount - 1; i++)
                {
                    if (player.transform.GetChild(i).gameObject.activeSelf == true)
                    {
                        kullanilanArac = player.transform.GetChild(i).gameObject;
                    }
                }

                kullanilanArac.transform.position = new Vector3(97, 1, -15);         // servis konumu
                kullanilanArac.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
                Debug.LogWarning("Para Yetersiz!..");
        }
    }

    public void TabletBakiyeGüncelle()
    {
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open();
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "SELECT Bakiye FROM Kayit WHERE Kayit_ID=1";
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();
            while (reader.Read())
                bakiyeText.text = (reader.GetInt32(0)).ToString()+" $";
        }
    }

    private void GorevleriGetir(int GorevTuru)
    {
        Button Buton1 = null;
        Button Buton2 = null;
        Button Buton3 = null;
        Button Buton4 = null;
        Button Buton5 = null;
        Button Buton6 = null;

        if (GorevTuru == LokantaGorevTuru)
        {
            //lokanta
            Buton1 = LokantaButon1;
            Buton2 = LokantaButon2;
            Buton3 = LokantaButon3;
            Buton4 = LokantaButon4;
            Buton5 = LokantaButon5;
            Buton6 = LokantaButon6;
        }
        else if (GorevTuru == MarketGorevTuru)
        {
            //market
            Buton1 = MarketButon1;
            Buton2 = MarketButon2;
            Buton3 = MarketButon3;
            Buton4 = MarketButon4;
            Buton5 = MarketButon5;
            Buton6 = MarketButon6;
        }
        else if (GorevTuru == NakliyeGorevTuru)
        {
            //nakliye
            Buton1 = NakliyeButon1;
            Buton2 = NakliyeButon2;
            Buton3 = NakliyeButon3;
            Buton4 = NakliyeButon4;
            Buton5 = NakliyeButon5;
            Buton6 = NakliyeButon6;
        }
        else if (GorevTuru == 4)
        {
            //nakliye
            Buton1 = YolcuButon1;
            Buton2 = YolcuButon2;
            Buton3 = YolcuButon3;
            Buton4 = YolcuButon4;
            Buton5 = YolcuButon5;
            Buton6 = YolcuButon6;
        }

        int secilmisGorevID = SeciliGorevID();

        //database bağlantısı
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open();
            //Sorgu
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "SELECT Gorev_ID, Baslangic_Kapi_NO, Bitis_Kapi_NO, Ucret, Tamamlanma_Durumu, Mahalle FROM GorevListesi WHERE Gorev_Turu=" + GorevTuru;
            dbcmd.CommandText = sqlQuery;

            //Görev var mı kontrol etmek için sayaç
            int SatirSayisi = Convert.ToInt32(dbcmd.ExecuteScalar());

            //sorguyu çalıştır
            reader = dbcmd.ExecuteReader();

            //Görev varsa butonlara ata
            if (SatirSayisi != 0)
            {
                int sayac = 0;

                while (reader.Read())
                {
                    int DB_Id = reader.GetInt32(0);

                    // Yolcu görevleri için özel değer
                    int DB_BaslangicAdresi = 0;
                    if (GorevTuru == 4)
                        DB_BaslangicAdresi = reader.GetInt32(1);

                    int DB_KapiNo = reader.GetInt32(2);
                    int DB_Ucret = reader.GetInt32(3);
                    int DB_Tamamlanma = reader.GetInt32(4);
                    String DB_Mahalle = reader.GetString(5);

                    if (sayac == 0)
                    {
                        if (GorevTuru == 4)
                            ButonText(DB_BaslangicAdresi, DB_KapiNo, DB_Ucret, Buton1);
                        else
                            ButonText(DB_KapiNo, DB_Ucret, Buton1, DB_Mahalle);

                        Buton1.GetComponent<Button>().onClick.RemoveAllListeners();
                        Buton1.GetComponent<Button>().onClick.AddListener(() => OnClick(DB_Id));
                        if (DB_Id != secilmisGorevID && secilmisGorevID != 0 || DB_Tamamlanma == 1)
                            Buton1.transform.GetComponent<Button>().interactable = false;
                        else
                            Buton1.transform.GetComponent<Button>().interactable = true;
                    }
                    else if (sayac == 1)
                    {
                        if (GorevTuru == 4)
                            ButonText(DB_BaslangicAdresi, DB_KapiNo, DB_Ucret, Buton2);
                        else
                            ButonText(DB_KapiNo, DB_Ucret, Buton2, DB_Mahalle);

                        Buton2.GetComponent<Button>().onClick.RemoveAllListeners();
                        Buton2.GetComponent<Button>().onClick.AddListener(() => OnClick(DB_Id));
                        if (DB_Id != secilmisGorevID && secilmisGorevID != 0 || DB_Tamamlanma == 1)
                            Buton2.transform.GetComponent<Button>().interactable = false;
                        else
                            Buton2.transform.GetComponent<Button>().interactable = true;
                    }
                    else if (sayac == 2)
                    {
                        if (GorevTuru == 4)
                            ButonText(DB_BaslangicAdresi, DB_KapiNo, DB_Ucret, Buton3);
                        else
                            ButonText(DB_KapiNo, DB_Ucret, Buton3, DB_Mahalle);

                        Buton3.GetComponent<Button>().onClick.RemoveAllListeners();
                        Buton3.GetComponent<Button>().onClick.AddListener(() => OnClick(DB_Id));
                        if (DB_Id != secilmisGorevID && secilmisGorevID != 0 || DB_Tamamlanma == 1)
                            Buton3.transform.GetComponent<Button>().interactable = false;
                        else
                            Buton3.transform.GetComponent<Button>().interactable = true;
                    }
                    else if (sayac == 3)
                    {
                        if (GorevTuru == 4)
                            ButonText(DB_BaslangicAdresi, DB_KapiNo, DB_Ucret, Buton4);
                        else
                            ButonText(DB_KapiNo, DB_Ucret, Buton4, DB_Mahalle);

                        Buton4.GetComponent<Button>().onClick.RemoveAllListeners();
                        Buton4.GetComponent<Button>().onClick.AddListener(() => OnClick(DB_Id));
                        if (DB_Id != secilmisGorevID && secilmisGorevID != 0 || DB_Tamamlanma == 1)
                            Buton4.transform.GetComponent<Button>().interactable = false;
                        else
                            Buton4.transform.GetComponent<Button>().interactable = true;
                    }
                    else if (sayac == 4)
                    {
                        if (GorevTuru == 4)
                            ButonText(DB_BaslangicAdresi, DB_KapiNo, DB_Ucret, Buton5);
                        else
                            ButonText(DB_KapiNo, DB_Ucret, Buton5, DB_Mahalle);

                        Buton5.GetComponent<Button>().onClick.RemoveAllListeners();
                        Buton5.GetComponent<Button>().onClick.AddListener(() => OnClick(DB_Id));
                        if (DB_Id != secilmisGorevID && secilmisGorevID != 0 || DB_Tamamlanma == 1)
                            Buton5.transform.GetComponent<Button>().interactable = false;
                        else
                            Buton5.transform.GetComponent<Button>().interactable = true;
                    }
                    else if (sayac == 5)
                    {
                        if (GorevTuru == 4)
                            ButonText(DB_BaslangicAdresi, DB_KapiNo, DB_Ucret, Buton6);
                        else
                            ButonText(DB_KapiNo, DB_Ucret, Buton6, DB_Mahalle);

                        Buton6.GetComponent<Button>().onClick.RemoveAllListeners();
                        Buton6.GetComponent<Button>().onClick.AddListener(() => OnClick(DB_Id));
                        if (DB_Id != secilmisGorevID && secilmisGorevID != 0 || DB_Tamamlanma == 1)
                            Buton6.transform.GetComponent<Button>().interactable = false;
                        else
                            Buton6.transform.GetComponent<Button>().interactable = true;
                    }
                    sayac++;
                }
            }
            //Eğer Görev yoksa 
            else GorevOlustur(GorevTuru);
        }
    }

    public void GetGorevleriGetir(int GorevTuru)
    {
        GorevleriGetir(GorevTuru);
    }

    // Butonlardaki yazıları değiştir
    private void ButonText(int DB_KapiNo, int DB_Ucret, Button buton, String DB_Mahalle)
    {
        buton.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = DB_Mahalle;
        buton.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = DB_KapiNo.ToString();
        buton.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = DB_Ucret.ToString();
    }

    // Yolcu Görevleri için Overloading
    private void ButonText(int DB_BaslangicKapiNo, int DB_KapiNo, int DB_Ucret, Button buton)
    {
        buton.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = DB_BaslangicKapiNo.ToString();
        buton.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = DB_KapiNo.ToString();
        buton.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = DB_Ucret.ToString();
    }

    public void GorevOlustur(int GorevTuru)
    {
        int MinUcret = 0;
        int MaxUcret = 0;
        int BaslangicAdres = 0;
        int YukAgirligi = 0;

        if (GorevTuru == LokantaGorevTuru)
        {
            //lokanta
            MinUcret = LokantaMinUcret;
            MaxUcret = LokantaMaxUcret;
            BaslangicAdres = LokantaBaslangicAdres;
            YukAgirligi = LokantaYukAgirligi;
        }
        else if (GorevTuru == MarketGorevTuru)
        {
            //market
            MinUcret = MarketMinUcret;
            MaxUcret = MarketMaxUcret;
            BaslangicAdres = MarketBaslangicAdres;
            YukAgirligi = MarketYukAgirligi;
        }
        else if (GorevTuru == NakliyeGorevTuru)
        {
            //nakliye
            MinUcret = NakliyeMinUcret;
            MaxUcret = NakliyeMaxUcret;
            BaslangicAdres = NakliyeBaslangicAdres;
            YukAgirligi = NakliyeYukAgirligi;
        }
        else if (GorevTuru == YolcuGorevTuru)
        {
            //yolcu
            MinUcret = YolcuMinUcret;
            MaxUcret = YolcuMaxUcret;
            BaslangicAdres = YolcuBaslangicAdres;
            YukAgirligi = YolcuYukAgirligi;
        }

        //DB Bağlantı
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open();

            //Görev var mı
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "SELECT Gorev_ID FROM GorevListesi WHERE Gorev_Turu=" + GorevTuru;
            dbcmd.CommandText = sqlQuery;
            int SatirSayisi = Convert.ToInt32(dbcmd.ExecuteScalar());

            // Gorev varsa
            if (SatirSayisi != 0)
            {
                dbcmd = dbconn.CreateCommand();
                sqlQuery = "SELECT Gorev_ID FROM GorevListesi WHERE Secilme_Durumu=1 OR Yuk_Durumu=1";
                dbcmd.CommandText = sqlQuery;
                SatirSayisi = Convert.ToInt32(dbcmd.ExecuteScalar());

                if (SatirSayisi != 0)
                    GorevYukUyariPanel.SetActive(true);
                else
                {
                    //Eski görevleri Sil
                    dbcmd = dbconn.CreateCommand();
                    sqlQuery = "DELETE FROM GorevListesi WHERE Gorev_Turu=" + GorevTuru;
                    dbcmd.CommandText = sqlQuery;
                    reader = dbcmd.ExecuteReader();

                    //Yeni Görev Oluştur
                    for (int i = 0; i < 6; i++)
                    {
                        int ucret = UnityEngine.Random.Range(MinUcret, MaxUcret);

                        // Yolcu görev türü için özel başlangıç konumu
                        if (GorevTuru == YolcuGorevTuru)
                            BaslangicAdres= UnityEngine.Random.Range(1, (AdresSayisi + 1));

                        int hedefAdres = UnityEngine.Random.Range(1, (AdresSayisi + 1));
                        while (hedefAdres == BaslangicAdres)
                            hedefAdres = UnityEngine.Random.Range(1, (AdresSayisi + 1));


                        String mahalle = null;
                        dbcmd = dbconn.CreateCommand();
                        sqlQuery = "SELECT Mahalle FROM Adresler WHERE Kapi_NO=" + hedefAdres;
                        dbcmd.CommandText = sqlQuery;
                        reader = dbcmd.ExecuteReader();
                        while (reader.Read())
                            mahalle = reader.GetString(0);

                        dbcmd = dbconn.CreateCommand();
                        sqlQuery = "INSERT INTO GorevListesi (Baslangic_Kapi_NO, Bitis_Kapi_NO, Ucret, Yuk_Agirligi, Secilme_Durumu, Yuk_Durumu, Gorev_Turu, Tamamlanma_Durumu, Mahalle) VALUES (" + BaslangicAdres + ", " + hedefAdres + ", " + ucret + ", " + YukAgirligi + ", 0, 0, " + GorevTuru + ", 0, '"+mahalle+"')";
                        dbcmd.CommandText = sqlQuery;

                        reader = dbcmd.ExecuteReader();
                    }
                }

            }
            else
            {
                //Görev Yoksa Görev oluştur
                for (int i = 0; i < 6; i++)
                {
                    int ucret = UnityEngine.Random.Range(MinUcret, MaxUcret);

                    // Yolcu görev türü için özel başlangıç konumu
                    if (GorevTuru == YolcuGorevTuru)
                        BaslangicAdres = UnityEngine.Random.Range(1, (AdresSayisi + 1));

                    int hedefAdres = UnityEngine.Random.Range(1, (AdresSayisi + 1));
                    while (hedefAdres == BaslangicAdres)
                        hedefAdres = UnityEngine.Random.Range(1, (AdresSayisi + 1));

                    String mahalle = null;
                    dbcmd = dbconn.CreateCommand();
                    sqlQuery = "SELECT Mahalle FROM Adresler WHERE Kapi_NO=" + hedefAdres;
                    dbcmd.CommandText = sqlQuery;
                    reader = dbcmd.ExecuteReader();
                    while (reader.Read())
                        mahalle = reader.GetString(0);

                    dbcmd = dbconn.CreateCommand();
                    sqlQuery = "INSERT INTO GorevListesi (Baslangic_Kapi_NO, Bitis_Kapi_NO, Ucret, Yuk_Agirligi, Secilme_Durumu, Yuk_Durumu, Gorev_Turu, Tamamlanma_Durumu, Mahalle) VALUES (" + BaslangicAdres + ", " + hedefAdres + ", " + ucret + ", " + YukAgirligi + ", 0, 0, " + GorevTuru + ", 0, '" + mahalle + "')";
                    dbcmd.CommandText = sqlQuery;
                    reader = dbcmd.ExecuteReader();
                }
            }



            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            GorevleriGetir(GorevTuru);

        }
    }

    // Aktif görevin idsini döndürür
    private int SeciliGorevID()
    {
        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open();
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "SELECT Gorev_ID FROM GorevListesi WHERE Secilme_Durumu=1";
            dbcmd.CommandText = sqlQuery;
            int SatirSayisi = Convert.ToInt32(dbcmd.ExecuteScalar());
            reader = dbcmd.ExecuteReader();
            int DB_SeciliGorevID = 0;
            if (SatirSayisi != 0)
            {
                while (reader.Read())
                {
                    DB_SeciliGorevID = reader.GetInt32(0);
                }
                reader.Close();
                reader = null;
                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;
                return DB_SeciliGorevID;
            }
            else
            {
                reader.Close();
                reader = null;
                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;
                return 0;
            }
        }
    }

    // Göreve tıklatıldığında görev seçilmemişse seçer seçiliyse yük durumuna göre görevi bırakır
    private void OnClick(int GorevID)
    {
        bool GorevSeciliMi = false;
        int gorevTuru = 0;
        bool YukAlinmisMi = false;

        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open();
            //Sorgu
            dbcmd = dbconn.CreateCommand();
            sqlQuery = "SELECT Secilme_Durumu, Gorev_Turu, Yuk_Durumu FROM GorevListesi WHERE Gorev_ID=" + GorevID;
            dbcmd.CommandText = sqlQuery;
            reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                int DB_SecilmeDurumu = reader.GetInt32(0);
                GorevSeciliMi = Convert.ToBoolean(DB_SecilmeDurumu);
                gorevTuru = reader.GetInt32(1);
                int DB_YukDurumu = reader.GetInt32(2);
                YukAlinmisMi = Convert.ToBoolean(DB_YukDurumu);
            }
            if (!YukAlinmisMi)
            {
                if (GorevSeciliMi)
                {

                    dbcmd = dbconn.CreateCommand();
                    sqlQuery = "UPDATE GorevListesi SET Secilme_Durumu=0 WHERE Gorev_ID=" + GorevID;
                    dbcmd.CommandText = sqlQuery;
                    reader = dbcmd.ExecuteReader();
                }
                else
                {
                    dbcmd = dbconn.CreateCommand();
                    sqlQuery = "UPDATE GorevListesi SET Secilme_Durumu=1 WHERE Gorev_ID=" + GorevID;
                    dbcmd.CommandText = sqlQuery;
                    reader = dbcmd.ExecuteReader();
                }

                GorevleriGetir(gorevTuru);
                gorev.GetComponent<Gorev>().GorevKontrol();
            }
            else
            {
                GorevYukUyariPanel.SetActive(true);
            }
        }
    }

    public void GorevYukUyariTamam()
    {
        GorevYukUyariPanel.SetActive(false);
    }

}
