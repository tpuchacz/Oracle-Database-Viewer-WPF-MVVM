using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace MVVMToolkit.Tools;

//Taken from https://stackoverflow.com/questions/8121906/resize-wpf-window-and-contents-depening-on-screen-resolution
//Takes current screen size and converts it based on given ratio (used for sizing windows/usercontrols)

[ValueConversion(typeof(string), typeof(string))]
public class ScreenRatioConverter : MarkupExtension, IValueConverter
{
    private static ScreenRatioConverter _instance;

    public ScreenRatioConverter() { }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    { // do not let the culture default to local to prevent variable outcome re decimal syntax
        double size = System.Convert.ToDouble(value) * System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
        return size.ToString("G0", CultureInfo.InvariantCulture);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    { // read only converter...
        throw new NotImplementedException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return _instance ?? (_instance = new ScreenRatioConverter());
    }
}
