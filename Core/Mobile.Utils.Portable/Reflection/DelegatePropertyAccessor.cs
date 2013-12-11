//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="DelegatePropertyAccessor.cs" company="sgmunn">
//    (c) sgmunn 2012  
//
//    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//    documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//    the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//    to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//    The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//    the Software.
//
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//    THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//    CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//    IN THE SOFTWARE.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------

namespace Mobile.Utils.Reflection
{
    using System;

    /// <summary>
    /// Provides a way to access a property given some delegates 
    /// </summary>
    /// <remarks>
    /// The primary purpose for this class is provide a means by which bindings can perform faster than using
    /// propertyInfo.GetValue / .SetValue
    /// A secondary reason allows arbitrarily complex bindings to complex properties - currently binding only support
    /// a simple property
    /// </remarks>
    public class DelegatePropertyAccessor<T, TV> : IPropertyAccessor
    {
        public DelegatePropertyAccessor(Func<T, TV> getter, Action<T, TV> setter)
        {
            this.Getter = getter;
            this.Setter = setter;
        }

        public Func<T, TV> Getter { get; private set; }

        public Action<T, TV> Setter { get; private set; }

        public virtual bool CanGetValue(T obj) 
        { 
            return this.Getter != null;
        }

        public virtual bool CanSetValue(T obj) 
        { 
            return this.Setter != null;
        }

        public TV GetValue(T obj)
        {
            if (this.CanGetValue(obj))
            {
                return this.Getter(obj);
            }

            return default(TV);
        }

        public void SetValue(T obj, TV value)
        {
            if (this.CanSetValue(obj))
            {
                this.Setter(obj, value);
            }
        }

        Type IPropertyAccessor.GetPropertyType(object obj)
        {
            return typeof(TV);
        }
        
        bool IPropertyAccessor.CanGetValue(object obj)
        {
            return this.CanGetValue((T)obj);
        }

        bool IPropertyAccessor.CanSetValue(object obj)
        {
            return this.CanSetValue((T)obj);
        }

        object IPropertyAccessor.GetValue(object obj)
        {
            return this.GetValue((T)obj);
        }

        void IPropertyAccessor.SetValue(object obj, object value)
        {
            this.SetValue((T)obj, (TV)value);
        }
    }

    public sealed class DelegatePropertyAccessor : DelegatePropertyAccessor<object, object>
    {
        public DelegatePropertyAccessor(Func<object, object> getter, Action<object, object> setter) : base(getter, setter)
        {
        }
    }
}
