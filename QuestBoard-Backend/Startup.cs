using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuestBoard.Context;
using QuestBoard.Hubs;
using QuestBoard.Models;
using System.Net;

namespace QuestBoard
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
            services.AddControllers();
            services.AddSignalR();

            services.AddDbContext<QuestboardContext>(options =>
            {
                options.UseNpgsql(Configuration.GetSection("DatabaseConfig")["PostgresSQL"]);
            });

            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = "localhost:6379";
                //options.Configuration = Configuration.GetConnectionString("Redis");
            });

            services.AddIdentity<User, IdentityRole>(opetions =>
            {
                opetions.Password = new PasswordOptions
                {
                    RequiredLength = 8,
                    RequireNonAlphanumeric = false,
                    RequiredUniqueChars = 1,
                    RequireDigit = false,
                    RequireLowercase = false,
                    RequireUppercase = false,
                };
            }).AddEntityFrameworkStores<QuestboardContext>().AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.SlidingExpiration = true;

                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.None;
            });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(IPAddress.Parse("127.0.0.1"));
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().Build();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, QuestboardContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } else
            {
                app.UseHsts();

                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });
            }

            app.UseHttpsRedirection();

            if (context != null)
            {
                context.Database.Migrate();
            }

            app.UseRouting();
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<KanbanHub>("/ws/board");
            });
        }
    }
}
