﻿using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

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
            if (e.Data.GetData(DataFormats.FileDrop) is not string[] payload)
            {
                e.Effects = DragDropEffects.None;
                return;
            }

            List<string> paths = payload.ToList();

            var path = paths[0];
            bool isCtrlPressed = e.KeyStates.HasFlag(DragDropKeyStates.ControlKey);
            bool isShiftPressed = e.KeyStates.HasFlag(DragDropKeyStates.ShiftKey);
            bool isAltPressed = e.KeyStates.HasFlag(DragDropKeyStates.AltKey);
            bool isSameDrive = Path.GetPathRoot(path) == Path.GetPathRoot(_targetDir);

            if (isAltPressed && paths.Count == 1 && Directory.Exists(path))
                e.Effects = DragDropEffects.Link;
            else if (isShiftPressed)
                e.Effects = DragDropEffects.Move;
            else if (isCtrlPressed)
                e.Effects = DragDropEffects.Copy;
            else
                if (isSameDrive)
                    e.Effects = DragDropEffects.Move;
                else
                    e.Effects = DragDropEffects.Copy;
        }

        /// <summary>
        /// Triggered when an item just got dropped in the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="InvalidOperationException"></exception>
        private void DropHandler(object sender, DragEventArgs e)
        {
            // get files from drop payload as FileDrop format
            if (e.Data.GetData(DataFormats.FileDrop) is not string[] payload)
            {
                return;
            }

            // if payload isn't null, get selected paths as string list
            List<string> paths = payload.ToList();
            string path = paths[0];
            string dest = GetDestination(path, _targetDir, _smartCopySourceDir);

            bool isCtrlPressed = e.KeyStates.HasFlag(DragDropKeyStates.ControlKey);
            bool isShiftPressed = e.KeyStates.HasFlag(DragDropKeyStates.ShiftKey);
            bool isAltPressed = e.KeyStates.HasFlag(DragDropKeyStates.AltKey);
            bool isSameDrive = Path.GetPathRoot(path) == Path.GetPathRoot(dest);

            if (isAltPressed && paths.Count == 1 && Directory.Exists(path))
                if (isShiftPressed) SmartCopySourceDirectory = path;
                else TargetedDirectory = path;
            else if (isAltPressed)
            {
                string title = "Unable to change target";
                if (paths.Count != 1) MessageBox.Show("New target must be a single folder!", title, MessageBoxButton.OK, MessageBoxImage.Error);
                else if (!Directory.Exists(path)) MessageBox.Show("The new target must be an existing folder!", title, MessageBoxButton.OK, MessageBoxImage.Error);
                else MessageBox.Show("Unexpected Error!", title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (isShiftPressed) NativeFileIO.Move(paths, dest);
            else if (isCtrlPressed) NativeFileIO.Copy(paths, dest);
            else if (isSameDrive) NativeFileIO.Move(paths, dest);
            else NativeFileIO.Copy(paths, dest);
        }

        private void SelectTarget_Click(object sender, RoutedEventArgs e)
        {
            var result = OpenFolderPickerDialog("Select new DnD target", GetParent(_targetDir));

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
                var result = OpenFolderPickerDialog("Select new smart copy source", GetParent(_targetDir));
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
            var initDir = _smartCopySourceDir;
            if (initDir is null) throw new InvalidOperationException();

            var result = OpenFolderPickerDialog("Select new smart copy source", GetParent(initDir));
            if (result is not null)
            {
                SmartCopySourceDirectory = result;
            }
        }

        private void UpdateTitle()
        {
            if (_smartCopySourceDir is not null)
                root.Title = "DnDApp - " + GetDirName(_smartCopySourceDir) + " -> " + GetDirName(_targetDir);
            else
                root.Title = "DnDApp - " + GetDirName(_targetDir);
        }

        public static string GetParent(string path)
        {
            DirectoryInfo? parent = Directory.GetParent(path);
            if (parent is null) return path;
            else return parent.FullName;
        }

        public static string GetDirName(string path)
        {
            return new DirectoryInfo(path).Name;
        }

        public static string GetDestination(string path, string targetDir, string? sourceDir)
        {
            // smart copy is enabled
            if (sourceDir is not null)
            {
                // validate input file in source dir
                if (path.StartsWith(sourceDir))
                {
                    string relativePath = path[sourceDir.Length..];
                    return Path.Join(targetDir, relativePath[..relativePath.LastIndexOf(Path.DirectorySeparatorChar)]);
                }
                else
                {
                    // Warn user that smartCopy can't occur
                    MessageBox.Show("SmartCopy is enabled! You can't move files outside source folder when SmartCopy is enabled.",
                                    "Invalid Operation",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    throw new InvalidOperationException("You can't move files outside source folder when SmartCopy is enabled.");
                }
            }
            else
                return targetDir;
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