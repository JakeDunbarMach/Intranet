using AppModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.ViewModels
{
    public class EntitySearchViewModel : SearchDetail
    {
        //Model for the search filters
        public EntitySearch Search { get; set; }

        //list of SearchResult for the partial view of results
        public IPagedList<EntitySearchResult> SearchResults { get; set; }

    }
}