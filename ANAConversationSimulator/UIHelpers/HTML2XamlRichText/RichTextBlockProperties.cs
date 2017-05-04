using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Data.Xml.Dom;
using Windows.Data.Xml.Xsl;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Markup;

namespace ANAConversationSimulator.UIHelpers.HTML2XamlRichText
{
    /// <summary>
    /// Usage: 
    /// 1) In a XAML file, declare the above namespace, e.g.:
    ///    xmlns:common="using:RichTextBlock.Html2Xaml"
    ///     
    /// 2) In RichTextBlock controls, set or databind the Html property, e.g.:
    /// 
    ///    <RichTextBlock common:Properties.Html="{Binding ...}"/>
    ///    
    ///    or
    ///    
    ///    <RichTextBlock>
    ///       <common:Properties.Html>
    ///         <![CDATA[
    ///             <p>This is a list:</p>
    ///             <ul>
    ///                 <li>Item 1</li>
    ///                 <li>Item 2</li>
    ///                 <li>Item 3</li>
    ///             </ul>
    ///         ]]>
    ///       </common:Properties.Html>
    ///    </RichTextBlock>
    /// </summary>
    public class Properties : DependencyObject
    {
        public static readonly DependencyProperty HtmlProperty =
            DependencyProperty.RegisterAttached("Html", typeof(string), typeof(Properties), new PropertyMetadata(null, HtmlChanged));

        public static void SetHtml(DependencyObject obj, string value)
        {
            obj.SetValue(HtmlProperty, value);
        }

        public static string GetHtml(DependencyObject obj)
        {
            return (string)obj.GetValue(HtmlProperty);
        }

        private static async void HtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Get the target RichTextBlock
            Windows.UI.Xaml.Controls.RichTextBlock richText = d as Windows.UI.Xaml.Controls.RichTextBlock;
            if (richText == null) return;

            // Wrap the value of the Html property in a div and convert it to a new RichTextBlock
            string xhtml = string.Format("<div><p>{0}</p></div>", e.NewValue as string);
            xhtml = xhtml.Replace("\r", "").Replace("\n", "<br />");
            xhtml = Regex.Replace(xhtml, @"<br\s*>", "<br/>", RegexOptions.IgnoreCase);
            Windows.UI.Xaml.Controls.RichTextBlock newRichText = null;
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // In design mode we swallow all exceptions to make editing more friendly
                string xaml = "";
                try
                {
                    xaml = await ConvertHtmlToXamlRichTextBlock(xhtml);
                    newRichText = (Windows.UI.Xaml.Controls.RichTextBlock)XamlReader.Load(xaml);
                }
                catch (Exception ex)
                {
                    string errorxaml = string.Format(@"
                        <RichTextBlock 
                         xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                         xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                        >
                            <Paragraph>An exception occurred while converting HTML to XAML: {0}</Paragraph>
                            <Paragraph />
                            <Paragraph>HTML:</Paragraph>
                            <Paragraph>{1}</Paragraph>
                            <Paragraph />
                            <Paragraph>XAML:</Paragraph>
                            <Paragraph>{2}</Paragraph>
                        </RichTextBlock>",
                        ex.Message,
                        EncodeXml(xhtml),
                        EncodeXml(xaml)
                    );
                    newRichText = (Windows.UI.Xaml.Controls.RichTextBlock)XamlReader.Load(errorxaml);
                } // Display a friendly error in design mode.
            }
            else
            {
                // When not in design mode, we let the application handle any exceptions
                string xaml = "";
                try
                {
                    xaml = await ConvertHtmlToXamlRichTextBlock(xhtml);
                    newRichText = (Windows.UI.Xaml.Controls.RichTextBlock)XamlReader.Load(xaml);
                }
                catch (Exception)
                {
                    var htmlStripedTags = Regex.Replace(xhtml, "<.*?>", "");
                    htmlStripedTags = System.Net.WebUtility.HtmlEncode(htmlStripedTags);
                    string errorxaml = $@"
                        <RichTextBlock 
                         xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                         xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                        >
                            <Paragraph>{htmlStripedTags}</Paragraph>
                        </RichTextBlock>";

                    newRichText = (Windows.UI.Xaml.Controls.RichTextBlock)XamlReader.Load(errorxaml);
                } // Display a friendly error in design mode.
            }

            // Move the blocks in the new RichTextBlock to the target RichTextBlock
            richText.Blocks.Clear();
            if (newRichText != null)
            {
                for (int i = newRichText.Blocks.Count - 1; i >= 0; i--)
                {
                    Block b = newRichText.Blocks[i];
                    newRichText.Blocks.RemoveAt(i);
                    richText.Blocks.Insert(0, b);
                }
            }
        }

        private static string EncodeXml(string xml)
        {
            string encodedXml = xml.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
            return encodedXml;
        }

        private static XsltProcessor html2XamlProcessor;

        private static async Task<string> ConvertHtmlToXamlRichTextBlock(string xhtml)
        {
            // Load XHTML fragment as XML document
            try
            {
                XmlDocument xhtmlDoc = new XmlDocument();
                xhtmlDoc.LoadXml(xhtml);

                if (html2XamlProcessor == null)
                {
                    // Read XSLT. In design mode we cannot access the xslt from the file system (with Build Action = Content), 
                    // so we use it as an embedded resource instead:
                    XmlDocument html2XamlXslDoc = new XmlDocument();
                    var xlstContent = await FileIO.ReadTextAsync(await Package.Current.InstalledLocation.GetFileAsync(@"UIHelpers\HTML2XamlRichText\RichTextBlockHtml2Xaml.xslt"));
                    html2XamlXslDoc.LoadXml(xlstContent);
                    html2XamlProcessor = new XsltProcessor(html2XamlXslDoc);
                }

                // Apply XSLT to XML
                string xaml = html2XamlProcessor.TransformToString(xhtmlDoc.FirstChild);
                return xaml;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }
}
