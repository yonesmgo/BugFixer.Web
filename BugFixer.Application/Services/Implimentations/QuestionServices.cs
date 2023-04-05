using BugFixer.Application.Extentions;
using BugFixer.Application.Securities;
using BugFixer.Application.Services.Interfaces;
using BugFixer.Application.Statics;
using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.Entities.Common;
using BugFixer.Domain.Entities.Questions;
using BugFixer.Domain.Entities.Tags;
using BugFixer.Domain.Enums;
using BugFixer.Domain.Interface;
using BugFixer.Domain.ViewModels.Question;
using BugFixer.Domain.ViewModels.UserPanel.Question;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json.Serialization;
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
        public async Task<IQueryable<Question>> GetQuestions()
        {
            //var minCount = _score.MinRequestCountCerifyTag;
            var res = await _question.GetAllQuestion();
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
        public async Task<bool> AnswerQuestion(AnswerQuestionViewModel model)
        {
            //var question = await GetQuestionByID(model.QuestionID);
            //if (question == null)
            //{
            //    return false;
            //}
            var answer = new Answer()
            {
                Content = model.Answer.SanitizeText(),
                UserId = model.UserID,
                QuestionId = model.QuestionID
            };
            await _question.AddAnswer(answer);
            await _question.Savechanges();
            await _userServices.UpdateUSerScoreAndMedal(model.UserID, _score.AddNewAnswerScore);
            return true;
        }
        public async Task<List<Answer>> GetallQuestionAnswer(long questionID)
        {
            var s = await _question.GetListOfAnswer(questionID);
            return s;
        }
        public async Task addViewForQuestion(string userIp, Question question)
        {
            bool res = await _question.IsExitViwByUserName(userIp, question.Id);

            if (res)
            {
                question.ViewCount += 1;
                await _question.UpdateQuestion(question);
                await _question.Savechanges();
                return;
            }
            else
            {
                var view = new QuestionView()
                {
                    QuestionId = question.Id,
                    UserIP = userIp,
                };
                await _question.AddQuestionView(view);
                question.ViewCount += 1;
                await _question.UpdateQuestion(question);
                await _question.Savechanges();
            }

        }
        public async Task<bool> HasUserAccessToSelectTrueAnswer(long userIdm, long AnswerId)
        {
            var answewer = await _question.GetAnswerByID(AnswerId);
            if (answewer is null)
            {
                return false;
            }
            var user = await _userServices.GetUserByID(userIdm);
            if (user is null)
            {
                return false;
            }
            if (user.IsAdmin)
            {
                return true;
            }
            if (answewer.Question.UserId != userIdm)
            {
                return false;
            }
            return true;
        }
        public async Task SelectTrueAnswer(long AnswerId)
        {
            var answewer = await _question.GetAnswerByID(AnswerId);
            if (answewer is null)
            {
                return;
            }
            answewer.IsTrue = !answewer.IsTrue;
            await _question.UpdateAnswer(answewer);
            await _question.Savechanges();

        }
        #endregion
        public async Task<CreateScoreForAnswerResult> CreateScoreForAnswer(long answerId, AnswerScoreType type, long userId)
        {
            var answewer = await _question.GetAnswerByID(answerId);
            if (answewer is null)
            {
                return CreateScoreForAnswerResult.error;
            }
            var user = await _userServices.GetUserByID(userId);
            if (user is null) return CreateScoreForAnswerResult.error;
            if (type == AnswerScoreType.Mines && user.Score < _score.MinScoreForDownScore)
            {
                return CreateScoreForAnswerResult.notEnoghScoreForDown;
            }
            if (type == AnswerScoreType.plus && user.Score < _score.MinScoreForUpScore)
            {
                return CreateScoreForAnswerResult.notEnoghScoreForUp;
            }
            bool res = await _question.IsExistUserScoreForAnswer(answerId, userId);
            if (res)
            {
                return CreateScoreForAnswerResult.UserCreateScoreBefore;
            }
            var score = new AnswerUserScore
            {
                AnswerID = answerId,
                UserId = userId,
                Type = type
            };
            await _question.AnswerUserScore(score);
            if (type == AnswerScoreType.Mines)
            {
                answewer.Score -= 1;
            }
            else
            {
                answewer.Score += 1;
            }
            await _question.UpdateAnswer(answewer);
            await _question.Savechanges();
            return CreateScoreForAnswerResult.success;
        }
        public async Task<CreateScoreForAnswerResult> CreateScoreForQuestion(long questionId, QuestionScoreType type, long userId)
        {

            var Question = await _question.GetQuestionByID(questionId);
            if (Question is null)
            {
                return CreateScoreForAnswerResult.error;
            }
            var user = await _userServices.GetUserByID(userId);
            if (user is null) return CreateScoreForAnswerResult.error;
            if (type == QuestionScoreType.Mines && user.Score < _score.MinScoreForDownScore)
            {
                return CreateScoreForAnswerResult.notEnoghScoreForDown;
            }
            if (type == QuestionScoreType.plus && user.Score < _score.MinScoreForUpScore)
            {
                return CreateScoreForAnswerResult.notEnoghScoreForUp;
            }
            bool res = await _question.IsExistUserScoreForQuestion(questionId, userId);
            if (res)
            {
                return CreateScoreForAnswerResult.UserCreateScoreBefore;
            }
            var score = new QuestionUserScore
            {
                QuestionId = questionId,
                UserId = userId,
                Type = type
            };
            await _question.QuestionUserScore(score);
            if (type == QuestionScoreType.Mines)
            {
                Question.Score -= 1;
            }
            else
            {
                Question.Score += 1;
            }
            await _question.UpdateQuestion(Question);
            await _question.Savechanges();
            return CreateScoreForAnswerResult.success;
        }
        #region Add Question To Bookmark
        public async Task<bool> AddQuestionToBookmark(long questionId, long userId)
        {
            var question = await GetQuestionByID(questionId);
            if (question == null)
            {
                return false;
            }
            if (await _question.isExistInUserBookmarks(questionId, userId))
            {
                var bookmark = await _question.GetbookmarkByQuestionUserId(questionId, userId);
                if (bookmark == null)
                {
                    return false;
                }
                _question.RemoveBookmark(bookmark);
            }
            else
            {
                var newBookmark = new UserQuestionBookmark { QuestionId = questionId, UserId = userId };
                await _question.AddBookmark(newBookmark);
            }
            await _question.Savechanges();
            return true;
        }
        public async Task<bool> isExistInUserBookmarks(long Question, long UserId)
        {
            return await _question.isExistInUserBookmarks(Question, UserId);
        }
        #endregion
        #region Edit Question
        public async Task<EditQuestionViewModel> FillEditQuestionViewModel(long Id, long userId)
        {
            var question = await GetQuestionByID(Id);
            if (question == null) { return null; }
            var user = await _userServices.GetUserByID(userId);
            if (user == null)
            {
                return null;
            }
            if (question.UserId != user.Id && !user.IsAdmin)
            {
                return null;
            }
            var tags = await GetTaglistByQuestionId(question.Id);
            var result = new EditQuestionViewModel
            {
                Id = question.Id,
                Description = question.Content,
                Title = question.Title,
                SelectedTagJson = JsonConvert.SerializeObject(tags)
            };
            return result;
        }
        public async Task<bool> EditQuestion(EditQuestionViewModel edit)
        {
            var question = await _question.GetQuestionByID(edit.Id);
            if (question is null)
            {
                return false;
            }
            var user = await _userServices.GetUserByID(edit.UserID);
            if (question.UserId != edit.UserID && !user.IsAdmin)
            {
                return false;
            }
            FileExtentions.ManageEditorImages(question.Content, edit.Description, PathTools.EditorImageServerPath);
            question.Title = edit.Title;
            question.Content = edit.Description;
            var curentTag = question.SelectQuestionTags.ToList();
            foreach (var item in curentTag)
            {
                await _question.RemoveSelectTag(item);
            }
            await _question.Savechanges();
            if (edit.Tags is not null && edit.Tags.Any())
            {
                foreach (var tag in edit.Tags)
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
            return true;

        }
        public async Task<EditAnswerViewModel?> FillEditANSWERViewModel(long AnswerId, long userId)
        {
            var answer = await _question.GetAnswerByID(AnswerId);
            if (answer == null) return null;
            var user = await _userServices.GetUserByID(userId);
            if (user == null) return null;
            if (answer.UserId != user.Id && !user.IsAdmin)
            {
                return null;
            }

            return new EditAnswerViewModel
            {
                Answer = answer.Content,
                AnswerID = AnswerId,
                QuestionId = answer.QuestionId
            };
        }
        public async Task<bool> EditAnswer(EditAnswerViewModel edit)
        {
            var answer = await _question.GetAnswerByID(edit.AnswerID);
            if (answer == null) return false;
            if (answer.QuestionId != edit.QuestionId) return false;
            var user = await _userServices.GetUserByID(edit.UserID);
            if (user == null) return false;
            if (answer.UserId != user.Id && !user.IsAdmin)
            {
                return false;
            }
            answer.Content = edit.Answer;
            await _question.UpdateAnswer(answer);
            await _question.Savechanges();
            return true;
        }


        #endregion

        #region User Panel Question Bookmark
        public async Task<FilterQuestionBooknarksViewModel> FilterQuestionBooknarks(FilterQuestionBooknarksViewModel filter)
        {
            var query = _question.GetAllBokkmarks();
            query = query.Where(s => s.UserId.Equals(filter.UserId));
            await filter.SetPaging(query.Select(s => s.Question).AsQueryable());
            return filter;
        }
        public async Task<FilterQuestionBooknarksViewModel> FilterQuestionByUser(FilterQuestionBooknarksViewModel filter)
        {
            var query = _question.GetAllBokkmarks();
            query = query.Where(s => s.UserId.Equals(filter.UserId));
            await filter.SetPaging(query.Select(s => s.Question).AsQueryable());
            return filter;
        }

        public async Task<FilterQuestionBooknarksViewModel> GetAllBokkmarksByUser(FilterQuestionBooknarksViewModel filter)
        {
            var query = _question.GetQuestionByUser(filter.UserId);
            query = query.Where(s => s.UserId.Equals(filter.UserId));
            await filter.SetPaging(query.AsQueryable());
            return filter;
        }
        #endregion
    }
}
