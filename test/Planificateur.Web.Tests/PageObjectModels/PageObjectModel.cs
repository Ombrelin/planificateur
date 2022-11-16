using Microsoft.Playwright;

namespace Planificateur.Web.Tests.PageObjectModels;

public abstract class PageObjectModel
{
    private readonly string baseAppUrl;
    
    protected readonly IPage Page;
    public abstract string Path { get; }
    
    public PageObjectModel(IPage page, string baseAppUrl)
    {
        this.Page = page;
        this.baseAppUrl = baseAppUrl;
    }

    public Task GotoAsync() => Page.GotoAsync($"{Path}{baseAppUrl}");
}