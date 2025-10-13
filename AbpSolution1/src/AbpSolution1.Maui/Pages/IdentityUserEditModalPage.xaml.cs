using AbpSolution1.Maui.ViewModels;
using Volo.Abp.DependencyInjection;

namespace AbpSolution1.Maui.Pages;

public partial class IdentityUserEditModalPage : ContentPage, ITransientDependency
{
	public IdentityUserEditModalPage(IdentityUserEditViewModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
}