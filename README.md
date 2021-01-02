# Open Photo Sorter

A Windows 10 utility that lets you select a folder, preview images, and order them into new folders/directories. 

## USE AT YOUR OWN RISK!

----------------------

### How to use

#### First run

* On first run, you may see a dialog with...
    * Windows protected your PC -- Microsoft Defender SmartScreen prevented an unrecognized app from starting. Running this app might put your PC at risk.
* Click the "More info" link.
* Click "Run anyway".
    * Often this would be a bad idea. 
    * It's okay here, I *swear*, but if you're sus, you can just [grab the source](https://github.com/ruffin--/openPhotoSorter), vet it, and build.

#### Subsequent runs

1. Open the app.
2. Click the "Select Folder" button.
3. The folder selection dialog is kinda weird. Navigate to the folder that has your images. Click "Save".
    * This does, however, let you `Alt-D` up to the top, type in the dir, return to nav, then `Alt-S` to "save". Speedy.
4. Images will load up in the list box on the right. Select one.
5. Choose a number between 0 to 9. 
    * A folder with the name `OpenPhotoSorter/0` (if you select `0`) will be created.
    * It will hold your image.
    * It should create a new name if the file already exists by appending a timestamp and... some other stuff if necessary.
    * If you have `Copy .mov from Live Photos` selected, it may also copy `.mov` files that match the name of your image.
6. The image preview now moves from the main preview window in the middle to the secondary window on the right. The next image, if there is one, will be in the main preview window.
    * You may review where you filed the previous and/or
    * File the next image.
7. To review refile an image, you can select it from the list box on the right.

----------------------

### Disclaimers

There are lots of great ideas I've got half-baked into this code, like de-duplication (that's what the hash code is doing) and traversing child folders.

> It might be neat to run a "file-by-date" utility (like [this one](https://github.com/dbader/photosorter) (I've not yet tested)) once you've weeded your existing images into 1 to 10 folders. Enjoy.

Again, that's half-baked and not yet operational. **Pull requests & bug reports welcome**, and if I find those features save me a lot of time, I'll probably add them. But for now, a few hours over a holiday break means this is what we've got for me to separate, you know, pictures I really want to keep from pictures of my toe or a bad driver or a screenshot of something I meant to follow-up on but never looked at again.

There's essentially no error handling. It's gonna blow up. Good luck.

----------------------

### [License](https://mozilla.org/MPL/2.0/)

This Source Code Form is subject to the terms of the Mozilla Public
License, v. 2.0. If a copy of the MPL was not distributed with this
file, You can obtain one at [https://mozilla.org/MPL/2.0/](https://mozilla.org/MPL/2.0/).

----------------------

<span style="font-size:smaller">Brought to you by the makers of [MarkUpDown](http://www.MarkUpDown.com), the most helpful Markdown editor for professional, efficient HTML editing.</span>