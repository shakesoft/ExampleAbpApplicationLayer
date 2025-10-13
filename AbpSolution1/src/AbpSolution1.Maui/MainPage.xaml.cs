using AbpSolution1.Maui.ViewModels;
using Volo.Abp.DependencyInjection;

namespace AbpSolution1.Maui;

public partial class MainPage : ContentPage, ITransientDependency
{
    public MainPage(
		MainPageViewModel vm)
	{
        BindingContext = vm;
        InitializeComponent();
    }
}