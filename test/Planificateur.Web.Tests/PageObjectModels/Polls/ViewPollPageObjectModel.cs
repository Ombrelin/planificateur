using Microsoft.Playwright;

namespace Planificateur.Web.Tests.PageObjectModels.Polls;

public class ViewPollPageObjectModel : PageObjectModel
{
    public ViewPollPageObjectModel(IPage page, string baseAppUrl) : base(page, baseAppUrl)
    {
    }

    public override string Path { get; }
}