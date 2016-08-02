﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
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
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Microsoft.Win32;
using Signer.ViewModel;
using Signer.DataModel;

namespace Signer {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		private MainViewModel _viewModel;
		
		public MainWindow() {
			InitializeComponent();
			_viewModel = new MainViewModel();
		}
		
		private async void MainWindow_OnLoaded(object sender,RoutedEventArgs e) {
			/*
			string encrypted_bin = "4a1d99e520b2f5404f25b9cee0ca8f30";
			byte[] encoded = Encoding.UTF8.GetBytes(encrypted_bin);
			byte[] key = Encoding.UTF8.GetBytes("1223456789012234");
			*/

			MainGrid.DataContext = _viewModel;
			if (!_viewModel.ConfigIsGo) {
				return;
			}

			string[] args = Environment.GetCommandLineArgs();
			//List<X509Certificate2> certs = CertificatesInSelectedStore.Items.Cast<X509Certificate2>().ToList();
			
			HttpResponseMessage serverSessionData = await _viewModel.GetServerSessionData(args[1]);

			if (serverSessionData.IsSuccessStatusCode) {
				_viewModel.MessageIsError = false;
				_viewModel.InitSession(await serverSessionData.Content.ReadAsStringAsync(),args[1]);
			} else {
				//means server returned not OK or connection timed out
				_viewModel.MessageIsError = true;
				_viewModel.ServerHtmlMessage = await serverSessionData.Content.ReadAsStringAsync();
			}
			//TODO:Interface changes on error
		}

		#region [SIGN]
		private async void SignButton_OnClick(object sender, RoutedEventArgs e) {
			if (SelectedSignatureCert.SelectedItem != null) {
				X509Certificate2 selectedCert = (X509Certificate2) SelectedSignatureCert.SelectedItem;
				HttpResponseMessage serverResponse = await _viewModel.SendDataBackToServer(_viewModel.SignWithSelectedCert(selectedCert));
				if (!serverResponse.IsSuccessStatusCode) {
					_viewModel.MessageIsError = true;
				}

				_viewModel.ServerHtmlMessage = await serverResponse.Content.ReadAsStringAsync();
				//TODO:Interface changes due to error
			} else {
				//means certificate not selected
				MessageBox.Show(
					"Пожалуйста, выберите сертификат подписи!",
					"Не выбран сертификат подписи",
					MessageBoxButton.OK,
					MessageBoxImage.Exclamation
				);
			}
		}
		#endregion

		#region [MAIN MENU]
		private void LoadPrivateConfig_OnClick(object sender, RoutedEventArgs e) {
			OpenFileDialog dlgOpenFile = new OpenFileDialog() {
				CheckFileExists = true,
				Multiselect = false,
				CheckPathExists = true,
				Filter = "Файлы конфигурации(*.CBIN;*.cbin)|*.CBIN;*.cbin"
			};
			dlgOpenFile.ShowDialog();
			_viewModel.SetConfig(dlgOpenFile.FileName);
		}

		private void LoadCertificate_OnClick(object sender, RoutedEventArgs e) {
			OpenFileDialog dlgOpenFile = new OpenFileDialog() {
				CheckFileExists = true,
				Multiselect = false,
				CheckPathExists = true,
				Filter = "Файлы сертификатов(*.CER;*.cer)|*.CER;*.cer"
			};
			dlgOpenFile.ShowDialog();
			_viewModel.SetCertificate(dlgOpenFile.FileName);
		}
		private void ProgramExit_OnClick(object sender, RoutedEventArgs e) {
			Close();
		}
		#endregion
	}
}
