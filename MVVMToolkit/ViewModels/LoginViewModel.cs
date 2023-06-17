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

    //Adres IP hosta, port oraz SID będzie można wybrać w jakimś menu logowania
    [ObservableProperty]
    private string? _hostname = "155.158.112.45";

    [ObservableProperty]
    private string? _sid = "oltpstud";

    private string? connStr;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ClickCommand))]
    private string? _login = "";

    public SecureString? SecurePassword { get; set; }

    [ObservableProperty]
    private string? _errorMsg;

    [ObservableProperty]
    private string _progressVisibility = "Hidden";

    [RelayCommand(CanExecute = nameof(CanClick))]
    private async void Click()
    {
        if(SecurePassword != null)
        {
            SecurePassword.MakeReadOnly();

            connStr = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)" +
                      $"(Host={ Hostname })(Port=1521)))(CONNECT_DATA=(SERVICE_NAME={ Sid })));";

            OracleCredential cred = new OracleCredential(Login, SecurePassword);

            using(BackgroundWorker bgw = new BackgroundWorker())
            {
                List<object> arguments = new List<object>();
                arguments.Add(connStr);
                arguments.Add(cred);
                bgw.RunWorkerCompleted += Bgw_RunWorkerCompleted;
                bgw.DoWork += Bgw_DoWork;
                bgw.RunWorkerAsync(arguments);
            }
        }
        else
        {
            ErrorMsg = "Podaj hasło";
        }
    }

    private void Bgw_DoWork(object? sender, DoWorkEventArgs e)
    {
        ProgressVisibility = "Visible";
        List<object> list = (List<object>)e.Argument;
        if (_databaseConnectionService.CheckCredentials((string)list.ElementAt(0), (OracleCredential)list.ElementAt(1)))
        {
            IMessenger messenger = Messenger;
            messenger.Send("change"); //Zmiana widoku poprzez wysłanie go do MainWindowViewModel
            SecurePassword.Dispose();
        }
        else
        {
            ErrorMsg = "Logowanie nieudane. Sprawdź dane";
        }
    }

    private void Bgw_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {
        ProgressVisibility = "Hidden";
    }

    //Sprawdzenie czy SecurePassword jest puste jest problematyczne
    //w ten sposób, więc na ten moment sprawdzam tylko Login
    private bool CanClick()
        => !string.IsNullOrWhiteSpace(Login);
}
