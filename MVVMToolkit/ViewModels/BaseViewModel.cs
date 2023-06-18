using CommunityToolkit.Mvvm.ComponentModel;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVMToolkit;

namespace MVVMToolkit.ViewModels;

public abstract partial class BaseViewModel : ObservableRecipient
{
    //Needed for changing currently shown UserControl
}
