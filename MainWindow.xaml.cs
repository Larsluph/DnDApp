using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DnDApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string? _smartCopySourceDir = null;
        public string? SmartCopySourceDirectory
        {
            set
            {
                if (value is null)
                {
                    smartCopyToggle.IsChecked = false;
                    smartCopySource.IsEnabled = false;
                    _smartCopySourceDir = value;
                }
                else if (Directory.Exists(value))
                {
                    smartCopyToggle.IsChecked = true;
                    smartCopySource.IsEnabled = true;
                    _smartCopySourceDir = value;
                }
                UpdateTitle();
            }
        }

        private string _targetDir;
        public string TargetedDirectory
        {
            set {
                if (Directory.Exists(value))
                {
                    _targetDir = value;
                }
                UpdateTitle();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            _targetDir = NS_KnownFolders.KnownFolders.DownloadFolder;
            UpdateTitle();
        }

        private void DragOverHandler(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ?
                e.KeyStates.HasFlag(DragDropKeyStates.ControlKey) ?
                    DragDropEffects.Copy
                    : DragDropEffects.Move
                : DragDropEffects.None;
        }

        // An item just got dropped in the frame
        private void DropHandler(object sender, DragEventArgs evt)
        {
            // get files from drop payload as FileDrop format
            var payload = evt.Data.GetData(DataFormats.FileDrop);

            // if payload isn't null, get selected paths as string array
            if (payload is string[] paths)
            {
                foreach (string path in paths)
                {
                    string dest;
                    try
                    {
                        // if smart copy is enabled
                        if (_smartCopySourceDir is not null)
                        {
                            // validate input file in source dir
                            if (path.StartsWith(_smartCopySourceDir))
                            {
                                string relativePath = path[_smartCopySourceDir.Length..];
                                dest = Path.Join(_targetDir, relativePath);
                            }
                            else
                            {
                                // Warn user that smartCopy can't occur
                                MessageBox.Show("SmartCopy is enabled! You can't move files outside source folder when SmartCopy is enabled.",
                                                "Invalid Operation",
                                                MessageBoxButton.OK,
                                                MessageBoxImage.Error);
                                return;
                            }
                        }
                        else
                        {
                            
                            if (Directory.Exists(path))
                            {
                                // Move folder
                                dest = Path.Combine(_targetDir, path.Split(Path.DirectorySeparatorChar).Last());
                            }
                            else if (File.Exists(path))
                            {
                                // Move file
                                dest = Path.Combine(_targetDir, Path.GetFileName(path));
                            }
                            else
                            {
                                // ???
                                throw new InvalidOperationException();
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.ToString());
                        continue;
                    }

                    try
                    {
                        HandlePathOperation(path, dest, evt.KeyStates);
                    } catch (Exception e)
                    {
                        MessageBox.Show(e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void HandlePathOperation(string path, string dest, DragDropKeyStates states)
        {
            bool isCtrlPressed = states.HasFlag(DragDropKeyStates.ControlKey);
            //bool isShiftPressed = states.HasFlag(DragDropKeyStates.ShiftKey);
            bool isAltPressed = states.HasFlag(DragDropKeyStates.AltKey);

            if (Directory.Exists(path))
            {
                // Move folder
                if (isAltPressed) TargetedDirectory = path;
                //TODO: Handle copy
                else Directory.Move(path, dest);
            }
            else if (File.Exists(path))
            {
                // Move file
                if (_smartCopySourceDir is not null)
                {
                    string? destDirName = Path.GetDirectoryName(dest);
                    if (destDirName is not null) Directory.CreateDirectory(destDirName);
                }

                if (isCtrlPressed) File.Copy(path, dest, true);
                else File.Move(path, dest, true);
            }
            else
            {
                // ???
                throw new InvalidOperationException();
            }
        }

        private void SelectTarget_Click(object sender, RoutedEventArgs e)
        {
            var result = OpenFolderPickerDialog("Select new DnD target", _targetDir);

            if (result is not null)
            {
                TargetedDirectory = result;
            }
        }

        private void Topmost_Click(object sender, RoutedEventArgs e)
        {
            root.Topmost = !root.Topmost;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ToggleSmartCopy_Click(object sender, RoutedEventArgs e)
        {
            if (_smartCopySourceDir is null)
            {
                var result = OpenFolderPickerDialog("Select new smart copy source", _targetDir);
                if (result is not null)
                {
                    SmartCopySourceDirectory = result;
                    return;
                }
            }

            SmartCopySourceDirectory = null;
        }

        private void SelectSmartCopySource_Click(object sender, RoutedEventArgs e)
        {
            var result = OpenFolderPickerDialog("Select new smart copy source", _smartCopySourceDir);
            if (result is not null)
            {
                SmartCopySourceDirectory = result;
            }
        }

        private void UpdateTitle()
        {
            if (_smartCopySourceDir is not null)
                root.Title = "DnDApp - " + _smartCopySourceDir + " -> " + _targetDir;
            else
                root.Title = "DnDApp - " + _targetDir;
        }

        public static string? OpenFolderPickerDialog(string title, string? initialDirectory)
        {
            var dlg = new CommonOpenFileDialog
            {
                Title = title,
                IsFolderPicker = true,
                AddToMostRecentlyUsedList = false,
                AllowNonFileSystemItems = false,
                EnsureFileExists = true,
                EnsurePathExists = true,
                EnsureReadOnly = false,
                EnsureValidNames = true,
                Multiselect = false,
                ShowPlacesList = true
            };

            if (initialDirectory is not null)
            {
                dlg.InitialDirectory = initialDirectory;
                dlg.DefaultDirectory = initialDirectory;
            }

            return (dlg.ShowDialog() == CommonFileDialogResult.Ok) ? dlg.FileName : null;
        }
    }
}
