// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateBundleExtensions.cs" company="sgmunn">
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
    using Android.OS;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.IO;

    public static class StateBundleExtensions
    {
        public static void SaveToBundle(this IStateBundle state, Bundle bundle)
        {
            var formatter = new BinaryFormatter();

            foreach (var kv in state.Data.Where(x => x.Value != null))
            {
                var value = kv.Value;

                if (value.GetType().IsSerializable)
                {
                    using (var stream = new MemoryStream())
                    {
                        formatter.Serialize(stream, value);
                        stream.Position = 0;

                        bundle.PutByteArray(kv.Key, stream.ToArray());
                    }
                }
            }
        }

        public static IStateBundle ToStateBundle(this Bundle bundle)
        {
            var state = new StateBundle();
            var formatter = new BinaryFormatter();

            if (bundle != null)
            {
                foreach (var key in bundle.KeySet())
                {
                    var bytes = bundle.GetByteArray(key);
                    if (bytes != null)
                    {
                        using (var stream = new MemoryStream(bytes))
                        {
                            var value = formatter.Deserialize(stream);
                            state.Data[key] = value;
                        }
                    }
                }
            }

            return state;
        }
    }
}
