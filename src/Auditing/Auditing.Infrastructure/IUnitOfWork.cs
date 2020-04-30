using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Auditing.Infrastructure
{
    public interface IUnitOfWork
    {
        IDbTransaction Transaction { get;  }

        void Commit();
    }
}
