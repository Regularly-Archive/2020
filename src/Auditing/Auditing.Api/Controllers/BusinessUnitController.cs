using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auditing.Domain;
using Auditing.Infrastructure;
using Auditing.Infrastructure.Interceptors;
using Auditing.Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Auditing.Api.Controllers
{
    [Route("[controller]")]
    [Consumes("application/json")]
    [ApiController]
    public class BusinessUnitController : Controller
    {
        private readonly ILogger<BusinessUnitController> _logger;
        private readonly IRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private IRepository _repositoryProxy;
        public BusinessUnitController(ILogger<BusinessUnitController> logger, IRepository repository, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _repository = repository;
            _unitOfWork = unitOfWork;
            //_repositoryProxy = (new Castle.DynamicProxy.ProxyGenerator().CreateInterfaceProxyWithTarget(typeof(IRepository), _repository, new AuditLogInterceptor())) as IRepository;
        }

        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<BusinessUnit> Get()
        {
            return _repository.GetAll<BusinessUnit>();
        }

        [HttpPost]
        public BusinessUnit Post(BusinessUnit item)
        {
            if (!item.CreatedAt.HasValue)
                item.CreatedAt = DateTime.Now;
            //_repository.Insert(item);
            _unitOfWork.Commit();
            return item;
        }
    }
}
