using Helpers.Algorithm;
using Model.Business.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public static class SearchService
    {
        public static List<SimpleFileViewModel> Search(string term)
        {
            Warehouse algorithm = new Warehouse();
            algorithm.ProcessAlgorithm(term);
            return algorithm.Collection;
        }
    }
}
