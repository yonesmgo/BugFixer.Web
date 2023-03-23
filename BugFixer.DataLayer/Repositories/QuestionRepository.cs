using BugFixer.DataLayer.Context;
using BugFixer.Domain.Entities.Questions;
using BugFixer.Domain.Entities.Tags;
using BugFixer.Domain.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BugFixer.DataLayer.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        #region ctor
        private readonly BugFixerDbContext _context;

        public QuestionRepository(BugFixerDbContext context)
        {
            _context = context;

        }


        #endregion
        #region Property
        public async Task<List<Tag>> GetAllTags()
        {
            return await _context.Tags.Where(s => !s.IsDelete).ToListAsync();
        }

        public async Task<bool> IsExitTagByName(string name)
        {
            return await _context.Tags.AnyAsync(s => s.Title.Equals(name) && !s.IsDelete);
        }
        public async Task<bool> CheckUserRequestedForTag(long userId, string tag)
        {
            return await _context.RequestTags.AnyAsync(s => s.Title.Equals(tag) && s.UserId == userId && !s.IsDelete);
        }

        public async Task AddRequestTag(RequestTag tag)
        {
            await _context.RequestTags.AddAsync(tag);
        }

        public async Task Savechanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<int> RequestCountForTag(string tag)
        {
            return await _context.RequestTags.CountAsync(a => a.Title.Equals(tag) && !a.IsDelete);
        }
        public async Task AddTag(Tag tag)
        {
            await _context.Tags.AddAsync(tag);
        }

        public async Task AddQuestion(Question question)
        {
            await _context.Questions.AddAsync(question);
        }

        public async Task<Tag?> GetTagByName(string tag)
        {
            var t = await _context.Tags.FirstOrDefaultAsync(a => a.Title == tag);
            return t;
        }

        public async Task AddSelectQuestionTag(SelectQuestionTag tag)
        {
            await _context.SelectQuestionTags.AddAsync(tag);
        }

        public async Task<IQueryable<Question>> GetAllQuestion()
        {
            return _context.Questions.Where(a => !a.IsDelete).AsQueryable();
        }

        public async Task UpdateTag(Tag tag)
        {
            _context.Tags.Update(tag);
        }

        public async Task<IQueryable<Tag>> GetAllTagsAsIQueryable()
        {
            return _context.Tags.Where(a => !a.IsDelete).AsQueryable();
        }

        public async Task<Question?> GetQuestionByID(long Id)
        {
            return await _context.Questions.Include(s => s.Answers).Include(s => s.User).FirstOrDefaultAsync(a => !a.IsDelete && a.Id == Id);
        }

        public async Task<List<string>> GetTaglistByQuestionId(long QuestionId)
        {
            var q = await _context.SelectQuestionTags
                 .Include(s => s.Tag)
                 .Where(s => s.QuestionId == QuestionId)
                 .Select(s => s.Tag.Title).ToListAsync();
            return q;

        }
        public async Task AddAnswer(Answer answer)
        {
            await _context.Answers.AddAsync(answer);
        }

        public async Task<List<Answer>> GetListOfAnswer(long questionID)
        {
            var answelist = await _context.Answers.
                Include(s => s.User).
                Where(a => a.QuestionId == questionID && !a.IsDelete).
                OrderByDescending(a => a.CreateDate).ToListAsync();
            return answelist;
        }


        public async Task<bool> IsExitViwByUserName(string userIp, long questionID)
        {
            return await _context.QuestionViews.AnyAsync(s => s.UserIP.Equals(userIp) && s.QuestionId == questionID);
        }

        public async Task AddQuestionView(QuestionView questionView)
        {
            await _context.QuestionViews.AddAsync(questionView);
        }
        #endregion
    }
}
