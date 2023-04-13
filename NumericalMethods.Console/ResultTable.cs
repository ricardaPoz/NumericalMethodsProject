using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericalMethods.Console
{
    public class ResultTable
    {
        private int _differentialOrder;
        // TODO: Возвращать неизменяемую коллекцию
        public List<string> Header { get; }
        private List<Dictionary<string, double>> Body { get; }
        public int CountRow => Body.Count;
        public ResultTable(int differentialOrder)
        {
            _differentialOrder = differentialOrder;
            Header = new List<string>(differentialOrder + 1);
            Header.Add("x");
            for (int i = 0; i < differentialOrder + 1; i++)
            {
                Header.Add($"y{i}");
            }
            Body = new List<Dictionary<string, double>>();
        }

        public List<(double x, double yi)> this[string variableName]
        {
            get
            {
                int index = Header.IndexOf(variableName);
                if (index == -1) throw new ArgumentException($"Invalid {nameof(variableName)}");
                List<(double x, double yi)> result = new List<(double x, double yi)>();
                foreach (Dictionary<string, double> row in Body)
                {
                    result.Add((row["x"], row[variableName]));
                }
                return result;
            }
        }

        public ResultTable Add(double x, Dictionary<string, double> ys)
        {
            Dictionary<string, double> sortedYs = ys
                .OrderBy(y => y.Key)
                .Prepend(new KeyValuePair<string, double>("x", x))
                .ToDictionary(pair => pair.Key, pair => pair.Value);
            Body.Add(sortedYs);
            return this;
        }

        public ResultTable EditRowItem(int row_index, string item_key, double new_value)
        {
            Body[row_index][item_key] = new_value;
            return this;
        }

        public List<Dictionary<string, double>> GetRows(Range range)
        {
            var (start, length) = range.GetOffsetAndLength(CountRow);
            return Body.GetRange(start, length);
        }

        public Dictionary<string, double> GetRow(int index)
        {
            return Body[index];
        }
    }
}
