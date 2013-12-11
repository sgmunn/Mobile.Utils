using System;
using NUnit.Framework;
using Mobile.Utils.Reflection;
using System.Collections.Generic;

namespace Mobile.Utils.UnitTests.Reflection.ComplexPaths
{
    public class RootClass
    {
        public RootClass()
        {
            this.Nested = new NestedClass();
        }

        public NestedClass Nested  { get; set; }

        public string Property1 { get; set; }
    }

    public class NestedClass
    {
        public NestedClass()
        {
            this.ListProperty = new List<IndexedClass>();
            this.StringListProperty = new List<string>();
        }

        public string Property1 { get; set; }

        public List<IndexedClass> ListProperty { get; set; }

        public List<string> StringListProperty { get; set; }
    }

    public class IndexedClass
    {
        public IndexedClass()
        {
        }

        public string IndexedProperty { get; set; }
    }



    [TestFixture]
    public class ReflectionPropertyAccessorSpec
    {
        public RootClass Root { get; private set; }

        [SetUp]
        public void SetUp()
        {
            this.Root = new RootClass();
            this.Root.Property1 = Guid.NewGuid().ToString();
            this.Root.Nested.Property1 = Guid.NewGuid().ToString();

            var i1 = new IndexedClass() { IndexedProperty = Guid.NewGuid().ToString() };
            var i2 = new IndexedClass() { IndexedProperty = Guid.NewGuid().ToString() };
            var i3 = new IndexedClass() { IndexedProperty = Guid.NewGuid().ToString() };

            this.Root.Nested.ListProperty.AddRange(new [] { i1, i2, i3 });

            this.Root.Nested.StringListProperty.AddRange(new [] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString() });
        }

        [Test]
        public void WhenGettingASimplePropertyPath_ThenTheCorrectValueIsReturned()
        {
            var oldValue = this.PerformGetValue("Property1");
            Assert.AreEqual(this.Root.Property1, oldValue);
        }

        [Test]
        public void WhenSettingASimplePropertyPath_ThenTheCorrectPropertyIsSet()
        {
            var newValue = Guid.NewGuid().ToString();
            this.PerformSetValue("Property1", newValue);
            Assert.AreEqual(this.Root.Property1, newValue);
        }

        [Test]
        public void WhenGettingANestedPropertyPath_ThenTheCorrectValueIsReturned()
        {
            var oldValue = this.PerformGetValue("Nested.Property1");
            Assert.AreEqual(this.Root.Nested.Property1, oldValue);
        }

        [Test]
        public void WhenSettingANestedPropertyPath_ThenTheCorrectPropertyIsSet()
        {
            var newValue = Guid.NewGuid().ToString();
            this.PerformSetValue("Nested.Property1", newValue);
            Assert.AreEqual(this.Root.Nested.Property1, newValue);
        }

        [Test]
        public void WhenGettingAnIndexedPropertyPath_ThenTheCorrectValueIsReturned()
        {
            var oldValue = this.PerformGetValue("Nested.ListProperty[1].IndexedProperty");
            Assert.AreEqual(this.Root.Nested.ListProperty[1].IndexedProperty, oldValue);
        }

        [Test]
        public void WhenSettingAnIndexedPropertyPath_ThenTheCorrectPropertyIsSet()
        {
            var newValue = Guid.NewGuid().ToString();
            this.PerformSetValue("Nested.ListProperty[1].IndexedProperty", newValue);
            Assert.AreEqual(this.Root.Nested.ListProperty[1].IndexedProperty, newValue);
        }

        [Test]
        public void WhenGettingAnIndexedPropertyPath_ThenTheCorrectValueIsReturned_1()
        {
            var oldValue = this.PerformGetValue("Nested.StringListProperty[1]");
            Assert.AreEqual(this.Root.Nested.StringListProperty[1], oldValue);
        }

        [Test]
        public void WhenSettingAnIndexedPropertyPath_ThenTheCorrectPropertyIsSet_1()
        {
            var newValue = Guid.NewGuid().ToString();
            this.PerformSetValue("Nested.StringListProperty[1]", newValue);
            Assert.AreEqual(this.Root.Nested.StringListProperty[1], newValue);
        }

        private void PerformSetValue(string path, string newValue)
        {
            this.GetAccessor(path).SetValue(this.Root, newValue);
        }

        private string PerformGetValue(string path)
        {
            return (string)this.GetAccessor(path).GetValue(this.Root);
        }

        private IPropertyAccessor GetAccessor(string path)
        {
            return new ReflectionPropertyAccessor(path);
        }
    }
}
