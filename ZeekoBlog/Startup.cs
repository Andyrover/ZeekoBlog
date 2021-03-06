using System;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.WebEncoders;
using ZeekoBlog.Core.Models;
using ZeekoBlog.Application.Services;
using ZeekoBlog.Fun;

namespace ZeekoBlog
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<WebEncoderOptions>(
                options =>
                    options.TextEncoderSettings = new TextEncoderSettings(
                        UnicodeRanges.BasicLatin,
                        UnicodeRanges.CjkUnifiedIdeographs));
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddDbContextPool<BlogContext>(
                options =>
                {
                    var dbUser = Environment.GetEnvironmentVariable("BLOG_DB_USER");
                    var dbPwd = Environment.GetEnvironmentVariable("BLOG_DB_PWD");
                    var dbAddr = Environment.GetEnvironmentVariable("BLOG_DB_ADDR");
                    var dbPort = Environment.GetEnvironmentVariable("BLOG_DB_PORT");
                    var connectionString = Configuration.GetConnectionString("pgsql")
                        .Replace("{BLOG_DB_ADDR}", dbAddr)
                        .Replace("{BLOG_DB_PORT}", dbPort)
                        .Replace("{BLOG_DB_USER}", dbUser)
                        .Replace("{BLOG_DB_PWD}", dbPwd);

                    options.UseNpgsql(connectionString, b => b.MigrationsAssembly("ZeekoBlog"));
                });
            services.AddEasyCaching(
                options =>
                {
                    options.UseInMemory();
                });
            var aiKey = Environment.GetEnvironmentVariable("AI_KEY");
            services.AddApplicationInsightsTelemetry(aiKey);
            services.AddHttpClient(
                    "text-maid",
                    client =>
                    {
                        client.BaseAddress = new Uri(Configuration.GetSection("Api")["TextMaid"]);
                    })
                .AddTypedClient<TextMaid.TextMaidClient>();
            services.AddScoped<IArticleRenderer, ArticleRenderer>();
            services.AddScoped<ArticleService>();
            services.AddScoped<AccountService>();
            services.AddMemoryCache();
            services.AddZeekoBlogFun();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStatusCodePagesWithReExecute("/oops/{0}");
            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers();
                });
            app.UseZeekoBlogFun();
        }
    }
}
