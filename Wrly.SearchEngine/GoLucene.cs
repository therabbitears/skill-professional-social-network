using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;
using Wrly.Data.Models.Extended;

namespace LuceneSearch.Service
{
    public static class GoLucene
    {
        // properties
        public static string _luceneDir =
            Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "lucene_index");
        private static FSDirectory _directoryTemp;
        private static FSDirectory _directory
        {
            get
            {
                if (_directoryTemp == null) _directoryTemp = FSDirectory.Open(new DirectoryInfo(_luceneDir));
                if (IndexWriter.IsLocked(_directoryTemp)) IndexWriter.Unlock(_directoryTemp);
                var lockFilePath = Path.Combine(_luceneDir, "write.lock");
                if (File.Exists(lockFilePath)) File.Delete(lockFilePath);
                return _directoryTemp;
            }
        }

        // search methods
        public static IEnumerable<LuceneObject> GetAllIndexRecords()
        {
            // validate search index
            if (!System.IO.Directory.EnumerateFiles(_luceneDir).Any()) return new List<LuceneObject>();

            // set up lucene searcher
            var searcher = new IndexSearcher(_directory, false);
            var reader = IndexReader.Open(_directory, false);
            var docs = new List<Document>();
            var term = reader.TermDocs();
            // v 2.9.4: use 'hit.Doc()'
            // v 3.0.3: use 'hit.Doc'
            while (term.Next()) docs.Add(searcher.Doc(term.Doc));
            reader.Dispose();
            searcher.Dispose();
            return _mapLuceneToDataList(docs);
        }
        public static IEnumerable<LuceneObject> Search(string input, string fieldName = "")
        {
            if (string.IsNullOrEmpty(input)) return new List<LuceneObject>();

            var terms = input.Trim().Replace("-", " ").Split(' ')
                .Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim() + "*");
            input = string.Join(" ", terms);

