using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.Entities.Common;
using BugFixer.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.Application.Services.Implimentations
{
    public class StateServices : IStateServices
    {
        private IStateRepository _stateRepository;
        public StateServices(IStateRepository stateRepository)
        {
            _stateRepository = stateRepository;
        }



        public async Task<List<SelectListViewModel>> GetAllState(long? StateID = null)
        {
            var state = await _stateRepository.GetStates(StateID);
            return state.Select(s => new SelectListViewModel
            {
                Id = s.Id,
                Title = s.Title,
            }).ToList();
        }
    }
}
