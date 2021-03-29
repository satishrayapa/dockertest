using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TR.OGT.QueryExtract.Data.Tests.Sql
{
    [TestClass]
    public class SqlBuilderTests
    {
        private static string SqlTemplate = @"
SELECT 
{0} 
FROM {1} tmp (NOLOCK)
{2} 
{3} 
{4} 
";

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Select_SingleColumn_ArgumentIsNull_Exception()
        {
            var builder = new SqlBuilder();
            builder.Select((string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Select_SingleColumn_ArgumentIsEmpty_Exception()
        {
            var builder = new SqlBuilder();
            builder.Select(string.Empty);
        }

        [TestMethod]
        public void Select_SingleColumn_ValidResultReturned()
        {
            const string ColumnToAdd = "test_column";
            var builder = new SqlBuilder();
            builder.Select(ColumnToAdd);

            var expectedResult = Format(ColumnToAdd);

            Assert.AreEqual(expectedResult, builder.Build());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Select_ColumnsCollection_ArgumentIsNull_Exception()
        {
            var builder = new SqlBuilder();
            builder.Select((IEnumerable<string>)null);
        }

        [TestMethod]
        public void Select_ColumnsCollectionEmpty_BuilderReturned_NoException()
        {
            var builder = new SqlBuilder();
            var returnedBuilder = builder.Select(Enumerable.Empty<string>());

            Assert.AreEqual(builder, returnedBuilder);
        }

        [TestMethod]
        public void Select_ColumnsCollection_ValidResultReturned()
        {
            var columnsToAdd = new[] { "test_column", "test_column_1" };
            var builder = new SqlBuilder();
            builder.Select(columnsToAdd);

            var expectedResult = Format(string.Join(",\r\n", columnsToAdd));

            Assert.AreEqual(expectedResult, builder.Build());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void From_ArgumentIsNull_Exception()
        {
            var builder = new SqlBuilder();
            builder.From(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void From_ArgumentIsEmpty_Exception()
        {
            var builder = new SqlBuilder();
            builder.From(string.Empty);
        }

        [TestMethod]
        public void From_ValidResultReturned()
        {
            const string From = "table";
            var builder = new SqlBuilder();
            builder.From(From);

            var expectedResult = Format(from: From);

            Assert.AreEqual(expectedResult, builder.Build());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Join_Single_ArgumentIsNull_Exception()
        {
            var builder = new SqlBuilder();
            builder.Join((string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Join_Single_ArgumentIsEmpty_Exception()
        {
            var builder = new SqlBuilder();
            builder.Join(string.Empty);
        }

        [TestMethod]
        public void Join_Single_ValidResultReturned()
        {
            const string JoinExpr = "join_expression";
            var builder = new SqlBuilder();
            builder.Join(JoinExpr);

            var expectedResult = Format(join: JoinExpr);

            Assert.AreEqual(expectedResult, builder.Build());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Join_CollectionIsNull_Exception()
        {
            var builder = new SqlBuilder();
            builder.Join((IEnumerable<string>)null);
        }

        [TestMethod]
        public void Join_CollectionEmpty_BuilderReturned_NoException()
        {
            var builder = new SqlBuilder();
            var returnedBuilder = builder.Join(Enumerable.Empty<string>());

            Assert.AreEqual(builder, returnedBuilder);
        }

        [TestMethod]
        public void Join_Multiple_ValidResultReturned()
        {
            var columnsToAdd = new[] { "join_expr_1", "join_expr_2" };
            var builder = new SqlBuilder();
            builder.Join(columnsToAdd);

            var expectedResult = Format(join: string.Join("\r\n", columnsToAdd).TrimEnd());

            Assert.AreEqual(expectedResult, builder.Build());
        }

        [TestMethod]
        public void Where_Single_ValidResultReturned()
        {
            const string WhereExpr = "where_expression";
            var builder = new SqlBuilder();
            builder.Where(WhereExpr);

            var expectedResult = Format(where: $"WHERE {WhereExpr}");

            Assert.AreEqual(expectedResult, builder.Build());
        }

        [TestMethod]
        public void Build_MultipleCalls_PureMethod()
        {
            var columnsToAdd = new[] { "join_expr_1", "join_expr_2" };
            var builder = new SqlBuilder();
            builder.Join(columnsToAdd);

            var firstCall = builder.Build();
            var secondCall = builder.Build();

            Assert.AreEqual(firstCall, secondCall);
            ReferenceEquals(firstCall, secondCall);
        }

        private string Format(
            string select = null,
            string from = null,
            string join = null,
            string where = null,
            string group = null)
            => string.Format(SqlTemplate, select, from, join, where, group);
    }
}
