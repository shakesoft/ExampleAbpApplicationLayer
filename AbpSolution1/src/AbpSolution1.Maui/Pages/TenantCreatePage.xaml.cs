using AbpSolution1.Maui.ViewModels;
using Volo.Abp.DependencyInjection;

namespace AbpSolution1.Maui.Pages;

public partial class TenantCreatePage : ContentPage, ITransientDependency
{
    public TenantCreatePage(TenantCreateViewModel vm)
    {
        BindingContext = vm;
        InitializeComponent();
    }
}
