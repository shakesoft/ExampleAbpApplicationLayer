using AbpSolution1.Maui.ViewModels;
using Volo.Abp.DependencyInjection;

namespace AbpSolution1.Maui.Pages;

public partial class IdentityUserPage : ContentPage, ITransientDependency
{
	public IdentityUserPage(IdentityUserPageViewModel vm)
	{
        InitializeComponent();
        BindingContext = vm;
	}
}