using BugFixer.DataLayer.Context;
using BugFixer.Domain.Entities.Location;
using BugFixer.Domain.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.DataLayer.Repositories
{
    public class StateRepository : IStateRepository
    {
        #region Ctor
        private readonly BugFixerDbContext _context;
        public StateRepository(BugFixerDbContext context)
        {
            _context = context;
        }

        #endregion

        #region  state Country
        public async Task<List<State>> GetStates(long? id = null)
        {
            var state = _context.States.Where(a => !a.IsDelete).AsQueryable();

            if (id.HasValue)
            {
                state = state.Where(a => a.ParentId.HasValue && a.ParentId.Value == id.Value);
            }
            else
            {
                state = state.Where(s => s.ParentId == null);
            }
            return await state.ToListAsync();
        }
        #endregion
    }
}
