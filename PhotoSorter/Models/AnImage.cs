/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
using System;
using System.IO;
using System.Security.Cryptography;

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

        public string Hash
        {
            get
            {
                return HashFile(Path);
            }
        }
        public int SortVal { get; set; } = -1;

        public bool Sorted {
            get
            {
                return SortVal >= 0 && SortVal <= 9;
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

            // TODO: There's probably a smarter way to do this using the BitmapImage to get a stream.
            // You are correct. Don't do this until you need to. That's a lot of resources, duh.
            // _hash = AnImage.HashFile(path);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(AnImage))
            {
                return false;
            }

            AnImage objAsType = (AnImage)obj;

            return objAsType.Path.Equals(this.Path);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
