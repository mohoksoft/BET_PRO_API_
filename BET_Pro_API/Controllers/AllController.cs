﻿using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BET_PRO_API.Controllers
{
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public class AllController : ApiController
    {
        List<RootUpcomingMatches> cRootUpcomingMatchesLoL = new List<RootUpcomingMatches>();
        List<RootUpcomingMatches> cRootUpcomingMatchesDOTA2 = new List<RootUpcomingMatches>();
        List<RootUpcomingMatches> cRootUpcomingMatchesCSGO = new List<RootUpcomingMatches>();
        List<RootUpcomingMatches> cRootUpcomingMatchesValorant = new List<RootUpcomingMatches>();
        List<RootUpcomingMatches> SviMecevi = new List<RootUpcomingMatches>();

        private const string INDENT_STRING = "    ";

        ///<Summary>
        /// XML Komentar
        ///</Summary>
        ///
        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("api/all/getupcomingmatches")]
        public List<Izlaz> Get()
        {
            string mecevi = RimbleAPI("lol");
            mecevi = CheckJson(mecevi);
            JsonConvert.PopulateObject(mecevi, cRootUpcomingMatchesLoL);
            SviMecevi.AddRange(cRootUpcomingMatchesLoL);

            mecevi = RimbleAPI("csgo");
            mecevi = CheckJson(mecevi);
            JsonConvert.PopulateObject(mecevi, cRootUpcomingMatchesCSGO);
            SviMecevi.AddRange(cRootUpcomingMatchesCSGO);

            mecevi = RimbleAPI("dota2");
            mecevi = CheckJson(mecevi);
            JsonConvert.PopulateObject(mecevi, cRootUpcomingMatchesDOTA2);
            SviMecevi.AddRange(cRootUpcomingMatchesDOTA2);

            mecevi = RimbleAPI("valorant");
            mecevi = CheckJson(mecevi);
            JsonConvert.PopulateObject(mecevi, cRootUpcomingMatchesValorant);
            SviMecevi.AddRange(cRootUpcomingMatchesValorant);

            List<Izlaz> newList = SviMecevi.Select(m => new Izlaz
            {
                team_1_name = m.team_1_name,
                team_2_name = m.team_2_name,
                date = m.date,
                time = m.time,
                match_format = m.match_format
            }).ToList();

            return newList;
        }

        private string RimbleAPI(string Igra)
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

        static string FormatJson(string json)
        {

            int indentation = 0;
            int quoteCount = 0;
            var result =
                from ch in json
                let quotes = ch == '"' ? quoteCount++ : quoteCount
                let lineBreak = ch == ',' && quotes % 2 == 0 ? ch + Environment.NewLine + String.Concat(Enumerable.Repeat(INDENT_STRING, indentation)) : null
                let openChar = ch == '{' || ch == '[' ? ch + Environment.NewLine + String.Concat(Enumerable.Repeat(INDENT_STRING, ++indentation)) : ch.ToString()
                let closeChar = ch == '}' || ch == ']' ? Environment.NewLine + String.Concat(Enumerable.Repeat(INDENT_STRING, --indentation)) + ch : ch.ToString()
                select lineBreak == null
                            ? openChar.Length > 1
                                ? openChar
                                : closeChar
                            : lineBreak;

            return String.Concat(result);
        }

        string CheckJson(string sJson)
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
                n = gde+2;
            }

            return sJson;
        }

        ///<Summary>
        /// XML Komentar
        ///</Summary>
        public List<RootUpcomingMatches> DajListuMeceva()
        {
            return SviMecevi;
        }
    }
}