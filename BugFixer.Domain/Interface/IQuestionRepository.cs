using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.Entities.Questions;
using BugFixer.Domain.Entities.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.Domain.Interface
{
    public interface IQuestionRepository
    {
        Task<List<Tag>> GetAllTags();
        Task<IQueryable<Tag>> GetAllTagsAsIQueryable();
        Task<bool> IsExitTagByName(string name);
        Task<bool> CheckUserRequestedForTag(long userId, string tag);
        Task AddRequestTag(RequestTag tag);
        Task Savechanges();
        Task<int> RequestCountForTag(string tag);
        Task AddTag(Tag tag);
        Task UpdateTag(Tag tag);
        Task AddQuestion(Question question);
        Task<Tag> GetTagByName(string tag);
        Task AddSelectQuestionTag(SelectQuestionTag tag);
        Task<IQueryable<Question>> GetAllQuestion();
        IQueryable<UserQuestionBookmark> GetAllBokkmarks();

        Task<Question?> GetQuestionByID(long Id);
        Task<List<string>> GetTaglistByQuestionId(long QuestionId);
        Task AddAnswer(Answer answer);
        Task<List<Answer>> GetListOfAnswer(long questionID);
        Task<bool> IsExitViwByUserName(string userIp, long questionID);
        Task AddQuestionView(QuestionView questionView);
        Task UpdateQuestion(Question question);
        Task<Answer?> GetAnswerByID(long answerId);
        Task UpdateAnswer(Answer answer);
        Task<bool> IsExistUserScoreForAnswer(long answerId, long userId);
        Task AnswerUserScore(AnswerUserScore score);
        Task QuestionUserScore(QuestionUserScore score);
        Task<bool> IsExistUserScoreForQuestion(long QuestionId, long userId);
        Task AddBookmark(UserQuestionBookmark bookmark);
        void RemoveBookmark(UserQuestionBookmark bookmark);
        Task<bool> isExistInUserBookmarks(long Question, long UserId);
        Task<UserQuestionBookmark?> GetbookmarkByQuestionUserId(long Question, long UserId);
        Task RemoveTag(Tag tag);
        Task RemoveSelectTag(SelectQuestionTag tag);
        IQueryable<Question> GetQuestionByUser(long Id);


    }
}
