namespace Eruru.ReaderWriterLockHelper {

	public delegate void ReaderWriterLockHelperRefAction<T> (ref T obj);

	public delegate void ReaderWriterLockHelperAction<T> (T obj);

}