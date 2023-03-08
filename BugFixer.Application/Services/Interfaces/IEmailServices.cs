﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.Application.Services.Interfaces
{
    public interface IEmailServices
    {
        Task<bool> SendEmail(string to, string subject, string body);
    }
}
