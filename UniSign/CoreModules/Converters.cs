﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace UniSign.CoreModules {
	class CertificateToSubjectConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			X509Certificate2 cert = null;
			try {
				cert = (X509Certificate2) value;
			} catch {
				//that's for control to not crash upon ObservableCollection.Clear() method
				return Binding.DoNothing;
			}
			if (cert == null) return "Сертификат поврежден";

			Regex re = new Regex("CN=(.+),");
			string cn = re.Match(cert.Subject).Success
				? re.Match(cert.Subject).Value
				: cert.Subject;
			
			return $"{cn} {cert.NotAfter.ToString("yyyy-MMMM-dd")} <{cert.Thumbprint}>";
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}

	public class EnumToBooleanConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value.Equals(parameter);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value.Equals(true) ? parameter : Binding.DoNothing;
		}
	}

	public class BoolToColorConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if ((bool) value) {
				return Brushes.Red;
			} else {
				return Brushes.Green;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (((Brush) value).Equals(Brushes.Green)) {
				return false;
			}
			return true;
		}
	}

}
