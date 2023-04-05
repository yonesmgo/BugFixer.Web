using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.Domain.Entities.Common
{
    public class ScoreManagementViewModel
    {
        public int MinRequestCountCerifyTag { get; set; }
        public int AddNewAnswerScore { get; set; }
        public int AddNewQuestionScore { get; set; }
        public int MinsScoreForBronzeMedal { get; set; }
        public int MinsScoreForSilverMedal { get; set; }
        public int MinsScoreForGoldMedal { get; set; }
        public int MinScoreForDownScore { get; set; }
        public int MinScoreForUpScore { get; set; }
    }
}
