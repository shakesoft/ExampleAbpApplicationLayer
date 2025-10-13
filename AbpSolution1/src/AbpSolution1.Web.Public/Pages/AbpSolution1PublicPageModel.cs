using AbpSolution1.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace AbpSolution1.Web.Public.Pages;

/* Inherit your Page Model classes from this class.
 */
public abstract class AbpSolution1PublicPageModel : AbpPageModel
{
    protected AbpSolution1PublicPageModel()
    {
        LocalizationResourceType = typeof(AbpSolution1Resource);
    }
}
