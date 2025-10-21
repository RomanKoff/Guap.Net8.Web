using Ans.Net8.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;

namespace Guap.Net8.Web.TagHelpers
{

	/*
	 * <a href-guap-doc="user@host.ru"></a>
	 */



	[HtmlTargetElement("a", Attributes = Href_Guap_Doc_AttributeName)]
	public partial class Exts_ATagHelper(
		IConfiguration config,
		IHtmlGenerator generator,
		CurrentContext current)
		: AnchorTagHelper(generator)
	{

		private const string Href_Guap_Doc_AttributeName = "href-guap-doc";

		private readonly LibOptions _options = config.GetOptions_GuapNet8Web();
		private readonly CurrentContext _current = current;


		/* properties */


		[HtmlAttributeName(Href_Guap_Doc_AttributeName)]
		public string HrefGuapDocData { get; set; }


		/* methods */


		public override void Process(
			TagHelperContext context,
			TagHelperOutput output)
		{
			base.Process(context, output);
			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;

			if (!string.IsNullOrEmpty(HrefGuapDocData))
				_makeGuapDoc(output, HrefGuapDocData);
		}


		/* privates */


		private void _makeGuapDoc(
			TagHelperOutput output,
			string doc)
		{
			output.Attributes.SetAttribute("href", new HtmlString($"{_options.HostDocs}/{doc}"));
			var s1 = output.GetChildContent();
			output.AppendHtml(string.IsNullOrEmpty(s1) ? doc : s1);
		}

	}

}
