using System;
using System.Threading;
using System.Threading.Tasks;
using Eruru.ReaderWriterLockHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1 {

	[TestClass]
	public class UnitTest1 {

		[TestMethod]
		public void TestMethod1 () {
			ReaderWriterLockHelper<int> readerWriterLockHelper = new ReaderWriterLockHelper<int> ();
			int count = 10000;
			int done = 0;
			Task.Run (() => {
				while (done < 2) {
					Task.Run (() => {
						readerWriterLockHelper.Read (value => {

						});
					});
				}
			});
			Task.Run (() => {
				for (int i = 0; i < count; i++) {
					readerWriterLockHelper.Write ((ref int value) => {
						value++;
					});
				}
				done++;
			});
			Task.Run (() => {
				for (int i = 0; i < count; i++) {
					readerWriterLockHelper.Read (value => {
						readerWriterLockHelper.Write ((ref int insideValue) => {
							readerWriterLockHelper.Read (innerValue => {

							});
							insideValue--;
						});
					});
				}
				done++;
			});
			while (done < 2) {
				Thread.Sleep (1);
			}
			readerWriterLockHelper.Read (value => {
				Console.WriteLine (value);
				Assert.AreEqual (0, value);
			});
		}

	}

}