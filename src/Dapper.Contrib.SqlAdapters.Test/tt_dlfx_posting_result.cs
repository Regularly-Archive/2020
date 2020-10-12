using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Contrib.SqlAdapters.Test
{
    [Table("\"tt_dlfx_posting_result\"")]
    public class tt_dlfx_posting_result
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        [ExplicitKey]
        public string UNIQUE_ID { get; set; }

        /// <summary>
        /// 运输需求号
        /// </summary>
        public string DRNUM { get; set; }

        /// <summary>
        /// 发车基地
        /// </summary>
        public string LOCATION { get; set; }

        /// <summary>
        /// 过账结果
        /// </summary>
        public string INDICATOR { get; set; }

        /// <summary>
        /// 传输日期
        /// </summary>
        public DateTime TRANSFER_DATE { get; set; }
    }
}
