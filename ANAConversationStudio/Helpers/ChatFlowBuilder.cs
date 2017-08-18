using MongoDB.Driver;
using ANAConversationStudio.Models;
using ANAConversationStudio.Models.Chat;
using ANAConversationStudio.Models.Chat.Sections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace ANAConversationStudio.Helpers
{
    public static class ChatFlowBuilder
    {
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
                        Logger.Write(ex);
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
                Logger.Write(ex);
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
                        imgSection.ContentId = imgContent._id;
                    }
                    break;

                case SectionTypeEnum.Text:
                    var textSection = emptySection as TextSection;
                    var textContent = contents.GetFor(textSection) as TextSectionContent;
                    if (textContent != null)
                    {
                        textSection.Text = textContent.SectionText;
                        textSection.ContentId = textSection._id;
                    }
                    break;

                case SectionTypeEnum.Audio:
                    var audioSection = emptySection as AudioSection;
                    var audioContent = contents.GetFor(audioSection) as TitleCaptionSectionContent;
                    if (audioContent != null)
                    {
                        audioSection.Title = audioContent.Title;
                        audioSection.Caption = audioContent.Caption;
                        audioSection.ContentId = audioContent._id;
                    }
                    break;

                case SectionTypeEnum.Video:
                    var videoSection = emptySection as VideoSection;
                    var videoContent = contents.GetFor(videoSection) as TitleCaptionSectionContent;
                    if (videoContent != null)
                    {
                        videoSection.Title = videoContent.Title;
                        videoSection.Caption = videoContent.Caption;
                        videoSection.ContentId = videoContent._id;
                    }
                    break;

                case SectionTypeEnum.EmbeddedHtml:
                    var embeddedHtmlSection = emptySection as EmbeddedHtmlSection;
                    var embeddedHtmlContent = contents.GetFor(embeddedHtmlSection) as TitleCaptionSectionContent;
                    if (embeddedHtmlContent != null)
                    {
                        embeddedHtmlSection.Title = embeddedHtmlContent.Title;
                        embeddedHtmlSection.Caption = embeddedHtmlContent.Caption;
                        embeddedHtmlSection.ContentId = embeddedHtmlContent._id;
                    }
                    break;

                case SectionTypeEnum.Carousel:
                    var carouselSection = emptySection as CarouselSection;
                    var carContent = contents.GetFor(carouselSection) as TitleCaptionSectionContent;
                    if (carContent != null)
                    {
                        carouselSection.Title = carContent.Title;
                        carouselSection.Caption = carContent.Caption;
                        carouselSection.ContentId = carContent._id;
                    }
                    if (carouselSection.Items != null)
                        foreach (var carItem in carouselSection.Items)
                        {
                            var content = contents.GetFor(carItem);
                            if (content != null)
                            {
                                carItem.Title = content.Title;
                                carItem.Caption = content.Caption;
                                carItem.ContentId = content._id;
                            }
                            if (carItem.Buttons != null)
                                foreach (var carBtn in carItem.Buttons)
                                {
                                    var carBtnContent = contents.GetFor(carBtn);
                                    carBtn.Text = carBtnContent?.ButtonText;
                                    carBtn.ContentId = carBtnContent?._id;
                                }
                        }
                    break;
            }
        }

        public static SectionContent GetFor(this IEnumerable<BaseContent> contentCollection, Section section)
        {
            if (string.IsNullOrWhiteSpace(section._id)) return null;
            return contentCollection.FirstOrDefault(x => x is SectionContent && !string.IsNullOrWhiteSpace((x as SectionContent).SectionId) && (x as SectionContent).SectionId == section._id) as SectionContent;
        }
        public static CarouselItemContent GetFor(this IEnumerable<BaseContent> contentCollection, CarouselItem carouselItem)
        {
            if (string.IsNullOrWhiteSpace(carouselItem._id)) return null;
            return contentCollection.FirstOrDefault(x => x is CarouselItemContent && !string.IsNullOrWhiteSpace((x as CarouselItemContent).CarouselItemId) && (x as CarouselItemContent).CarouselItemId == carouselItem._id) as CarouselItemContent;
        }
        public static CarouselButtonContent GetFor(this IEnumerable<BaseContent> contentCollection, CarouselButton carouselBtn)
        {
            if (string.IsNullOrWhiteSpace(carouselBtn._id)) return null;
            return contentCollection.FirstOrDefault(x => x is CarouselButtonContent && !string.IsNullOrWhiteSpace((x as CarouselButtonContent).CarouselButtonId) && (x as CarouselButtonContent).CarouselButtonId == carouselBtn._id) as CarouselButtonContent;
        }
        public static ButtonContent GetFor(this IEnumerable<BaseContent> contentCollection, Button btn)
        {
            return contentCollection.FirstOrDefault(x => x is ButtonContent && !string.IsNullOrWhiteSpace((x as ButtonContent).ButtonId) && (x as ButtonContent).ButtonId == btn._id) as ButtonContent;
        }
    }
}
