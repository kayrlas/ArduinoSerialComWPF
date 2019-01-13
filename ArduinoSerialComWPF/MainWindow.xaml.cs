// copyright Kayrlas(https://github.com/kayrlas)

using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ArduinoSerialComWPF
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			SelPort();
		}
		System.IO.Ports.SerialPort serialPort = null;
		public class ComPort
		{
			public string DeviceID { get; set; }
			public string Description { get; set; }
		}

		/*----------------------------
         * Emurate serial-ports
         *---------------------------*/
		private void SelPort()
		{
			// Get serial-port name
			string[] PortList = SerialPort.GetPortNames();
			var MyList = new ObservableCollection<ComPort>();
			foreach (string p in PortList)
			{
				System.Console.WriteLine(p);
				MyList.Add(new ComPort { DeviceID = p, Description = p });
			}
			cmb.ItemsSource = MyList;
			cmb.SelectedValuePath = "DeviceID";
			cmb.DisplayMemberPath = "Description";
		}

		/*----------------------------
         * When push "connect" button
         *---------------------------*/
		private void Connect_Click(object sender, RoutedEventArgs e)
		{
			// When selected serial-port
			if (cmb.SelectedValue != null)
			{
				// Get serial-port name
				var port = cmb.SelectedValue.ToString();
				// When disconnected
				if (serialPort == null)
				{
					// Setting serial-port
					serialPort = new SerialPort
					{
						PortName = port,
						BaudRate = 9600,
						DataBits = 8,
						Parity = Parity.None,
						StopBits = StopBits.One,
						Encoding = Encoding.UTF8,
						WriteTimeout = 100000
					};

					// When received data
					serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(SerialPort_DataReceived);

					// Try to connect
					try
					{
						// Open serial-port
						serialPort.Open();
						statusBar.Text = "Opening";
						statusBar.Background = Brushes.LimeGreen;
						connect.IsEnabled = false;
						disconnect.IsEnabled = true;
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message);
					}
				}
			}
		}

		/*----------------------------
         * When push "disconnect" button
         *---------------------------*/
		private void Disconnect_Click(object sender, RoutedEventArgs e)
		{
			// Check connected or disconnected
			if (serialPort != null && serialPort.IsOpen == true)
			{
				serialPort.Close();
				statusBar.Text = "Closed";
				statusBar.Background = Brushes.LightGray;
				disconnect.IsEnabled = false;
				connect.IsEnabled = true;
				serialPort = null;
			}
		}

		/*----------------------------
         * When push "send" button
         *---------------------------*/
		private void Send_Click(object sender, RoutedEventArgs e)
		{
			// Check connected or disconnected
			if (serialPort == null) return;
			if (serialPort.IsOpen == false) return;

			// Get text from textbox
			String data = sendText.Text + "\r\n";

			try
			{
				// Send text
				serialPort.Write(data);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		// When received data
		private void SerialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
		{
			// Check connected or disconnected
			if (serialPort == null) return;
			if (serialPort.IsOpen == false) return;

			// Show in the textbox
			try
			{
				receiveText.Dispatcher.Invoke(
					new Action(() =>
					{
						receiveText.Text = serialPort.ReadLine();
					})
				);
			}
			catch
			{
				receiveText.Dispatcher.Invoke(
					new Action(() =>
					{
						receiveText.Text = "!Error! cannot connect" + serialPort.PortName;
					})
				);
			}
		}
	}
}
