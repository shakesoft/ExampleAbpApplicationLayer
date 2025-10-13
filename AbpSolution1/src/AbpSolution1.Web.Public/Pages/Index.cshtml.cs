using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace AbpSolution1.Web.Public.Pages;

public class IndexModel : AbpSolution1PublicPageModel
{
    public void OnGet()
    {

    }

    public async Task OnPostLoginAsync()
    {
        await HttpContext.ChallengeAsync("oidc");
    }
}
