using MVVMToolkit;
using System;
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
using MahApps.Metro.Controls;

namespace MVVMToolkit.Views;
public partial class MainWindow : MetroWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }
    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        //Repositioning window after resizing
        //Taken from https://stackoverflow.com/questions/16455931/keep-window-centered-after-sizetocontent-smoothly
        base.OnRenderSizeChanged(sizeInfo);

        if (sizeInfo.HeightChanged)
            this.Top += (sizeInfo.PreviousSize.Height - sizeInfo.NewSize.Height) / 2;

        if (sizeInfo.WidthChanged)
            this.Left += (sizeInfo.PreviousSize.Width - sizeInfo.NewSize.Width) / 2;
    }
}
