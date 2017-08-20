using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LuceneSearch.Service;
using System.IO;
using Wrly.Data.Models.Extended;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.Infrastructure.Processors.Implementations;
using System.Threading.Tasks;

namespace Wrly.Controllers
{
    public class LuceneController : BaseController
    {
        private ISearchProcessor _searchProcessor;

        public ISearchProcessor SearchProcessorLucene
        {
            get
            {
                if (_searchProcessor == null)
                {
                    _searchProcessor = new SearchProcessor();
                }
                return _searchProcessor;
            }
        }


        public ActionResult Index(string searchTerm, string searchField, bool? searchDefault, int? limit)
        {
            // create default Lucene search index directory
            if (!Directory.Exists(GoLucene._luceneDir)) Directory.CreateDirectory(GoLucene._luceneDir);

            // perform Lucene search
            List<LuceneObject> _searchResults;
            if (searchDefault == true)
                _searchResults = (string.IsNullOrEmpty(searchField)
                                   ? GoLucene.SearchDefault(searchTerm)
                                   : GoLucene.SearchDefault(searchTerm, searchField)).ToList();
            else
                _searchResults = (string.IsNullOrEmpty(searchField)
                                   ? GoLucene.Search(searchTerm)
                                   : GoLucene.Search(searchTerm, searchField)).ToList();
            if (string.IsNullOrEmpty(searchTerm) && !_searchResults.Any())
                _searchResults = GoLucene.GetAllIndexRecords().ToList();
            
            return View(_searchResults);

            //// setup and return view model
            //var search_field_list = new
            //    List<SelectedList> {
            //                        new SelectedList {Text = "(All Fields)", Value = ""},
            //                        new SelectedList {Text = "Id", Value = "Id"},
            //                        new SelectedList {Text = "Name", Value = "Name"},
            //                        new SelectedList {Text = "Description", Value = "Description"}
            //                     };


            //// limit display number of database records
            //var limitDb = limit == null ? 3 : Convert.ToInt32(limit);
            //List<LuceneObject> allSampleData;
            //if (limitDb > 0)
            //{
            //    allSampleData = SampleDataRepository.GetAll().ToList().Take(limitDb).ToList();
            //    ViewBag.Limit = SampleDataRepository.GetAll().Count - limitDb;
            //}
            //else allSampleData = SampleDataRepository.GetAll();

            //return View(new IndexViewModel
            //{
            //    AllSampleData = allSampleData,
            //    SearchIndexData = _searchResults,
            //    SampleData = new LuceneObject { Id = 9, Name = "El-Paso", Description = "City in Texas" },
            //    SearchFieldList = search_field_list,
            //});
        }

        public ActionResult Search(string searchTerm, string searchField, string searchDefault)
        {
            return RedirectToAction("Index", new { searchTerm, searchField, searchDefault });
        }

        public async Task<ActionResult> CreateIndex()
        {
            var results = await SearchProcessorLucene.GetLuceneIndexableData();
            GoLucene.AddUpdateLuceneIndex(results);
            TempData["Result"] = "Search index was created successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AddToIndex(LuceneObject sampleData)
        {
            GoLucene.AddUpdateLuceneIndex(sampleData);
            TempData["Result"] = "Record was added to search index successfully!";
            return RedirectToAction("Index");
        }

        public ActionResult ClearIndex()
        {
            if (GoLucene.ClearLuceneIndex())
                TempData["Result"] = "Search index was cleared successfully!";
            else
                TempData["ResultFail"] = "Index is locked and cannot be cleared, try again later or clear manually!";
            return RedirectToAction("Index");
        }

        public ActionResult ClearIndexRecord(int id)
        {
            GoLucene.ClearLuceneIndexRecord(id);
            TempData["Result"] = "Search index record was deleted successfully!";
            return RedirectToAction("Index");
        }

        public ActionResult OptimizeIndex()
        {
            GoLucene.Optimize();
            TempData["Result"] = "Search index was optimized successfully!";
            return RedirectToAction("Index");
        }
    }
}