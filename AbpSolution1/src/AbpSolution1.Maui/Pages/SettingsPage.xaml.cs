using AbpSolution1.Maui.ViewModels;
using Volo.Abp.DependencyInjection;

namespace AbpSolution1.Maui.Pages;

public partial class SettingsPage : ContentPage, ITransientDependency
{
	public SettingsPage(SettingsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}