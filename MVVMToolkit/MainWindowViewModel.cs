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

namespace MVVMToolkit;
public partial class MainWindowViewModel : ObservableRecipient, IRecipient<DatabaseViewModel>
{
    [ObservableProperty]
	private BaseViewModel _selectedViewModel = new LoginViewModel();

	public MainWindowViewModel()
    {
        IMessenger messenger = Messenger;
        messenger.Register<DatabaseViewModel>(this);
    }
    public void Receive(DatabaseViewModel message)
    {
        SelectedViewModel = message;
    }
}
