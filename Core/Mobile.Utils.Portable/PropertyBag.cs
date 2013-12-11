// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyBag.cs" company="sgmunn">
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

    public sealed class PropertyBag 
    {
        private readonly Dictionary<string, object> bag;
        private readonly Action<string> onPropertySet;

        public PropertyBag()
        {
            this.bag = new Dictionary<string, object>();
        }
        
        public PropertyBag(Action<string> onPropertySet)
        {
            this.bag = new Dictionary<string, object>();
            this.onPropertySet = onPropertySet;
        }

        public void SetProperty(string propertyName, object value)
        {
            var current = this.GetProperty(propertyName, null);
            if (current == null && value == null)
            {
                return;
            }

            if (!current.SafeEquals(value))
            {
                this.bag[propertyName] = value;
                if (this.onPropertySet != null)
                {
                    this.onPropertySet(propertyName);
                }
            }
        }

        public object GetProperty(string propertyName, object defaultValue)
        {
            object result;
            if (this.bag.TryGetValue(propertyName, out result))
            {
                return result;
            }

            return defaultValue;
        }
    }
}
