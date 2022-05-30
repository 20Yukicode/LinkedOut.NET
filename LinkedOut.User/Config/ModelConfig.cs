﻿using LinkedOut.Common.Config;
using LinkedOut.Common.Domain;
using LinkedOut.DB;
using LinkedOut.User.Manager;
using LinkedOut.User.Service;
using LinkedOut.User.Service.Impl;

namespace LinkedOut.User.Config;

public static class ModelConfig
{
    public static IServiceCollection AddModuleService(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddLinkedOutSwagger(new SwaggerProperties
        {
            Title = "用户子系统",
            Description = "user",
            Version = "1.0",
            ContactEmail = "1094554173@qq.com",
            ContactName = "nzh",
            ContactUrl = new Uri("https://www.baidu.com")
        });
        services.AddScoped<LinkedOutContext>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IEduService, EduService>();
        services.AddScoped<IEnterpriseService, EnterpriseService>();
        services.AddScoped<IJobService, JobService>();
        
        services.AddScoped<UserInfoManager>();
        services.AddScoped<SubscribedManager>();
        services.AddScoped<UserManager>();
        services.AddScoped<EnterpriseInfoManager>();

        return services;
    }
}