using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace BET_PRO_API
{

    // A class that handles pagination of an IQueryable
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public class CNPagedList<Match>
    {
        /*
        * list - The full list of items you would like to paginate</param>
        * page - (optional) The current page number</param>
        * pageSize - (optional) The size of the page</param>
        */
        ///<Summary>
        /// XML Komentar
        ///</Summary>
        public CNPagedList(List<Match> list, int? page = null, int? pageSize = null)
        {
            _list = list;
            _page = page;
            _pageSize = pageSize;
        }

        private List<Match> _list;

        // The paginated result
        ///<Summary>
        /// XML Komentar
        ///</Summary>
        public List<Match> items
        {
            get
            {
                if (_list == null) return null;

                //var json = JsonConvert.SerializeObject(_list.Skip((page - 1) * pageSize).Take(pageSize).ToArray());
                List<Match> json = _list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return json;
            }
        }

        private int? _page;

        //  The current page.
        ///<Summary>
        /// XML Komentar
        ///</Summary>
        public int page
        {
            get
            {
                if (!_page.HasValue)
                {
                    return 1;
                }
                else
                {
                    return _page.Value;
                }
            }
        }

        private int? _pageSize;

        // The size of the page.
        ///<Summary>
        /// XML Komentar
        ///</Summary>
        public int pageSize
        {
            get
            {
                if (!_pageSize.HasValue)
                {
                    return _list == null ? 0 : _list.Count();
                }
                else
                {
                    return _pageSize.Value;
                }
            }
        }


        // The total number of items in the original list of items.
        ///<Summary>
        /// XML Komentar
        ///</Summary>
        public int totalItemCount
        {
            get
            {
                return _list == null ? 0 : _list.Count();
            }
        }
    }

}