            return _search(input, fieldName);
        }
        public static IEnumerable<LuceneObject> SearchDefault(string input, string fieldName = "")
        {
            return string.IsNullOrEmpty(input) ? new List<LuceneObject>() : _search(input, fieldName);
        }

        // main search method
        private static IEnumerable<LuceneObject> _search(string searchQuery, string searchField = "")
        {
            // validation
            if (string.IsNullOrEmpty(searchQuery.Replace("*", "").Replace("?", ""))) return new List<LuceneObject>();

            // set up lucene searcher
            using (var searcher = new IndexSearcher(_directory, false))
            {
                var hits_limit = 1000;
                var analyzer = new StandardAnalyzer(Version.LUCENE_30);

                // search by single field
                if (!string.IsNullOrEmpty(searchField))
                {
                    var parser = new QueryParser(Version.LUCENE_30, searchField, analyzer);
                    var query = parseQuery(searchQuery, parser);
                    var hits = searcher.Search(query, hits_limit).ScoreDocs;
                    var results = _mapLuceneToDataList(hits, searcher);
                    analyzer.Close();
                    searcher.Dispose();
                    return results;
                }
                // search by multiple fields (ordered by RELEVANCE)
                else
                {
                    var parser = new MultiFieldQueryParser(Version.LUCENE_30, new[] { "DisplayName", "Headiing", "SkillText", "WorkHistoryText", "EducationHistoryText" }, analyzer);
                    var query = parseQuery(searchQuery, parser);
                    var hits = searcher.Search(query, null, hits_limit, Sort.INDEXORDER).ScoreDocs;
                    var results = _mapLuceneToDataList(hits, searcher);
                    analyzer.Close();
                    searcher.Dispose();
                    return results;
                }
            }
        }
        private static Query parseQuery(string searchQuery, QueryParser parser)
        {
            Query query;
            try
            {
                query = parser.Parse(searchQuery.Trim());
            }
            catch (ParseException)
            {
                query = parser.Parse(QueryParser.Escape(searchQuery.Trim()));
            }
            return query;
        }

        // map Lucene search index to data
        private static IEnumerable<LuceneObject> _mapLuceneToDataList(IEnumerable<Document> hits)
        {
            return hits.Select(_mapLuceneDocumentToData).ToList();
        }
        private static IEnumerable<LuceneObject> _mapLuceneToDataList(IEnumerable<ScoreDoc> hits, IndexSearcher searcher)
        {
            // v 2.9.4: use 'hit.doc'
            // v 3.0.3: use 'hit.Doc'
            return hits.Select(hit => _mapLuceneDocumentToData(searcher.Doc(hit.Doc))).ToList();
        }
        private static LuceneObject _mapLuceneDocumentToData(Document doc)
        {
            return new LuceneObject
            {
                EntityID = Convert.ToInt64(doc.Get("EntityID")),
                EntityType = Convert.ToInt32(doc.Get("EntityType")),
                DisplayName = doc.Get("DisplayName"),
                EducationHistoryText = doc.Get("EducationHistoryText"),
                Headiing = doc.Get("Headiing"),
                LastModified = !string.IsNullOrEmpty(doc.Get("LastModified")) ? Convert.ToDateTime(doc.Get("LastModified")) : new DateTime(2017, 2, 24),
                ProfilePicUrl = doc.Get("ProfilePicUrl"),
                SkillText = doc.Get("SkillText"),
                Url = doc.Get("Url"),
                SubType = !string.IsNullOrEmpty(doc.Get("SubType")) ? Convert.ToByte(doc.Get("SubType")) : (default(byte?)),
                WorkHistoryText = doc.Get("WorkHistoryText")
            };
        }

        // add/update/clear search index data 
        public static void AddUpdateLuceneIndex(LuceneObject data)
        {
            AddUpdateLuceneIndex(new List<LuceneObject> { data });
        }
        public static void AddUpdateLuceneIndex(IEnumerable<LuceneObject> dataList)
        {
            // init lucene
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                // add data to lucene search index (replaces older entries if any)
                foreach (var data in dataList) _addToLuceneIndex(data, writer);

                // close handles
                analyzer.Close();
                writer.Dispose();
            }
        }
        public static void ClearLuceneIndexRecord(int record_id)
        {
            // init lucene
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                // remove older index entry
                var searchQuery = new TermQuery(new Term("Id", record_id.ToString()));
                writer.DeleteDocuments(searchQuery);

                // close handles
                analyzer.Close();
                writer.Dispose();
            }
        }
        public static bool ClearLuceneIndex()
        {
            try
            {
                var analyzer = new StandardAnalyzer(Version.LUCENE_30);
                using (var writer = new IndexWriter(_directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    // remove older index entries
                    writer.DeleteAll();

                    // close handles
                    analyzer.Close();
                    writer.Dispose();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public static void Optimize()
        {
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                analyzer.Close();
                writer.Optimize();
                writer.Dispose();
            }
        }

        private static void _addToLuceneIndex(LuceneObject data, IndexWriter writer)
        {
            // remove older index entry
            var searchQuery = new TermQuery(new Term("EntityID", data.EntityID.ToString()));
            writer.DeleteDocuments(searchQuery);

            // add new index entry
            var doc = new Document();

            // add lucene fields mapped to db fields
            doc.Add(new Field("EntityID", data.EntityID.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("EntityType", data.EntityType.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Url", data.Url.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("ProfilePicUrl", data.ProfilePicUrl.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("LastModified", data.LastModified.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));


            // Not analyzed means not will searched
            doc.Add(new Field("DisplayName", data.DisplayName, Field.Store.YES, Field.Index.ANALYZED) { Boost = 3 });
            doc.Add(new Field("SkillText", data.SkillText, Field.Store.YES, Field.Index.ANALYZED) { Boost = 1 });
            doc.Add(new Field("WorkHistoryText", data.WorkHistoryText, Field.Store.YES, Field.Index.ANALYZED) { Boost = 1 });
            doc.Add(new Field("EducationHistoryText", data.EducationHistoryText, Field.Store.YES, Field.Index.ANALYZED) { Boost = 1 });
            doc.Add(new Field("Headiing", data.Headiing, Field.Store.YES, Field.Index.ANALYZED) { Boost = 2 });
            doc.Add(new Field("SubType", data.SubType.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

            // add entry to index
            writer.AddDocument(doc);
        }

    }
}