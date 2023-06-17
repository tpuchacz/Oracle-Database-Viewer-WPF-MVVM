using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using MVVMToolkit.Services;
using MVVMToolkit.ViewModels;
using MVVMToolkit;
using System.Windows.Navigation;

namespace MVVMToolkit.ViewModels;
public partial class MainWindowViewModel : BaseViewModel, IRecipient<string>
{
    private INavigationService _navigation;

    public INavigationService Navigation
    {
        get { return _navigation; }
        set { _navigation = value; }
    }

    public MainWindowViewModel(INavigationService navigationService)
    {
        Navigation = navigationService;
        Navigation.NavigateTo<LoginViewModel>(); //First view shown - LoginView
        IMessenger messenger = Messenger;
        messenger.Register<string>(this); //This message will inform MainWindowWievModel to change the view to DatabaseView
    }

    public void Receive(string message)
    {
        if (message.Equals("change"))
            Navigation.NavigateTo<DatabaseViewModel>();
    }
}
