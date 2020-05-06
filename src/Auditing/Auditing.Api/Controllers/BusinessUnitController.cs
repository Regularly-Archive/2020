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
        public BusinessUnitController(ILogger<BusinessUnitController> logger, IRepository repository, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _repository = repository;
            _unitOfWork = unitOfWork;
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
            _repository.Insert(item);
            _unitOfWork.Commit();
            return item;
        }

        [HttpPut]
        public BusinessUnit Put(BusinessUnit item)
        {
            var record = _repository.GetByID<BusinessUnit>(item.Id);
            if (record == null)
                return null;

            record.OrgCode = item.OrgCode;
            record.OrgName = item.OrgName;
            record.IsActive = item.IsActive;
            _repository.Update(record);
            _unitOfWork.Commit();
            return record;
        }
    }
}
