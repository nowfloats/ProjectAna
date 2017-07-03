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
using System.Threading.Tasks;
using System.Collections.Concurrent;
using ANAConversationPlatform.Controllers;
using Microsoft.Extensions.Logging;
using static ANAConversationPlatform.Helpers.Constants;

namespace ANAConversationPlatform.Helpers
{
    public static class MongoHelper
    {
        static MongoHelper()
        {
            ConventionRegistry.Register(typeof(IgnoreExtraElementsConvention).Name, new ConventionPack { new IgnoreExtraElementsConvention(true) }, t => true);
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

        private static ConcurrentBag<Content> _contents;
        private static ConcurrentBag<Content> Contents
        {
            get
            {
                if (_contents == null)
                    _contents = new ConcurrentBag<Content>(GetContentCollection());
                return _contents;
            }
        }

        public static List<Content> GetContentCollection() => ChatDB.GetCollection<Content>(Settings.ContentCollectionName).Find(new BsonDocument()).ToList();

        public static void RefreshContentInMemory()
        {
            _contents = new ConcurrentBag<Content>(GetContentCollection());
        }

        public static List<ChatNode> RetrieveRecordsFromChatNode()
        {
            try
            {
                if (!Settings.CacheContent)
                    RefreshContentInMemory();

                var nodeTemplateCollection = ChatDB.GetCollection<BsonDocument>(Settings.TemplateCollectionName);

                // Retrieving records, if no/invalid limit is specified then all records are retrieved otherwise records as per specified limit and offset are retrieved
                var nodeList = nodeTemplateCollection.Find(new BsonDocument()).Project(Builders<BsonDocument>.Projection.Exclude("Sections._t").Exclude("Buttons._t")).ToList();

                var chatNodes = new ConcurrentBag<ChatNode>();
                Parallel.ForEach(nodeList, node =>
                {
                    try
                    {
                        var chatNode = BsonSerializer.Deserialize<ChatNode>(node);

                        chatNode.Sections = new List<Section>();
                        chatNode.Buttons = new List<Button>();

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
                        chatNodes.Add(chatNode);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(new EventId((int)LoggerEventId.MONGO_HELPER_ERROR), ex, "RetrieveRecordsFromChatNode Error: {0}", ex.Message);
                    }
                });

                var startNode = chatNodes.FirstOrDefault(x => x.IsStartNode);
                if (startNode != null) //If start chat node is present, move it up
                {
                    var result = chatNodes.Where(x => x != startNode).ToList();
                    result.Insert(0, startNode);
                    return result;
                }
                return chatNodes.ToList();
            }
            catch (Exception ex)
            {
                Logger.LogError(new EventId((int)LoggerEventId.MONGO_HELPER_ERROR), ex, "RetrieveRecordsFromChatNode Error: {0}", ex.Message);
            }
            return null;
        }

        static Random rand = new Random();
        private static Section GetSection(BsonDocument sectionBsonDocument)
        {
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
                    {
                        textSectObj.Text = textContent.SectionText;
                        if (textSectObj.DelayInMs == 0)
                            textSectObj.DelayInMs = Math.Min(Utils.Settings.MaxCapTimeTakenToType, textSectObj.Text.Length * (Utils.Settings.BaseTimeTakenToTypePerChar + rand.Next(0, Utils.Settings.VariableTimeTakenToTypePerChar)));
                    }
                    sectObj = textSectObj;
                    break;

                case "graph":
                    {
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
                    }

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

                case "carousel":
                    var carouselObj = BsonSerializer.Deserialize<CarouselSection>(sectionBsonDocument);
                    var carContent = Contents.GetFor(carouselObj);
                    if (carContent != null)
                    {
                        carouselObj.Title = carContent.Title;
                        carouselObj.Caption = carContent.Caption;
                    }
                    if (carouselObj.Items != null)
                        foreach (var carItem in carouselObj.Items)
                        {
                            var content = Contents.GetFor(carItem);
                            if (content != null)
                            {
                                carItem.Title = content.Title;
                                carItem.Caption = content.Caption;
                            }
                            if (carItem.Buttons != null)
                                foreach (var carBtn in carItem.Buttons)
                                {
                                    var carBtnContent = Contents.GetFor(carBtn);
                                    carBtn.Text = carBtnContent?.ButtonText;
                                }
                        }
                    sectObj = carouselObj;
                    break;
                case "printotp":
                    PrintOTPSection printOTPSection = BsonSerializer.Deserialize<PrintOTPSection>(sectionBsonDocument);
                    //No content to load for Section Type: PrintOTP
                    sectObj = printOTPSection;
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