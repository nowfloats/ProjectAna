using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using ANAConversationPlatform.Models;
using ANAConversationPlatform.Models.Sections;
using ANAConversationPlatform.Models.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using ANAConversationPlatform.Models.Activity;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using static ANAConversationPlatform.Helpers.Constants;

namespace ANAConversationPlatform.Helpers
{
    public static class MongoHelper
    {
        static MongoHelper()
        {
            #region Initialize Mongo Driver
            ConventionRegistry.Register(nameof(IgnoreExtraElementsConvention), new ConventionPack { new IgnoreExtraElementsConvention(true) }, t => true);
            ConventionRegistry.Register(nameof(EnumRepresentationConvention), new ConventionPack { new EnumRepresentationConvention(BsonType.String) }, t => true);
            if (!BsonClassMap.IsClassMapRegistered(typeof(GifSection)))
                BsonClassMap.RegisterClassMap<GifSection>();

            if (!BsonClassMap.IsClassMapRegistered(typeof(TextSection)))
                BsonClassMap.RegisterClassMap<TextSection>();

            if (!BsonClassMap.IsClassMapRegistered(typeof(ImageSection)))
                BsonClassMap.RegisterClassMap<ImageSection>();

            if (!BsonClassMap.IsClassMapRegistered(typeof(AudioSection)))
                BsonClassMap.RegisterClassMap<AudioSection>();

            if (!BsonClassMap.IsClassMapRegistered(typeof(VideoSection)))
                BsonClassMap.RegisterClassMap<VideoSection>();

            if (!BsonClassMap.IsClassMapRegistered(typeof(EmbeddedHtmlSection)))
                BsonClassMap.RegisterClassMap<EmbeddedHtmlSection>();

            if (!BsonClassMap.IsClassMapRegistered(typeof(CarouselSection)))
                BsonClassMap.RegisterClassMap<CarouselSection>();

            if (!BsonClassMap.IsClassMapRegistered(typeof(SectionContent)))
                BsonClassMap.RegisterClassMap<SectionContent>();

            if (!BsonClassMap.IsClassMapRegistered(typeof(TextSectionContent)))
                BsonClassMap.RegisterClassMap<TextSectionContent>();

            if (!BsonClassMap.IsClassMapRegistered(typeof(TitleCaptionSectionContent)))
                BsonClassMap.RegisterClassMap<TitleCaptionSectionContent>();

            if (!BsonClassMap.IsClassMapRegistered(typeof(ImageSectionContent)))
                BsonClassMap.RegisterClassMap<ImageSectionContent>();

            if (!BsonClassMap.IsClassMapRegistered(typeof(ButtonContent)))
                BsonClassMap.RegisterClassMap<ButtonContent>();

            if (!BsonClassMap.IsClassMapRegistered(typeof(PrintOTPSection)))
                BsonClassMap.RegisterClassMap<PrintOTPSection>();

            if (!BsonClassMap.IsClassMapRegistered(typeof(CarouselButtonContent)))
                BsonClassMap.RegisterClassMap<CarouselButtonContent>();

            if (!BsonClassMap.IsClassMapRegistered(typeof(CarouselItemContent)))
                BsonClassMap.RegisterClassMap<CarouselItemContent>();
            #endregion
        }
        public static ILogger Logger { get; set; }
        public static DatabaseConnectionSettings Settings { get; set; }

        private static MongoClient _chatClient;
        private static IMongoDatabase _chatDB;
        private static IMongoDatabase ChatDB
        {
            get
            {
                if (Settings == null)
                    throw new Exception("MongoHelper.Settings is null");

                if (_chatClient == null || _chatClient == null)
                {
                    _chatClient = new MongoClient(Settings.ConnectionString);
                    _chatDB = _chatClient.GetDatabase(Settings.DatabaseName);
                }
                return _chatDB;
            }
        }

