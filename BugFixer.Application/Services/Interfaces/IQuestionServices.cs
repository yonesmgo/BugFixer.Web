using BugFixer.Domain.Entities.Questions;
using BugFixer.Domain.Entities.Tags;
using BugFixer.Domain.Enums;
using BugFixer.Domain.ViewModels.Question;
using BugFixer.Domain.ViewModels.UserPanel.Question;
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
        Task<IQueryable<Question>> GetQuestions();
        Task<CreateQuestionResul> CheckTagValidation(List<string>? tags, long userID);

        Task<bool> CreateQuestion(CreateQuestionViewModel viewModel);
        Task<FilterTagViewModel> FilterTags(FilterTagViewModel filter);

        Task<Question?> GetQuestionByID(long Id);
        #region Questions
        public Task<FilterQuestionViewModel> FilterQuestion(FilterQuestionViewModel filter);
        #endregion

        Task<List<string>> GetTaglistByQuestionId(long QuestionId);
        Task<bool> AnswerQuestion(AnswerQuestionViewModel model);
        Task<List<Answer>> GetallQuestionAnswer(long questionID);
        Task addViewForQuestion(string userIp, Question question);

        Task<bool> HasUserAccessToSelectTrueAnswer(long userIdm, long AnswerId);
        Task SelectTrueAnswer(long AnswerId);
        Task<CreateScoreForAnswerResult> CreateScoreForAnswer(long answerId, AnswerScoreType type, long userId);
        Task<CreateScoreForAnswerResult> CreateScoreForQuestion(long questionId, QuestionScoreType type, long userId);
        Task<bool> AddQuestionToBookmark(long questionId, long userId);
        Task<bool> isExistInUserBookmarks(long Question, long UserId);
        Task<EditQuestionViewModel> FillEditQuestionViewModel(long Id,long userId);
        Task<bool> EditQuestion(EditQuestionViewModel edit);

        Task<EditAnswerViewModel?> FillEditANSWERViewModel(long AnswerId, long userId);
        Task<bool> EditAnswer(EditAnswerViewModel edit);
        Task<FilterQuestionBooknarksViewModel> FilterQuestionBooknarks(FilterQuestionBooknarksViewModel filter);
        Task<FilterQuestionBooknarksViewModel> GetAllBokkmarksByUser(FilterQuestionBooknarksViewModel filter);
        
    }
}
