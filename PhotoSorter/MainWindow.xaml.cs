/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using PhotoSorter.Models;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace PhotoSorter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // public AnFolder HomeFolder = new AnFolder(@"C:\temp");

        public AnFolder HomeFolder = new AnFolder(@"C:\");
        private AnFolder currentFolder = new AnFolder();
        private AnImage currentImage = null;
        private AnImage lastImage = null;

        private string[] imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

        public MainWindow()
        {
            InitializeComponent();

            //HomeFolder = new AnFolder(@"C:\temp");
            //this.lblCurrentFolder.Content = this.HomeFolder.Path;
            //this.loadFolder(this.HomeFolder);
        }

        private void RefreshUI()
        {
            var source = currentFolder.ImagesByHash.SelectMany(x => x.Value);
            this.lstImages.ItemsSource = this.lstImages.ItemsSource = source;
        }

        private void loadFolder(AnFolder folder)
        {
            var files = Directory.GetFiles(folder.Path);

            foreach (string filePath in files)
            {
                if (imageExtensions.Any(x => filePath.EndsWith(x)))
                {
                    System.Diagnostics.Debug.WriteLine(filePath);

                    var anImage = new AnImage(filePath);

                    if (!folder.ImagesByHash.ContainsKey(anImage.Hash))
                    {
                        folder.ImagesByHash.Add(anImage.Hash, new System.Collections.Generic.List<AnImage>());
                    }

                    folder.ImagesByHash[anImage.Hash].Add(anImage);
                }
            }

            var folders = Directory.GetDirectories(folder.Path);

            foreach (string folderPath in folders)
            {
                folder.ChildFolders.Add(new AnFolder(folderPath));
            }

            this.currentFolder = HomeFolder;
            RefreshUI();
        }

        private void btnSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            // Create a "Save As" dialog for selecting a directory (HACK)
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.InitialDirectory = this.HomeFolder.Path; // Use current value for initial dir
            dialog.Title = "Select a Directory"; // instead of default "Save As"
            dialog.Filter = "Directory|*.this.directory"; // Prevents displaying files
            dialog.FileName = "select"; // Filename will then be "select.this.directory"
            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                // Remove fake filename from resulting path
                path = path.Replace("\\select.this.directory", "");
                path = path.Replace(".this.directory", "");
                // If user has changed the filename, create the new directory
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }

                this.HomeFolder.Path = path;
                this.lblCurrentFolder.Content = path;

                this.loadFolder(this.HomeFolder);
            }
        }

        private void lstImages_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (lstImages.SelectedItem != null)
            {
                this.currentImage = (lstImages.SelectedItem as AnImage);
                this.viewNew.Source = this.currentImage.Image;
                this.lblCurrFiledState.Content = this.currentImage.Sorted
                    ? $"Image has been sorted to {this.currentImage.SortVal}. Press 0-9 to copy again."
                    : "UNFILED. Choose folder for image, 0 through 9.";
                this.lblCurrFiledState.Foreground = this.currentImage.Sorted
                    ? Brushes.Red
                    : Brushes.Black;
            }
        }

        private void _copyImage(AnImage image, int folderNum)
        {
            if (image != null)
            {
                var numAsString = folderNum.ToString();
                var folderDir = Path.Combine(this.HomeFolder.Path, "OpenPhotoSorter", numAsString);
                Directory.CreateDirectory(folderDir);

                // Recall that we've already limited to only dealing with files with image extensions.
                var extensionIndex = image.ImageName.LastIndexOf(".");
                var extension = image.ImageName.Substring(extensionIndex + 1);
                var nameBeforeExtension = image.ImageName.Substring(0, extensionIndex);

                var newLoc = Path.Combine(folderDir, image.ImageName);
                if (File.Exists(newLoc))
                {
                    var timeSuffix = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
                    newLoc = Path.Combine(folderDir, $"{nameBeforeExtension}_{timeSuffix}.{extension}");

                    // Really shouldn't happen often at all. Horn of a rabbit thing.
                    while (File.Exists(newLoc))
                    {
                        nameBeforeExtension += "_";
                        newLoc = Path.Combine(folderDir, $"{nameBeforeExtension}_{timeSuffix}.{extension}");
                    }
                }

                File.Copy(image.Path, newLoc);

                // This optimistic extension matching is, natch, how edge conditions are made.
                // But this is a utility, not a commercial app! EDGY!!!
                var possibleLivePhotoPath = image.Path.Replace(extension, ".mov");
                if (this.chkLivePhoto.IsChecked == true && File.Exists(possibleLivePhotoPath))
                {
                    newLoc = newLoc.Replace(extension, ".mov");
                    File.Copy(possibleLivePhotoPath, newLoc);
                }
                image.SortVal = folderNum;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    System.Diagnostics.Debug.WriteLine("LEFT");
                    if (this.lstImages.SelectedIndex > 0)
                    {
                        this.lstImages.SelectedIndex--;
                    }
                    break;

                case Key.Right:
                    System.Diagnostics.Debug.WriteLine("Right");
                    if (this.lstImages.SelectedIndex < lstImages.Items.Count)
                    {
                        this.lstImages.SelectedIndex++;
                    }
                    break;

                // This kinda argues against switch and for a range, huh?
                case Key.D0:
                case Key.D1:
                case Key.D2:
                case Key.D3:
                case Key.D4:
                case Key.D5:
                case Key.D6:
                case Key.D7:
                case Key.D8:
                case Key.D9:
                    System.Diagnostics.Debug.WriteLine(e.Key);

                    if (this.currentImage != null)
                    {
                        var folderNum = (int)e.Key - 34;
                        _copyImage(this.currentImage, folderNum);

                        this.lastImage = this.currentImage;
                        this.viewOld.Source = this.lastImage.Image;
                        this.lblPrevFolderNum.Content = $"{folderNum}";

                        this.lstImages.SelectedIndex++;
                    }

                    break;
            }
        }
    }
}
