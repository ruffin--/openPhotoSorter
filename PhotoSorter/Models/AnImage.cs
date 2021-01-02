/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
using System;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Media.Imaging;

namespace PhotoSorter.Models
{
    // You know, like "An art"
    // https://www.zazzle.com/an_art_mug-168306589274499153
    public class AnImage
    {
        // https://stackoverflow.com/a/10520086/1028230
        public static string HashFile(string path)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        public string Path { get; set; }

        private string _hash = null;

        public string Hash
        {
            get
            {
                return _hash;
            }
        }
        public int SortVal { get; set; } = -1;

        public bool Sorted {
            get
            {
                return SortVal >= 0 && SortVal <= 9;
            }
        }

        private BitmapImage _image;
        public BitmapImage Image
        {
            get
            {
                return _image;
            }
        }

        public string ImageName
        {
            get
            {
                return this.Path.Contains(@"\")
                    ? this.Path.Substring(this.Path.LastIndexOf(@"\") + 1)
                    : this.Path;
            }
        }

        public AnImage(string path)
        {
            this.Path = path;
            Uri fileUri = new Uri(path);
            _image = new BitmapImage(fileUri);

            // TODO: There's probably a smarter way to do this using the BitmapImage to get a stream.
            _hash = AnImage.HashFile(path);
        }
    }
}
