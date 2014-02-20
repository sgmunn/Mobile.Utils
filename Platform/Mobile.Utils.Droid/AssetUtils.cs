// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssetUtils.cs" company="sgmunn">
//   (c) sgmunn 2013  
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//   the Software.
//
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//   THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//   IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Mobile.Utils
{
    using System;
    using System.IO;
    using Android.App;

    public static class AssetUtils
    {
        public static void SyncAssets(string assetFolder, string targetDir)
        {
            string[] assets = Application.Context.Assets.List (assetFolder);

            foreach (string asset in assets) 
            {
                string[] subAssets = Application.Context.Assets.List (assetFolder + "/" + asset);

                // if it has a length, it's a folder
                if (subAssets.Length > 0)
                {
                    SyncAssets(assetFolder + "/" + asset, targetDir);
                }
                else
                {
                    // it's a file
                    using (var source = Application.Context.Assets.Open(assetFolder + "/" + asset))
                    {
                        if (!System.IO.Directory.Exists(targetDir + assetFolder))
                        {
                            System.IO.Directory.CreateDirectory(targetDir + assetFolder);
                        }

                        using (var dest = System.IO.File.Create(targetDir + assetFolder + "/" + asset))
                        {
                            source.CopyTo(dest);
                        }
                    }
                }
            }
        }
    }
}
