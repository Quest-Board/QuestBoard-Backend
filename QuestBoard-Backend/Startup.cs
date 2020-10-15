using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuestBoard.Context;
using QuestBoard.Hubs;
using QuestBoard.Models;

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

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
