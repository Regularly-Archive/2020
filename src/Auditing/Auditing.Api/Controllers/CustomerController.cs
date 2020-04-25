using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auditing.Domain;
using Auditing.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Auditing.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Consumes("application/json")]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly CustomerContext _context;

        public CustomerController(ILogger<CustomerController> logger,CustomerContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Customer> Get()
        {
            return _context.Customer.ToList();
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> Post(Customer customer)
        {
            _context.Customer.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        [HttpPut]
        public async Task<Customer> Put(Customer customer)
        {
            var entity = _context.Customer.Where(x => x.Id == customer.Id).FirstOrDefault();
            if (entity == null)
                throw new Exception($"客户{customer.Id}不存在");
            entity.Name = customer.Name;
            entity.Email = customer.Email;
            entity.Address = customer.Address;
            entity.Tel = customer.Tel;
            _context.Customer.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        [HttpDelete]
        public async Task Delete(int id)
        {
            var entity = _context.Customer.Where(x => x.Id == id).FirstOrDefault();
            if (entity == null)
                throw new Exception($"客户{id}不存在");
            _context.Customer.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
