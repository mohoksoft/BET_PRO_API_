using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Http;

namespace BET_PRO_API.Controllers
{
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public class GetListController : ApiController
    {
        ///<Summary>
        /// XML Komentar
        ///</Summary>
        public UpdateTimer utOdds = new UpdateTimer();

        ///<Summary>
        /// XML Komentar
        ///</Summary>
        [HttpGet]
        [Route("api/updateodds")]
        public List<cMatch> Get()
        {
            return UpdateTimer.Mecevi;

        }


    }
}