using BET_PRO_API;
using log4net;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Timers;
using System.Web.Http.Tracing;
using Timer = System.Threading.Timer;

///<Summary>
/// XML Komentar
///</Summary>
public class UpdateTimer
{
    static List<RootUpcomingMatches> cRootUpcomingMatchesLoL = new List<RootUpcomingMatches>();
    static List<RootUpcomingMatches> cRootUpcomingMatchesDOTA2 = new List<RootUpcomingMatches>();
    static List<RootUpcomingMatches> cRootUpcomingMatchesCSGO = new List<RootUpcomingMatches>();
    static List<RootUpcomingMatches> cRootUpcomingMatchesValorant = new List<RootUpcomingMatches>();
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public static List<RootUpcomingMatches> SviMecevi = new List<RootUpcomingMatches>();
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public static List<string> LinkoviKvota = new List<string>();
    //List<Kladionica> Kladionice = new List<Kladionica>();
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public static List<cMatch> Mecevi = new List<cMatch>();
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public static List<cMatch> MeceviAPI = new List<cMatch>();


    static string CheckJson(string sJson)
    {
        string stari = Strings.Chr(34) + "count" + Strings.Chr(34) + ": null";
        string novi = Strings.Chr(34) + "count" + Strings.Chr(34) + ": 3";

        for (int n = 0; n < sJson.Length;)
        {
            int gde = Strings.InStr(sJson, stari);
            if (gde == 0) break;
            string prvideo = Strings.Mid(sJson, 1, gde - 1);
            string drugideo = Strings.Mid(sJson, gde + 13);
            sJson = prvideo + novi + drugideo;
            n = gde + 2;
        }

        return sJson;
    }

    ///<Summary>
    /// XML Komentar
    ///</Summary>
    static string RimbleAPI(string Igra)
    {
        var url = "https://rimbleanalytics.com/raw/" + Igra + "/upcoming-matches/";

        WebRequest request = WebRequest.Create(url);
        request.ContentType = "application/json; charset=UTF-8";

        request.Headers.Add("x-api-key", "z8Wz5bhQBCapgQ57fFlv89NAeZLuxDyz7jig7jIn");
        string responseText = "";
        try
        {
            WebResponse response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());

            responseText = reader.ReadToEnd();
        }
        catch (Exception e)
        {
            responseText = e.Message;
        }

