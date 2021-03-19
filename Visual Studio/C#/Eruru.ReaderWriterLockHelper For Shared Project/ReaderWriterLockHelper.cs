using System;
using System.Threading;

namespace Eruru.ReaderWriterLockHelper {

	public class ReaderWriterLockHelper<T> {

		public int MillisecondsTimeout { get; set; } = Timeout.Infinite;

		readonly ReaderWriterLock ReaderWriterLock = new ReaderWriterLock ();

		T Instance;

		public ReaderWriterLockHelper () {

		}
		public ReaderWriterLockHelper (T instance) {
			Instance = instance;
		}

		public void Read (ReaderWriterLockHelperAction<T> action) {
			if (action is null) {
				throw new ArgumentNullException (nameof (action));
			}
			ReaderWriterLock.AcquireReaderLock (MillisecondsTimeout);
			try {
				action (ref Instance);
			} finally {
				ReaderWriterLock.ReleaseReaderLock ();
			}
		}

		public void Write (ReaderWriterLockHelperAction<T> action) {
			if (action is null) {
				throw new ArgumentNullException (nameof (action));
			}
			if (ReaderWriterLock.IsReaderLockHeld) {
				LockCookie lockCookie = ReaderWriterLock.UpgradeToWriterLock (MillisecondsTimeout);
				try {
					action (ref Instance);
				} finally {
					ReaderWriterLock.DowngradeFromWriterLock (ref lockCookie);
				}
				return;
			}
			if (ReaderWriterLock.IsWriterLockHeld) {
				action (ref Instance);
				return;
			}
			ReaderWriterLock.AcquireWriterLock (MillisecondsTimeout);
			try {
				action (ref Instance);
			} finally {
				ReaderWriterLock.ReleaseWriterLock ();
			}
		}

	}

}