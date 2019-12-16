// Win-PS2EXE v1.0.0.3
// Front end to Powershell-Script-to-EXE-Compiler PS2EXE.ps1: https://gallery.technet.microsoft.com/PS2EXE-GUI-Convert-e7cb69d5
// Markus Scholtes, 2019
//
// WPF "all in one file" program, no Visual Studio or MSBuild is needed to compile
// Version for .Net 4.x

/* compile with:
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe /target:winexe Win-PS2EXE.cs /r:"%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\WPF\presentationframework.dll" /r:"%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\WPF\windowsbase.dll" /r:"%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\WPF\presentationcore.dll" /r:"%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\System.Xaml.dll" /win32icon:MScholtes.ico
*/

using System;
using System.Xml;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

// set attributes
using System.Reflection;
[assembly:AssemblyTitle("Graphical front end to Invoke-PS2EXE")]
[assembly:AssemblyDescription("Graphical front end to Invoke-PS2EXE")]
[assembly:AssemblyConfiguration("")]
[assembly:AssemblyCompany("MS")]
[assembly:AssemblyProduct("Win-PS2EXE")]
[assembly:AssemblyCopyright("© Markus Scholtes 2019")]
[assembly:AssemblyTrademark("")]
[assembly:AssemblyCulture("")]
[assembly:AssemblyVersion("1.0.0.3")]
[assembly:AssemblyFileVersion("1.0.0.3")]

namespace WPFApplication
{
	public class CustomWindow : Window
	{
		// create window object out of XAML string
		public static CustomWindow LoadWindowFromXaml(string xamlString)
		{ // Get the XAML content from a string.
			// prepare XML document
			XmlDocument XAML = new XmlDocument();
			// read XAML string
			XAML.LoadXml(xamlString);
			// and convert to XML
			XmlNodeReader XMLReader = new XmlNodeReader(XAML);
			// generate WPF object tree
			CustomWindow objWindow = (CustomWindow)XamlReader.Load(XMLReader);

			// return CustomWindow object
			return objWindow;
		}

		// helper function that "climbs up" the parent object chain from a window object until the root window object is reached
		private FrameworkElement FindParentWindow(object sender)
		{
			FrameworkElement GUIControl = (FrameworkElement)sender;
			while ((GUIControl.Parent != null) && (GUIControl.GetType() != typeof(CustomWindow)))
			{
				GUIControl = (FrameworkElement)GUIControl.Parent;
			}

			if (GUIControl.GetType() == typeof(CustomWindow))
				return GUIControl;
			else
				return null;
		}

		// event handlers

		// left mouse click
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			// event is handled afterwards
			e.Handled = true;

			// retrieve window parent object
			Window objWindow = (Window)FindParentWindow(sender);
			// if not found then end
			if (objWindow == null) { return; }

