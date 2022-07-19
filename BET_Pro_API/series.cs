// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
using System;
using System.Collections.Generic;


///<Summary>
/// XML Komentar
///</Summary>
public class cMatch
{
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public cMatch()
    {
        participants = new List<cParticipant>();
        odds = new List<cOdds>();
    }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public long id { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string startDate { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string endDate { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public long gameId { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string lifecycle { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public object tournamentId { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string title { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public List<cParticipant> participants { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public cGame game { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public List<cOdds> odds { get; set; }
}


///<Summary>
/// XML Komentar
///</Summary>
public class Best
{
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public int id { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public int providerId { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public double value { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string link { get; set; }
}

///<Summary>
/// XML Komentar
///</Summary>
public class cGame
{
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public int id { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string abbreviation { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string title { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string default_match_type { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string color { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public DateTime created_at { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public DateTime updated_at { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public List<cImage> images { get; set; }
}

///<Summary>
/// XML Komentar
///</Summary>
public class cImage
{
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public int id { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string url { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string thumbnail { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string fallback { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string entity_type { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public int entity_id { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public DateTime created_at { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public DateTime updated_at { get; set; }
}

///<Summary>
/// XML Komentar
///</Summary>
public class cOdds
{
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string name { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public int providerId { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string link { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public long id { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public float kvota1 { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public float kvota2 { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public float proc { get; set; }
}

///<Summary>
/// XML Komentar
///</Summary>
public class cParticipant
{
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public int id { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public long natchId { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public long teamId { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string name { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string image { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public List<cOdds> odds { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public Best best { get; set; }
}



