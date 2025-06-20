using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce_BE_API.DbContext.Models
{
    public class Paging
    {
        public int TotalRows { get; set; }
        public int LimitPage { get; set; }
        public int CurPage { get; set; }
        public decimal TotalPage
        {
            get { return Math.Ceiling(TotalRows / (decimal)LimitPage); }
        }
        public int start
        {
            get { return (CurPage - 1) * LimitPage; }
        }
        public int offset { get { return LimitPage; } }
        public int startIndex { get { return start; } }

        public Paging(int _TotalRows, int _CurPage, int _limit)
        {
            TotalRows = _TotalRows;
            CurPage = _CurPage > 0 ? _CurPage : 1;
            LimitPage = _limit;
        }
    }

    public class SortField
    {
        public string FieldName { get; set; }
        public bool IsDesc { get; set; }

    }
}
