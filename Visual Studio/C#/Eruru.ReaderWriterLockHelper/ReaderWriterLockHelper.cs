using System;
using System.Threading;

namespace Eruru.ReaderWriterLockHelper {

	public class ReaderWriterLockHelper<T> : IDisposable {

		public int MillisecondsTimeout { get; set; } = Timeout.Infinite;

#if NET35_OR_GREATER || NETSTANDARD
		readonly ReaderWriterLockSlim ReaderWriterLock = new ReaderWriterLockSlim ();
#else
		readonly ReaderWriterLock ReaderWriterLock = new ReaderWriterLock ();
#endif

		T Instance;

		public ReaderWriterLockHelper () {

		}
		public ReaderWriterLockHelper (T instance) {
			Instance = instance;
		}

		public bool Read (ReaderWriterLockHelperRefAction<T> action, int millisecondsTimeout) {
			if (action is null) {
				throw new ArgumentNullException (nameof (action));
			}
#if NET35_OR_GREATER || NETSTANDARD
			if (ReaderWriterLock.IsReadLockHeld || ReaderWriterLock.IsWriteLockHeld) {
				action (ref Instance);
				return true;
			}
			if (!ReaderWriterLock.TryEnterReadLock (millisecondsTimeout)) {
				return false;
			}
			try {
				action (ref Instance);
			} finally {
				if (ReaderWriterLock.IsReadLockHeld) {
					ReaderWriterLock.ExitReadLock ();
				}
			}
#else
			if (ReaderWriterLock.IsReaderLockHeld || ReaderWriterLock.IsWriterLockHeld) {
				action (ref Instance);
				return true;
			}
			try {
				ReaderWriterLock.AcquireReaderLock (millisecondsTimeout);
			} catch {
				return false;
			}
			try {
				action (ref Instance);
			} finally {
				ReaderWriterLock.ReleaseReaderLock ();
			}
#endif
			return true;
		}
		public bool Read (ReaderWriterLockHelperRefAction<T> action, TimeSpan timeSpan) {
			return Read (action, (int)timeSpan.TotalMilliseconds);
		}
		public bool Read (ReaderWriterLockHelperRefAction<T> action) {
			return Read (action, MillisecondsTimeout);
		}
		public bool Read (ReaderWriterLockHelperAction<T> action, int millisecondsTimeout) {
			if (action is null) {
				throw new ArgumentNullException (nameof (action));
			}
			return Read ((ref T instance) => action (instance), millisecondsTimeout);
		}
		public bool Read (ReaderWriterLockHelperAction<T> action, TimeSpan timeSpan) {
			if (action is null) {
				throw new ArgumentNullException (nameof (action));
			}
			return Read ((ref T instance) => action (instance), timeSpan);
		}
		public bool Read (ReaderWriterLockHelperAction<T> action) {
			if (action is null) {
				throw new ArgumentNullException (nameof (action));
			}
			return Read ((ref T instance) => action (instance));
		}

		public bool Write (ReaderWriterLockHelperRefAction<T> action, int millisecondsTimeout) {
			if (action is null) {
				throw new ArgumentNullException (nameof (action));
			}
#if NET35_OR_GREATER || NETSTANDARD
			if (ReaderWriterLock.IsWriteLockHeld) {
				action (ref Instance);
				return true;
			}
			bool recoveryRead = false;
			if (ReaderWriterLock.IsReadLockHeld) {
				recoveryRead = true;
				ReaderWriterLock.ExitReadLock ();
			}
			if (!ReaderWriterLock.TryEnterWriteLock (millisecondsTimeout)) {
				return false;
			}
			try {
				action (ref Instance);
			} finally {
				ReaderWriterLock.ExitWriteLock ();
				if (recoveryRead) {
					ReaderWriterLock.TryEnterReadLock (millisecondsTimeout);
				}
			}
#else
			if (ReaderWriterLock.IsWriterLockHeld) {
				action (ref Instance);
				return true;
			}
			if (ReaderWriterLock.IsReaderLockHeld) {
				LockCookie lockCookie;
				try {
					lockCookie = ReaderWriterLock.UpgradeToWriterLock (millisecondsTimeout);
				} catch {
					return false;
				}
				try {
					action (ref Instance);
				} finally {
					ReaderWriterLock.DowngradeFromWriterLock (ref lockCookie);
				}
				return true;
			}
			try {
				ReaderWriterLock.AcquireWriterLock (millisecondsTimeout);
			} catch {
				return false;
			}
			try {
				action (ref Instance);
			} finally {
				ReaderWriterLock.ReleaseWriterLock ();
			}
#endif
			return true;
		}
		public bool Write (ReaderWriterLockHelperRefAction<T> action, TimeSpan timeSpan) {
			return Write (action, (int)timeSpan.TotalMilliseconds);
		}
		public bool Write (ReaderWriterLockHelperRefAction<T> action) {
			return Write (action, MillisecondsTimeout);
		}
		public bool Write (ReaderWriterLockHelperAction<T> action, int millisecondsTimeout) {
			if (action is null) {
				throw new ArgumentNullException (nameof (action));
			}
			return Write ((ref T instance) => action (instance), millisecondsTimeout);
		}
		public bool Write (ReaderWriterLockHelperAction<T> action, TimeSpan timeSpan) {
			if (action is null) {
				throw new ArgumentNullException (nameof (action));
			}
			return Write ((ref T instance) => action (instance), timeSpan);
		}
		public bool Write (ReaderWriterLockHelperAction<T> action) {
			if (action is null) {
				throw new ArgumentNullException (nameof (action));
			}
			return Write ((ref T instance) => action (instance));
		}

		public void Dispose () {
#if NET35_OR_GREATER || NETSTANDARD
			ReaderWriterLock.Dispose ();
#endif
		}

	}

}