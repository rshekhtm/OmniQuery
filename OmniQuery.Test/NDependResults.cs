using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmniQuery.Test
{
    public class NDependResults : List<NDependResults.ResultRow>
    {
        private int _itemsExamined = 0;

        public int ItemsExamined
        {
            get { return _itemsExamined; }
        }

        public NDependResults() { }

        public NDependResults(int itemsExamined, IEnumerable<ResultRow> rows)
        {
            _itemsExamined = itemsExamined;
            AddRange(rows);
        }

        public class ResultRow
        {
            private string _name;
            private string _fullName;
            private string _containerName;
            private int[] _measurements;

            public string Name
            {
                get { return _name; }
            }

            public string FullName
            {
                get { return _fullName; }
            }

            public string ContainerName
            {
                get { return _containerName; }
            }

            public int[] Measurements
            {
                get { return _measurements; }
            }

            public ResultRow(string name, string fullName, int[] measurements)
            {
                _name = name;
                _fullName = fullName;
                _containerName = _fullName.Replace(_name, string.Empty).TrimEnd('.');
                _measurements = measurements;
            }
        }
    }
}
