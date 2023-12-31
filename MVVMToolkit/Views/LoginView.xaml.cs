﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MVVMToolkit;

namespace MVVMToolkit.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
    }


    //Taken from https://stackoverflow.com/questions/1483892/how-to-bind-to-a-passwordbox-in-mvvm

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (this.DataContext != null)
        {
            ((dynamic)this.DataContext).SecurePassword = ((PasswordBox)sender).SecurePassword;
        }
    }
}
