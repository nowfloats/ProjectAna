using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using ANAConversationPlatform.Models;
using ANAConversationPlatform.Models.Sections;
using ANAConversationPlatform.Models.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ANAConversationPlatform.Models.Activity;

namespace ANAConversationPlatform.Helpers
{
    public static class MongoHelper
    {
        static MongoHelper()
        {
            ConventionRegistry.Register(typeof(IgnoreExtraElementsConvention).Name, new ConventionPack { new IgnoreExtraElementsConvention(true) }, t => true);
        }

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

        private static List<Content> _contents;
        private static List<Content> Contents
        {
            get
            {
                if (Settings.CacheContent)
                {
                    if (_contents == null)
                        _contents = GetContentCollection();
                    return _contents;
                }
                else
                    return GetContentCollection();
            }
        }

        public static List<Content> GetContentCollection() => ChatDB.GetCollection<Content>(Settings.ContentCollectionName).Find(new BsonDocument()).ToList();

        public static void RefreshContentInMemory()
        {
            _contents = GetContentCollection();
        }

        public static List<ChatNode> RetrieveRecordsFromChatNode()
        {
            try
            {
                var nodeTemplateCollection = ChatDB.GetCollection<BsonDocument>(Settings.TemplateCollectionName);

                // Creating Filter for Date Range Greater than Equal to Start Date and Less than End Date
                FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;

                var filter = new BsonDocument();
                List<BsonDocument> nodeList;

                // Retrieving records, if no/invalid limit is specified then all records are retrieved otherwise records as per specified limit and offset are retrieved
                nodeList = nodeTemplateCollection.Find(filter).Project(Builders<BsonDocument>.Projection.Exclude("Sections._t").Exclude("Buttons._t")).ToList();

                List<ChatNode> chatNodes = new List<ChatNode>();
                foreach (BsonDocument node in nodeList)
                {
                    try
                    {
                        var chatNode = BsonSerializer.Deserialize<ChatNode>(node);

                        chatNode.Sections = new List<Section>();
                        chatNode.Buttons = new List<Button>();

                        //Adding Header Text
                        Content nodeContent = Contents.GetFor(chatNode);

                        if (nodeContent != null)
                            chatNode.HeaderText = nodeContent.NodeHeaderText;

                        BsonArray sectionBsonArray = node.GetValue("Sections").AsBsonArray;
                        foreach (BsonDocument sectionBsonDocument in sectionBsonArray)
                        {
                            Section sectObj = GetSection(sectionBsonDocument);
                            chatNode.Sections.Add(sectObj);
                        }

                        BsonArray buttonBsonArray = node.GetValue("Buttons").AsBsonArray;
                        foreach (BsonDocument buttonBsonDocument in buttonBsonArray)
                        {
                            Button btn = BsonSerializer.Deserialize<Button>(buttonBsonDocument);
                            Content buttonContent = Contents.GetFor(btn);
                            btn.ButtonName = buttonContent?.ButtonName;
                            btn.ButtonText = buttonContent?.ButtonText;
                            chatNode.Buttons.Add(btn);
                        }

                        if (node.Contains("IsStartNode") && node["IsStartNode"] != null && (bool)node["IsStartNode"])
                            chatNodes.Insert(0, chatNode);
                        else
                            chatNodes.Add(chatNode);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                }
                return chatNodes;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message + "\n" + e.StackTrace);
            }
            return null;
        }

