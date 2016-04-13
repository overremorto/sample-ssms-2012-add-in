using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMSAddin
{
    public class SelectedCell
    {
        public SelectedCell()
        {
            Values = new List<string>();
        }

        public string ColumnName { get; set; }

        public List<string> Values { get; set; }

        public string WhereClause()
        {
            if (Values?.Count == 1)
            {
                return $" = {Values[0]}";
            }
            else if (Values?.Count > 1)
            {
                return $" IN ({string.Join(",", Values)})";
            }

            return string.Empty;
        }
    }
}
