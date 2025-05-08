using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoManagment.Core.Domain.Events;
using TodoManagment.Core.Handlers;
using TodoManagment.Core.ServiceContract;
using TodoManagment.Core.Services;
using TodoManagment.Core.Validators.TodoValidator;

namespace TodoManagment.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<TodoAddRequestValidator>();
            services.AddScoped<ITodoService, TodoService>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

            services.AddTransient<INotificationHandler<TodoCompletedEvent>, TodoCompletedEventHandler>();
            services.AddTransient<INotificationHandler<TodoCompletedEvent>, TodoCompletedNotificationHandler>();

            services.AddScoped<INotificationService, NotificationService>(); 

            return services;
        }
    }
}
