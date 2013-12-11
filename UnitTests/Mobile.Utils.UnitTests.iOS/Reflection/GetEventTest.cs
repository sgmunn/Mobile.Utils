
using System;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace Mobile.Utils.UnitTests.Reflection
{
    public class EventTest
    {
        #pragma warning disable 67
        public event EventHandler InstanceName;

        public static event EventHandler StaticName;
        #pragma warning restore 67
    }


    [TestFixture]
    public class GetEventTest
    {
        [Test]
        public void WhenGettingAnInstanceEvent_ThenTheEventIsNotNull()
        {
            var evInfo = Mobile.Utils.ReflectionExtensions.GetEvent(typeof(EventTest), "InstanceName");
            Assert.AreNotEqual(null, evInfo);
        }

        [Test]
        public void WhenGettingAStaticEvent_ThenTheEventIsNotNull()
        {
            var evInfo = Mobile.Utils.ReflectionExtensions.GetEvent(typeof(EventTest), "StaticName");
            Assert.AreNotEqual(null, evInfo);
        }

        [Test]
        public void WhenGettingAnUndefinedEvent_ThenTheEventIsNull()
        {
            var evInfo = Mobile.Utils.ReflectionExtensions.GetEvent(typeof(EventTest), "NotDefined");
            Assert.AreEqual(null, evInfo);
        }
    }
}
