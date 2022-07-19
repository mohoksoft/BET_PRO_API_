using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Http;
using static UpdateTimer;

namespace BET_PRO_API.Controllers
{
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public class GamesController : ApiController
    {
        DBTool DBase = new DBTool();

        List<RootUpcomingMatches> SviMecevi = new List<RootUpcomingMatches>();

        ///<Summary>
        /// XML Komentar
        ///</Summary>
        [HttpGet]
        [Route("api/getupcomingmatches/{currentPage}/{pageSize}")]
        public List<cMatch> Get(int currentPage,int pageSize)
        {
            DajMeceveIzBaze();
            return new CNPagedList<cMatch>(MeceviAPI, currentPage, pageSize).items;
        }

        ///<Summary>
        /// XML Komentar
        ///</Summary>
        [HttpGet]
        [Route("api/getupcomingmatches/noupdate")]
        public List<cMatch> GetBezUpdate()
        {
            return FinalnaListaBexUpdate();
        }

        ///<Summary>
        /// XML Komentar
        ///</Summary>
        [HttpGet]
        [Route("api/getupcomingmatches/{currentPage}/{pageSize}/{Datum}")]
        public List<cMatch> GetSaDatumom(int currentPage, int pageSize, string Datum)
        {
            DajMeceveIzBazeZaDatum(Datum,0);
            return new CNPagedList<cMatch>(MeceviAPI, currentPage, pageSize).items;
        }

        ///<Summary>
        /// XML Komentar
        ///</Summary>
        [HttpPost]
        [Route("api/upcomingmatches")]
        public List<cMatch> GetSaDatumomBody([FromBody] parGame model)
        {
            DajMeceveIzBazeZaDatum(model.Datum,model.Game);
            return MeceviAPI;
        }

        ///<Summary>
        /// XML Komentar
        ///</Summary>
        [HttpGet]
        [Route("api/ngames")]
        public IEnumerable<SifarnikIgara> GetNGames()
        {
            List<SifarnikIgara> Igre = new List<SifarnikIgara>();
            using (BET_PROEntities1 dbContext = new BET_PROEntities1())
            {
                Igre = dbContext.SifarnikIgaras.ToList();
            }

            return Igre;
        }

        ///<Summary>
        /// XML Komentar
        ///</Summary>
        [HttpGet]
        [Route("api/nbookmakers")]
        public IEnumerable<Kladionice> GetNBookmakers()
        {
            List<Kladionice> klads = new List<Kladionice>();
            using (BET_PROEntities1 dbContext = new BET_PROEntities1())
            {
                klads = dbContext.Kladionices.ToList();
            }

            return klads;
        }

        static IEnumerable<Dictionary<string, object>> Serialize(SqlDataReader reader)
        {
            var results = new List<Dictionary<string, object>>();
            var cols = new List<string>();
            for (var i = 0; i < reader.FieldCount; i++)
                cols.Add(reader.GetName(i));

            while (reader.Read())
                results.Add(SerializeRow(cols, reader));

            return results;
        }
        static Dictionary<string, object> SerializeRow(IEnumerable<string> cols, SqlDataReader reader)
        {
            var result = new Dictionary<string, object>();
            foreach (var col in cols)
                result.Add(col, reader[col]);
            return result;
        }

        ///<Summary>
        /// XML Komentar
        ///</Summary>
        [HttpGet]
        [Route("api/update")]
        public void GetUpdate()
        {
            FinalnaLista();
        }

        ///<Summary>
        /// XML Komentar
        ///</Summary>
        [HttpGet]
        [Route("api/svimecevi")]
        public List<RootUpcomingMatches> GetSviMecevi()
        {
            return UpdateTimer.SviMecevi;
        }

        ///<Summary>
        /// XML Komentar
        ///</Summary>
        [HttpGet]
        [Route("api/linkovikvota")]
        public List<string> GetLinkoviKvota()
        {
            return LinkoviKvota;
        }

        ///<Summary>
        /// XML Komentar
        ///</Summary>
        [HttpGet]
        [Route("api/diag")]
        public DiagnosticRet GetDiag()
        {
            DiagnosticRet dr = new DiagnosticRet();
            dr.VremeUkljucenja = Diagnostic.VremeUkljucenja;
            dr.VremePravljenjaFinalneListe = Diagnostic.VremePravljenjaFinalneListe;
            //dr.Datumi = Diagnostic.Datumi;
            //dr.SviMecevi = Diagnostic.SviMecevi;
            //dr.LinkoviKvota = Diagnostic.LinkoviKvota;
            //dr.Kladionice = Diagnostic.Kladionice;
            dr.Status = Diagnostic.Status;

            if (Diagnostic.Datumi != null)
            {
                using (BET_PROEntities1 dbContext = new BET_PROEntities1())
                {
                    foreach (var d in Diagnostic.Datumi)
                    {
                        string dd = Convert.ToDateTime(d).ToString("yyyy-MM-dd");
                        var p = dbContext.Database.SqlQuery<int?>("SELECT COUNT(id) AS ids FROM dbo.Match WHERE(startDate = '" + dd + "')").FirstOrDefault();
                        if (p != null)
                        {
                            dr.Datumi.Add(d, (int)p);
                        }
                    }
                }
            }

            return dr;
        }

        ///<Summary>
        /// XML Komentar
        ///</Summary>
        [HttpGet]
        [Route("api/getrows")]
        public Dictionary<string,int> GetRows()
        {
            Dictionary<string, int> resp = new Dictionary<string, int>();

                using (BET_PROEntities1 dbContext = new BET_PROEntities1())
                {
                    List<string> datumi = dbContext.Database.SqlQuery<string>("SELECT startDate FROM dbo.Match GROUP BY startDate ORDER BY startDate").ToList();

                    foreach (var d in datumi)
                    {
                        string dd = Convert.ToDateTime(d).ToString("yyyy-MM-dd");
                        var p = dbContext.Database.SqlQuery<int?>("SELECT COUNT(id) AS ids FROM dbo.Match WHERE(startDate = '" + dd + "')").FirstOrDefault();
                        if (p != null)
                        {
                            resp.Add(d, (int)p);
                        }
                    }
                }
            return resp;
        }

        ///<Summary>
        /// XML Komentar
        ///</Summary>
        [HttpGet]
        [Route("api/setdiag/{vreme1}/{vreme2}/{status}")]
        public DiagnosticRet SetDiag(string Vreme1,string Vreme2,string Status)
        {
            DiagnosticRet dr = new DiagnosticRet();
            dr.VremeUkljucenja = Vreme1;
            dr.VremePravljenjaFinalneListe = Vreme2;
            dr.Status = Status;
            Diagnostic.Status = dr.Status;
            Diagnostic.VremeUkljucenja = dr.VremeUkljucenja;
            Diagnostic.VremePravljenjaFinalneListe = dr.VremePravljenjaFinalneListe;
            return dr;
        }

        static string DajMeceveIzBaze()
        {
            SqlCommand cmd;
            SqlDataReader reader, pReader, oReader;
            BET_PROEntities1 dbContext = new BET_PROEntities1();
            dbContext.Database.Connection.Open();

            cmd = new SqlCommand("SELECT id, startDate, endDate, gameId, lifecycle, turnamentId, title FROM dbo.Match ORDER BY startDate", (SqlConnection)dbContext.Database.Connection);
            reader = cmd.ExecuteReader();


            while (reader.Read() == true)
            {
                cMatch heder = new cMatch();
                heder.id = Convert.ToInt64(reader["id"]);
                heder.startDate = reader["startDate"].ToString();
                heder.endDate = reader["endDate"].ToString();
                heder.gameId = (long)reader["gameId"];
                heder.lifecycle = reader["lifecycle"].ToString();
                heder.title = reader["title"].ToString();

                cmd = new SqlCommand("SELECT * FROM Participant WHERE matchId = " + heder.id, (SqlConnection)dbContext.Database.Connection);
                pReader = cmd.ExecuteReader();
                while (pReader.Read() == true)
                {
                    cParticipant tim = new cParticipant();
                    tim.id = Convert.ToInt32(pReader["id"]);
                    tim.natchId = Convert.ToInt64(pReader["matchId"]);
                    tim.teamId = Convert.ToInt64(pReader["teamId"]);
                    tim.name = pReader["name"].ToString();
                    tim.image = pReader["image"].ToString();
                    heder.participants.Add(tim);
                }

                cmd = new SqlCommand("SELECT * FROM Odds WHERE matchId = " + heder.id, (SqlConnection)dbContext.Database.Connection);
                oReader = cmd.ExecuteReader();
                while (oReader.Read() == true)
                {
                    if (oReader["matchId"] != DBNull.Value)
                    {
                        cOdds kvote = new cOdds();
                        kvote.id = Convert.ToInt64(oReader["matchId"]);
                        kvote.name = oReader["name"].ToString();
                        kvote.link = oReader["link"].ToString();
                        kvote.kvota1 = (float)Convert.ToDouble(oReader["kvota1"].ToString());
                        kvote.kvota2 = (float)Convert.ToDouble(oReader["kvota2"].ToString());
                        kvote.proc = (float)Convert.ToDouble(oReader["procenat"].ToString());
                        heder.odds.Add(kvote);
                    }

                }

                MeceviAPI.Add(heder);
            }

            dbContext.Database.Connection.Close();
            string Json = JsonConvert.SerializeObject(Mecevi, Formatting.Indented);
            return Json;
        }

        static string DajMeceveIzBazeZaDatum(string Datum,int Game)
        {
            SqlCommand cmd;
            SqlDataReader reader, pReader, oReader;
            BET_PROEntities1 dbContext = new BET_PROEntities1();
            dbContext.Database.Connection.Open();

            MeceviAPI.Clear();

            if(Game == 0)
                cmd = new SqlCommand("SELECT id, startDate, endDate, gameId, lifecycle, turnamentId, title FROM dbo.Match WHERE startDate = '" + Datum + "'", (SqlConnection)dbContext.Database.Connection);
            else
                cmd = new SqlCommand("SELECT id, startDate, endDate, gameId, lifecycle, turnamentId, title FROM dbo.Match WHERE startDate = '" 
                    + Datum + "' AND gameid = " + Game, (SqlConnection)dbContext.Database.Connection);
            reader = cmd.ExecuteReader();


            while (reader.Read() == true)
            {
                cMatch heder = new cMatch();
                heder.id = Convert.ToInt64(reader["id"]);
                heder.startDate = reader["startDate"].ToString();
                heder.endDate = reader["endDate"].ToString();
                heder.gameId = (long)reader["gameId"];
                heder.lifecycle = reader["lifecycle"].ToString();
                heder.title = reader["title"].ToString();

                cmd = new SqlCommand("SELECT * FROM Participant WHERE matchId = " + heder.id, (SqlConnection)dbContext.Database.Connection);
                pReader = cmd.ExecuteReader();
                while (pReader.Read() == true)
                {
                    cParticipant tim = new cParticipant();
                    tim.id = Convert.ToInt32(pReader["id"]);
                    tim.natchId = Convert.ToInt64(pReader["matchId"]);
                    tim.teamId = Convert.ToInt64(pReader["teamId"]);
                    tim.name = pReader["name"].ToString();
                    tim.image = pReader["image"].ToString();
                    heder.participants.Add(tim);
                }

                cmd = new SqlCommand("SELECT * FROM Odds WHERE matchId = " + heder.id, (SqlConnection)dbContext.Database.Connection);
                oReader = cmd.ExecuteReader();
                while (oReader.Read() == true)
                {
                    if (oReader["matchId"] != DBNull.Value)
                    {
                        cOdds kvote = new cOdds();
                        kvote.id = Convert.ToInt64(oReader["matchId"]);
                        kvote.name = oReader["name"].ToString();
                        kvote.link = oReader["link"].ToString();
                        kvote.kvota1 = (float)Convert.ToDouble(oReader["kvota1"].ToString());
                        kvote.kvota2 = (float)Convert.ToDouble(oReader["kvota2"].ToString());
                        kvote.proc = (float)Convert.ToDouble(oReader["procenat"].ToString());
                        heder.odds.Add(kvote);
                    }

                }

                MeceviAPI.Add(heder);
            }

            dbContext.Database.Connection.Close();
            string Json = JsonConvert.SerializeObject(Mecevi, Formatting.Indented);
            return Json;
        }

    }
}