/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
using System.Collections.Generic;

namespace PhotoSorter.Models
{
    public class AnFolder
    {
        public string Path { get; set; } = @"C:\";

        public Dictionary<string, List<AnImage>> ImagesByHash { get; set; }

        public List<AnFolder> ChildFolders { get; set; }

        public AnFolder()
        {
            ImagesByHash = new Dictionary<string, List<AnImage>>();
            ChildFolders = new List<AnFolder>();
        }

        public AnFolder(string path) : this()
        {
            Path = path;
        }
    }
}