        private static Section GetSection(BsonDocument sectionBsonDocument)
        {
            Random rnd = new Random();

            Section sectObj;

            string sectionType = sectionBsonDocument.GetValue("SectionType")?.ToString();
            Debug.WriteLine("Section Type:" + sectionType);
            switch (sectionType?.ToLower())
            {
                case "image":
                    ImageSection imgSectObj = BsonSerializer.Deserialize<ImageSection>(sectionBsonDocument);
                    Content imgContent = Contents.GetFor(imgSectObj);
                    if (imgContent != null)
                    {
                        imgSectObj.Title = imgContent.Title;
                        imgSectObj.Caption = imgContent.Caption;
                    }

                    sectObj = imgSectObj;

                    break;

                case "text":
                    TextSection textSectObj = BsonSerializer.Deserialize<TextSection>(sectionBsonDocument);
                    Content textContent = Contents.GetFor(textSectObj);
                    if (textContent != null)
                        textSectObj.Text = textContent.SectionText;
                    sectObj = textSectObj;
                    break;

                case "graph":

                    GraphSection gphObj = BsonSerializer.Deserialize<GraphSection>(sectionBsonDocument);

                    Content docContent = Contents.GetFor(gphObj);
                    if (docContent != null)
                    {
                        gphObj.Caption = docContent.Caption;

                        gphObj.X.Label = docContent.XLabel;
                        gphObj.Y.Label = docContent.YLabel;
                    }
                    gphObj.CoordinatesSet = new List<Coordinates>();
                    BsonArray coordinateSetBsonArray = sectionBsonDocument.GetValue("CoordinatesSet").AsBsonArray;


                    if (coordinateSetBsonArray != null)
                        foreach (BsonDocument coordinateSetBsonDoc in coordinateSetBsonArray)
                        {
                            var coordinateListId = coordinateSetBsonDoc.GetValue("CoordinateListId")?.ToString();
                            if (!string.IsNullOrWhiteSpace(coordinateListId))
                                continue;

                            var coordinatesObj = new Coordinates()
                            {
                                CoordinateListId = coordinateListId
                            };

                            var coordinateContent = Contents.GetFor(coordinatesObj);
                            coordinatesObj.LegendName = coordinateContent?.CoordinateListLegend;

                            if (coordinateSetBsonDoc.TryGetValue("CoordinateList", out BsonValue tempCoordinateList))
                            {
                                BsonArray coordinateListBsonArray = tempCoordinateList?.AsBsonArray;
                                if (coordinateListBsonArray != null)
                                    foreach (BsonDocument coordinateBsonDoc in coordinateListBsonArray)
                                    {
                                        string x = coordinateBsonDoc.GetValue("X")?.AsString;
                                        string y = coordinateBsonDoc.GetValue("Y")?.AsString;

                                        string coordinateText = coordinateContent?.CoordinateText;

                                        if (string.IsNullOrWhiteSpace(coordinateText))
                                            coordinatesObj.AddXYCoordinates(x, y);
                                        else
                                            coordinatesObj.AddXYCoordinates(x, y, coordinateText);

                                        Debug.WriteLine(coordinatesObj.ToJson());
                                    }
                            }
                            gphObj.CoordinatesSet.Add(coordinatesObj);
                        }
                    sectObj = gphObj;

                    break;

                case "gif":
                    GifSection gifObj = BsonSerializer.Deserialize<GifSection>(sectionBsonDocument);
                    Content gifContent = Contents.GetFor(gifObj);
                    if (gifContent != null)
                    {
                        gifObj.Title = gifContent.Title;
                        gifObj.Caption = gifContent.Caption;
                    }
                    sectObj = gifObj;
                    break;

                case "audio":
                    AudioSection audioObj = BsonSerializer.Deserialize<AudioSection>(sectionBsonDocument);
                    Content audioContent = Contents.GetFor(audioObj);
                    if (audioContent != null)
                    {
                        audioObj.Title = audioContent.Title;
                        audioObj.Caption = audioContent.Caption;
                    }

                    sectObj = audioObj;
                    break;

                case "video":
                    VideoSection videoObj = BsonSerializer.Deserialize<VideoSection>(sectionBsonDocument);
                    Content videoContent = Contents.GetFor(videoObj);
                    if (videoContent != null)
                    {
                        videoObj.Title = videoContent.Title;
                        videoObj.Caption = videoContent.Caption;
                    }
                    sectObj = videoObj;
                    break;

                case "link":
                    UrlSection urlObj = BsonSerializer.Deserialize<UrlSection>(sectionBsonDocument);
                    sectObj = urlObj;
                    break;

                case "embeddedhtml":
                    EmbeddedHtmlSection embeddedHtmlObj = BsonSerializer.Deserialize<EmbeddedHtmlSection>(sectionBsonDocument);
                    Content embeddedHtmlContent = Contents.GetFor(embeddedHtmlObj);
                    if (embeddedHtmlContent != null)
                    {
                        embeddedHtmlObj.Title = embeddedHtmlContent.Title;
                        embeddedHtmlObj.Caption = embeddedHtmlContent.Caption;
                    }
                    sectObj = embeddedHtmlObj;
                    break;

                default:
                    sectObj = null;
                    break;
            }
            return sectObj;
        }

        public static void InsertActivityEvent(ChatActivityEvent activityEvent)
        {
            if (activityEvent != null && string.IsNullOrWhiteSpace(activityEvent._id))
                activityEvent._id = ObjectId.GenerateNewId().ToString();

            var coll = ChatDB.GetCollection<ChatActivityEvent>(Settings.ActivityEventLogCollectionName);
            coll.InsertOne(activityEvent);
        }
    }
}