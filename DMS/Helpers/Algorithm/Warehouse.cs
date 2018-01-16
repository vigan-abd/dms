using Model.Business.ViewModel;
using Model.Domain_Model;
using Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Algorithm
{
    public class Warehouse
    {
        const double WA_PROPERTY_WCR = 0.3;
        const double WA_PROPERTY_KCR = 0.7;

        protected List<SimpleFileViewModel> collection;
        public List<SimpleFileViewModel> Collection { get { return collection; } }

        protected void AtLeastOnceKeywordRule(string term)
        {
            string[] keywords = term.Split(' ');
            string searchTerm = String.Format("'{0}'", keywords[0]);
            for (int i = 1; i < keywords.Length; i++)
            {
                searchTerm += String.Format(", '{0}'", keywords[i]);
            }
            using (var db = UnitOfWorkFactory.Create())
            {
                db.SearchRepository.Open();
                collection = db.SearchRepository.SpAtLeastOnceKeyword(searchTerm);
                db.SearchRepository.Close();
            }
        }

        protected double WordClosenessRule(string property, string term)
        {
            //C -> Numri total i fjaleve ne property
            //N -> Numri total i fjaleve ne kerkim (keyword count)
            //F -> Numri total i perputhjeve (matches count)
            int C = WordCount(property);
            int N = WordCount(term);
            int F = 0;
            double WCR = 0;
            int i = 0, k = N;
            string[] wordGroups = new string[] { term };
            while (k > 1)
            {
                F = CheckForMatches(property, wordGroups);
                if (F > 0)
                {
                    WCR = (double)F * ((double)N - (double)i) / ((double)N * (double)C);
                    break;
                }
                else
                {
                    i++; k--;
                    wordGroups = SubgroupTerm(term, k);
                }
            }

            return WCR;
        }

        protected string[] SubgroupTerm(string term, int groupLength)
        {
            string[] words = term.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int count = words.Length;
            List<string> groups = new List<string>();

            for (int i = 0; i < count; i++)
            {
                if ((i + groupLength) <= count)
                {
                    string[] subgroup = words.ToList().GetRange(i, groupLength).ToArray();
                    groups.Add(string.Join(" ", subgroup));
                }
            }

            return groups.ToArray();
        }

        protected int CheckForMatches(string text, string[] searchList)
        {
            int matches = 0;
            for (int i = 0; i < searchList.Length; i++)
            {
                if (text.ToLower().Contains(searchList[i]))
                    matches++;
            }
            return matches;
        }

        protected int WordCount(string text)
        {
            string[] words = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return words.Length;
        }

        protected double PropertyPriorityRule(double wcr, double kcr)
        {
            return wcr * WA_PROPERTY_WCR + kcr * WA_PROPERTY_KCR;
        }

        public void ProcessAlgorithm(string term)
        {
            AtLeastOnceKeywordRule(term);
            for (int i = 0; i < collection.Count; i++)
            {
                collection[i].Score.WCR = WordClosenessRule(collection[i].Title, term);
                collection[i].Score.Total = PropertyPriorityRule(collection[i].Score.WCR, collection[i].Score.KCR);
            }

            collection = collection.OrderByDescending(c => c.Score.Total).ToList();
        }
    }
}
