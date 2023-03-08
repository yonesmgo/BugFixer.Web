﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BugFixer.Domain.Entities.Account;
using BugFixer.Domain.Entities.Common;

namespace BugFixer.Domain.Entities.Tags
{
    public class RequestTag : BaseEntity
    {
        #region Properties

        [Display(Name = "عنوان")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Title { get; set; }

        public long UserId { get; set; }

        #endregion

        #region Relations

        public User User { get; set; }

        #endregion
    }
}
