// Win-PS2EXE v1.0.1.2
// Front end to Powershell-Script-to-EXE-Compiler PS2EXE.ps1: https://github.com/MScholtes/PS2EXE
// Markus Scholtes, 2023
//
// WPF "all in one file" program, no Visual Studio or MSBuild is needed to compile
// Version for .Net 4.x

/* compile with:
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe /target:winexe Win-PS2EXE.cs /r:"%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\WPF\presentationframework.dll" /r:"%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\WPF\windowsbase.dll" /r:"%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\WPF\presentationcore.dll" /r:"%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\System.Xaml.dll" /win32icon:MScholtes.ico
*/

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;

// set attributes
using System.Reflection;
[assembly:AssemblyTitle("Graphical front end to Invoke-PS2EXE")]
[assembly:AssemblyDescription("Graphical front end to Invoke-PS2EXE")]
[assembly:AssemblyConfiguration("")]
[assembly:AssemblyCompany("MS")]
[assembly:AssemblyProduct("Win-PS2EXE")]
[assembly:AssemblyCopyright("© Markus Scholtes 2023")]
[assembly:AssemblyTrademark("")]
[assembly:AssemblyCulture("")]
[assembly:AssemblyVersion("1.0.1.2")]
[assembly:AssemblyFileVersion("1.0.1.2")]

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
					if (System.IO.Directory.Exists(objTargetFile.Text))
					{ // if directory then append source file name
						arguments += " -outputFile '" + System.IO.Path.Combine(objTargetFile.Text, System.IO.Path.GetFileNameWithoutExtension(objSourceFile.Text)) + ".exe'";
					}
					else
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

				// read content of TextBox control
				TextBox objAdditionalParameters = (TextBox)objWindow.FindName("AdditionParameters");
				if (objAdditionalParameters.Text != "")
				{
					arguments += " " + objAdditionalParameters.Text.Replace("\"", "\\\"");
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

			if (((Button)sender).Name != "TargetFilePicker")
			{
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
			else
			{ // use custom dialog for folder selection because there is no WPF folder dialog!!!
				TextBox objTargetFile = (TextBox)objWindow.FindName("TargetFile");

				// create OpenFolderDialog control
				OpenFolderDialog.OpenFolderDialog objOpenFolderDialog = new OpenFolderDialog.OpenFolderDialog();
				if (objTargetFile.Text != "")
				{ // set starting directory for folder picker
					if (System.IO.Directory.Exists(objTargetFile.Text))
						objOpenFolderDialog.InitialFolder = objTargetFile.Text;
					else
						objOpenFolderDialog.InitialFolder = System.IO.Path.GetDirectoryName(objTargetFile.Text);
				}
				else
				{ // no starting directory for folder picker
					objOpenFolderDialog.InitialFolder = "";
				}

				// display folder picker dialog
				System.Windows.Interop.WindowInteropHelper windowHwnd = new System.Windows.Interop.WindowInteropHelper(this);
				Nullable<bool> result = objOpenFolderDialog.ShowDialog(windowHwnd.Handle);

				if ((result.HasValue) && (result == true))
				{ // get result only if a folder was selected
					objTargetFile.Text = objOpenFolderDialog.Folder;
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
	Background=""#FFE8E8E8""  Width=""504"" Height=""392"" ShowInTaskbar=""True"">
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
			<RowDefinition Height=""auto"" />
			<RowDefinition Height=""*"" />
		</Grid.RowDefinitions>
		<TextBlock Height=""32"" Margin=""0,10,0,0"" FontSize=""16"" Grid.Row=""0"" Grid.Column=""1"" >Win-PS2EXE: Graphical front-end to PS2EXE</TextBlock>

		<Label Grid.Row=""1"" Grid.Column=""0"">Source file: </Label>
		<TextBox x:Name=""SourceFile"" Height=""18"" Width=""362"" Margin=""0,0,10,0"" AllowDrop=""True"" ToolTip=""Path and name of the source file (the only mandatory field)"" Grid.Row=""1"" Grid.Column=""1""
			PreviewDragEnter=""TextBox_PreviewDragOver"" PreviewDragOver=""TextBox_PreviewDragOver"" PreviewDrop=""TextBox_PreviewDrop"" />
		<Button x:Name=""SourceFilePicker"" Background=""#FFD0D0D0"" Height=""18"" Width=""24"" Content=""..."" ToolTip=""File picker for source file"" Grid.Row=""1"" Grid.Column=""2""
			Click=""FilePicker_Click"" />

		<Label Grid.Row=""2"" Grid.Column=""0"">Target file: </Label>
		<TextBox x:Name=""TargetFile"" Height=""18"" Width=""362"" Margin=""0,0,10,0"" AllowDrop=""True"" ToolTip=""Optional: Name and possibly path of the target file or target directory"" Grid.Row=""2"" Grid.Column=""1""
			PreviewDragEnter=""TextBox_PreviewDragOver"" PreviewDragOver=""TextBox_PreviewDragOver"" PreviewDrop=""TextBox_PreviewDrop"" />
		<Button x:Name=""TargetFilePicker"" Background=""#FFD0D0D0"" Height=""18"" Width=""24"" Content=""..."" ToolTip=""Directory picker for target directory"" Grid.Row=""2"" Grid.Column=""2""
			Click=""FilePicker_Click"" />

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

		<Label Grid.Row=""11"" Grid.Column=""0"">Parameters:</Label>
		<TextBox x:Name=""AdditionParameters"" Height=""18"" Width=""362"" Margin=""0,0,10,0"" AllowDrop=""False"" ToolTip=""Optional: Additional parameters"" Grid.Row=""11"" Grid.Column=""1"" />

		<WrapPanel Margin=""0,5,0,0"" HorizontalAlignment=""Right"" Grid.Row=""12"" Grid.Column=""1"" >
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


// namespace OpenFolderDialog: Copyright (c) 2011 Josip Medved <jmedved@jmedved.com>  http://www.jmedved.com
// Source: https://www.medo64.com/2011/12/openfolderdialog/
// with some cuts from Markus Scholtes
namespace OpenFolderDialog
{
	internal class OpenFolderDialog : IDisposable
	{
		public string InitialFolder { get; set; }

		public string DefaultFolder { get; set; }

		public string Folder { get; private set; }

		internal Nullable<bool> ShowDialog()
		{
			return ShowDialog(IntPtr.Zero);
		}

		internal Nullable<bool> ShowDialog(IntPtr ownerHandle)
		{
			var frm = (NativeMethods.IFileDialog)(new NativeMethods.FileOpenDialogRCW());
			uint options;
			frm.GetOptions(out options);
			options |= NativeMethods.FOS_PICKFOLDERS | NativeMethods.FOS_FORCEFILESYSTEM | NativeMethods.FOS_NOVALIDATE | NativeMethods.FOS_NOTESTFILECREATE | NativeMethods.FOS_DONTADDTORECENT;
			frm.SetOptions(options);
			if (this.InitialFolder != null)
			{
				NativeMethods.IShellItem directoryShellItem;
				var riid = new Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE"); //IShellItem
				if (NativeMethods.SHCreateItemFromParsingName(this.InitialFolder, IntPtr.Zero, ref riid, out directoryShellItem) == NativeMethods.S_OK)
				{
					frm.SetFolder(directoryShellItem);
				}
			}
			if (this.DefaultFolder != null)
			{
				NativeMethods.IShellItem directoryShellItem;
				var riid = new Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE"); //IShellItem
				if (NativeMethods.SHCreateItemFromParsingName(this.DefaultFolder, IntPtr.Zero, ref riid, out directoryShellItem) == NativeMethods.S_OK)
				{
					frm.SetDefaultFolder(directoryShellItem);
				}
			}

			if (frm.Show(ownerHandle) == NativeMethods.S_OK)
			{
				NativeMethods.IShellItem shellItem;
				if (frm.GetResult(out shellItem) == NativeMethods.S_OK)
				{
					IntPtr pszString;
					if (shellItem.GetDisplayName(NativeMethods.SIGDN_FILESYSPATH, out pszString) == NativeMethods.S_OK)
					{
						if (pszString != IntPtr.Zero)
						{
							try {
								this.Folder = Marshal.PtrToStringAuto(pszString);
								return true;
							}
							finally {
								Marshal.FreeCoTaskMem(pszString);
							}
						}
					}
				}
			}
			return false;
		}

		public void Dispose() { } // just to have the possibility of the using statement
	}

	internal static class NativeMethods
	{
		public const uint FOS_PICKFOLDERS = 0x00000020;
		public const uint FOS_FORCEFILESYSTEM = 0x00000040;
		public const uint FOS_NOVALIDATE = 0x00000100;
		public const uint FOS_NOTESTFILECREATE = 0x00010000;
		public const uint FOS_DONTADDTORECENT = 0x02000000;

		public const uint S_OK = 0x0000;

		public const uint SIGDN_FILESYSPATH = 0x80058000;

		[ComImport, ClassInterface(ClassInterfaceType.None), TypeLibType(TypeLibTypeFlags.FCanCreate), Guid("DC1C5A9C-E88A-4DDE-A5A1-60F82A20AEF7")]
		internal class FileOpenDialogRCW { }

		[ComImport(), Guid("42F85136-DB7E-439C-85F1-E4075D135FC8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface IFileDialog
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[PreserveSig()]
			uint Show([In, Optional] IntPtr hwndOwner); // inherited from IModalWindow

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint SetFileTypes([In] uint cFileTypes, [In, MarshalAs(UnmanagedType.LPArray)] IntPtr rgFilterSpec);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint SetFileTypeIndex([In] uint iFileType);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint GetFileTypeIndex(out uint piFileType);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint Advise([In, MarshalAs(UnmanagedType.Interface)] IntPtr pfde, out uint pdwCookie);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint Unadvise([In] uint dwCookie);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint SetOptions([In] uint fos);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint GetOptions(out uint fos);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			void SetDefaultFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint SetFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, uint fdap);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint Close([MarshalAs(UnmanagedType.Error)] uint hr);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint SetClientGuid([In] ref Guid guid);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint ClearClientData();

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);
		}

		[ComImport, Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface IShellItem
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint BindToHandler([In] IntPtr pbc, [In] ref Guid rbhid, [In] ref Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out IntPtr ppvOut);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint GetDisplayName([In] uint sigdnName, out IntPtr ppszName);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint GetAttributes([In] uint sfgaoMask, out uint psfgaoAttribs);

			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			uint Compare([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, [In] uint hint, out int piOrder);
		}

		[DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern int SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IntPtr pbc, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out IShellItem ppv);
	}
} // end of namespace OpenFolderDialog: Copyright (c) 2011 Josip Medved <jmedved@jmedved.com>  http://www.jmedved.com
