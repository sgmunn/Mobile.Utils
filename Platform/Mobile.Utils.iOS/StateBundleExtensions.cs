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
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.IO;
    using MonoTouch.Foundation;

    public static class StateBundleExtensions
    {
        public static void SaveToDictionary(this IStateBundle state, NSMutableDictionary bundle)
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
                        var bytes = stream.ToArray();
                        var array = new NSMutableArray();
                        foreach (var b in bytes)
                        {
                            array.Add(NSNumber.FromByte(b));
                        }

                        bundle.Add(new NSString(kv.Key), array);
                    }
                }
            }
        }

        public static IStateBundle ToStateBundle(this NSDictionary bundle)
        {
            var state = new StateBundle();
            var formatter = new BinaryFormatter();

            if (bundle != null)
            {
                foreach (var key in bundle.Keys)
                {
                    var byteArray = bundle.ObjectForKey(key) as NSArray;
                    if (byteArray != null)
                    {
                        var bytes = new List<byte>();

                        for (int i = 0; i < byteArray.Count; i++)
                        {
                            var v = byteArray.GetItem<NSNumber>(i);
                            bytes.Add(v.ByteValue);
                        }

                        using (var stream = new MemoryStream(bytes.ToArray()))
                        {
                            var value = formatter.Deserialize(stream);
                            state.Data[key.ToString()] = value;
                        }
                    }
                }
            }

            return state;
        }
    }
}
