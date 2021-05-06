using System;
using System.Reflection;
using System.Threading;
using Eruru.ReaderWriterLockHelper;

namespace ConsoleApp1 {

	class Program {

		static void Main (string[] args) {
			Console.Title = string.Empty;
			ReaderWriterLockHelper<int> readerWriterLockHelper = new ReaderWriterLockHelper<int> ();
			Console.WriteLine (readerWriterLockHelper.GetType ().GetField (nameof (ReaderWriterLock), BindingFlags.Instance | BindingFlags.NonPublic).GetValue (readerWriterLockHelper));
			new Thread (() => {
				while (true) {
					new Thread (() => {
						readerWriterLockHelper.Read (value => {
							Console.WriteLine (value);
						});
					}) {
						IsBackground = true
					}.Start ();
					Thread.Sleep (100);
				}
			}) {
				IsBackground = true
			}.Start ();
			new Thread (() => {
				while (true) {
					new Thread (() => {
						readerWriterLockHelper.Read (value => {
							readerWriterLockHelper.Write ((ref int insideValue) => {
								readerWriterLockHelper.Read (innerValue => {

								});
								Console.WriteLine ($"{insideValue} To {++insideValue}");
								Thread.Sleep (1000);
							});
						});
					}) {
						IsBackground = true
					}.Start ();
					Thread.Sleep (1000);
				}
			}) {
				IsBackground = true
			}.Start ();
			Console.ReadLine ();
		}

	}

}