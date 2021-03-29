using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TR.OGT.QueryExtract.Data
{
    /*
	 * TODO:
	 * CTE support
	 * Add comments
	 */
    public class SqlBuilder
    {
        private readonly string SqlTemplate = @"
SELECT 
{0} 
FROM {1} tmp (NOLOCK)
{2} 
{3} 
{4} 
";
        private readonly StringBuilder _select;
        private readonly StringBuilder _join;
        private readonly StringBuilder _where;

        private string FromTable { get; set; }

        public SqlBuilder()
        {
            _select = new StringBuilder();
            _join = new StringBuilder();
            _where = new StringBuilder();
        }

        public SqlBuilder Select(IEnumerable<string> columns)
        {
            if (columns == null)
                throw new ArgumentNullException($"Argument {nameof(columns)} null or empty.");

            if (!columns.Any())
                return this;

            foreach (var column in columns)
                Select(column);
            return this;
        }

        public SqlBuilder Select(string column)
        {
            if (string.IsNullOrEmpty(column))
                throw new ArgumentNullException($"Argument {nameof(column)} null or empty.");

            _select.AppendLine($"{column},");
            return this;
        }

        public SqlBuilder From(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException($"Argument {nameof(tableName)} null or empty.");

            FromTable = tableName;
            return this;
        }

        public SqlBuilder Join(IEnumerable<string> joinExpressions)
        {
            if (joinExpressions == null)
                throw new ArgumentNullException($"Argument {nameof(joinExpressions)} null or empty.");

            if (!joinExpressions.Any())
                return this;

            foreach (var expr in joinExpressions)
                Join(expr);
            return this;
        }

        public SqlBuilder Join(string joinExpression)
        {
            if (string.IsNullOrEmpty(joinExpression))
                throw new ArgumentNullException($"Argument {nameof(joinExpression)} is null or empty.");

            _join.AppendLine(joinExpression);
            return this;
        }

        public SqlBuilder Where(string whereExpression)
        {
            if (whereExpression == null)
                throw new ArgumentNullException($"Argument {nameof(whereExpression)} is null.");

            if (whereExpression.Equals(string.Empty))
                return this;

            _where.AppendLine($"WHERE {whereExpression}");
            return this;
        }

        public string Build()
            => string.Format(
                SqlTemplate,
                _select.ToString().TrimEnd().TrimEnd(','),
                FromTable,
                _join.ToString().TrimEnd(),
                _where.ToString().TrimEnd(),
                string.Empty);
    }
}
