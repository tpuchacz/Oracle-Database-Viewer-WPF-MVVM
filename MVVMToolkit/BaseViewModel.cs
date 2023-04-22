using CommunityToolkit.Mvvm.ComponentModel;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMToolkit
{
    public partial class BaseViewModel : ObservableRecipient
    {
        public BaseViewModel(){}
        public BaseViewModel(SuccesfulLogin loginInfo){}
    }
}
