using System;

namespace scsm
{
    public class Testing: IDisposable
    {
        public Testing(bool testing)
        {
            IsTest = testing;
        }

        public static  bool IsTest { get; private set; }

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            IsTest = false;
        }

        #endregion
    }
}
