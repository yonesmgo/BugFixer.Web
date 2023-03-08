using BugFixer.Application.Extentions;
using BugFixer.Application.Securities;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Domain.Entities.Common;
using BugFixer.Domain.Entities.Questions;
using BugFixer.Domain.Entities.Tags;
using BugFixer.Domain.Interface;
using BugFixer.Domain.ViewModels.Question;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.Application.Services.Implimentations
{
    public class QuestionServices : IQuestionServices
    {
        #region Ctor
        private readonly IQuestionRepository _question;
        private readonly ScoreManagementViewModel _score;
        private readonly IUserServices _userServices;
        public QuestionServices(IQuestionRepository question, IUserServices userServices, IOptions<ScoreManagementViewModel> score)
        {
            _question = question;
            _score = score.Value;
            _userServices = userServices;
        }
        #endregion
        #region Property
        public async Task<List<Tag>> GetTags()
        {
            //var minCount = _score.MinRequestCountCerifyTag;
            var res = await _question.GetAllTags();
            return res;
        }
        public async Task<CreateQuestionResul> CheckTagValidation(List<string>? tags, long userID)
        {
            if (tags is not null && tags.Any())
            {
                foreach (var tag in tags)
                {
                    var existTag = await _question.IsExitTagByName(tag.SanitizeText().Trim().ToLower());
                    if (existTag) continue;
                    var isUserRequestedForTag = await _question.CheckUserRequestedForTag(userID, tag.SanitizeText().Trim().ToLower());
                    if (isUserRequestedForTag)
                    {
                        return new CreateQuestionResul
                        {
                            Status = CreateQuestionResultEnum.NotValidTag,
                            Message = $" تگ {tag} برای اعتبار سنجی نیاز به درخواست  {_score.MinRequestCountCerifyTag}  کارمند دارد  "
                        };
                    }
                    var tagrequest = new RequestTag
                    {
                        Title = tag.SanitizeText().Trim().ToLower(),
                        UserId = userID,
                    };
                    await _question.AddRequestTag(tagrequest);
                    await _question.Savechanges();

                    var requestedCount = await _question.RequestCountForTag(tag.SanitizeText().Trim().ToLower());
                    if (requestedCount < _score.MinRequestCountCerifyTag)
                    {
                        return new CreateQuestionResul
                        {
                            Status = CreateQuestionResultEnum.NotValidTag,
                            Message = $"تگ {tag}برای اعتبار سنجی نیاز به {_score.MinRequestCountCerifyTag} دارد "
                        };
                    }
                    var tagNew = new Tag
                    {
                        Title = tag.SanitizeText().Trim().ToLower(),
                    };
                    await _question.AddTag(tagNew);
                    await _question.Savechanges();

                }
                return new CreateQuestionResul
                {
                    Status = CreateQuestionResultEnum.Success,
                    Message = "تگ های ورودی معتبر میباشد "
                };
            }
            return new CreateQuestionResul
            {
                Status = CreateQuestionResultEnum.NotValidTag,
                Message = "تگ های ورود ی نمیتواند خالی باشد "
            };
        }
        public async Task<bool> CreateQuestion(CreateQuestionViewModel viewModel)
        {
            var question = new Question
            {
                Title = viewModel.Title,
                Content = viewModel.Description,
                UserId = viewModel.UserID
            };
            await _question.AddQuestion(question);
            await _question.Savechanges();
            if (viewModel.Tags is not null && viewModel.Tags.Any())
            {
                foreach (var tag in viewModel.Tags)
                {
                    var tagId = await _question.GetTagByName(tag.SanitizeText().Trim().ToLower());
                    if (tagId is null) continue;
                    tagId.UseCount += 1;
                    await _question.UpdateTag(tagId);
                    var selectedTag = new SelectQuestionTag
                    {
                        QuestionId = question.Id,
                        TagId = int.Parse(tagId.Id.ToString())
                    };
                    await _question.AddSelectQuestionTag(selectedTag);
                }
                await _question.Savechanges();
            }
            await _userServices.UpdateUSerScoreAndMedal(viewModel.UserID, _score.AddNewQuestionScore);
            return true;
        }
        public async Task<FilterTagViewModel> FilterTags(FilterTagViewModel filter)
        {

            var query = await _question.GetAllTagsAsIQueryable();
            if (!string.IsNullOrEmpty(filter.Title))
            {
                query = query.Where(s => s.Title.Contains(filter.Title));
            }
            switch (filter.Sort)
            {
                case FilterTagEnum.NewToOld:
                    query = query.OrderByDescending(s => s.CreateDate);
                    break;
                case FilterTagEnum.OldToNew:
                    query = query.OrderBy(s => s.CreateDate);

                    break;
                case FilterTagEnum.UseCountHighToLow:
                    query = query.OrderByDescending(s => s.UseCount);
                    break;
                case FilterTagEnum.UseCountLowToHigh:
                    query = query.OrderBy(s => s.UseCount);
                    break;
            }
            await filter.SetPaging(query);
            return filter;
        }

        #endregion
        #region Questions
        public async Task<FilterQuestionViewModel> FilterQuestion(FilterQuestionViewModel filter)
        {
            var questions = await _question.GetAllQuestion();
            #region Filter By Tag
            if (!string.IsNullOrEmpty(filter.TagTitle))
            {
                questions = questions.Include(s => s.SelectQuestionTags)
                    .ThenInclude(s => s.Tag)
                    .Where(s => s.SelectQuestionTags.Any(a => a.Tag.Title.Equals(filter.TagTitle)));
            }

            #endregion
            if (!string.IsNullOrEmpty(filter.Title))
            {
                questions = questions.Where(s => s.Title.Contains(filter.Title.SanitizeText().Trim()));

            }
            switch (filter.sort)
            {
                case FilterQuestuionSortEnum.NewToOld:
                    questions = questions.OrderByDescending(s => s.CreateDate);
                    break;
                case FilterQuestuionSortEnum.OldToNew:
                    questions = questions.OrderBy(s => s.CreateDate);
                    break;
                case FilterQuestuionSortEnum.ScoreHighToLow:
                    questions = questions.OrderByDescending(s => s.Score);
                    break;
                case FilterQuestuionSortEnum.ScoreLowToHigh:
                    questions = questions.OrderBy(s => s.Score);
                    break;
            }

            var result = questions
                .Include(s => s.Answers)
                .Include(s => s.SelectQuestionTags)
                .ThenInclude(s => s.Tag)
                .Include(s => s.User)
                .Select(s => new QuestionListViewModel()
                {
                    AnswerSCount = s.Answers.Count(a => !a.IsDelete),
                    HasAnyAnswer = s.Answers.Any(a => !a.IsDelete),
                    HasTrueAnswer = s.Answers.Any(a => !a.IsDelete && a.IsTrue),
                    Score = s.Score,
                    Title = s.Title,
                    ViewCount = s.ViewCount,
                    UserDisplayName = s.User.GetUserDisplayname(),
                    TagsList = s.SelectQuestionTags.Where(a => !a.Tag.IsDelete).Select(a => a.Tag.Title).ToList(),
                    AnswerByDisplayName = s.Answers.Any(a => !a.IsDelete) ? s.Answers.OrderByDescending(s => s.CreateDate).First().User.GetUserDisplayname() : null,
                    AnswerByCreatedDate = s.Answers.Any(a => !a.IsDelete) ? s.Answers.OrderByDescending(s => s.CreateDate).First().CreateDate.AsTimeAgo() : null,
                    CreatedDate = s.CreateDate.AsTimeAgo(),
                    Id = s.Id,
                }).AsQueryable();
            await filter.SetPaging(result);
            return filter;
        }

        public async Task<Question?> GetQuestionByID(long Id)
        {
            return await _question.GetQuestionByID(Id);
        }

        public async Task<List<string>> GetTaglistByQuestionId(long QuestionId)
        {
            return await _question.GetTaglistByQuestionId(QuestionId);
        }


        #endregion
    }
}
