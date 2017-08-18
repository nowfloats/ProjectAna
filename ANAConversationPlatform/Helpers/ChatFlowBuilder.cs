using MongoDB.Driver;
using ANAConversationPlatform.Models;
using ANAConversationPlatform.Models.Sections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using static ANAConversationPlatform.Helpers.Constants;

namespace ANAConversationPlatform.Helpers
{
    public static class ChatFlowBuilder
    {
        public static ILogger Logger { get; set; }

        public static List<ChatNode> Build(ChatFlowPack chatFlow)
        {
            try
            {
                var filter = Builders<ChatFlowPack>.Filter;

                var nodesList = new ConcurrentBag<ChatNode>(chatFlow.ChatNodes);
                var content = new ConcurrentBag<BaseContent>(chatFlow.ChatContent);
                Parallel.ForEach(nodesList, chatNode =>
                {
                    try
                    {
                        foreach (Section section in chatNode.Sections)
                            FillSectionWithContent(section, content);
                        foreach (Button button in chatNode.Buttons)
                            button.ButtonText = content.GetFor(button)?.ButtonText;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(new EventId((int)LoggerEventId.MONGO_HELPER_ERROR), ex, "RetrieveRecordsFromChatNode Error: {0}", ex.Message);
                    }
                });

                var startNode = nodesList.FirstOrDefault(x => x.IsStartNode);
                if (startNode != null) //If start chat node is present, move it up
                {
                    var result = nodesList.Where(x => x != startNode).ToList();
                    result.Insert(0, startNode);
                    return result;
                }
                return nodesList.ToList();
            }
            catch (Exception ex)
            {
                Logger.LogError(new EventId((int)LoggerEventId.MONGO_HELPER_ERROR), ex, "RetrieveRecordsFromChatNode Error: {0}", ex.Message);
            }
            return null;
        }

        private static readonly Random rand = new Random();
        private static void FillSectionWithContent(Section emptySection, ConcurrentBag<BaseContent> contents)
        {
            switch (emptySection.SectionType)
            {
                case SectionTypeEnum.Image:
                case SectionTypeEnum.Gif:
                    var imgSection = emptySection as ImageSection;
                    var imgContent = contents.GetFor(imgSection) as TitleCaptionSectionContent;
                    if (imgContent != null)
                    {
                        imgSection.Title = imgContent.Title;
                        imgSection.Caption = imgContent.Caption;
                    }
                    break;

                case SectionTypeEnum.Text:
                    var textSection = emptySection as TextSection;
                    var textContent = contents.GetFor(textSection) as TextSectionContent;
                    if (textContent != null)
                    {
                        textSection.Text = textContent.SectionText;
                        if (textSection.DelayInMs == 0)
                            textSection.DelayInMs = Math.Min(Utils.Settings.MaxCapTimeTakenToType, textSection.Text.Length * (Utils.Settings.BaseTimeTakenToTypePerChar + rand.Next(0, Utils.Settings.VariableTimeTakenToTypePerChar)));
                    }
                    break;

                case SectionTypeEnum.Audio:
                    var audioSection = emptySection as AudioSection;
                    var audioContent = contents.GetFor(audioSection) as TitleCaptionSectionContent;
                    if (audioContent != null)
                    {
                        audioSection.Title = audioContent.Title;
                        audioSection.Caption = audioContent.Caption;
                    }
                    break;

                case SectionTypeEnum.Video:
                    var videoSection = emptySection as VideoSection;
                    var videoContent = contents.GetFor(videoSection) as TitleCaptionSectionContent;
                    if (videoContent != null)
                    {
                        videoSection.Title = videoContent.Title;
                        videoSection.Caption = videoContent.Caption;
                    }
                    break;

                case SectionTypeEnum.EmbeddedHtml:
                    var embeddedHtmlSection = emptySection as EmbeddedHtmlSection;
                    var embeddedHtmlContent = contents.GetFor(embeddedHtmlSection) as TitleCaptionSectionContent;
                    if (embeddedHtmlContent != null)
                    {
                        embeddedHtmlSection.Title = embeddedHtmlContent.Title;
                        embeddedHtmlSection.Caption = embeddedHtmlContent.Caption;
                    }
                    break;

                case SectionTypeEnum.Carousel:
                    var carouselSection = emptySection as CarouselSection;
                    var carContent = contents.GetFor(carouselSection) as TitleCaptionSectionContent;
                    if (carContent != null)
                    {
                        carouselSection.Title = carContent.Title;
                        carouselSection.Caption = carContent.Caption;
                    }
                    if (carouselSection.Items != null)
                        foreach (var carItem in carouselSection.Items)
                        {
                            var content = contents.GetFor(carItem);
                            if (content != null)
                            {
                                carItem.Title = content.Title;
                                carItem.Caption = content.Caption;
                            }
                            if (carItem.Buttons != null)
                                foreach (var carBtn in carItem.Buttons)
                                {
                                    var carBtnContent = contents.GetFor(carBtn);
                                    carBtn.Text = carBtnContent?.ButtonText;
                                }
                        }
                    break;
            }
        }

    }
}
