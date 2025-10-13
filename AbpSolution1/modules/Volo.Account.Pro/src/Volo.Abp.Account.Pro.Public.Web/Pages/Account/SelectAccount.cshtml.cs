using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Identity;

namespace Volo.Abp.Account.Public.Web.Pages.Account;

public class SelectAccountModel : AccountPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string RedirectUri { get; set; }

    public CurrentUserInfo UserInfo { get; set; }

    public virtual Task<IActionResult> OnGetAsync()
    {
        if (!CurrentUser.IsAuthenticated)
        {
            return Task.FromResult(RedirectTo(GetLoginUrl(RedirectUri)));
        }

        SetCurrentUserInfo();

        return Task.FromResult<IActionResult>(Page());
    }

    public virtual async Task<IActionResult> OnPostAsync(string action)
    {
        switch (action)
        {
            case "continue":
                return RedirectTo(RedirectUri);
            case "login":
                await SignOutAsync();
                return RedirectTo(GetLoginUrl(RedirectUri));
            case "register":
                await SignOutAsync();
                return RedirectTo(GetRegisterUrl(RedirectUri));
        }

        SetCurrentUserInfo();

        return Page();
    }

    protected virtual void SetCurrentUserInfo()
    {
        UserInfo = new CurrentUserInfo
        {
            UserName = CurrentUser.UserName,
            Email = CurrentUser.Email,
            Avatar = Url.Content($"~/api/account/profile-picture-file/{CurrentUser.Id}")
        };
    }

    protected virtual async Task SignOutAsync()
    {
        if (CurrentUser.IsAuthenticated)
        {
            await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
            {
                Identity = IdentitySecurityLogIdentityConsts.Identity,
                Action = IdentitySecurityLogActionConsts.Logout
            });
        }

        await SignInManager.SignOutAsync();
    }

    protected virtual string GetLoginUrl(string redirectUri)
    {
        return redirectUri.IsNullOrWhiteSpace()
            ? Url.Content("~/Account/Login")
            : Url.Content($"~/Account/Login?ReturnUrl={UrlEncoder.Default.Encode(redirectUri)}");
    }

    protected virtual string GetRegisterUrl(string redirectUri)
    {
        return redirectUri.IsNullOrWhiteSpace()
            ? Url.Content("~/Account/Register")
            : Url.Content($"~/Account/Register?ReturnUrl={UrlEncoder.Default.Encode(redirectUri)}");
    }

    protected virtual IActionResult RedirectTo(string url)
    {
        if (url.IsNullOrWhiteSpace() || !Url.IsLocalUrl(url))
        {
            return RedirectToPage(Url.Content("~/Account/Login"));
        }

        return Redirect(url);
    }

    public class CurrentUserInfo
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Avatar { get; set; }
    }
}
