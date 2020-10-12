using Dapper.Contrib.Extensions;
using NUnit.Framework;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Dapper.Contrib.SqlAdapters.Test
{
    public class OracleTest
    {
        /// <summary>
        /// IDbConnection
        /// </summary>
        private IDbConnection connection;

        /// <summary>
        /// Setup
        /// </summary>
        [SetUp]
        public void Setup()
        {
            connection = new OracleConnection(ConnectionStrings.Default);
            connection.UseSqlAdapter(new OracleSqlAdapter());
        }

        [Test]
        public void Test_Insert()
        {
            var uniqueId = Guid.NewGuid().ToString("N");
            var postingResult = new tt_dlfx_posting_result();
            postingResult.UNIQUE_ID = uniqueId;
            postingResult.DRNUM = $"DR{DateTime.Now.ToString("yyyyMMddHHmmss")}";
            postingResult.LOCATION = "ÄþÏÄÖÐÎÀÊÐÉ³ÆÂÍ·Çø";
            postingResult.INDICATOR = "Y";
            postingResult.TRANSFER_DATE = DateTime.Now;

            connection.Insert(postingResult);
            connection.DeleteAll<tt_dlfx_posting_result>();
            var list = connection.Get<tt_dlfx_posting_result>(uniqueId);
            Assert.IsNotNull(list);

        }

        [Test]
        public void Test_InsertBatch()
        {
            var list = MockMany().ToList();
            connection.Insert(list);
        }

        public void Test_Get()
        {

        }

        public void Test_GetALl()
        {

        }

        public void Test_Delete()
        {

        }

        public void Test_Update()
        {

        }

        private IEnumerable<tt_dlfx_posting_result> MockMany(int limit = 500)
        {
            for (var index = 0; index < limit; index++)
            {
                var uniqueId = Guid.NewGuid().ToString("N");
                var postingResult = new tt_dlfx_posting_result();
                postingResult.UNIQUE_ID = uniqueId;
                postingResult.DRNUM = $"DR{DateTime.Now.ToString("yyyyMMddHHmmss")}";
                postingResult.LOCATION = "ÄþÏÄÖÐÎÀÊÐÉ³ÆÂÍ·Çø";
                postingResult.INDICATOR = "Y";
                postingResult.TRANSFER_DATE = DateTime.Now;
                yield return postingResult;
            }
        }
    }
}