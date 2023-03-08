using BugFixer.Domain.Entities.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.Domain.Interface
{
    public interface IStateRepository
    {
        Task<List<State>> GetStates(long? id = null);
    }
}
