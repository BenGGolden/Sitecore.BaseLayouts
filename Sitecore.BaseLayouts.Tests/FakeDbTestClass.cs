using System;
using Sitecore.FakeDb;

namespace Sitecore.BaseLayouts.Tests
{
    public class FakeDbTestClass : IDisposable
    {
        protected readonly Db Db;

        public FakeDbTestClass()
        {
            Db = new Db();
        }

        public void Dispose()
        {
            Db.Dispose();
        }
    }
}