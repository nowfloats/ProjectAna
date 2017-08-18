using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ANAConversationPlatform.Models.Settings;
using ANAConversationPlatform.Models.AgentChat;
using ANAConversationPlatform.Helpers;
using System.Threading.Tasks;
using static ANAConversationPlatform.Helpers.Constants;
using Newtonsoft.Json.Serialization;

namespace ANAConversationPlatform
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            //Load the DB configurations into the static helper
            MongoHelper.Settings = Configuration.GetSection(nameof(DatabaseConnectionSettings)).Get<DatabaseConnectionSettings>();
            RocketChatSDK.Settings = Configuration.GetSection(nameof(AgentChatSettings)).Get<AgentChatSettings>();
            LiveClientSocketsHelper.Settings = Configuration.GetSection(nameof(LiveClientSocketsServerSettings)).Get<LiveClientSocketsServerSettings>();
            Utils.Settings = Configuration.GetSection(nameof(Helpers.Settings)).Get<Helpers.Settings>();
            Utils.BasicAuth = Configuration.GetSection(nameof(Helpers.BasicAuth)).Get<Helpers.BasicAuth>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver()); ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory
                .AddConsole(Configuration.GetSection("Logging"))
                .AddFile(Configuration.GetSection("FileLogging"));

            RocketChatSDK.Logger = loggerFactory.CreateLogger<RocketChatSDK>();
            MongoHelper.Logger = loggerFactory.CreateLogger(nameof(MongoHelper));
            ChatFlowBuilder.Logger = loggerFactory.CreateLogger(nameof(ChatFlowBuilder));
            Task.Run(async () =>
            {
                try
                {
                    var login = await RocketChatSDK.Login(RocketChatSDK.Settings.APIUserName, RocketChatSDK.Settings.APIPassword);
                    RocketChatSDK.Admin = new RocketChatSDK(login.Data.UserId, login.Data.AuthToken);
                }
                catch (System.Exception ex)
                {
                    RocketChatSDK.Logger.LogError(new EventId((int)LoggerEventId.ROCKET_CHAT_SDK_INIT_ERROR), ex, "Rocket Chat SDK Init Error: {0}", ex.Message);
                }
            }).Wait();

            app.UseMvc(routes => routes.MapRoute("default", "api/{controller=Conversation}/{action=Chat}/{id?}"));

            app.UseStaticFiles();
        }
    }
}
