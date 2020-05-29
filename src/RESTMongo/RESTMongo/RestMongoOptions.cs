using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTMongo
{
    public class RestMongoOptions
    {
        /// <summary>
        /// 路由前缀
        /// </summary>
        public string RoutePrefix { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 是否允许创建数据库
        /// </summary>
        public bool AutoCreateDatabase { get; set; }

        /// <summary>
        /// 是否允许创建集合
        /// </summary>
        public bool AutoCreateCollection { get; set; }

        /// <summary>
        /// 是否为封闭集合
        /// </summary>
        public bool IsCappedCollection { get; set; }

        /// <summary>
        /// 集合大小
        /// </summary>
        public long? CappedCollectionSize { get; set; }

        /// <summary>
        /// 文档数目
        /// </summary>
        public long? CappedCollectionMaxItems { get; set; }
    }
}
