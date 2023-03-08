using BugFixer.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.Application.Services.Interfaces
{
    public interface IStateServices
    {
        Task<List<SelectListViewModel>> GetAllState(long? StateID = null);           
    }
}
