using BugFixer.Domain.Entities.Questions;
using BugFixer.Domain.Entities.Tags;
using BugFixer.Domain.ViewModels.Question;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.Application.Services.Interfaces
{
    public interface IQuestionServices
    {
        Task<List<Tag>> GetTags();
        Task<CreateQuestionResul> CheckTagValidation(List<string>? tags, long userID);

        Task<bool> CreateQuestion(CreateQuestionViewModel viewModel);
        Task<FilterTagViewModel> FilterTags(FilterTagViewModel filter);

        Task<Question?> GetQuestionByID(long Id);


        #region Questions
        public Task<FilterQuestionViewModel> FilterQuestion(FilterQuestionViewModel filter);
        #endregion

        Task<List<string>> GetTaglistByQuestionId(long QuestionId);


    }
}
