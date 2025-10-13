using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Security.Encryption;

namespace Volo.Abp.Account.ExternalProviders;

public class AccountExternalProviderAppService : ApplicationService, IAccountExternalProviderAppService
{
    protected ExternalProviderSettingsHelper ExternalProviderSettingsHelper { get; }
    protected IStringEncryptionService StringEncryptionService { get; }

    public AccountExternalProviderAppService(ExternalProviderSettingsHelper externalProviderSettingsHelper,
        IStringEncryptionService stringEncryptionService)
    {
        ExternalProviderSettingsHelper = externalProviderSettingsHelper;
        StringEncryptionService = stringEncryptionService;
    }

    public virtual async Task<ExternalProviderDto> GetAllAsync()
    {
        var settings = await ExternalProviderSettingsHelper.GetAllAsync();

        return new ExternalProviderDto
        {
            Providers = ObjectMapper.Map<List<ExternalProviderSettings>, List<ExternalProviderItemDto>>(settings)
        };
    }

    public virtual async Task<ExternalProviderItemWithSecretDto> GetByNameAsync(GetByNameInput input)
    {
        using (CurrentTenant.Change(input.TenantId))
        {
            var setting= await ExternalProviderSettingsHelper.GetByNameAsync(input.Name);
            if (setting == null)
            {
                return new ExternalProviderItemWithSecretDto { Success = false };
            }

            //Encrypt the secret values.
            setting.SecretProperties = setting.SecretProperties
                .Select(secretValue => new ExternalProviderSettingsProperty(secretValue.Name, StringEncryptionService.Encrypt(secretValue.Value)))
                .ToList();

            var dto = ObjectMapper.Map<ExternalProviderSettings, ExternalProviderItemWithSecretDto>(setting);
            dto.Success = true;
            return dto;
        }
    }
}
