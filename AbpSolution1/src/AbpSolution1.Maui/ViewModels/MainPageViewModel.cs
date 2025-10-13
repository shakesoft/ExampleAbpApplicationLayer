using CommunityToolkit.Mvvm.Input;
using Volo.Abp.DependencyInjection;
using AbpSolution1.Maui.Oidc;

namespace AbpSolution1.Maui.ViewModels;

public partial class MainPageViewModel : AbpSolution1ViewModelBase, ITransientDependency
{
    protected ILoginService LoginService { get; }
    
    public MainPageViewModel(ILoginService loginService)
    {
        LoginService = loginService;
    }

    [RelayCommand]
    async Task SeeAllUsers()
    {
        await Shell.Current.GoToAsync("///users");
    }
    
    [RelayCommand]
    async Task Login()
    {
        await Shell.Current.GoToAsync("///login");
    }
}
