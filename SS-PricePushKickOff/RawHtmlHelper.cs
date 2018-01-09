using RazorEngine.Templating;
using RazorEngine.Text;

namespace SS_PricePushKickOff
{
    public class RawHtmlHelper
    {
        public IEncodedString Raw(string rawString)
        {
            return new RawString(rawString);
        }
    }

    public abstract class HtmlSupportTemplateBase<T> : TemplateBase<T>
    {
        public HtmlSupportTemplateBase()
        {
            Html = new RawHtmlHelper();
        }

        public RawHtmlHelper Html { get; set; }
    }
}
