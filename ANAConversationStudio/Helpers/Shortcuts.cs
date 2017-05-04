using MongoDB.Bson;
using ANAConversationStudio.Models;
using ANAConversationStudio.Models.Chat;
using ANAConversationStudio.ViewModels;
using ANAConversationStudio.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ANAConversationStudio.Helpers
{
    public static class Shortcuts
    {
        public static void CloneNode(NodeViewModel node)
        {
            if (node == null) return;
            var copyChatNode = node.ChatNode.DeepCopy();
            var contentBankOfNode = MongoHelper.Current.Contents.Where(x => x.NodeId == copyChatNode.Id).ToList().DeepCopy();

            var nodeContents = contentBankOfNode.Where(x => x is NodeContent).ToList();
            List<BaseContent> btnContents = null;
            List<BaseContent> sectionContents = null;
            if (copyChatNode.Buttons != null)
                btnContents = contentBankOfNode.Where(x => x is ButtonContent && copyChatNode.Buttons.Select(y => y._id).Contains((x as ButtonContent).ButtonId)).ToList();
            if (copyChatNode.Sections != null)
                sectionContents = contentBankOfNode.Where(x => x is SectionContent && copyChatNode.Sections.Select(y => y._id).Contains((x as SectionContent).SectionId)).ToList();

            copyChatNode.Id = ObjectId.GenerateNewId().ToString();
            copyChatNode.Name += " Copy";
            copyChatNode.NextNodeId = null;
            foreach (var nodeRelatedContent in contentBankOfNode)
            {
                nodeRelatedContent.NodeId = copyChatNode.Id;
                nodeRelatedContent.UpdatedOn = DateTime.Now;
                nodeRelatedContent.CreatedOn = DateTime.Now;
                nodeRelatedContent._id = ObjectId.GenerateNewId().ToString();
            }
            foreach (var btn in copyChatNode.Buttons)
            {
                var oldBtnId = btn._id;
                btn._id = ObjectId.GenerateNewId().ToString();
                btn.NextNodeId = null;
                if (btnContents != null)
                    foreach (var btnContent in btnContents.Where(x => x is ButtonContent && (x as ButtonContent).ButtonId == oldBtnId))
                        (btnContent as ButtonContent).ButtonId = btn._id;
            }
            foreach (var section in copyChatNode.Sections)
            {
                var oldSectionId = section._id;
                section._id = ObjectId.GenerateNewId().ToString();
                if (sectionContents != null)
                    foreach (var sectionContent in sectionContents.Where(x => x is SectionContent && (x as SectionContent).SectionId == oldSectionId))
                        (sectionContent as SectionContent).SectionId = section._id;
            }

            MongoHelper.Current.Contents.AddRange(contentBankOfNode);
            MainWindow.Current.ViewModel.CreateNode(copyChatNode, new Point(node.X + 100, node.Y + 100));
        }
    }
}