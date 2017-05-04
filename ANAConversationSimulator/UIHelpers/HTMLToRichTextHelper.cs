using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace ANAConversationSimulator.UIHelpers
{
    public static class HTMLToRichTextHelper
    {
        const string XML_FIRST_NODE = "<?xml version=\"1.0\" encoding=\"utf - 8\" ?>";
        public static string GetHtml(DependencyObject obj)
        {
            return (string)obj.GetValue(HtmlProperty);
        }

        public static void SetHtml(DependencyObject obj, string value)
        {
            obj.SetValue(HtmlProperty, value);
        }

        // Using a DependencyProperty as the backing store for Html.  This enables animation, styling, binding, etc... 
        public static readonly DependencyProperty HtmlProperty =
            DependencyProperty.RegisterAttached("Html", typeof(string), typeof(HTMLToRichTextHelper), new PropertyMetadata("", OnHtmlChanged));

        private static void OnHtmlChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            Windows.UI.Xaml.Controls.RichTextBlock parent = (Windows.UI.Xaml.Controls.RichTextBlock)sender;
            parent.Blocks.Clear();

            XmlDocument document = new XmlDocument();
            var text = (string)eventArgs.NewValue;
            if (!text.StartsWith(XML_FIRST_NODE))
                text = XML_FIRST_NODE + $"<body>{text}</body>";
            document.LoadXml(text);

            ParseElement((XmlElement)document.GetElementsByTagName("body")[0], new RichTextBlockTextContainer(parent));
        }

        private static void ParseElement(XmlElement element, ITextContainer parent)
        {
            foreach (var child in element.ChildNodes)
            {
                if (child is Windows.Data.Xml.Dom.XmlText childText)
                {
                    if (string.IsNullOrWhiteSpace(childText.InnerText))
                        continue;
                    parent.Add(new Run { Text = childText.InnerText });
                }
                else if (child is XmlElement childXml)
                {
                    switch (childXml.TagName.ToUpper())
                    {
                        case "P":
                            var paragraph = new Paragraph();
                            parent.Add(paragraph);
                            ParseElement(childXml, new ParagraphTextContainer(paragraph));
                            break;
                        case "STRONG":
                        case "B":
                            var bold = new Bold();
                            parent.Add(bold);
                            ParseElement(childXml, new SpanTextContainer(bold));
                            break;
                        case "U":
                            var underline = new Underline();
                            parent.Add(underline);
                            ParseElement(childXml, new SpanTextContainer(underline));
                            break;
                        case "A":
                            ParseElement(childXml, parent);
                            break;
                        case "BR":
                            parent.Add(new LineBreak());
                            break;
                    }
                }
            }
        }

        private interface ITextContainer
        {
            void Add(Inline text);
            void Add(Block paragraph);
        }

        private sealed class SpanTextContainer : ITextContainer
        {
            private readonly Span _span;

            public SpanTextContainer(Span span)
            {
                _span = span;
            }

            public void Add(Inline text)
            {
                _span.Inlines.Add(text);
            }

            public void Add(Block paragraph)
            {
                throw new NotSupportedException();
            }
        }

        private sealed class ParagraphTextContainer : ITextContainer
        {
            private readonly Paragraph _block;

            public ParagraphTextContainer(Paragraph block)
            {
                _block = block;
            }

            public void Add(Inline text)
            {
                _block.Inlines.Add(text);
            }

            public void Add(Block paragraph)
            {
                throw new NotSupportedException();
            }
        }

        private sealed class RichTextBlockTextContainer : ITextContainer
        {
            private readonly Windows.UI.Xaml.Controls.RichTextBlock _richTextBlock;

            public RichTextBlockTextContainer(Windows.UI.Xaml.Controls.RichTextBlock richTextBlock)
            {
                _richTextBlock = richTextBlock;
            }

            public void Add(Inline text)
            {
                var paragraph = new Paragraph();
                paragraph.Inlines.Add(text);

                _richTextBlock.Blocks.Add(paragraph);
            }

            public void Add(Block paragraph)
            {
                _richTextBlock.Blocks.Add(paragraph);
            }
        }

    }
}
