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
    public class EntitySearchController : SystemControllerBase
    {

        /// <summary>
        /// Load the search page. If called from paging the only reload the partial view _Results with the paged information
        /// </summary>
        /// <param name="SearchViewModel">model supplied from search</param>
        /// <param name="page">page number selected</param>
        /// <returns>Full view if initial load otherwise partial view _Results with the page requested</returns>
        public ActionResult Search(EntitySearchViewModel entitySearchViewModel, int? page, bool order = false)
        {
            if (entitySearchViewModel.Search == null && (EntitySearchViewModel)Session["entitySearchCriteria"] != null)
            {
                entitySearchViewModel = (EntitySearchViewModel)Session["entitySearchCriteria"];
                entitySearchViewModel.ExportResults = false;
            }

            if (order == true)
            {
                EntitySearchViewModel entitySearchViewModelOrder = (EntitySearchViewModel)Session["entitySearchCriteria"];
                entitySearchViewModel.PageNumber = entitySearchViewModelOrder.PageNumber;
                Session.Add("entitySearchCriteria", entitySearchViewModel);
            }

            //If the session model doesn't match the model on the form update the session model
            if ((EntitySearchViewModel)Session["entitySearchCriteria"] != entitySearchViewModel)
            {
                entitySearchViewModel.PageNumber = page ?? 1;
                Session.Add("entitySearchCriteria", entitySearchViewModel);
            }

            if (entitySearchViewModel.Search == null)
            {
                entitySearchViewModel.Search = new EntitySearch();
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
                string orderBy = "";
                if (entitySearchViewModel.OrderBy != null)
                {
                    orderBy = entitySearchViewModel.OrderBy.Trim();
                }

                switch (orderBy)
                {
                    case "Ref_desc":
                        searchResults = searchResults.OrderByDescending(r => r.EntityRef).ToList();
                        break;
                    case "Ref":
                        searchResults = searchResults.OrderBy(r => r.EntityRef).ToList();
                        break;
                    case "Type_desc":
                        searchResults = searchResults.OrderByDescending(r => r.EntityTypeName).ToList();
                        break;
                    case "Type":
                        searchResults = searchResults.OrderBy(r => r.EntityTypeName).ToList();
                        break;
                    case "Date_desc":
                        searchResults = searchResults.OrderByDescending(r => r.EntityDate).ToList();
                        break;
                    case "Date":
                        searchResults = searchResults.OrderBy(r => r.EntityDate).ToList();
                        break;
                    default:
                        //set the default OrderBy
                        searchResults = searchResults.OrderByDescending(r => r.EntityRef).ToList();
                        entitySearchViewModel.OrderBy = "Ref_desc";
                        break;
                }

                //Read the pagingrows property from the webconfig and set the model searchResults to be the paged results
                int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings[AppData.Constants.Configuration.PAGING_ROWS]);
                int pageNumber = (page ?? 1);
                if (entitySearchViewModel.PageNumber != 0)
                {
                    pageNumber = entitySearchViewModel.PageNumber;
                }
                else
                {
                    pageNumber = (page ?? 1);
                }
                if (searchResults != null)
                {
                    entitySearchViewModel.SearchResults = searchResults.ToPagedList(pageNumber, pageSize);
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