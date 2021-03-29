using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TR.OGT.ChangeLedger.Common.Tests
{
    [TestClass]
    public class DistinctCollectionTests
    {
        [TestMethod]
        public void Add_AddsUniqueItems_StoresAllItems()
        {
            var disctinctCollection = new DistinctCollection<int>();

            disctinctCollection.Add(1);
            disctinctCollection.Add(2);
            disctinctCollection.Add(3);

            Assert.AreEqual(3, disctinctCollection.Count);
        }

        [TestMethod]
        public void Add_AddsDuplicateItems_StoresUniqueItems()
        {
            var disctinctCollection = new DistinctCollection<int>();

            disctinctCollection.Add(1);
            disctinctCollection.Add(2);
            disctinctCollection.Add(2);
            disctinctCollection.Add(2);
            disctinctCollection.Add(3);

            Assert.AreEqual(3, disctinctCollection.Count);
        }

        [TestMethod]
        public void AddRange_AddsUniqueItems_StoresAllItems()
        {
            var disctinctCollection = new DistinctCollection<int>();
            int[] array = new int[] { 1, 2, 3 };

            disctinctCollection.AddRange(array);

            Assert.AreEqual(3, disctinctCollection.Count);
        }

        [TestMethod]
        public void AddRange_AddsDuplicateItems_StoresUniqueItems()
        {
            var disctinctCollection = new DistinctCollection<int>();
            int[] array = new int[] { 1, 1, 1, 1, 2, 3, 3 };

            disctinctCollection.AddRange(array);

            Assert.AreEqual(3, disctinctCollection.Count);
        }
    }
}