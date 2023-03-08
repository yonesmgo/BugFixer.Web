﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BugFixer.Domain.Entities.Common;
using BugFixer.Domain.Entities.Location;
using BugFixer.Domain.Entities.Questions;
using BugFixer.Domain.Entities.Tags;
using BugFixer.Domain.Enums;

namespace BugFixer.Domain.Entities.Account
{
    public class User : BaseEntity
    {
        #region Properties

        [Display(Name = "نام")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string? FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string? LastName { get; set; }

        [Display(Name = "شماره تماس")]
        [MaxLength(20, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "ایمیل")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نمی باشد .")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Email { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string Password { get; set; }

        [Display(Name = "توضیحات")]
        public string? Description { get; set; }

        [Display(Name = "تاریخ تولد")]
        public DateTime? BirthDate { get; set; }

        public long? CountryId { get; set; }

        public long? CityId { get; set; }

        public bool GetNewsLetter { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsBan { get; set; }

        public string EmailActivationCode { get; set; }

        public string Avatar { get; set; }

        [Display(Name = "امتیاز")]
        public int Score { get; set; } = 0;

        [Display(Name = "مدال")]
        public UserMedal? Medal { get; set; }

        #endregion

        #region Relations

        [InverseProperty("UserCountries")]
        public State? Country { get; set; }

        [InverseProperty("UserCities")]
        public State? City { get; set; }

        public ICollection<Question> Questions { get; set; }

        public ICollection<RequestTag> RequestTags { get; set; }

        public ICollection<Answer> Answers { get; set; }

        public ICollection<UserQuestionBookmark> UserQuestionBookmarks { get; set; }
        public ICollection<QuestionUserScore> questionUserScores { get; set; }

        #endregion
    }
}
