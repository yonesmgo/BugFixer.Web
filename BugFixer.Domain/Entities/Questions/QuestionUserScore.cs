using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.Entities.Common;
using BugFixer.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.Domain.Entities.Questions
{
    public class QuestionUserScore : BaseEntity
    {
        #region Property
        public QuestionScoreType Type { get; set; }
        public long UserId { get; set; }
        public long QuestionId { get; set; }
        #endregion
        #region Property
        public User User { get; set; }
        public Question Question { get; set; }
        #endregion
    }
}
