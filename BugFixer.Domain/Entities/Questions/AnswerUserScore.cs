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
    public class AnswerUserScore:BaseEntity
    {
        #region Property
        public AnswerScoreType Type { get; set; }
        public long UserId { get; set; }
        public long AnswerID { get; set; }
        #endregion
        #region Property
        public User User { get; set; }
        public Answer Answer { get; set; }
        #endregion
    }
}
