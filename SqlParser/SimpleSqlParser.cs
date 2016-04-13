using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlParser
{
    public class SimpleSqlParser
    {
        public string GetTableFromSql(string sql, string column)
        {
            sql = sql.ToLower();
            var index = sql.LastIndexOf("from") + "from".Length;
            var sb = new StringBuilder();

            // skip to first non whitespace character
            while (index < sql.Length && char.IsWhiteSpace(sql[index]))
            {
                index++;
            }

            while (index < sql.Length && !char.IsWhiteSpace(sql[index]))
            {
                sb.Append(sql[index]);
                index++;
            }


            return sb.ToString();
        }
    }
}
