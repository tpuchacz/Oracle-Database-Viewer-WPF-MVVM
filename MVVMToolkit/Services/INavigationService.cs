using MVVMToolkit;
using MVVMToolkit.ViewModels;

namespace MVVMToolkit.Services;

public interface INavigationService
{
    BaseViewModel CurrentView { get; }
    void NavigateTo<T>() where T : BaseViewModel;
}
