
using BET_PRO_API;
using System.Collections.Generic;
using System.Linq;


///<Summary>
/// XML Komentar
///</Summary>
public class DBTool
{
    BET_PROEntities1 dbContext = new BET_PROEntities1();

    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public List<SifarnikIgara> DajIgre()
    {
        List<SifarnikIgara> Igre = new List<SifarnikIgara>();
        using (BET_PROEntities1 dbContext = new BET_PROEntities1())
        {
            Igre = dbContext.SifarnikIgaras.SqlQuery("Select * FROM SifarnikIgara ORDER BY id").ToList();
        }

        return Igre;
    }
    //public string STask(int UserID, Sort sOrder, bool fDone, string fDeadLine, int? pageLimit = null, int? page = null)
    //{
    //    pageLimit = pageLimit ?? 2;

    //    using (BET_PROEntities dbContext = new BET_PROEntities())
    //    {

    //        string orderBY = "DeadLine";

    //        switch (sOrder)
    //        {
    //            case (Sort)1:
    //                orderBY = "Title";
    //                break;
    //            case (Sort)2:
    //                orderBY = "UserID";
    //                break;
    //        }
    //        int fd = 0;
    //        if (fDone == true) fd = 1;
    //        IQueryable<Task> taskS = dbContext.Tasks.SqlQuery("SELECT * FROM Tasks WHERE UserID = " + UserID + " AND DeadLine = '" + fDeadLine + "' AND Done = " + fd + " ORDER BY " + orderBY).AsQueryable();

    //        return new CNPagedList<Task>(taskS, page, pageLimit).items;
    //    }
    //}
}