using Auditing.Infrastructure.Repository;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Auditing.Test
{
    public class SearchParameterTest
    {
        private List<Foo> _fooList;

        [SetUp]
        public void Setup()
        {
            _fooList = new List<Foo>()
            {
                new Foo(){IntValue = 10, NullableDecimalValue = null, NullableDateTimeValue = DateTime.Now, StringValue = "学而时习之" },
                new Foo() { IntValue = -10, NullableDecimalValue =12.25M, NullableDateTimeValue = DateTime.Now, StringValue = "不亦乐乎" },
                new Foo(){IntValue = 12, NullableDecimalValue = null, NullableDateTimeValue = DateTime.Now, StringValue = "有朋自远方来" },
                new Foo() { IntValue = -10, NullableDecimalValue =12.25M, NullableDateTimeValue = DateTime.Now, StringValue = "智者乐山" }
            };
        }

        [Test]
        public void Null_Or_Empty_Conditions_Should_Return_Ture()
        {
            var searchParameters = new SearchParameters();
            searchParameters.Query = new QueryModel();
            var lambda = LambdaExpressionBuilder.BuildLambda<Foo>(searchParameters.Query);
            var where = lambda.Compile();
            var result = _fooList.Where(where);
            Assert.IsTrue(result.Count() == 4);
        }

        [Test]
        public void Equals_Test()
        {
            var searchParameters = new SearchParameters();
            searchParameters.Query = new QueryModel();
            searchParameters.Query.Add(new Condition() { Field = "IntValue", Op = Operation.Equals, Value = 10 });
            searchParameters.Query.Add(new Condition() { Field = "IntValue", Op = Operation.Equals, Value = "10" });
            var lambda = LambdaExpressionBuilder.BuildLambda<Foo>(searchParameters.Query);
            var where = lambda.Compile();
            var result = _fooList.Where(where);
            Assert.IsTrue(result.Count() == 1);
        }

        [Test]
        public void NotEquals_Test()
        {
            var searchParameters = new SearchParameters();
            searchParameters.Query = new QueryModel();
            searchParameters.Query.Add(new Condition() { Field = "IntValue", Op = Operation.NotEquals, Value = "10" });
            var lambda = LambdaExpressionBuilder.BuildLambda<Foo>(searchParameters.Query);
            var where = lambda.Compile();
            var result = _fooList.Where(where);
            Assert.IsTrue(result.Count() == 3);
        }

        [Test]
        public void Contains_Test()
        {
            var searchParameters = new SearchParameters();
            searchParameters.Query = new QueryModel();
            searchParameters.Query.Add(new Condition() { Field = "StringValue", Op = Operation.Contains, Value = "有朋" });
            var lambda = LambdaExpressionBuilder.BuildLambda<Foo>(searchParameters.Query);
            var where = lambda.Compile();
            var result = _fooList.Where(where);
            Assert.IsTrue(result.Count() == 1);
        }

        [Test]
        public void StdIn_Test()
        {
            var searchParameters = new SearchParameters();
            searchParameters.Query = new QueryModel();
            searchParameters.Query.Add(new Condition() { Field = "IntValue", Op = Operation.StdIn, Value = new List<int> { 10, 12 } });
            var lambda = LambdaExpressionBuilder.BuildLambda<Foo>(searchParameters.Query);
            var where = lambda.Compile();
            var result = _fooList.Where(where);
            Assert.IsTrue(result.Count() == 2);
        }

        [Test]
        public void Complex_Test()
        {
            var searchParameters = new SearchParameters();
            searchParameters.Query = new QueryModel();
            searchParameters.Query.Add(new Condition() { Field = "IntValue", Op = Operation.LessThan, Value = 30 });
            searchParameters.Query.Add(new Condition() { Field = "StringValue", Op = Operation.Contains, Value = "山", OrGroup = "StringValue"});
            searchParameters.Query.Add(new Condition() { Field = "StringValue", Op = Operation.Contains, Value = "有朋", OrGroup = "StringValue" });
            var lambda = LambdaExpressionBuilder.BuildLambda<Foo>(searchParameters.Query);
            var where = lambda.Compile();
            var result = _fooList.Where(where);
            Assert.IsTrue(result.Count() == 2);
        }
    }

    class Foo
    {
        public int IntValue { get; set; }
        public decimal? NullableDecimalValue { get; set; }

        public DateTime? NullableDateTimeValue { get; set; }
        public string StringValue { get; set; }
    }
}