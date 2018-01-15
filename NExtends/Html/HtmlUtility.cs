using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NExtends.Html
{
    /// <summary>
    /// This is an HTML cleanup utility combining the benefits of the
    /// HtmlAgilityPack to parse raw HTML and the AntiXss library
    /// to remove potentially dangerous user input.
    ///
    /// Additionally it uses a list created by Robert Beal to limit
    /// the number of allowed tags and attributes to a sensible level
    /// 
    /// Insipired by http://eksith.wordpress.com/2012/02/13/antixss-4-2-breaks-everything/
    /// </summary>
    public class HtmlUtility : IHtmlUtility
    {
        // Original list courtesy of Robert Beal :
        // http://www.robertbeal.com/
        private static readonly IReadOnlyDictionary<string, string[]> ValidHtmlTags = new Dictionary<string, string[]>
        {
            { "p", new string[]          { "style", "class", "align" } },
            { "div", new string[]        { "style", "class", "align" } },
            { "span", new string[]       { "style", "class" } },
            { "br", new string[]         { "style", "class" } },
            { "hr", new string[]         { "style", "class" } },
            { "label", new string[]      { "style", "class" } },

            { "h1", new string[]         { "style", "class" } },
            { "h2", new string[]         { "style", "class" } },
            { "h3", new string[]         { "style", "class" } },
            { "h4", new string[]         { "style", "class" } },
            { "h5", new string[]         { "style", "class" } },
            { "h6", new string[]         { "style", "class" } },

            { "font", new string[]       { "style", "class", "color", "face", "size" } },
            { "strong", new string[]     { "style", "class" } },
            { "b", new string[]          { "style", "class" } },
            { "em", new string[]         { "style", "class" } },
            { "i", new string[]          { "style", "class" } },
            { "u", new string[]          { "style", "class" } },
            { "strike", new string[]     { "style", "class" } },
            { "ol", new string[]         { "style", "class" } },
            { "ul", new string[]         { "style", "class" } },
            { "li", new string[]         { "style", "class" } },
            { "blockquote", new string[] { "style", "class" } },
            { "code", new string[]       { "style", "class" } },
            { "pre", new string[]       { "style", "class" } },

            { "a", new string[]          { "style", "class", "href", "title" } },
            { "img", new string[]        { "style", "class", "src", "height", "width", "alt", "title", "hspace", "vspace", "border" } },

            { "table", new string[]      { "style", "class" } },
            { "thead", new string[]      { "style", "class" } },
            { "tbody", new string[]      { "style", "class" } },
            { "tfoot", new string[]      { "style", "class" } },
            { "th", new string[]         { "style", "class", "scope" } },
            { "tr", new string[]         { "style", "class" } },
            { "td", new string[]         { "style", "class", "colspan" } },

            { "q", new string[]          { "style", "class", "cite" } },
            { "cite", new string[]       { "style", "class" } },
            { "abbr", new string[]       { "style", "class" } },
            { "acronym", new string[]    { "style", "class" } },
            { "del", new string[]        { "style", "class" } },
            { "ins", new string[]        { "style", "class" } }
        };

        /// <summary>
        /// Takes raw HTML input and cleans against a whitelist
        /// </summary>
        /// <param name="source">Html source</param>
        /// <returns>Clean output</returns>
        public string SanitizeHtml(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }

            HtmlDocument html = GetHtml(source);
            if (html == null) { return string.Empty; }

            HtmlNode allNodes = html.DocumentNode;
            CleanNodes(allNodes, new HashSet<string>(ValidHtmlTags.Keys));

            foreach (KeyValuePair<string, string[]> tag in ValidHtmlTags)
            {
                IEnumerable<HtmlNode> nodes = allNodes.DescendantsAndSelf().Where(n => n.Name == tag.Key);

                // No nodes? Skip.
                if (nodes == null) { continue; }

                foreach (HtmlNode node in nodes)
                {
                    if (!node.HasAttributes) { continue; }

                    foreach (HtmlAttribute a in node.Attributes)
                    {
                        if (!tag.Value.Contains(a.Name))
                        {
                            a.Remove();
                        }
                        else if (a.Name == "href" || a.Name == "src")
                        {
                            a.Value = (!string.IsNullOrEmpty(a.Value)) ? a.Value.Replace("\r", "").Replace("\n", "") : "";
                            a.Value = (!string.IsNullOrEmpty(a.Value) && (a.Value.IndexOf("javascript") < 10 || a.Value.IndexOf("eval") < 10))
                                    ? a.Value.Replace("javascript", "").Replace("eval", "")
                                    : a.Value;
                        }
                        else if (a.Name == "class" || a.Name == "style")
                        {
                            a.Value = Microsoft.Security.Application.Encoder.CssEncode(a.Value);
                        }
                        else
                        {
                            a.Value = Microsoft.Security.Application.Encoder.HtmlAttributeEncode(a.Value);
                        }
                    }
                }
            }

            return allNodes.InnerHtml;
        }

        /// <summary>
        /// Takes a raw source and removes all HTML tags
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public string StripHtml(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }

            source = SanitizeHtml(source);

            HtmlDocument html = GetHtml(source);
            var result = new StringBuilder();

            foreach (HtmlNode node in html.DocumentNode.ChildNodes)
            {
                result.Append(node.InnerText);
            }

            return result.ToString();
        }

        /// <summary>
        /// Recursively delete nodes not in the whitelist
        /// </summary>
        private static void CleanNodes(HtmlNode node, HashSet<string> whitelist)
        {
            if (node.NodeType == HtmlNodeType.Element && !whitelist.Contains(node.Name))
            {
                node.ParentNode.RemoveChild(node);
                return;
            }

            if (node.HasChildNodes)
            {
                CleanChildren(node, whitelist);
            }
        }

        /// <summary>
        /// Apply CleanNodes to each of the child nodes
        /// </summary>
        private static void CleanChildren(HtmlNode parent, HashSet<string> whitelist)
        {
            for (int i = parent.ChildNodes.Count - 1; i >= 0; i--)
            {
                CleanNodes(parent.ChildNodes[i], whitelist);
            }
        }

        /// <summary>
        /// Helper function that returns an HTML document from text
        /// </summary>
        private static HtmlDocument GetHtml(string source)
        {
            var html = new HtmlDocument { OptionFixNestedTags = true, OptionAutoCloseOnEnd = true, OptionDefaultStreamEncoding = Encoding.UTF8 };

            html.LoadHtml(source);

            // Encode any code blocks independently so they won't
            // be stripped out completely when we do a final cleanup
            foreach (HtmlNode node in html.DocumentNode.DescendantsAndSelf())
            {
                if (node.Name == "code")
                {
                    //** Code tag attribute vulnerability fix 28-9-12 (thanks to Natd)
                    var attributesToRemove = node.Attributes.Where(a => a.Name != "style" && a.Name != "class");
                    foreach (var attribute in attributesToRemove)
                    {
                        attribute.Remove();
                    }
                    node.InnerHtml = Microsoft.Security.Application.Encoder.HtmlEncode(node.InnerHtml);
                }
            }

            return html;
        }
    }
}