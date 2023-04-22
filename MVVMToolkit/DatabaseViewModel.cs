using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVMToolkit
{
    public partial class DatabaseViewModel : BaseViewModel, IRecipient<SuccesfulLogin>
    {
        private OracleConnection? conn;

        [ObservableProperty]
        private string? connStr;

        [ObservableProperty]
        private OracleCredential? cred;

        [ObservableProperty]
        private string? _errorMsg;

        public DatabaseViewModel()
        {
            IMessenger messenger = Messenger;
            messenger.Register<SuccesfulLogin>(this);
        }

        //Przyjmujemy SuccesfulLogin od LoginViewModel
        public void Receive(SuccesfulLogin message)
        {
            ConnStr = message.ConnectionString;
            Cred = message.Credentials;
        }

        [ObservableProperty]
        private DataTable? _oracleDBData;


        [RelayCommand]
        private void LoadData()
        {
            IMessenger messenger = Messenger;
            messenger.Send("GetConnection");//Poprzez wysłanie wiadomości do LoginViewModel uzyskuje SuccesfulLogin
            if (string.IsNullOrWhiteSpace(ConnStr) || Cred == null) { ErrorMsg = "Pusto"; }
            else
            {
                using (conn = new OracleConnection(ConnStr, Cred))
                {
                    conn.Open();
                    using (OracleCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM EMPLOYEES";
                        cmd.CommandType = CommandType.Text;
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            OracleDBData = new DataTable();
                            OracleDBData.Load(reader);
                        }
                    }
                }
            }
        }
    }
}
