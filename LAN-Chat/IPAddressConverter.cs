using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LAN_Chat
{
    class IPAddressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || (IPAddress)value == IPAddress.None)
                return "";
            return ((IPAddress)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IPAddress address;

            if (IPAddress.TryParse((string)value, out address))
            {
                return address;
            }

            return IPAddress.None;
        }
    }
}
