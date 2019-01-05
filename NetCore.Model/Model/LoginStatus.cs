using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Model.Model
{
    public enum LoginStatus
    {
        Fail = 0,
        Success = 1,
        NonActive = 2,
        PasswordExpiring = 3,
        PasswordExpired = 4,
        RetryLimits = 5,
        UpdatePassword = 6
    }
}
