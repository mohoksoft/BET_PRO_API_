using System;
using System.Collections.Generic;


///<Summary>
/// XML Komentar
///</Summary>
public class LeagueDetails
{
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string name { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string id { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string image { get; set; }
}

///<Summary>
/// XML Komentar
///</Summary>
public class MatchFormat
{
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string type { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public int count { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public List<Metadata> metadata { get; set; }
}

///<Summary>
/// XML Komentar
///</Summary>
public class Metadata
{
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public int game_number { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string est_date { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string est_time { get; set; }
}

///<Summary>
/// XML Komentar
///</Summary>
public class Player
{
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string id { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string username { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string DOB { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string image { get; set; }
}

///<Summary>
/// XML Komentar
///</Summary>
public class RootUpcomingMatches
{
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string date { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string time { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string source_url { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string league { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public LeagueDetails league_details { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string team_1_name { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string team_2_name { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public MatchFormat match_format { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public List<string> stream_links { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public List<Team> teams { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public long matchid { get; set; }


}

///<Summary>
/// XML Komentar
///</Summary>
public class Team
{
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string id { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string name { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public int designation { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string image { get; set; }
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public List<Player> players { get; set; }
}

///<Summary>
/// XML Komentar
///</Summary>
public static class CheckHelper
{
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public static List<int> AllIndexesOf(this string str, string value)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException("the string to find may not be empty", "value");
        List<int> indexes = new List<int>();
        for (int index = 0; ; index += value.Length)
        {
            index = str.IndexOf(value, index);
            if (index == -1)
                return indexes;
            indexes.Add(index);
        }
    }
}

///<Summary>
/// XML Komentar
///</Summary>
public class Izlaz
{
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string team_1_name { get; set; }

    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string team_2_name { get; set; }

    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string date { get; set; }

    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public string time { get; set; }

    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public MatchFormat match_format { get; set; }
}

