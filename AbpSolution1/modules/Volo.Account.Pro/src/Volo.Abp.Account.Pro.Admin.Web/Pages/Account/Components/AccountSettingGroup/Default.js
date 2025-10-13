(function ($) {
    $(function () {
        var l = abp.localization.getResource("AbpAccount");

        var currentTab = $(window.location.hash + "-tab");
        if (currentTab.length) {
            (new bootstrap.Tab(currentTab)).show()
            location.hash = "";
        }

        $("#AccountSettingsForm").on("submit", function (event) {
            event.preventDefault();
            var form = $(this).serializeFormToObject();

            volo.abp.account.accountSettings.update(form).then(function (result) {
                $(document).trigger("AbpSettingSaved");
            });
        });

        $("#AccountTwoFactorSettingsForm").on("submit", function (event) {
            event.preventDefault();
            var form = $(this).serializeFormToObject();

            volo.abp.account.accountSettings
                .updateTwoFactor(form)
                .then(function (result) {
                    $(document).trigger("AbpSettingSaved");
                });
        });

        $("#AccountCaptchaSettingsForm").on('submit', function (event) {
            event.preventDefault();
            var form = $(this).serializeFormToObject();
            volo.abp.account.accountSettings
                .updateRecaptcha(form)
                .then(function (result) {
                    $(document).trigger("AbpSettingSaved");
                });
        });

        $("#AccountTwoFactorSettings_TwoFactorBehaviour").change(function () {
            if (this.value !== "0") {
                $("#AccountTwoFactorSettings_UsersCanChange").parent().hide();
            } else {
                $("#AccountTwoFactorSettings_UsersCanChange").parent().show();
            }
        }).change();

        $("#AccountExternalProviderSettingsForm input[type='checkbox'][data-collapse]").change(function () {
            if (!$(this).prop("checked")) {
                $("#" + $(this).data("collapse")).collapse("hide");
            } else {
                $("#" + $(this).data("collapse")).collapse("show");
            }
        });

        $("#AccountExternalProviderSettingsForm input[type='checkbox'][data-tenant-collapse]").change(function () {
            $("#" + $(this).data("tenant-collapse") + " input").val("");
            if (!$(this).prop("checked")) {
                $("#" + $(this).data("tenant-collapse")).collapse("hide");
            } else {
                $("#" + $(this).data("tenant-collapse")).collapse("show");
            }
        });

        $("#AccountExternalProviderSettingsForm").on("submit", function (event) {
            event.preventDefault();
            var form = {
                "verifyPasswordDuringExternalLogin": $("#AccountExternalProviderSettings_VerifyPasswordDuringExternalLogin").is(':checked'),
                "externalProviders": []
            };

            $(".provider_container").each(function () {
                var obj = $(this).find("input").serializeFormToObject(false);
                form.externalProviders.push({
                    "name": obj.Name,
                    "enabled": obj.Enabled,
                    "enabledForTenantUser": obj.EnabledForTenantUser,
                    "useCustomSettings": abp.currentTenant.isAvailable ? obj.UseCustomSettings : true,
                    "properties": obj.Properties ? Object.keys(obj.Properties).map(e => ({ "name": e, "value": obj.Properties[e] })) : [],
                    "secretProperties": obj.SecretProperties ? Object.keys(obj.SecretProperties).map(e => ({ "name": e, "value": obj.SecretProperties[e] })) : [],
                });
            });
            volo.abp.account.accountSettings
                .updateExternalProvider(form)
                .then(function (result) {
                    $(document).trigger("AbpSettingSaved");
                });
        });

        $("#AccountIdleSettingsDto_Enabled").change(function () {
            if ($(this).is(':checked')) {
                $("#IdleSessionTimeoutSetting").show();
            } else {
                $("#IdleSessionTimeoutSetting").hide();
            }
        });

        $("#AccountIdleSettingsDto_IdleTimeoutMinutes").change(function () {
            if (this.value !== "0") {
                $("#CustomIdleTimeoutMinutes").hide().val("1");
            } else {
                $("#CustomIdleTimeoutMinutes").show();
            }
        });

        $("#AccountIdleSettingsForm").on("submit", function (event) {
            event.preventDefault();
            var form = {
                "Enabled": $("#AccountIdleSettingsDto_Enabled").is(':checked'),
                "IdleTimeoutMinutes": $("#AccountIdleSettingsDto_IdleTimeoutMinutes").val(),
            };

            if (form.IdleTimeoutMinutes === "0") {
                form.IdleTimeoutMinutes = $("#CustomIdleTimeoutMinutes").val();
            }

            var idleTimeoutMinutes = parseInt(form.IdleTimeoutMinutes);
            if (isNaN(idleTimeoutMinutes) || idleTimeoutMinutes <= 0) {
               abp.notify.warn(l("IdleTimeoutMinutesMustBeGreaterThanZero"));
               $("#CustomIdleTimeoutMinutes").val("1");
               return;
            }

            form.IdleTimeoutMinutes = idleTimeoutMinutes;

            volo.abp.account.accountSettings
                .updateIdle(form)
                .then(function (result) {
                    $(document).trigger("AbpSettingSaved");
                    setTimeout(function () {
                        location.hash = "#IdleSessionTimeoutTab";
                        location.reload();
                    }, 300);
                });
        });

        var accountSettingPage = {

            $scoreFormGroup : $('input[name=Score]').parent(),

            $versionSelect : $('select[name=Version]'),

            onCaptchaVersionChange : function(){
                accountSettingPage.$versionSelect.change(()=>{
                    accountSettingPage.showOrHideScoreFormGroup();
                })
            },

            showOrHideScoreFormGroup : function(){
                var version = accountSettingPage.$versionSelect.val();

                if (version === '3') {
                    accountSettingPage.$scoreFormGroup.show();
                } else {
                    accountSettingPage.$scoreFormGroup.hide();
                }
            },

            init : function(){
                accountSettingPage.showOrHideScoreFormGroup();
                accountSettingPage.onCaptchaVersionChange();
            }
        };

        accountSettingPage.init();

    });
})(jQuery);
