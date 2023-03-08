using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BugFixer.Domain.ViewModels.Question
{
    public class Paging<T>
    {
        public Paging()
        {
            CurrentPage = 1;
            HowManyShowAfterBefore = 3;
            TakeEntity = 10;
            Entites = new List<T>();
        }
        public int CurrentPage { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }
        public int HowManyShowAfterBefore { get; set; }
        public int TakeEntity { get; set; }
        public int SkipEntity { get; set; }
        public int AllEntityCount { get; set; }
        public int TotalPage { get; set; }
        public List<T> Entites { get; set; }
        public async Task SetPaging(IQueryable<T> values)
        {
            AllEntityCount = values.Count();
            TotalPage = (int)Math.Ceiling(AllEntityCount / (double)TakeEntity);
            CurrentPage = CurrentPage < 1 ? 1 : CurrentPage;
            CurrentPage = CurrentPage > TotalPage ? 1 : CurrentPage;
            SkipEntity = (CurrentPage - 1) * TakeEntity;
            StartPage = CurrentPage - HowManyShowAfterBefore > 0
                ? CurrentPage - HowManyShowAfterBefore
                : 1;
            EndPage = CurrentPage + HowManyShowAfterBefore > TotalPage
                ? TotalPage
                : CurrentPage + HowManyShowAfterBefore;
            Entites = await values.Skip(SkipEntity).Take(TakeEntity).ToListAsync();
        }
        public PagingViewModel GetPaging()
        {
            var result = new PagingViewModel
            {
                CurrentPage = this.CurrentPage,
                EndPage = this.EndPage,
                StartPage = this.StartPage,
            };
            return result;
        }
    }
    public class PagingViewModel
    {
        public int CurrentPage { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }

    }
}
