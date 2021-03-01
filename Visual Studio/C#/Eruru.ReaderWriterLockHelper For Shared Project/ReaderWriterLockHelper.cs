using System;
using System.Threading;

namespace Eruru.ReaderWriterLockHelper {

	public class ReaderWriterLockHelper {

		public int MillisecondsTimeout { get; set; } = Timeout.Infinite;

		readonly ReaderWriterLock ReaderWriterLock = new ReaderWriterLock ();

		public void Read (ReaderWriterLockHelperAction action) {
			if (action is null) {
				throw new ArgumentNullException (nameof (action));
			}
			ReaderWriterLock.AcquireReaderLock (MillisecondsTimeout);
			try {
				action ();
			} finally {
				ReaderWriterLock.ReleaseReaderLock ();
			}
		}

		public void Write (ReaderWriterLockHelperAction action) {
			if (action is null) {
				throw new ArgumentNullException (nameof (action));
			}
			if (ReaderWriterLock.IsReaderLockHeld) {
				LockCookie lockCookie = ReaderWriterLock.UpgradeToWriterLock (MillisecondsTimeout);
				try {
					action ();
				} finally {
					ReaderWriterLock.DowngradeFromWriterLock (ref lockCookie);
				}
				return;
			}
			ReaderWriterLock.AcquireWriterLock (MillisecondsTimeout);
			try {
				action ();
			} finally {
				ReaderWriterLock.ReleaseWriterLock ();
			}
		}

	}

}