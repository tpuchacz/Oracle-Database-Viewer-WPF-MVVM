using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging.Messages;
using MVVMToolkit.Services;
using MVVMToolkit.ViewModels;
using MVVMToolkit;
using Microsoft.Extensions.Hosting;
using System.ComponentModel;

namespace MVVMToolkit.ViewModels;
public partial class LoginViewModel : BaseViewModel
{
    private readonly IDatabaseConnectionService _databaseConnectionService;

    public LoginViewModel(IDatabaseConnectionService databaseConnectionService)
    {
        _databaseConnectionService = databaseConnectionService;
    }

    //Implement being able to change hostname/sid
    [ObservableProperty]
    private string? _hostname = "155.158.112.45";

    [ObservableProperty]
    private string? _sid = "oltpstud";

    private string? connStr;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ClickCommand))]
    private string? _login = "";

    public SecureString? SecurePassword { get; set; } //Password is encrypted in a SecureString, not converted to string
                                                      //Implementation in code-behind
                                                      //https://stackoverflow.com/questions/1483892/how-to-bind-to-a-passwordbox-in-mvvm

    [ObservableProperty]
    private string? _errorMsg;

    [ObservableProperty]
    private string _progressVisibility = "Hidden";

    [RelayCommand(CanExecute = nameof(CanClick))]
    private void Click()
    {
        if(SecurePassword != null)
        {
            SecurePassword.MakeReadOnly();

            connStr = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)" +
                      $"(Host={ Hostname })(Port=1521)))(CONNECT_DATA=(SERVICE_NAME={ Sid })));";

            OracleCredential cred = new OracleCredential(Login, SecurePassword);

            using(BackgroundWorker bgw = new BackgroundWorker())
            {
                List<object> arguments = new List<object>
                {
                    connStr,
                    cred
                };
                bgw.RunWorkerCompleted += Bgw_RunWorkerCompleted;
                bgw.DoWork += Bgw_DoWork;
                bgw.RunWorkerAsync(arguments); //Passing credentials as parameter
            }
        }
        else
        {
            ErrorMsg = "Podaj hasło";
        }
    }

    private bool CanClick()
    => !string.IsNullOrWhiteSpace(Login);

    private void Bgw_DoWork(object? sender, DoWorkEventArgs e)
    {
        ProgressVisibility = "Visible";
        List<object> list = e.Argument as List<object>; //Retrieving credentials from args
        if (_databaseConnectionService.CheckCredentials((string)list.ElementAt(0), (OracleCredential)list.ElementAt(1)))
        {
            IMessenger messenger = Messenger;
            messenger.Send("change"); //Letting MainWindowViewModel know to change view
            if(SecurePassword != null)
                SecurePassword.Dispose();
        }
        else
            ErrorMsg = "Logowanie nieudane. Sprawdź dane";
    }

    private void Bgw_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {
        ProgressVisibility = "Hidden";
    }
}