			if (((Button)sender).Name == "Cancel")
			{	// button "Cancel" -> close window
				objWindow.Close();
			}
			else
			{	// button "Compile" -> call PS2EXE
				// read content of TextBox control
				TextBox objSourceFile = (TextBox)objWindow.FindName("SourceFile");
				if (objSourceFile.Text == "")
				{
					MessageBox.Show("No source file specified", "Compile", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				string arguments = "-NoProfile -NoLogo -EP Bypass -Command \"Invoke-ps2exe -inputFile '" + objSourceFile.Text + "'";

				// read content of TextBox control
				TextBox objTargetFile = (TextBox)objWindow.FindName("TargetFile");
				if (objTargetFile.Text != "")
				{
					arguments += " -outputFile '" + objTargetFile.Text + "'";
				}

				// read content of TextBox control
				TextBox objIconFile = (TextBox)objWindow.FindName("IconFile");
				if (objIconFile.Text != "")
				{
					arguments += " -iconFile '" + objIconFile.Text + "'";
				}

				// read content of TextBox control
				TextBox objFileVersion = (TextBox)objWindow.FindName("FileVersion");
				if (objFileVersion.Text != "")
				{
					arguments += " -version '" + objFileVersion.Text + "'";
				}

				// read content of TextBox control
				TextBox objFileDescription = (TextBox)objWindow.FindName("FileDescription");
				if (objFileDescription.Text != "")
				{
					arguments += " -title '" + objFileDescription.Text + "'";
				}

				// read content of TextBox control
				TextBox objProductName = (TextBox)objWindow.FindName("ProductName");
				if (objProductName.Text != "")
				{
					arguments += " -product '" + objProductName.Text + "'";
				}

				// read content of TextBox control
				TextBox objCopyright = (TextBox)objWindow.FindName("Copyright");
				if (objCopyright.Text != "")
				{
					arguments += " -copyright '" + objCopyright.Text + "'";
				}

				// read state of CheckBox control
				CheckBox objCheckBox = (CheckBox)objWindow.FindName("noConsole");
				if (objCheckBox.IsChecked.Value)
				{
					arguments += " -noConsole";
				}

				// read state of CheckBox control
				CheckBox objCheckBox2 = (CheckBox)objWindow.FindName("noOutput");
				if (objCheckBox2.IsChecked.Value)
				{
					arguments += " -noOutput";
				}

				// read state of CheckBox control
				CheckBox objCheckBox3 = (CheckBox)objWindow.FindName("noError");
				if (objCheckBox3.IsChecked.Value)
				{
					arguments += " -noError";
				}

				// read state of CheckBox control
				CheckBox objCheckBox4 = (CheckBox)objWindow.FindName("requireAdmin");
				if (objCheckBox4.IsChecked.Value)
				{
					arguments += " -requireAdmin";
				}

				// read state of CheckBox control
				CheckBox objCheckBox5 = (CheckBox)objWindow.FindName("configFile");
				if (objCheckBox5.IsChecked.Value)
				{
					arguments += " -configFile";
				}

				// read state of RadioButton control
				RadioButton objRadioButton = (RadioButton)objWindow.FindName("STA");
				if (objRadioButton.IsChecked.Value)
				{
					arguments += " -STA";
				}
				else
				{
					arguments += " -MTA";
				}

				// read content of ComboBox control
				ComboBox objComboBox = (ComboBox)objWindow.FindName("Platform");
				ComboBoxItem objComboBoxItem = (ComboBoxItem)objComboBox.SelectedItem;
				string selectedItem = objComboBoxItem.Content.ToString();
				if (selectedItem != "AnyCPU")
				{
					if (selectedItem == "x64")
					{
						arguments += " -x64";
					}
					else
					{
						arguments += " -x86";
					}
				}

				// create powershell process with ps2exe command line
				ProcessStartInfo psi = new ProcessStartInfo("powershell.exe", arguments + " -verbose; Read-Host \\\"`nPress Enter to leave\\\"\"");
				// working directory is the directory of the source file
				psi.WorkingDirectory = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(objSourceFile.Text));
				psi.UseShellExecute = false;

				try
				{ // start process
					Process.Start(psi);
				}
				catch (System.ComponentModel.Win32Exception ex)
				{ // error
					MessageBox.Show("Error " + ex.NativeErrorCode + " starting the process\r\n" + ex.Message + "\r\n", "Compile", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				catch (System.InvalidOperationException ex)
				{ // error
					MessageBox.Show("Error starting the process\r\n" + ex.Message + "\r\n", "Compile", MessageBoxButton.OK, MessageBoxImage.Error);
				}

			}
		}

		// mouse moves into button area
		private void Button_MouseEnter(object sender, MouseEventArgs e)
		{
			// retrieve window parent object
			Window objWindow = (Window)FindParentWindow(sender);
			// if found change mouse form
			if (objWindow != null) { objWindow.Cursor = System.Windows.Input.Cursors.Hand; }
		}

		// mouse moves out of button area
		private void Button_MouseLeave(object sender, MouseEventArgs e)
		{
			// retrieve window parent object
			Window objWindow = (Window)FindParentWindow(sender);
			// if found change mouse form
			if (objWindow != null) { objWindow.Cursor = System.Windows.Input.Cursors.Arrow; }
		}

		// click on file picker button ("...")
		private void FilePicker_Click(object sender, RoutedEventArgs e)
		{
			// retrieve window parent object
			Window objWindow = (Window)FindParentWindow(sender);

			// if not found then end
			if (objWindow == null) { return; }

			// create OpenFileDialog control
			Microsoft.Win32.OpenFileDialog objFileDialog = new Microsoft.Win32.OpenFileDialog();

			// set file extension filters
			if (((Button)sender).Name == "SourceFilePicker")
			{	// button to TextBox "SourceFile"
				objFileDialog.DefaultExt = ".ps1";
				objFileDialog.Filter = "PS1 Files (*.ps1)|*.ps1|All Files (*.*)|*.*";
			}
			else
			{	// button to TextBox "IconFile"
				objFileDialog.DefaultExt = ".ico";
				objFileDialog.Filter = "Icon Files (*.ico)|*.ico|All Files (*.*)|*.*";
			}

			// display file picker dialog
			Nullable<bool> result = objFileDialog.ShowDialog();

			// file selected?
			if (result.HasValue && result.Value)
			{ // fill Texbox with file name
				if (((Button)sender).Name == "SourceFilePicker")
				{	// button to TextBox "SourceFile"
					TextBox objSourceFile = (TextBox)objWindow.FindName("SourceFile");
					objSourceFile.Text = objFileDialog.FileName;
				}
				else
				{	// button to TextBox "IconFile"
					TextBox objIconFile = (TextBox)objWindow.FindName("IconFile");
					objIconFile.Text = objFileDialog.FileName;
				}
			}
		}

		// "empty" drag handler
		private void TextBox_PreviewDragOver(object sender, DragEventArgs e)
		{
			e.Effects = DragDropEffects.All;
			e.Handled = true;
		}

		// drop handler: insert filename to textbox
		private void TextBox_PreviewDrop(object sender, DragEventArgs e)
		{
			object objText = e.Data.GetData(DataFormats.FileDrop);
			TextBox objTextBox = sender as TextBox;
			if ((objTextBox != null) && (objText != null))
			{
				objTextBox.Text = string.Format("{0}",((string[])objText)[0]);
			}
		}


	} // end of CustomWindow

	public class Program
	{
		// WPF requires STA model, since C# default to MTA threading, the following directive is mandatory
		[STAThread]
		public static void Main()
		{
			// XAML string defining the window controls
			string strXAML = @"
<local:CustomWindow
	xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
	xmlns:local=""clr-namespace:WPFApplication;assembly=***ASSEMBLY***""
	xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
	x:Name=""Window"" Title=""Win-PS2EXE"" WindowStartupLocation=""CenterScreen""
	Background=""#FFE8E8E8""  Width=""504"" Height=""370"" ShowInTaskbar=""True"">
	<Grid>
		<Grid.ColumnDefinitions>
			<ColumnDefinition Width=""auto"" />
			<ColumnDefinition Width=""auto"" />
			<ColumnDefinition Width=""auto"" />
		</Grid.ColumnDefinitions>
		<Grid.RowDefinitions>
			<RowDefinition Height=""auto"" />
			<RowDefinition Height=""auto"" />
			<RowDefinition Height=""auto"" />
			<RowDefinition Height=""auto"" />
			<RowDefinition Height=""auto"" />
			<RowDefinition Height=""auto"" />
			<RowDefinition Height=""auto"" />
			<RowDefinition Height=""auto"" />
			<RowDefinition Height=""auto"" />
			<RowDefinition Height=""auto"" />
			<RowDefinition Height=""auto"" />
			<RowDefinition Height=""*"" />
		</Grid.RowDefinitions>
		<TextBlock Height=""32"" Margin=""0,10,0,0"" FontSize=""16"" Grid.Row=""0"" Grid.Column=""1"" >Win-PS2EXE: Graphical front end to PS2EXE-GUI</TextBlock>

		<Label Grid.Row=""1"" Grid.Column=""0"">Source file: </Label>
		<TextBox x:Name=""SourceFile"" Height=""18"" Width=""362"" Margin=""0,0,10,0"" AllowDrop=""True"" ToolTip=""Path and name of the source file (the only mandatory field)"" Grid.Row=""1"" Grid.Column=""1""
			PreviewDragEnter=""TextBox_PreviewDragOver"" PreviewDragOver=""TextBox_PreviewDragOver"" PreviewDrop=""TextBox_PreviewDrop"" />
		<Button x:Name=""SourceFilePicker"" Background=""#FFD0D0D0"" Height=""18"" Width=""24"" Content=""..."" ToolTip=""File picker for source file"" Grid.Row=""1"" Grid.Column=""2""
			Click=""FilePicker_Click"" />

		<Label Grid.Row=""2"" Grid.Column=""0"">Target file: </Label>
		<TextBox x:Name=""TargetFile"" Height=""18"" Width=""362"" Margin=""0,0,10,0"" AllowDrop=""True"" ToolTip=""Optional: Name and possibly path of the target file"" Grid.Row=""2"" Grid.Column=""1""
			PreviewDragEnter=""TextBox_PreviewDragOver"" PreviewDragOver=""TextBox_PreviewDragOver"" PreviewDrop=""TextBox_PreviewDrop"" />

		<Label Grid.Row=""3"" Grid.Column=""0"">Icon file: </Label>
		<TextBox x:Name=""IconFile"" Height=""18"" Width=""362"" Margin=""0,0,10,0"" AllowDrop=""True"" ToolTip=""Optional: Name and possibly path of the icon file"" Grid.Row=""3"" Grid.Column=""1""
			PreviewDragEnter=""TextBox_PreviewDragOver"" PreviewDragOver=""TextBox_PreviewDragOver"" PreviewDrop=""TextBox_PreviewDrop"" />
		<Button x:Name=""IconFilePicker"" Background=""#FFD0D0D0"" Height=""18"" Width=""24"" Content=""..."" ToolTip=""File picker for icon file"" Grid.Row=""3"" Grid.Column=""2""
			Click=""FilePicker_Click"" />

		<Label Margin=""0,10,0,0"" Grid.Row=""4"" Grid.Column=""0"">Version:</Label>
		<WrapPanel Margin=""0,10,0,0"" Grid.Row=""4"" Grid.Column=""1"" >
			<TextBox x:Name=""FileVersion"" Height=""18"" Width=""72"" Margin=""0,0,10,0"" ToolTip=""Optional: Version number in format n.n.n.n"" />
			<Label Margin=""30,0,0,0"" >File description: </Label>
			<TextBox x:Name=""FileDescription"" Height=""18"" Width=""156"" ToolTip=""Optional: File description displayed in executable's properties"" />
		</WrapPanel>

		<Label Grid.Row=""5"" Grid.Column=""0"">Product name:</Label>
		<WrapPanel Grid.Row=""5"" Grid.Column=""1"" >
			<TextBox x:Name=""ProductName"" Height=""18"" Width=""100"" Margin=""0,0,10,0"" ToolTip=""Optional: Product name displayed in executable's properties"" />
			<Label Margin=""30,0,0,0"" >Copyright: </Label>
			<TextBox x:Name=""Copyright"" Height=""18"" Width=""156"" ToolTip=""Optional: Copyright displayed in executable's properties"" />
		</WrapPanel>

		<CheckBox x:Name=""noConsole"" IsChecked=""True"" Margin=""0,10,0,0"" ToolTip=""Generate a Windows application instead of a console application"" Grid.Row=""6"" Grid.Column=""1"">Compile a graphic windows program (parameter -noConsole)</CheckBox>

		<WrapPanel Grid.Row=""7"" Grid.Column=""1"" >
			<CheckBox x:Name=""noOutput"" IsChecked=""False"" ToolTip=""Supress any output including verbose and informational output"" >Suppress output (-noOutput)</CheckBox>
			<CheckBox x:Name=""noError"" IsChecked=""False"" Margin=""6,0,0,0"" ToolTip=""Supress any error message including warning and debug output"" >Suppress error output (-noError)</CheckBox>
		</WrapPanel>

		<CheckBox x:Name=""requireAdmin"" IsChecked=""False"" ToolTip=""Request administrative rights (UAC) at runtime if not already present"" Grid.Row=""8"" Grid.Column=""1"">Require administrator rights at runtime (parameter -requireAdmin)</CheckBox>

		<CheckBox x:Name=""configFile"" IsChecked=""False"" ToolTip=""Enable creation of OUTPUTFILE.exe.config"" Grid.Row=""9"" Grid.Column=""1"">Generate config file (parameter -configFile)</CheckBox>

		<WrapPanel Grid.Row=""10"" Grid.Column=""1"" >
			<Label>Thread Apartment State: </Label>
			<RadioButton x:Name=""STA"" VerticalAlignment=""Center"" IsChecked=""True"" GroupName=""ThreadAppartment"" Content=""STA"" ToolTip=""'Single Thread Apartment' mode (recommended)"" />
			<RadioButton x:Name=""MTA"" Margin=""4,0,0,0"" VerticalAlignment=""Center"" IsChecked=""False"" GroupName=""ThreadAppartment"" Content=""MTA"" ToolTip=""'Multi Thread Apartment' mode"" />
			<Label Margin=""6,0,0,0"" >Platform: </Label>
			<ComboBox x:Name=""Platform"" Height=""22"" Margin=""2,0,0,0"" ToolTip=""Designated CPU platform"" >
				<ComboBoxItem IsSelected=""True"">AnyCPU</ComboBoxItem>
				<ComboBoxItem>x64</ComboBoxItem>
				<ComboBoxItem>x86</ComboBoxItem>
			</ComboBox>
		</WrapPanel>

		<WrapPanel Margin=""0,5,0,0"" HorizontalAlignment=""Right"" Grid.Row=""11"" Grid.Column=""1"" >
			<Button x:Name=""Compile"" Background=""#FFD0D0D0"" Height=""22"" Width=""72"" Margin=""10"" Content=""Compile"" ToolTip=""Compile source file to an executable"" IsDefault=""True""
				Click=""Button_Click"" MouseEnter=""Button_MouseEnter"" MouseLeave=""Button_MouseLeave"" />
			<Button x:Name=""Cancel"" Background=""#FFD0D0D0"" Height=""22"" Width=""72"" Margin=""10"" Content=""Cancel"" ToolTip=""End program without action"" IsCancel=""True""
				Click=""Button_Click"" MouseEnter=""Button_MouseEnter"" MouseLeave=""Button_MouseLeave"" />
		</WrapPanel>
	</Grid>
</local:CustomWindow>";

			// generate WPF object tree
			CustomWindow objWindow;
			try
			{	// assign XAML root object
				objWindow = CustomWindow.LoadWindowFromXaml(strXAML.Replace("***ASSEMBLY***", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name));
			}
			catch (Exception ex)
			{ // on error in XAML definition XamlReader sometimes generates an exception
				MessageBox.Show("Error creating the window objects from XAML description\r\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			// and show window
			objWindow.ShowDialog();
		}
	} // end of Program

}  // end of WPFApplication
