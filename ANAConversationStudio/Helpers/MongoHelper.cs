using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ANAConversationStudio.Models;
using ANAConversationStudio.Models.Chat;
using ANAConversationStudio.Models.Chat.Sections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace ANAConversationStudio.Helpers
{
    public class MongoHelper
    {
        public static MongoHelper Current { get; set; }

        private DatabaseConnection _connection;
        public MongoHelper(DatabaseConnection connection)
        {
            #region Init
            ConventionRegistry.Register(typeof(IgnoreExtraElementsConvention).Name, new ConventionPack { new IgnoreExtraElementsConvention(true) }, t => true);
            ConventionRegistry.Register("EnumStringConvention", new ConventionPack { new EnumRepresentationConvention(BsonType.String) }, t => true);
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

            if (!BsonClassMap.IsClassMapRegistered(typeof(NodeContent)))
                BsonClassMap.RegisterClassMap<NodeContent>();

            if (!BsonClassMap.IsClassMapRegistered(typeof(SectionContent)))
                BsonClassMap.RegisterClassMap<SectionContent>();

            if (!BsonClassMap.IsClassMapRegistered(typeof(TextSectionContent)))
                BsonClassMap.RegisterClassMap<TextSectionContent>();

            if (!BsonClassMap.IsClassMapRegistered(typeof(TitleCaptionSectionContent)))
                BsonClassMap.RegisterClassMap<TitleCaptionSectionContent>();

            if (!BsonClassMap.IsClassMapRegistered(typeof(ImageSectionContent)))
                BsonClassMap.RegisterClassMap<ImageSectionContent>();

            if (!BsonClassMap.IsClassMapRegistered(typeof(GraphSectionContent)))
                BsonClassMap.RegisterClassMap<GraphSectionContent>();

            if (!BsonClassMap.IsClassMapRegistered(typeof(ButtonContent)))
                BsonClassMap.RegisterClassMap<ButtonContent>();

            if (!BsonClassMap.IsClassMapRegistered(typeof(CarouselButtonContent)))
                BsonClassMap.RegisterClassMap<CarouselButtonContent>();

            if (!BsonClassMap.IsClassMapRegistered(typeof(CarouselItemContent)))
                BsonClassMap.RegisterClassMap<CarouselItemContent>();
            #endregion
            _connection = connection;
        }

        private MongoClient _chatDBClient;
        public MongoClient ChatDBClient
        {
            get
            {
                if (_chatDBClient == null)
                    _chatDBClient = new MongoClient(_connection.ConnectionString);
                return _chatDBClient;
            }
        }

        private IMongoDatabase _chatDB;
        public IMongoDatabase ChatDB
        {
            get
            {
                if (_chatDB == null)
                    _chatDB = ChatDBClient.GetDatabase(_connection.DatabaseName);
                return _chatDB;
            }
        }

        private List<ChatNode> _chatNodes;
        public List<ChatNode> ChatNodes
        {
            get
            {
                if (_chatNodes == null)
                {
                    _chatNodes = ChatDB.GetCollection<ChatNode>(_connection.TemplateCollectionName).Find(new BsonDocument()).ToList();
                    if (_chatNodes == null)
                        _chatNodes = new List<ChatNode>();
                }
                return _chatNodes;
            }
        }

        private List<BaseContent> _contents;
        public List<BaseContent> Contents
        {
            get
            {
                if (_contents == null)
                {
                    _contents = ChatDB.GetCollection<BaseContent>(_connection.ContentCollectionName).Find(new BsonDocument()).ToList();
                    if (_contents == null)
                        _contents = new List<BaseContent>();
                }
                return _contents;
            }
        }

        private Dictionary<string, Point> _NodeLocations;
        public Dictionary<string, Point> NodeLocations
        {
            get
            {
                if (_NodeLocations == null)
                {
                    //Can be improved
                    var layout = ChatDB.GetCollection<Dictionary<string, LayoutPoint>>(_connection.LayoutCollectionName).Find(new BsonDocument()).Project<Dictionary<string, LayoutPoint>>(Builders<Dictionary<string, LayoutPoint>>.Projection.Exclude("_id")).FirstOrDefault();
                    _NodeLocations = layout.ToDictionary(x => x.Key, x => new Point(x.Value.X, x.Value.Y));
                    if (_NodeLocations == null)
                        _NodeLocations = new Dictionary<string, Point>();
                }
                return _NodeLocations;
            }
        }

        public Point? GetPointForNode(string chatNodeId)
        {
            if (NodeLocations.ContainsKey(chatNodeId))
                return NodeLocations[chatNodeId];
            return null;
        }

        public void SaveNodeLocations(Dictionary<string, Point> nodeLocations)
        {
            if (nodeLocations.Count > 0)
            {
                var coll = ChatDB.GetCollection<Dictionary<string, LayoutPoint>>(_connection.LayoutCollectionName);
                coll.DeleteMany(new BsonDocument()); //Clear All
                coll.InsertOne(nodeLocations.ToDictionary(x => x.Key, x => new LayoutPoint { X = x.Value.X, Y = x.Value.Y }));
            }
        }

        public void SaveChatNodes(List<ChatNode> nodes)
        {
            if (nodes.Count > 0)
            {
                var coll = ChatDB.GetCollection<ChatNode>(_connection.TemplateCollectionName);
                coll.DeleteMany(new BsonDocument()); //Clear All
                coll.InsertMany(nodes);
                try
                {
                    if (!Directory.Exists("backup"))
                        Directory.CreateDirectory("backup");
                    File.WriteAllText("backup/nodes-" + ObjectId.GenerateNewId().ToString() + ".json", JsonConvert.SerializeObject(nodes, new StringEnumConverter()));
                }
                catch { }
            }
        }
        public void SaveChatContent()
        {
            if (Contents.Count > 0)
            {
                var coll = ChatDB.GetCollection<BaseContent>(_connection.ContentCollectionName);
                coll.DeleteMany(new BsonDocument()); //Clear All
                coll.InsertMany(Contents);
                try
                {
                    if (!Directory.Exists("backup"))
                        Directory.CreateDirectory("backup");
                    File.WriteAllText("backup/nodes-content-" + ObjectId.GenerateNewId().ToString() + ".json", JsonConvert.SerializeObject(Contents, new StringEnumConverter()));
                }
                catch { }
            }
        }
    }
}
