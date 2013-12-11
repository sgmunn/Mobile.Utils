// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionUtils.cs" company="sgmunn">
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
// Taken in part from:
// http://rx.codeplex.com/SourceControl/latest#Rx.NET/Source/System.Reactive.Linq/Reactive/Internal/ReflectionUtils.cs

namespace Mobile.Utils.Reflection
{
    using System;
    using System.Reflection;
    using System.Globalization;

    public static class ReflectionUtils
    {
        public static Delegate CreateDelegate(Type delegateType, object o, MethodInfo method)
        {
            #if CRIPPLED_REFLECTION
            return method.CreateDelegate(delegateType, o);
            #else
            ////return Delegate.CreateDelegate(delegateType, o, method);
            return method.CreateDelegate(delegateType, o);
            #endif
        }
        
        /// <summary>
        /// Gets the event methods for a named event
        /// </summary>
        public static void GetEventMethods(Type type, string eventName, out MethodInfo addMethod, out MethodInfo removeMethod, out Type delegateType, out bool isWinRT)
        {
            var e = type.GetEventEx(eventName, false);
            if (e == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "COULD_NOT_FIND_INSTANCE_EVENT", eventName, type.FullName));
            }

            ////addMethod = e.GetAddMethod();
            ////removeMethod = e.GetRemoveMethod();
            addMethod = e.AddMethod;
            removeMethod = e.RemoveMethod;

            if (addMethod == null)
            {
                throw new InvalidOperationException("EVENT_MISSING_ADD_METHOD");
            }

            if (removeMethod == null)
            {
                throw new InvalidOperationException("EVENT_MISSING_REMOVE_METHOD");
            }

            var psa = addMethod.GetParameters();
            if (psa.Length != 1)
            {
                throw new InvalidOperationException("EVENT_ADD_METHOD_SHOULD_TAKE_ONE_PARAMETER");
            }

            var psr = removeMethod.GetParameters();
            if (psr.Length != 1)
            {
                throw new InvalidOperationException("EVENT_REMOVE_METHOD_SHOULD_TAKE_ONE_PARAMETER");
            }

            isWinRT = false;

            #if HAS_WINRT
            if (addMethod.ReturnType == typeof(EventRegistrationToken))
            {
                isWinRT = true;

                var pet = psr[0];
                if (pet.ParameterType != typeof(EventRegistrationToken))
                    throw new InvalidOperationException(Strings_Linq.EVENT_WINRT_REMOVE_METHOD_SHOULD_TAKE_ERT);
            }
            #endif

            delegateType = psa[0].ParameterType;

            var invokeMethod = delegateType.GetMethod("Invoke");

            var parameters = invokeMethod.GetParameters();

            if (parameters.Length != 2)
            {
                throw new InvalidOperationException("EVENT_PATTERN_REQUIRES_TWO_PARAMETERS");
            }

            ////if (!typeof(TSender).IsAssignableFrom(parameters[0].ParameterType))
            ////    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "EVENT_SENDER_NOT_ASSIGNABLE", typeof(TSender).FullName));

            ////if (!typeof(TEventArgs).IsAssignableFrom(parameters[1].ParameterType))
            ////{
            ////    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "EVENT_ARGS_NOT_ASSIGNABLE", typeof(TEventArgs).FullName));
            ////}

            if (invokeMethod.ReturnType != typeof(void))
            {
                throw new InvalidOperationException("EVENT_MUST_RETURN_VOID");
            }
        }

        /// <summary>
        /// Gets the event methods for a named event. if target is null, looks for a static event member
        /// </summary>
        public static void GetEventMethods<TSource, TEventArgs>(object source, string eventName, out MethodInfo addMethod, out MethodInfo removeMethod, out Type delegateType, out bool isWinRT)
            where TEventArgs : EventArgs
        {
            var sourceType = typeof(TSource);
            var e = default(EventInfo);

            if (source == null)
            {
                e = sourceType.GetEventEx(eventName, true);
                if (e == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "COULD_NOT_FIND_STATIC_EVENT", eventName, sourceType.FullName));
                }
            }
            else
            {
                e = sourceType.GetEventEx(eventName, false);
                if (e == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "COULD_NOT_FIND_INSTANCE_EVENT", eventName, sourceType.FullName));
                }
            }

            ////addMethod = e.GetAddMethod();
            ////removeMethod = e.GetRemoveMethod();
            addMethod = e.AddMethod;
            removeMethod = e.RemoveMethod;

            if (addMethod == null)
            {
                throw new InvalidOperationException("EVENT_MISSING_ADD_METHOD");
            }

            if (removeMethod == null)
            {
                throw new InvalidOperationException("EVENT_MISSING_REMOVE_METHOD");
            }

            var psa = addMethod.GetParameters();
            if (psa.Length != 1)
            {
                throw new InvalidOperationException("EVENT_ADD_METHOD_SHOULD_TAKE_ONE_PARAMETER");
            }

            var psr = removeMethod.GetParameters();
            if (psr.Length != 1)
            {
                throw new InvalidOperationException("EVENT_REMOVE_METHOD_SHOULD_TAKE_ONE_PARAMETER");
            }

            isWinRT = false;

            #if HAS_WINRT
            if (addMethod.ReturnType == typeof(EventRegistrationToken))
            {
                isWinRT = true;

                var pet = psr[0];
                if (pet.ParameterType != typeof(EventRegistrationToken))
                    throw new InvalidOperationException(Strings_Linq.EVENT_WINRT_REMOVE_METHOD_SHOULD_TAKE_ERT);
            }
            #endif

            delegateType = psa[0].ParameterType;

            var invokeMethod = delegateType.GetMethod("Invoke");

            var parameters = invokeMethod.GetParameters();

            if (parameters.Length != 2)
            {
                throw new InvalidOperationException("EVENT_PATTERN_REQUIRES_TWO_PARAMETERS");
            }

            ////if (!typeof(TSender).IsAssignableFrom(parameters[0].ParameterType))
            ////    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "EVENT_SENDER_NOT_ASSIGNABLE", typeof(TSender).FullName));

            if (!typeof(TEventArgs).GetTypeInfo().IsAssignableFrom(parameters[1].ParameterType.GetTypeInfo()))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "EVENT_ARGS_NOT_ASSIGNABLE", typeof(TEventArgs).FullName));
            }

            if (invokeMethod.ReturnType != typeof(void))
            {
                throw new InvalidOperationException("EVENT_MUST_RETURN_VOID");
            }
        }

        public static EventInfo GetEventEx(this Type type, string name, bool isStatic)
        {
            ////return type.GetEvent(name, isStatic ? BindingFlags.Public | BindingFlags.Static : BindingFlags.Public | BindingFlags.Instance);
            return type.GetEvent(name);//, isStatic ? BindingFlags.Public | BindingFlags.Static : BindingFlags.Public | BindingFlags.Instance);
        }

//        #if CRIPPLED_REFLECTION
//        public static MethodInfo GetMethod(this Type type, string name)
//        {
//            return type.GetTypeInfo().GetDeclaredMethod(name);
//        }
//
//        public static MethodInfo GetAddMethod(this EventInfo eventInfo)
//        {
//            return eventInfo.AddMethod;
//        }
//
//        public static MethodInfo GetRemoveMethod(this EventInfo eventInfo)
//        {
//            return eventInfo.RemoveMethod;
//        }
//
//        public static bool IsAssignableFrom(this Type type1, Type type2)
//        {
//            return type1.GetTypeInfo().IsAssignableFrom(type2.GetTypeInfo());
//        }
//        #endif
    }
}
