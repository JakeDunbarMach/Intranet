using AppData;
using AppModel;
using WebUI.Controllers.Custom;
using WebUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Configuration;
using AppModel.Constants;
using System.Text;

namespace WebUI.Controllers
{
    [CustomAuthorize(SectionID = (int)SectionEnum.Sections.ENTITY, SystemID = 9999)]
    public class EntitySearchDTController : SystemControllerBase
    {

        /// <summary>
        /// Load the search page. If called from paging the only reload the partial view _Results with the paged information
        /// </summary>
        /// <param name="SearchViewModel">model supplied from search</param>
        /// <param name="page">page number selected</param>
        /// <returns>Full view if initial load otherwise partial view _Results with the page requested</returns>
        public ActionResult Search(EntitySearchViewModel entitySearchViewModel, int? page)
        {
            if (entitySearchViewModel.Search == null && (EntitySearchViewModel)Session["entitySearchCriteria"] != null)
            {
                entitySearchViewModel = (EntitySearchViewModel)Session["entitySearchCriteria"];
                entitySearchViewModel.ExportResults = false;
            }

            //If the session model doesn't match the model on the form update the session model
            if ((EntitySearchViewModel)Session["entitySearchCriteria"] != entitySearchViewModel)
            {
                Session.Add("entitySearchCriteria", entitySearchViewModel);
            }

            if (entitySearchViewModel.Search == null)
            {
                entitySearchViewModel.Search = new AppModel.EntitySearch();
            }

            int userID = Convert.ToInt32(HttpContext.Session["UserID"]);
            AppData.EntitySearchResult searchResult = new AppData.EntitySearchResult();
            List<AppModel.EntitySearchResult> searchResults = searchResult.GetEntityList(entitySearchViewModel.Search, userID);


            if (searchResults == null)
            {
                searchResults = new List<AppModel.EntitySearchResult>();
            }

            if (entitySearchViewModel.ExportResults == true)
            {
                StringBuilder stringBuilder = new StringBuilder();
                // Add a line for headings
                stringBuilder.AppendLine("\"Entity Ref\",\"Entity Date\",\"Entity Type\"");

                if (searchResults != null)
                {
                    // Add searchResultsList items
                    for (int i = 0; i < searchResults.Count; ++i)
                    {
                        stringBuilder.AppendLine("\"" + CSVFieldFormat(searchResults[i].EntityRef) + "\",\"" + searchResults[i].EntityDate + "\",\""
                            + CSVFieldFormat(searchResults[i].EntityTypeName) + "\"");
                    }
                }
                byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                // Return bytes with the file name to the browser
                return File(bytes, "application/ms-excel", "Entities Export.csv");
            }
            else
            {
                if (searchResults != null)
                {
                    //entitySearchViewModel.SearchResults = searchResults;
                }

                FillLookupLists();

                //If come from jQuery request
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_Results", entitySearchViewModel);
                }
                return View(entitySearchViewModel);
            }

        }


        /// <summary>
        /// Populate the lookup lists in the ViewBag using values from the model provided
        /// </summary>
        private void FillLookupLists()
        {
            AppData.GenericList genericListData = new AppData.GenericList();
            ViewBag.EntityTypeList = CreateSelectList(genericListData.GetEntityTypeList(), x => x.ID, x => x.Name);

        }

    }
}