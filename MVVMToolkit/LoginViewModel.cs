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

namespace MVVMToolkit;

public record class SuccesfulLogin(string ConnectionString, OracleCredential Credentials);
public partial class LoginViewModel : BaseViewModel, IRecipient<string>
{
    IMessenger messenger;

    [ObservableProperty]
    private SuccesfulLogin sLogin;

    //Adres IP hosta, port oraz SID będzie można wybrać w jakimś menu logowania
    [ObservableProperty]
    private string? _hostname = "155.158.112.45";

    [ObservableProperty]
    private string? _sid = "oltpstud";

    private string? connStr;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ClickCommand))]
    private string? _login = "msbd11";

    public SecureString? SecurePassword { get; set; }

    [ObservableProperty]
    private string? _errorMsg;

    [RelayCommand(CanExecute = nameof(CanClick))]
    private void Click()
    {
        if(SecurePassword != null)
        {
            SecurePassword.MakeReadOnly();

            connStr = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)" +
                      $"(Host={ Hostname })(Port=1521)))(CONNECT_DATA=(SERVICE_NAME={ Sid })));";

            OracleCredential cred = new OracleCredential(Login, SecurePassword);

            SLogin = new SuccesfulLogin(connStr, cred); //SLogin będzie wysyłany do DatabaseViewModel

            using (OracleConnection conn = new OracleConnection(connStr, cred))
            {
                try
                {
                    conn.Open();

                    messenger = Messenger;
                    messenger.Register<string>(this);
                    messenger.Send(new DatabaseViewModel()); //Zmiana widoku poprzez wysłanie go do MainWindowViewModel

                    SecurePassword.Dispose();
                }
                catch (OracleException ex)
                {
                    ErrorMsg = ex.Message;
                }
            }
        }
        else
        {
            ErrorMsg = "Podaj hasło";
        }
    }
    //Sprawdzenie czy SecurePassword jest puste jest problematyczne
    //w ten sposób, więc na ten moment sprawdzam tylko Login
    private bool CanClick()
        => !string.IsNullOrWhiteSpace(Login);

    //Uzyskanie zapytania o wysłanie logowania
    public void Receive(string message)
    {
        if (message.Equals("GetConnection"))
            SendLogin();
    }

    //Wysyłanie danych logowania do DatabaseView
    private void SendLogin()
    {
        messenger = Messenger;
        messenger.Send(new SuccesfulLogin(SLogin.ConnectionString, SLogin.Credentials));
    }
}
