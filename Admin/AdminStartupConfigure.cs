using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Toolkit;
    using Toolkit.Entitys;
    using Toolkit.Enums;
    using DbFrame.DbContext.SqlServer;
    using UEditor.Core;
    using Swashbuckle.AspNetCore.Swagger;
    using System.IO;
    using Toolkit.LogService;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Hosting;
    using Swashbuckle.AspNetCore.Filters;
    using Microsoft.OpenApi.Models;
    using Logic.Class;
    using Entitys.SysClass;

    public static class AdminStartupConfigure
    {
        private static readonly IEnumerable<string> _VersionList = typeof(ApiVersionsEnum).GetEnumNames().ToList().OrderByDescending(w => w);

        public static void AdminConfigureServices(this IServiceCollection services, IConfiguration Configuration, IWebHostEnvironment env)
        {
            #region 跨域配置
            //配置跨域处理
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("AppAny", builder =>
            //    {
            //        builder.AllowAnyOrigin() //允许任何来源的主机访问
            //        .AllowAnyMethod()
            //        .AllowAnyHeader()
            //        .AllowCredentials();//指定处理cookie
            //    });
            //});
            #endregion

            #region 自定义视图
            //自定义 视图 
            services.Configure<Microsoft.AspNetCore.Mvc.Razor.RazorViewEngineOptions>(item =>
            {
                item.AreaViewLocationFormats.Clear();
                item.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");

                item.AreaViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}.cshtml");
                item.AreaViewLocationFormats.Add("/Areas/{2}/Views/Shared/{0}.cshtml");
                item.AreaViewLocationFormats.Add("/Areas/{2}/Views/Sys/{1}/{0}.cshtml");//系统管理
                item.AreaViewLocationFormats.Add("/Areas/{2}/Views/Base/{1}/{0}.cshtml");//基础信息管理
                item.AreaViewLocationFormats.Add("/Areas/{2}/Views/Operate/{1}/{0}.cshtml");//运营管理
                item.AreaViewLocationFormats.Add("/Areas/{2}/Views/Statistics/{1}/{0}.cshtml");//统计管理
            });
            #endregion

            #region Session
            //session 注册
            services.AddSession(item =>
            {
                item.IdleTimeout = TimeSpan.FromMinutes(60 * 2);
                item.Cookie.HttpOnly = true;
                item.Cookie.IsEssential = true;
                //防止edge浏览器访问session丢失问题
                item.Cookie.SameSite = SameSiteMode.None;
            });
            #endregion

            #region HttpContext
            //httpcontext
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            #endregion

            #region AppConfig
            //配置 AppConfigs
            AppConfig.AdminAppConfig = Configuration.GetSection("AppConfig").Get<AdminAppConfig>();
            #endregion

            #region DbFrame
            //注入链接字符串
            DbContextSqlServer.Register(AppConfig.AdminAppConfig.SqlServerConnStr, (tabs) =>
            {
                //注册Models
                Entitys.Class.EntitySet.Register(tabs);
            });
            //将DbContextSqlServer对象注册Logic层静态对象中
            Logic.Class.AppBase.db = new DbContextSqlServer();
            //如果上面 OFFSET 不兼容 则使用 row number 分页
            //Logic.Class.AppBase.db = new DbContextSqlServer(PagingMode.ROW_NUMBER);
            //将DbContextSqlServer对象注册UI层构造函数中
            //services.AddSingleton(Logic.Class.BaseLogic.db.GetType());
            #endregion

            #region JWT
            services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,//是否验证Issuer
                        ValidateAudience = true,//是否验证Audience
                        ValidateLifetime = true,//是否验证失效时间
                        ValidateIssuerSigningKey = true,//是否验证SecurityKey
                        ValidAudience = AppConfig.AdminAppConfig.JwtKeyName,//Audience
                        ValidIssuer = AppConfig.AdminAppConfig.JwtKeyName,//Issuer，这两项和前面签发jwt的设置一致
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(AppConfig.AdminAppConfig.JwtSecurityKey))//拿到SecurityKey
                    };
                });
            #endregion

            #region Swagger 注册Swagger生成器，定义一个和多个Swagger 文档
            services.AddSwaggerGen(options =>
            {
                foreach (var item in _VersionList) options.SwaggerDoc(item, new OpenApiInfo { Version = item });
                //为 Swagger JSON and UI设置xml文档注释路径
                var xmlPath = Path.Combine(System.AppContext.BaseDirectory, "Admin.xml");
                options.IncludeXmlComments(xmlPath, true);

                #region Jwt token 配置
                //option.OperationFilter<AppService.SwaggerParameterFilter>(); // 给每个接口配置授权码传入参数文本框
                //
                options.OperationFilter<AddResponseHeadersFilter>();
                options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                //很重要！这里配置安全校验，和之前的版本不一样
                options.OperationFilter<SecurityRequirementsOperationFilter>();
                //开启 oauth2 安全描述
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    //Scheme = "basic",
                });

                #endregion

            });
            #endregion

            #region Ueditor
            //Ueditor  编辑器 服务端 注入  configFileRelativePath: "wwwroot/admin/libs/nUeditor/net/config.json", isCacheConfig: false, basePath: "C:/basepath"
            services.AddUEditorService(
                    configFileRelativePath: env.WebRootPath + "/admin/libs/neditor/net/config.json",
                    isCacheConfig: false,
                    basePath: env.WebRootPath + "/admin/libs/neditor/net/"
                );
            #endregion


            #region websocket 中间件 注入
            WebSocketWork.RegisterService(services);
            #endregion
        }

        public static void AdminConfigure(this IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}

            #region App 生命周期监听
            //程序启动
            applicationLifetime.ApplicationStarted.Register(() =>
            {
                Tools.Log.Write("App启动");
                //清除前30天数据
                AppBase.db.DeleteAsync<Sys_AppLog>(w => w.t1.AppLog_CreateTime <= DateTime.Now.AddDays(-30));
            });
            //程序正在结束中
            applicationLifetime.ApplicationStopping.Register(() =>
            {
                Tools.Log.Write("App结束中...");
            });
            //程序已结束
            applicationLifetime.ApplicationStopped.Register(() =>
            {
                Tools.Log.Write("App已结束");
            });
            //applicationLifetime.StopApplication();//停止程序
            #endregion

            app.UseExceptionHandler("/Home/Error");

            #region JWT
            //注意添加这一句，启用jwt验证
            app.UseAuthentication();
            #endregion

            #region Session
            //session 注册
            app.UseSession();
            #endregion

            #region HttpContext
            //将 对象 IHttpContextAccessor 注入 HttpContextHelper 静态对象中
            Toolkit.HttpContextService.HttpContextHelper.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
            #endregion

            #region Swagger
            //启用中间件服务生成Swagger作为JSON终结点
            app.UseSwagger();
            //启用中间件服务对swagger-ui，指定Swagger JSON终结点
            app.UseSwaggerUI(option =>
            {
                foreach (var item in _VersionList) option.SwaggerEndpoint($"/swagger/{item}/swagger.json", item);
                option.RoutePrefix = "swagger";
            });
            #endregion

            #region 使用跨域
            //app.UseCors("ApiAny");
            #endregion

            #region WebSocket

            //WebSocket
            WebSocketWork.Register(app);

            #endregion

        }


    }
}
