using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using TR.OGT.ChangeLedger.Domain.Entities;

namespace TR.OGT.ChangeLedger.Domain.Tests
{
    [TestClass]
    public class ChangeTrackingChangeTests
    {
        [TestMethod]
        public void GetHashcode_AddsUniqueObjectsToHashSet_StoresAllItems()
        {
            var collection = new HashSet<ChangeTrackingChange>();

            collection.Add(new ChangeTrackingChange() { GuidChanged = Guid.NewGuid(), EventType = ChangeEventType.Insert });
            collection.Add(new ChangeTrackingChange() { GuidChanged = Guid.NewGuid(), EventType = ChangeEventType.Insert });
            collection.Add(new ChangeTrackingChange() { GuidChanged = Guid.NewGuid(), EventType = ChangeEventType.Insert });

            Assert.AreEqual(3, collection.Count);
        }

        [TestMethod]
        public void GetHashcode_AddsDuplicateObjectsToHashSet_StoresUniqueItems()
        {
            var collection = new HashSet<ChangeTrackingChange>();

            var duplicateGuid = Guid.NewGuid();

            collection.Add(new ChangeTrackingChange() { GuidChanged = duplicateGuid, EventType = ChangeEventType.Insert });
            collection.Add(new ChangeTrackingChange() { GuidChanged = duplicateGuid, EventType = ChangeEventType.Update });
            collection.Add(new ChangeTrackingChange() { GuidChanged = duplicateGuid, EventType = ChangeEventType.Delete });
            collection.Add(new ChangeTrackingChange() { GuidChanged = Guid.NewGuid(), EventType = ChangeEventType.Insert });
            collection.Add(new ChangeTrackingChange() { GuidChanged = Guid.NewGuid(), EventType = ChangeEventType.Insert });

            Assert.AreEqual(3, collection.Count);
        }
    }
}
