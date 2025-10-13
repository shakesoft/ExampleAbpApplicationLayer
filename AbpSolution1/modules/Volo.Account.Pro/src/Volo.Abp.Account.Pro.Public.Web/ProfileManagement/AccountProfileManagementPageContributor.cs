﻿using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Volo.Abp.Account.Localization;
using Volo.Abp.Account.Public.Web.Pages.Account.Components.ProfileManagementGroup.Password;
using Volo.Abp.Account.Public.Web.Pages.Account.Components.ProfileManagementGroup.PersonalInfo;
using Volo.Abp.Account.Public.Web.Pages.Account.Components.ProfileManagementGroup.ProfilePicture;
using Volo.Abp.Account.Public.Web.Pages.Account.Components.ProfileManagementGroup.TwoFactor;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Settings;
using Volo.Abp.Settings;
using Volo.Abp.Users;

namespace Volo.Abp.Account.Public.Web.ProfileManagement;

public class AccountProfileManagementPageContributor : IProfileManagementPageContributor
{
    public async Task ConfigureAsync(ProfileManagementPageCreationContext context)
    {
        var l = context.ServiceProvider.GetRequiredService<IStringLocalizer<AccountResource>>();

        context.Groups.Add(
            new ProfileManagementPageGroup(
                "Volo-Abp-Account-PersonalInfo",
                l["ProfileTab:PersonalInfo"],
                typeof(AccountProfilePersonalInfoManagementGroupViewComponent)
            )
        );

        context.Groups.Add(
            new ProfileManagementPageGroup(
                "Volo-Abp-Account-Picture",
                l["ProfileTab:Picture"],
                typeof(AccountProfilePictureManagementGroupViewComponent)
            )
        );

        if (await IsPasswordChangeEnabled(context))
        {
            context.Groups.Add(
                new ProfileManagementPageGroup(
                    "Volo-Abp-Account-Password",
                    l["ProfileTab:Password"],
                    typeof(AccountProfilePasswordManagementGroupViewComponent)
                )
            );
        }

        var identityTwoFactorManager = context.ServiceProvider.GetRequiredService<IdentityProTwoFactorManager>();
        var settingProvider = context.ServiceProvider.GetRequiredService<ISettingProvider>();
        if (!await identityTwoFactorManager.IsForcedDisableAsync() &&
            await settingProvider.GetAsync<bool>(IdentityProSettingNames.TwoFactor.UsersCanChange))
        {
            var profileAppService = context.ServiceProvider.GetRequiredService<IProfileAppService>();
            if (await profileAppService.CanEnableTwoFactorAsync())
            {
                context.Groups.Add(
                    new ProfileManagementPageGroup(
                        "Volo-Abp-Account-TwoFactor",
                        l["ProfileTab:TwoFactor"],
                        typeof(AccountProfileTwoFactorManagementGroupViewComponent)
                    )
                );
            }
        }
    }

    protected virtual async Task<bool> IsPasswordChangeEnabled(ProfileManagementPageCreationContext context)
    {
        var userManager = context.ServiceProvider.GetRequiredService<IdentityUserManager>();
        var currentUser = context.ServiceProvider.GetRequiredService<ICurrentUser>();

        var user = await userManager.GetByIdAsync(currentUser.GetId());

        return !user.IsExternal;
    }
}
