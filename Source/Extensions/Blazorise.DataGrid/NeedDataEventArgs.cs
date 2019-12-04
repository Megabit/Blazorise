using System;
using System.Collections.Generic;
using System.Text;

namespace Blazorise.DataGrid
{
    public class NeedDataEventArgs<TItem>
    {
        private int _currentPage;
        private int _pageSize;
        private IEnumerable<BaseDataGridColumn<TItem>> _columns;
        private IEnumerable<TItem> _data;
        private int _numberOfRows;

        public NeedDataEventArgs(int currentPage, int pageSize,
            IEnumerable<BaseDataGridColumn<TItem>> columns
            ,int numberOfRows, IEnumerable<TItem> data)
        {
            _currentPage = currentPage;
            _pageSize = pageSize;
            _columns = columns;
            _data = data;
            _numberOfRows = numberOfRows;
        }

        public int CurrentPage => _currentPage;
        public int PageSize => _pageSize;
        public int EndPageRange => _currentPage * _pageSize;
        public int StartPageRange => EndPageRange - (_pageSize - 1);
        public IEnumerable<BaseDataGridColumn<TItem>> Columns => _columns;
        public int NumberOfRows { get; set; }
        public IEnumerable<TItem> Data
        {
            get { return _data; }
            set { _data = value; }
        }
    }
}
