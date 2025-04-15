using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Paginations
{
    public class PaginationResponse<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public List<T> Data { get; set; }

        public PaginationResponse(List<T> data, int pageNumber, int pageSize,  int totalRecords)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
          
            TotalRecords = totalRecords;
            Data = data;
        }
    }
}
