using System.Threading.Tasks;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ZeekoBlog.Services;

namespace ZeekoBlog.TagHelpers
{
    [HtmlTargetElement("markdown")]
    public class MarkdownTagHelper : TagHelper
    {
        private readonly MarkdownService _mdService;

        public ModelExpression Content { get; set; }

        public MarkdownTagHelper(MarkdownService mdService)
        {
            _mdService = mdService;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var content = output.Content.IsModified ? output.Content.GetContent() :
                (await output.GetChildContentAsync()).GetContent();
            var html = _mdService.Render(content);
            output.Content.SetHtmlContent(html);
            output.TagName = null;
        }
    }
}
