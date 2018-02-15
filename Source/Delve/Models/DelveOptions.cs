﻿namespace Delve.Models
{
    public class DelveOptions
    {
        private int _defaultPageSize = 5;
        public int DefaultPageSize
        {
            get { return _defaultPageSize; }
            set
            {
                if (value > 0) { _defaultPageSize = value; }
            }
        }

        private int _maxPageSize = 25;
        public int MaxPageSize
        {
            get { return _maxPageSize; }
            set
            {
                if (value > 0) { _maxPageSize = value; }
            }
        }

        public bool IgnoreNullOnSerilazation { get; set; } = true;
    }
}
