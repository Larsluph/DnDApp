using Microsoft.WindowsAPICodePack.Dialogs;
using System;
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
        private string _targetDir;
        public string TargetedDirectory
        {
            get { return _targetDir; }
            set {
                _targetDir = value;
                root.Title = "DnDApp - " + _targetDir;
            }
        }

        public static RoutedUICommand toggle_topmost = new("Toggle Topmost", "toggle_topmost", typeof(MainWindow));

        public MainWindow()
        {
            InitializeComponent();
            TargetedDirectory = NS_KnownFolders.KnownFolders.DownloadFolder;
        }

        private void DragOverHandler(object sender, DragEventArgs e)
        {
            e.Effects = (e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Move : DragDropEffects.None);
        }

        // An item just got dropped in the frame
        private void DropHandler(object sender, DragEventArgs e)
        {
            // get files from drop payload as FileDrop format
            var payload = e.Data.GetData(DataFormats.FileDrop);

            // if payload isn't null, get selected paths as string array
            if (payload is string[] paths)
            {
                foreach (string path in paths)
                {
                    try
                    {
                        if (Directory.Exists(path))
                        {
                            // Move folder
                            Directory.Move(path, Path.Combine(TargetedDirectory, path.Split(Path.DirectorySeparatorChar).Last()));
                        }
                        else if (File.Exists(path))
                        {
                            // Move file
                            File.Move(path, Path.Combine(TargetedDirectory, Path.GetFileName(path)), true);
                        }
                        else
                        {
                            // ???
                            throw new InvalidDataException();
                        }
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.ToString());
                    }
                }
            }
        }

        private void SelectTarget_Click(object sender, RoutedEventArgs e)
        {
            

            var dlg = new CommonOpenFileDialog();
            dlg.Title = "My Title";
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = _targetDir;

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.DefaultDirectory = _targetDir;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                TargetedDirectory = dlg.FileName;
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
    }
}
