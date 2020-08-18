using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using VotingApp.Hubs;
using Microsoft.AspNetCore.Authentication;
using VotingApp.Data;
using Microsoft.EntityFrameworkCore;
using VotingApp.Services;
using VotingApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using ISession = VotingApp.Services.ICouncilSession;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace VotingApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }
        private readonly ILogger _logger;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //Configure Datasources
            services.AddDbContext<VotingAppDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("VotingApp")));
            services.AddScoped<ICouncilSession, SqlSession>();
            services.AddScoped<IBallot, SqlBallotData>();
            services.AddScoped<Services.Interfaces.IVote, SqlVoteData>();
            services.AddScoped<IMember, SqlMemberData>();
            services.AddScoped<IMemberRegister, SqlMemberRegister>();
            services.AddScoped<ISeatService, SqlMemberSeatService>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = Configuration.GetSection("AzureAdB2C").GetValue<string>("SignUpSignInPolicyId"); ;
            })
            .AddOpenIdConnect(Configuration.GetSection("AzureAdB2C").GetValue<string>("SignUpSignInPolicyId"), options =>
            {
                Configuration.Bind("AzureAdB2C", options);
                options.ResponseType = OpenIdConnectResponseType.IdToken;
                options.TokenValidationParameters.NameClaimType = "name";
            }).AddCookie();
            
            
            //Policies for controlling access to the different areas of the site
            services.AddAuthorization(options =>
            {
                options.AddPolicy("MemberOnly", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new MemberRequirement());
                });
            });

            services.AddAuthorization(options =>
            {
               options.AddPolicy("AdminOnly", policy => {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new AdminRequirement());
                });
            });
            //This singleton is used for injection of dependancies into the AuthorizationHandler implementations.
            services.AddSingleton<IAuthorizationHandler, IsMemberHandler>();
            services.AddSingleton<IAuthorizationHandler, IsAdminHandler>();

            services.AddSignalR();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMemberRegister memberRegister, ISession session)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseStatusCodePagesWithReExecute("/error/{0}");

            //This code is used to stop iOS devices getting into an infinite loop trying to log into the application.
            //Basically they treat B2C as not this site and stop the cookie's being read allowing an infinite loop to occur.
            //See https://brockallen.com/2019/01/11/same-site-cookies-asp-net-core-and-external-authentication-providers/ for details
            app.Use(async (ctx, next) =>
            {
                await next();

                if (ctx.Request.Path == "/signin-oidc" &&
                    ctx.Response.StatusCode == 302)
                {
                    var location = ctx.Response.Headers["location"];
                    ctx.Response.StatusCode = 200;
                    var html = $@"
             <html><head>
                <meta http-equiv='refresh' content='0;url={location}' />
             </head></html>";
                    await ctx.Response.WriteAsync(html);
                }
            });

            app.UseAuthentication();
            app.UseCookiePolicy();

            app.UseSignalR(routes =>
            {
                routes.MapHub<VoteHub>("/voteHub");
            });
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }


    }

    internal class IsMemberHandler : AuthorizationHandler<MemberRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MemberRequirement requirement)
        {
            //If users token has a claim with JobTitle as Member then the user can view that content.
            foreach (var claim in context.User.Claims)
            {
                if (claim.Type == "jobTitle" && claim.Value == "Member")
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }

    internal class MemberRequirement : IAuthorizationRequirement
    {
        public MemberRequirement()
        {
        }
    }
    
    internal class IsAdminHandler : AuthorizationHandler<AdminRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
        {
            //If users token has a claim with JobTitle as Member then the user can view that content.
            foreach (var claim in context.User.Claims)
            {
                //Member is admin claim
                if (claim.Type == "jobTitle" && claim.Value == "MemberAdmin")
                {
                    context.Succeed(requirement);
                }
            }
                return Task.CompletedTask;
        }
    }
     
    internal class AdminRequirement : IAuthorizationRequirement
    {
        public AdminRequirement()
        {
        }
    }
}
