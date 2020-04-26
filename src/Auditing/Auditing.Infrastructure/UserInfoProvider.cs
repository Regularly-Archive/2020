using System;
using System.Collections.Generic;
using System.Text;

namespace Auditing.Infrastructure
{
    public class UserInfoProvider : IUserInfoProvider
    {
        public string GetUserId()
        {
            return Environment.UserName;
        }
    }
}
