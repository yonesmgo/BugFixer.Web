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

        Task<Question?> GetQuestionByID(long Id);

        Task<List<string>> GetTaglistByQuestionId(long QuestionId);
    }
}
