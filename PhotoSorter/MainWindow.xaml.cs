/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using PhotoSorter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoSorter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public AnFolder HomeFolder;

        private AnImage _currentImage = null;
        private AnImage _lastImage = null;
        private BitmapImage _currentImageAsBitmap;

        private Dictionary<string, AnImage> _imagesByHash = new Dictionary<string, AnImage>();
        private Dictionary<string, Queue<AnImage>> _dupedImagesByHash = new Dictionary<string, Queue<AnImage>>();

        public MainWindow()
        {
            InitializeComponent();

            //HomeFolder = new AnFolder(@"C:\temp");
            //this.lblCurrentFolder.Content = this.HomeFolder.Path;
            //this.loadFolder(this.HomeFolder);
        }

        private void RefreshUI()
        {
            IEnumerable<AnImage> source = this.HomeFolder.ImagesByPath.Values;
            this.lstImages.ItemsSource = this.lstImages.ItemsSource = source;
        }

        // Called after a folder is selected.
        private void LoadFolder2(string path)
        {
        }

        private void btnSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            // Create a "Save As" dialog for selecting a directory (HACK)
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.InitialDirectory = this.HomeFolder?.Path ?? @"C:\"; // Use current value for initial dir
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
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                this.lblCurrentFolder.Content = path;

                this.HomeFolder = new AnFolder(path);
                RefreshUI();
            }
        }

        private void lstImages_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (lstImages.SelectedItem != null)
            {
                _currentImage = (lstImages.SelectedItem as AnImage);

                Uri fileUri = new Uri(_currentImage.Path);
                _currentImageAsBitmap = new BitmapImage(fileUri);
                this.viewNew.Source = _currentImageAsBitmap;

                AnImage alredayExists;
                var hash = _currentImage.Hash;

                if (_imagesByHash.TryGetValue(hash, out alredayExists))
                {
                    if (!alredayExists.Path.Equals(_currentImage.Path))
                    {
                        // then we have a duplicated image. If it's our first dupe,
                        // we'll need to initialize the dupe queue for that hash.
                        Queue<AnImage> existingDupes;
                        if (!_dupedImagesByHash.TryGetValue(hash, out existingDupes))
                        {
                            existingDupes = new Queue<AnImage>();
                            existingDupes.Enqueue(alredayExists); // add the first instance that's now duped.
                            _dupedImagesByHash.Add(hash, existingDupes);
                        }

                        existingDupes.Enqueue(_currentImage);

                        _showDupeDialog(
                            alredayExists.Path,
                            _currentImage.Path,
                            existingDupes.Count()-1);

                    }

                    // else it's the same one we've already seen with this hash again.
                    // carry on; nothing to add to the dictionary.
                }
                else
                {
                    _imagesByHash.Add(hash, _currentImage);
                }

                this.lblCurrFiledState.Content = this._currentImage.Sorted
                    ? $"Image has been sorted to {this._currentImage.SortVal}. Press 0-9 to place an additional copy into a folder."
                    : "UNFILED. Choose folder for image, 0 through 9.";
                this.lblCurrFiledState.Foreground = this._currentImage.Sorted
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

        private void _showDupeDialog(string first, string second, int count)
        {
            MessageBoxResult result = MessageBox.Show($@"This file was already seen in another location.

First:
    {first}
Current:
    {second}

Extra copies of the same image: {count}
",
                                          "Duplicate Image",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Question);
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

                    if (this._currentImage != null)
                    {
                        var folderNum = (int)e.Key - 34;
                        _copyImage(this._currentImage, folderNum);

                        this._lastImage = this._currentImage;
                        this.viewOld.Source = _currentImageAsBitmap;
                        this.lblPrevFolderNum.Content = $"{folderNum}";

                        this.lstImages.SelectedIndex++;
                    }

                    break;
            }
        }
    }
}
