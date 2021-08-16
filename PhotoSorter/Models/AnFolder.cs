/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhotoSorter.Models
{
    public class AnFolder
    {

        private static string[] _ImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

        public string Path { get; set; } = @"C:\";

        // public Dictionary<string, List<AnImage>> ImagesByHash { get; set; }

        public Dictionary<string, AnImage> ImagesByPath { get; set; }

        public Dictionary<string, string> SeenHashesWithPaths { get; set; }

        public List<AnFolder> ChildFolders { get; set; }

        public AnFolder()
        {
            ImagesByPath = new Dictionary<string, AnImage>();
            SeenHashesWithPaths = new Dictionary<string, string>();
            ChildFolders = new List<AnFolder>();
        }

        public AnFolder(IEnumerable<string> filePaths) : this()
        {
            foreach (string filePath in filePaths)
            {
                if (_ImageExtensions.Any(x => filePath.ToLower().EndsWith(x)))
                {
                    System.Diagnostics.Debug.WriteLine(filePath);

                    var anImage = new AnImage(filePath);
                    this.ImagesByPath.Add(filePath, anImage);
                }
            }

        }

        public AnFolder(string path) : this()
        {
            this.Path = path;
            var filePaths = Directory.GetFiles(path);
            foreach (string filePath in filePaths)
            {
                if (_ImageExtensions.Any(x => filePath.ToLower().EndsWith(x)))
                {
                    System.Diagnostics.Debug.WriteLine(filePath);

                    var anImage = new AnImage(filePath);
                    this.ImagesByPath.Add(filePath, anImage);
                }
            }

            var folders = Directory.GetDirectories(this.Path);
            foreach (string folderPath in folders)
            {
                this.ChildFolders.Add(new AnFolder(folderPath));
            }
        }
    }
}
