using System;
using System.Threading;
using Sitecore.FakeDb;

namespace Sitecore.BaseLayouts.Tests
{
    public class FakeDbTestClass : IDisposable
    {
        protected readonly Db MasterDb;
        protected readonly FakesFactory MasterFakesFactory;

        public FakeDbTestClass()
        {
            MasterDb = new Db("master");
            MasterFakesFactory = new FakesFactory(MasterDb);
        }

        public void Dispose()
        {
            MasterDb.Dispose();
        }
    }
}