        return responseText;
    }

    ///<Summary>
    /// XML Komentar
    ///</Summary>
    static void UpdateRimble()
    {
        string mecevi = RimbleAPI("lol");
        mecevi = CheckJson(mecevi);
        JsonConvert.PopulateObject(mecevi, cRootUpcomingMatchesLoL);
        cRootUpcomingMatchesLoL.ForEach(x => x.match_format.metadata.ForEach(m => m.game_number = 1));
        SviMecevi.AddRange(cRootUpcomingMatchesLoL);

        mecevi = RimbleAPI("csgo");
        mecevi = CheckJson(mecevi);
        JsonConvert.PopulateObject(mecevi, cRootUpcomingMatchesCSGO);
        cRootUpcomingMatchesCSGO.ForEach(x => x.match_format.metadata.ForEach(m => m.game_number = 3));
        SviMecevi.AddRange(cRootUpcomingMatchesCSGO);

        mecevi = RimbleAPI("dota2");
        mecevi = CheckJson(mecevi);
        JsonConvert.PopulateObject(mecevi, cRootUpcomingMatchesDOTA2);
        cRootUpcomingMatchesDOTA2.ForEach(x => x.match_format.metadata.ForEach(m => m.game_number = 4));
        SviMecevi.AddRange(cRootUpcomingMatchesDOTA2);

        mecevi = RimbleAPI("valorant");
        mecevi = CheckJson(mecevi);
        JsonConvert.PopulateObject(mecevi, cRootUpcomingMatchesValorant);
        cRootUpcomingMatchesValorant.ForEach(x => x.match_format.metadata.ForEach(m => m.game_number = 2));
        SviMecevi.AddRange(cRootUpcomingMatchesValorant);
    }

    ///<Summary>
    /// XML Komentar
    ///</Summary>
    static List<string> UpdateOddsMatches(string Datum)
    {
        List<string> Lista = new List<string>();

        ChromeOptions options = new ChromeOptions();
        options.AddArguments("--silent-launch");
        options.AddArguments("--no - startup - window");
        options.AddArguments("no-sandbox");
        options.AddArguments("headless");

        var chromeDriverService = ChromeDriverService.CreateDefaultService();
        chromeDriverService.HideCommandPromptWindow = true;

        using (IWebDriver driver = new ChromeDriver(chromeDriverService, options))
        {
            bool staleElement = true;

            driver.Navigate().GoToUrl("https://oddsportal.com/matches/esports/" + Datum);

            while (staleElement)
            {
                try
                {

                    var items = driver.FindElements(By.XPath("//*[@href]"));
                    foreach (var item in items.Where(x => (x.GetAttribute("href")).Contains("esports") == true))
                    {
                        Lista.Add(item.GetAttribute("href").ToString());
                    }

                    staleElement = false;
                }
                catch 
                {
                    staleElement = true;
                }
            }
            
            driver.Quit();
        }

        return Lista;
    }

    static void UpisUBazuSvihPodataka()
    {
        SviMecevi.Clear();
        UpdateRimble();

        UnosNazivaTimova();

        var Datumi = SviMecevi.Select(d => d.date).Distinct().ToList();
        Diagnostic.Datumi = Datumi;
        BrisiMecevePoDatumu(Datumi);

        for (int n = 0; n < Datumi.Count; n++)
        {
            List<RootUpcomingMatches> MeceviZaDatum = SviMecevi.Where(d => d.date == Datumi[n]).ToList();

            DateTime Datum = Convert.ToDateTime(Datumi[n]);

            LinkoviKvota.Clear();
            LinkoviKvota = UpdateOddsMatches(Datum.ToString("yyyyMMdd"));

            for (int i = 0; i < MeceviZaDatum.Count; i++)
            {
                CompleteTeam tim = new CompleteTeam();
                bool OddsPortalNaziv1Nadjen = false;
                bool OddsPortalNaziv2Nadjen = false;

                tim = ProveraNaziva(MeceviZaDatum[i].team_1_name, MeceviZaDatum[i].team_2_name);

                if (tim.OddsPortalNaziv1 == "" || tim.OddsPortalNaziv1 == null)
                {
                    tim.OddsPortalNaziv1 = MeceviZaDatum[i].team_1_name.ToLower();
                    if (tim.OddsPortalNaziv1.Contains(' ') == true) { tim.OddsPortalNaziv1 = tim.OddsPortalNaziv1.Replace(' ', '-'); }
                    if (tim.OddsPortalNaziv1.Contains('.') == true) { tim.OddsPortalNaziv1 = tim.OddsPortalNaziv1.Replace('.', '-'); }
                }
                else
                    OddsPortalNaziv1Nadjen = true;

                if (tim.OddsPortalNaziv2 == "" || tim.OddsPortalNaziv2 == null)
                {
                    tim.OddsPortalNaziv2 = MeceviZaDatum[i].team_2_name.ToLower();
                    if (tim.OddsPortalNaziv2.Contains(' ') == true) { tim.OddsPortalNaziv2 = tim.OddsPortalNaziv2.Replace(' ', '-'); }
                    if (tim.OddsPortalNaziv2.Contains('.') == true) { tim.OddsPortalNaziv2 = tim.OddsPortalNaziv2.Replace('.', '-'); }
                }
                else
                    OddsPortalNaziv2Nadjen = true;

                tim.OddsPortalNaziv1 = tim.OddsPortalNaziv1.Replace(";", "'");
                tim.OddsPortalNaziv2 = tim.OddsPortalNaziv1.Replace(";", "'");
                string[] LinkK = (from num in LinkoviKvota
                                  where num.Contains(tim.OddsPortalNaziv1)
                                  where num.Contains(tim.OddsPortalNaziv2)
                                  where num.Contains("inplay-odds") == false
                                  select num).ToArray();

                List<cOdds> Kladionice = new List<cOdds>();
                if (LinkK.Count() > 0)
                {
                    Kladionice = UpdateOdds(LinkK[0]);
                    Kladionice.RemoveAll(x => x.name.Contains("Click") == true);
                    if (OddsPortalNaziv1Nadjen == false)
                    {
                        AzurirajOddsPortalNazuv(tim.RimbleNaziv1, tim.OddsPortalNaziv1);
                    }

                    if (OddsPortalNaziv2Nadjen == false)
                    {
                        AzurirajOddsPortalNazuv(tim.RimbleNaziv2, tim.OddsPortalNaziv2);
                    }
                }
                else { cOdds kl = new cOdds(); kl.proc = 0; kl.kvota1 = 0; kl.kvota2 = 0; kl.name = null; Kladionice.Add(kl); }

                //UpisiKladionice(Kladionice);

                cMatch mec = new cMatch();
                mec.startDate = Datumi[n];
                mec.endDate = Datumi[n];
                mec.id = MeceviZaDatum[i].matchid;
                mec.title = MeceviZaDatum[i].league_details.name;
                mec.odds = Kladionice;
                mec.gameId = MeceviZaDatum[i].match_format.metadata[0].game_number;
                
                List<cParticipant> timovi = new List<cParticipant>();
                cParticipant ucesnik = new cParticipant();
                ucesnik.name = MeceviZaDatum[i].team_1_name;
                //ucesnik.odds = Kladionice;
                ucesnik.image = MeceviZaDatum[i].teams[0].image;
                ucesnik.teamId = tim.IdTeam1;
                timovi.Add(ucesnik);
                ucesnik = new cParticipant();
                ucesnik.name = MeceviZaDatum[i].team_2_name;
                ucesnik.image = MeceviZaDatum[i].teams[1].image;
                //ucesnik.odds = Kladionice;
                ucesnik.teamId = tim.IdTeam2;
                timovi.Add(ucesnik);

                mec.participants = timovi;

                Mecevi.Add(mec);
                UpisiMec(mec);
            }
        }
    }

    static void UpisiMec(cMatch mec)
    {
        BET_PROEntities1 dbContext = new BET_PROEntities1();

        mec.title = mec.title.Replace("'", " ");

        List<Tims> t2 = dbContext.Database.SqlQuery<Tims>("SELECT dbo.Timovi.RimbleNaziv, dbo.Timovi.OddsPortalNaziv, dbo.Timovi.Sifra " +
                     "FROM dbo.Match INNER JOIN dbo.Participant ON dbo.Match.id = dbo.Participant.matchId INNER JOIN " +
                     "dbo.Timovi ON dbo.Participant.teamId = dbo.Timovi.Sifra WHERE(dbo.Match.startDate = '" + mec.startDate + "') " +
                     "AND(dbo.Match.endDate = '" + mec.endDate + "') AND(dbo.Participant.teamId = " + mec.participants[0].teamId +
                     " OR dbo.Participant.teamId = " + mec.participants[1].teamId + ") AND (Match.id = " + mec.id + ")").ToList();

        if (t2.Count == 0)
        {
            dbContext.Database.ExecuteSqlCommand("INSERT INTO Match VALUES(" + mec.id + ",'" + mec.startDate + "','" + mec.endDate + "'," + mec.gameId + ",'Over',0,'" + mec.title + "')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO Participant (matchId,teamId,name,image) VALUES(" + mec.id + "," + mec.participants[0].teamId + ",'" + mec.participants[0].name + "','" + mec.participants[0].image + "')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO Participant (matchId,teamId,name,image) VALUES(" + mec.id + "," + mec.participants[1].teamId + ",'" + mec.participants[1].name + "','" + mec.participants[1].image + "')");

            foreach (var od in mec.odds)
            {
                dbContext.Database.ExecuteSqlCommand("INSERT INTO Odds (matchId,teamId,name,providerId,link,kvota1,kvota2,procenat) VALUES("
                    + mec.id + ",0,'" + od.name + "',0,'" + od.link + "','" + od.kvota1 + "','" + od.kvota2 + "','" + od.proc + "')");
            }
        }
        //else
        //{
        //    Console.WriteLine("Tim 1: " + mec.participants[0].teamId.ToString() + "  Tim 2: " + mec.participants[1].teamId.ToString());
        //}
    }

    static void AzurirajOddsPortalNazuv(string RimbleNaziv, string OddsPortalNaziv)
    {
        BET_PROEntities1 dbContext = new BET_PROEntities1();

        dbContext.Database.ExecuteSqlCommand("UPDATE Timovi SET OddsPortalNaziv = '" + OddsPortalNaziv + "' WHERE RimbleNaziv = '" + RimbleNaziv + "'");

    }

    static CompleteTeam ProveraNaziva(string Naziv1, string Naziv2)
    {
        CompleteTeam Resp = new CompleteTeam();
        BET_PROEntities1 dbContext = new BET_PROEntities1();

        List<Tims> t = dbContext.Database.SqlQuery<Tims>("SELECT * FROM Timovi WHERE RimbleNaziv = '" + Naziv1 + "'").ToList();

        if (t.Count != 0)
        {
            if (t[0].RimbleNaziv != null) { Resp.RimbleNaziv1 = t[0].RimbleNaziv; Resp.IdTeam1 = Convert.ToInt64(t[0].Sifra.ToString()); }
            if (t[0].OddsPortalNaziv != null) Resp.OddsPortalNaziv1 = t[0].OddsPortalNaziv;
        }

        List<Tims> t2 = dbContext.Database.SqlQuery<Tims>("SELECT * FROM Timovi WHERE RimbleNaziv = '" + Naziv2 + "'").ToList();

        if (t2.Count != 0)
        {
            if (t2[0].RimbleNaziv != null) { Resp.RimbleNaziv2 = t2[0].RimbleNaziv; Resp.IdTeam2 = Convert.ToInt64(t2[0].Sifra.ToString()); }
            if (t2[0].OddsPortalNaziv != null) Resp.OddsPortalNaziv2 = t2[0].OddsPortalNaziv;
        }

        return Resp;
    }

    static void UnosNazivaTimova()
    {

        BET_PROEntities1 dbContext = new BET_PROEntities1();
        //int NSifra = 1;

        //var p = dbContext.Database.SqlQuery<int?>("Select MAX(Sifra) AS Poslednji FROM Timovi").FirstOrDefault();

        //if (p != null) NSifra = Convert.ToInt32(p) + 1;

        foreach (var tm in SviMecevi)
        {
            tm.team_1_name = tm.team_1_name.Replace("'", ";");
            List<Tims> t = dbContext.Database.SqlQuery<Tims>("SELECT * FROM Timovi WHERE RimbleNaziv = '" + tm.team_1_name + "'").ToList();

            if (t.Count == 0)
            {
                dbContext.Database.ExecuteSqlCommand("INSERT INTO Timovi VALUES (" + tm.teams[0].id + ",'" + tm.team_1_name + "','')");
                //NSifra++;
            }
            tm.team_2_name = tm.team_2_name.Replace("'", ";");
            List<Tims> t2 = dbContext.Database.SqlQuery<Tims>("SELECT * FROM Timovi WHERE RimbleNaziv = '" + tm.team_2_name + "'").ToList();
            if (t2.Count == 0)
            {
                dbContext.Database.ExecuteSqlCommand("INSERT INTO Timovi VALUES (" + tm.teams[1].id + ",'" + tm.team_2_name + "','')");
                //NSifra++;
            }
        }

    }

    static void BrisiMecevePoDatumu(List<string> dDatumi)
    {
        BET_PROEntities1 dbContext = new BET_PROEntities1();

        for (int d = 0; d < dDatumi.Count; d++)
        {
            string dd = Convert.ToDateTime(dDatumi[d]).ToString("yyyy-MM-dd");

            dbContext.Database.ExecuteSqlCommand("DELETE Odds FROM Odds WHERE Odds.matchid IN (SELECT Match.Id  FROM Match WHERE Odds.matchId = " +
                "dbo.Match.Id AND Match.startDate = '" + dd + "')");
            dbContext.Database.ExecuteSqlCommand("DELETE Participant FROM Participant WHERE Participant.matchid IN (SELECT Match.Id  FROM Match WHERE Participant.matchId = " +
                "dbo.Match.Id AND Match.startDate = '" + dd + "')");
            dbContext.Database.ExecuteSqlCommand("DELETE FROM Match WHERE startDate = '" + dd + "'");
        }

    }

    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public static List<cMatch> FinalnaLista()
    {
        SviMecevi.Clear();
        UpdateRimble();
        Diagnostic.SviMecevi = SviMecevi.Count;

        var Datumi = SviMecevi.Select(d => d.date).Distinct().ToList();
        Diagnostic.Datumi = Datumi;

        for (int n = 0; n < Datumi.Count - 1; n++)
        {
            List<RootUpcomingMatches> MeceviZaDatum = SviMecevi.Where(d => d.date == Datumi[n]).ToList();

            DateTime Datum = Convert.ToDateTime(Datumi[n]);

            LinkoviKvota.Clear();
            LinkoviKvota = UpdateOddsMatches(Datum.ToString("yyyyMMdd"));
            Diagnostic.LinkoviKvota = LinkoviKvota.Count;

                for (int i = 0; i < MeceviZaDatum.Count; i++)
                {
                    string tim1 = MeceviZaDatum[i].team_1_name.ToLower();
                    string tim2 = MeceviZaDatum[i].team_2_name.ToLower();

                    if (tim1.Contains(' ') == true) { tim1 = tim1.Replace(' ', '-'); }
                    if (tim2.Contains(' ') == true) { tim2 = tim2.Replace(' ', '-'); }

                    if (tim1.Contains('.') == true) { tim1 = tim1.Replace('.', '-'); }
                    if (tim2.Contains('.') == true) { tim2 = tim2.Replace('.', '-'); }

                    string[] LinkK = { "" };

                    if (LinkoviKvota.Count >0)
                    {
                        LinkK = (from num in LinkoviKvota
                                      where num.Contains(tim1)
                                      where num.Contains(tim2)
                                      where num.Contains("inplay-odds") == false
                                      select num).ToArray();
                    }

                    List<cOdds> Kladionice = new List<cOdds>();
                    if (LinkK.Count() > 0)
                    {
                        Kladionice = UpdateOdds(LinkK[0]);
                        Kladionice.RemoveAll(x => x.name.Contains("Click") == true);
                    }
                    else { cOdds kl = new cOdds(); kl.proc = 0; kl.kvota1 = 0; kl.kvota2 = 0; kl.name = null; Kladionice.Add(kl); }

                    cMatch mec = new cMatch();
                    mec.startDate = Datum.ToString();
                    mec.endDate = Datum.ToString();
                    mec.gameId = MeceviZaDatum[i].matchid;
                    mec.title = MeceviZaDatum[i].league_details.name;

                    List<cParticipant> timovi = new List<cParticipant>();
                    cParticipant tim = new cParticipant();
                    tim.name = MeceviZaDatum[i].team_1_name;
                    tim.odds = Kladionice;
                    tim.image = MeceviZaDatum[i].teams[0].image;
                    timovi.Add(tim);
                    tim = new cParticipant();
                    tim.name = MeceviZaDatum[i].team_2_name;
                    tim.image = MeceviZaDatum[i].teams[1].image;
                    tim.odds = Kladionice;
                    timovi.Add(tim);

                    mec.participants = timovi;

                    Diagnostic.Kladionice = Kladionice;

                    Mecevi.Add(mec);
                }
            
        }
        
        return Mecevi;
    }

    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public static List<cMatch> FinalnaListaBexUpdate()
    {
        //UpdateRimble();

        var Datumi = SviMecevi.Select(d => d.date).Distinct().ToList();

        for (int n = 0; n < Datumi.Count - 1; n++)
        {
            List<RootUpcomingMatches> MeceviZaDatum = SviMecevi.Where(d => d.date == Datumi[n]).ToList();

            DateTime Datum = Convert.ToDateTime(Datumi[n]);

            //LinkoviKvota.Clear();
            //LinkoviKvota = UpdateOddsMatches(Datum.ToString("yyyyMMdd"));

            for (int i = 0; i < MeceviZaDatum.Count; i++)
            {
                string tim1 = MeceviZaDatum[i].team_1_name.ToLower();
                string tim2 = MeceviZaDatum[i].team_2_name.ToLower();

                if (tim1.Contains(' ') == true) { tim1 = tim1.Replace(' ', '-'); }
                if (tim2.Contains(' ') == true) { tim2 = tim2.Replace(' ', '-'); }

                if (tim1.Contains('.') == true) { tim1 = tim1.Replace('.', '-'); }
                if (tim2.Contains('.') == true) { tim2 = tim2.Replace('.', '-'); }

                string[] LinkK = (from num in LinkoviKvota
                                  where num.Contains(tim1)
                                  where num.Contains(tim2)
                                  where num.Contains("inplay-odds") == false
                                  select num).ToArray();

                List<cOdds> Kladionice = new List<cOdds>();
                if (LinkK.Count() > 0)
                {
                    Kladionice = UpdateOdds(LinkK[0]);
                    Kladionice.RemoveAll(x => x.name.Contains("Click") == true);
                }
                else { cOdds kl = new cOdds(); kl.proc = 0; kl.kvota1 = 0; kl.kvota2 = 0; kl.name = null; Kladionice.Add(kl); }

                cMatch mec = new cMatch();
                mec.startDate = Datum.ToString();
                mec.endDate = Datum.ToString();
                mec.gameId = MeceviZaDatum[i].matchid;
                mec.title = MeceviZaDatum[i].league_details.name;

                List<cParticipant> timovi = new List<cParticipant>();
                cParticipant tim = new cParticipant();
                tim.name = MeceviZaDatum[i].team_1_name;
                tim.odds = Kladionice;
                tim.image = MeceviZaDatum[i].teams[0].image;
                timovi.Add(tim);
                tim = new cParticipant();
                tim.name = MeceviZaDatum[i].team_2_name;
                tim.image = MeceviZaDatum[i].teams[1].image;
                tim.odds = Kladionice;
                timovi.Add(tim);

                mec.participants = timovi;

                Mecevi.Add(mec);
            }
        }

        return Mecevi;
    }

    ///<Summary>
    /// XML Komentar
    ///</Summary>
    static List<cOdds> UpdateOdds(string Link)
    {
        List<cOdds> Lista = new List<cOdds>();

        ChromeOptions options = new ChromeOptions();
        options.AddArguments("--silent-launch");
        options.AddArguments("--no - startup - window");
        options.AddArguments("no-sandbox");
        options.AddArguments("headless");

        var chromeDriverService = ChromeDriverService.CreateDefaultService();
        chromeDriverService.HideCommandPromptWindow = true;

        //List<string> lKladionice = new List<string>();

        //using (IWebDriver driverL = new ChromeDriver(chromeDriverService, options))
        //{
        //    driverL.Navigate().GoToUrl(Link);
        //    var qLinks = driverL.FindElements(By.XPath("//*[@href]")).ToList();
        //    foreach (var item in qLinks.Where(x => x.GetAttribute("href").Contains("bookmaker")))
        //    {
        //        lKladionice.Add(item.GetAttribute("href").ToString());
        //    }
        //    driverL.Quit();
        //}

        using (IWebDriver driver = new ChromeDriver(chromeDriverService, options))
        {

            driver.Navigate().GoToUrl(Link);
            var itemsQ = driver.FindElements(By.CssSelector("div"));

            if (itemsQ.Count > 0)
            {
                List<string> qq = itemsQ[0].Text.Split('\r', '\n').ToList();
                qq.RemoveAll(x => x.Length == 0);

                bool Pocni = false;

                for (int k = 0; k < qq.Count - 1; k++)
                {
                    if (Pocni == true)
                    {
                        cOdds kk = new cOdds();

                        kk.name = qq[k++];
                        //var linc = lKladionice.Where(f => f.Contains(kk.name.Trim(' ').ToLower() + "/link/")).ToList();
                        //if(linc.Count > 0)
                        //{
                        kk.link = "https://www.oddsportal.com/bookmaker/" + kk.name.Trim(' ').ToLower() + "/link/";
                        //}

                        if (k >= qq.Count) { Lista.Add(kk); Pocni = false; break; }
                        if (kk.name.Contains("My Coupon") == true) { Lista.Add(kk); Pocni = false; break; }
                        kk.kvota1 = DajDecimalnuKvotu(qq[k++]);
                        if (qq[k - 1].Contains("My Coupon") == true) { Lista.Add(kk); Pocni = false; break; }
                        kk.kvota2 = DajDecimalnuKvotu(qq[k++]);
                        if (k >= qq.Count) { Lista.Add(kk); Pocni = false; break; }
                        if (qq[k - 1].Contains("My Coupon") == true) { Lista.Add(kk); Pocni = false; break; }
                        if (qq[k].Contains("%") == true)
                        {
                            string ppp = qq[k].Replace('%', '0');
                            if (Information.IsNumeric(ppp))
                                kk.proc = (float)Convert.ToDecimal(ppp);
                            else
                                kk.proc = 0;
                        }
                        else
                            kk.proc = 0;
                        if (qq[k].Contains("My Coupon") == true) { Lista.Add(kk); Pocni = false; break; }
                        Lista.Add(kk);

                    }
                    if (qq[k].Contains("Bookmaker") == true) Pocni = true;

                }
            }

            driver.Quit();
        }

        return Lista;
    }

    static float DajDecimalnuKvotu(string aKvota)
    {
        float resp;

        if (Information.IsNumeric(aKvota))
        {
            resp = (float)Convert.ToDecimal(aKvota);
            if (resp < 0)
            {
                resp = 1 - (100 / resp);
                resp = (float)Convert.ToDecimal(resp.ToString("##0.00"));
            }
            else
            {
                resp = 1 + (resp / 100);
                resp = (float)Convert.ToDecimal(resp.ToString("##0.00"));
            }
        }
        else
            resp = 0;

        return resp;
    }


    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public static class ListTimer
    {
        private static System.Timers.Timer DataTimer;
        private static int Minut = 0;
        

        ///<Summary>
        /// XML Komentar
        ///</Summary>
        public static void StartDataTimer(int Interval_ms)
        {
            DataTimer = new System.Timers.Timer();
            DataTimer.Elapsed +=  new ElapsedEventHandler(OnTimedEvent);
            DataTimer.Interval = Interval_ms;
            DataTimer.Enabled = true;
        }

        ///<Summary>
        /// XML Komentar
        ///</Summary>
        public static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Minut++;
            if (Minut == 1)
            {
                Diagnostic.Reset();
                //try
                //{
                Diagnostic.Status = "Updating...";
                //    //FinalnaLista();
                UpisUBazuSvihPodataka();
                Diagnostic.VremeUkljucenja = DateTime.Now.ToString("HH:mm:ss");
                Diagnostic.VremePravljenjaFinalneListe = DateTime.Now.ToString("HH:mm:ss");
                Diagnostic.Status = "OK";
                MeceviAPI = Mecevi;
                //}
                //catch
                //{
                //    Diagnostic.VremePravljenjaFinalneListe = DateTime.Now.ToString("HH:mm:ss");
                //    Diagnostic.Status = "Greska";
                //}
            }

            if (Minut >= 20)
            {
                try
                {
                    Diagnostic.Status = "Updating...";
                    //FinalnaLista();
                    UpisUBazuSvihPodataka();
                    Diagnostic.VremePravljenjaFinalneListe = DateTime.Now.ToString("HH:mm:ss");
                    Diagnostic.Status = "OK";
                    MeceviAPI = Mecevi;
                }
                catch
                {
                    Diagnostic.VremePravljenjaFinalneListe = DateTime.Now.ToString("HH:mm:ss");
                    Diagnostic.Status = "Greska";
                }
                Minut = 2;
            }
        }


    }

    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public static class Diagnostic
    {

            ///<Summary>
            /// XML Komentar
            ///</Summary>
            public static string VremeUkljucenja { get; set; }

        ///<Summary>
        /// XML Komentar
        ///</Summary>
        public static string Status { get; set; }

        ///<Summary>
        /// XML Komentar
        ///</Summary>
        public static string VremePravljenjaFinalneListe { get; set; }

            ///<Summary>
            /// XML Komentar
            ///</Summary>
            public static int SviMecevi { get; set; }

            ///<Summary>
            /// XML Komentar
            ///</Summary>
            public static int LinkoviKvota { get; set; }

        ///<Summary>
        /// XML Komentar
        ///</Summary>
        public static List<string> Datumi { get; set; }

        ///<Summary>
        /// XML Komentar
        ///</Summary>
        public static List<cOdds> Kladionice = new List<cOdds>();

        ///<Summary>
        /// XML Komentar
        ///</Summary>
        public static void Reset()
        {
            if(Datumi != null) Datumi.Clear();
            if (Kladionice != null) Kladionice.Clear();
            
        }
    }

    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public class DiagnosticRet
    {

        ///<Summary>
        /// XML Komentar
        ///</Summary>
        public string VremeUkljucenja { get; set; }

        ///<Summary>
        /// XML Komentar
        ///</Summary>
        public string Status { get; set; }

        ///<Summary>
        /// XML Komentar
        ///</Summary>
        public string VremePravljenjaFinalneListe { get; set; }


        ///<Summary>
        /// XML Komentar
        ///</Summary>
        public Dictionary<string,int> Datumi = new Dictionary<string, int>();


        ///<Summary>
        /// XML Komentar
        ///</Summary>
        public void Reset()
        {
            Datumi.Clear();
        }
    }

    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public class MyAPITracer : ITraceWriter
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MyAPITracer));

        ///<Summary>
        /// XML Komentar
        ///</Summary>
        public void Trace(HttpRequestMessage request, string category, TraceLevel level, Action<TraceRecord> traceAction)
        {
            TraceRecord rec = new TraceRecord(request, category, level);

            traceAction(rec);

            WriteLog(rec);


        }

        private void WriteLog(TraceRecord rec)
        {
            if (rec.Exception == null)
            {
                string infoLog = string.Format("{0};{1};{3}", rec.Category, rec.Operator, rec.Operation, rec.Message);
                log.Info(infoLog);
            }
            else
            {
                string errorLog = string.Format("{0};{1};{3}", rec.Category, rec.Operator, rec.Operation, rec.Message);
                log.Error(errorLog, rec.Exception);
            }
        }
    }

    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public class ListtoDataTableConverter
    {
        ///<Summary>
        /// XML Komentar
        ///</Summary>
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }

    class Tims
    {
        public long Sifra { get; set; }
        public string RimbleNaziv { get; set; }
        public string OddsPortalNaziv { get; set; }
    }

    class CompleteTeam
    {
        public string RimbleNaziv1 { get; set; }
        public string OddsPortalNaziv1 { get; set; }
        public string RimbleNaziv2 { get; set; }
        public string OddsPortalNaziv2 { get; set; }
        public long IdTeam1 { get; set; }
        public long IdTeam2 { get; set; }
    }
}