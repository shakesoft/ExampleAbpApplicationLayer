using AbpSolution1.Maui.ViewModels;
using Volo.Abp.DependencyInjection;

namespace AbpSolution1.Maui.Pages;

public partial class TenantEditPage : ContentPage, ITransientDependency
{
	public TenantEditPage(TenantEditViewModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
}
