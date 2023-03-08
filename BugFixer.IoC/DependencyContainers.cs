using BugFixer.Application.Services.Implimentations;
using BugFixer.Application.Services.Interfaces;
using BugFixer.DataLayer.Repositories;
using BugFixer.Domain.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace BugFixer.IoC
{
    public class DependencyContainers
    {
        public static void RegisteDependency(IServiceCollection services)
        {
            #region Services
            services.AddScoped<IUserServices, UserServices>();
            services.AddScoped<IStateServices, StateServices>();
            services.AddScoped<IQuestionServices, QuestionServices>();
            services.AddScoped<IEmailServices, EmailServices>();
            #endregion
            #region Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IStateRepository, StateRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<ISettingSiteReporistory, SettingSiteReporistory>();
            #endregion
        }
    }
}