        public static async Task InsertActivityEventAsync(ChatActivityEvent activityEvent)
        {
            try
            {
                if (activityEvent != null && string.IsNullOrWhiteSpace(activityEvent._id))
                    activityEvent._id = ObjectId.GenerateNewId().ToString();

                var coll = ChatDB.GetCollection<ChatActivityEvent>(Settings.ActivityEventLogCollectionName);
                await coll.InsertOneAsync(activityEvent);
            }
            catch (Exception ex)
            {
                Logger.LogError(new EventId((int)LoggerEventId.MONGO_HELPER_ERROR), ex, "InsertActivityEvent: {0}", ex.Message);
            }
        }

        public static async Task<List<ANAProject>> GetProjectsAsync()
        {
            try
            {
                return await ChatDB.GetCollection<ANAProject>(Settings.ProjectsCollectionName).Find(new BsonDocument()).ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(new EventId((int)LoggerEventId.MONGO_HELPER_ERROR), ex, "GetProjectsAsync: {0}", ex.Message);
                return null;
            }
        }

        public static async Task<ANAProject> GetProjectAsync(string projectId)
        {
            try
            {
                return await ChatDB.GetCollection<ANAProject>(Settings.ProjectsCollectionName).Find(x => x._id == projectId).SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(new EventId((int)LoggerEventId.MONGO_HELPER_ERROR), ex, "GetProjectAsync: {0}", ex.Message);
                return null;
            }
        }

        public static async Task<ANAProject> SaveProjectAsync(ANAProject project)
        {
            try
            {
                var coll = ChatDB.GetCollection<ANAProject>(Settings.ProjectsCollectionName);
                if (string.IsNullOrWhiteSpace(project._id))
                    project._id = ObjectId.GenerateNewId().ToString();

                if (coll.Count(x => x._id == project._id, new CountOptions { Limit = 1 }) > 0)
                {
                    project.UpdatedOn = DateTime.UtcNow;
                    await coll.ReplaceOneAsync(x => x._id == project._id, project);
                    return project;
                }

                project.CreatedOn = project.UpdatedOn = DateTime.UtcNow;
                await coll.InsertOneAsync(project);
            }
            catch (Exception ex)
            {
                Logger.LogError(new EventId((int)LoggerEventId.MONGO_HELPER_ERROR), ex, "SaveProject: {0}", ex.Message);
            }
            return null;
        }

        public static async Task<bool> SaveChatFlowAsync(ChatFlowPack chatFlow)
        {
            try
            {
                var chatsColl = ChatDB.GetCollection<ChatFlowPack>(Settings.ChatFlowPacksCollectionName);
                if (string.IsNullOrWhiteSpace(chatFlow.ProjectId))
                    chatFlow.ProjectId = ObjectId.GenerateNewId().ToString();

                if (await chatsColl.CountAsync(x => x.ProjectId == chatFlow.ProjectId, new CountOptions { Limit = 1 }) > 0)
                {
                    chatFlow.UpdatedOn = DateTime.UtcNow;
                    await chatsColl.ReplaceOneAsync(x => x.ProjectId == chatFlow.ProjectId, chatFlow);
                    return true;
                }
                chatFlow.CreatedOn = chatFlow.UpdatedOn = DateTime.UtcNow;
                await chatsColl.InsertOneAsync(chatFlow);
            }
            catch (Exception ex)
            {
                Logger.LogError(new EventId((int)LoggerEventId.MONGO_HELPER_ERROR), ex, "SaveChatFlowAsync: {0}", ex.Message);
            }
            return false;
        }

        public static async Task<ChatFlowPack> GetChatFlowPackAsync(string projectId)
        {
            try
            {
                return await ChatDB.GetCollection<ChatFlowPack>(Settings.ChatFlowPacksCollectionName).Find(x => x.ProjectId == projectId).SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(new EventId((int)LoggerEventId.MONGO_HELPER_ERROR), ex, "GetChatFlowPackAsync: {0}", ex.Message);
                return null;
            }
        }
    }
}