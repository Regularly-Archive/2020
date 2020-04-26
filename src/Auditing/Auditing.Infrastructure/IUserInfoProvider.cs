using System;
using System.Collections.Generic;
using System.Text;

namespace Auditing.Infrastructure
{
    public interface IUserInfoProvider
    {
        string GetUserId();
    }
}
