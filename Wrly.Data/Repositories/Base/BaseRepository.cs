

#region ' ---- Includes ----  '

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.Common;
using System.Data;

#endregion

namespace Wrly.Data
{
    public abstract class BaseRepository : IDisposable
    {
        #region '---- Members ----'

        // Used for data access block.

        public Database _ObjDatabase;
        protected DbTransaction _SqlTransaction = null;
        protected DbConnection _objConnection = null;

        // Used for creating connection while executing data access block in transaction.
        protected string _strConnectionString { get { return CommonData.MainDbConnectionString; } }

        private bool _blDisposed;

        #endregion

        #region '---- Properties ----'


        /// <summary>
        /// Gets or sets the SQL database context.
        /// </summary>

        /// <Remarks>
        /// </Remarks>
        public Database _Database
        {
            get
            {
                if (_ObjDatabase == null)
                {
                    _ObjDatabase = new SqlDatabase(_strConnectionString);
                }
                return _ObjDatabase;
            }
            set
            {
                _ObjDatabase = value;
            }
        }

        /// <summary>
        /// Gets the context.
        /// </summary>

        #endregion

        #region '---- Constructor ----'

        /// <summary>
        /// Initializes a new instance of the BaseRepository class.
        /// </summary>

        /// <Remarks>
        /// </Remarks>
        protected BaseRepository()
        {

        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the BaseRepository is reclaimed by garbage collection.
        /// </summary>

        /// <Remarks>
        /// </Remarks>
        ~BaseRepository()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }

        #endregion

        #region '---- Methods ----'

        /// <summary>
        /// Starts the connection.
        /// </summary>

        /// <Remarks>
        /// </Remarks>
        protected void StartConnection()
        {
            _ObjDatabase = new SqlDatabase(_strConnectionString);
            _objConnection = _ObjDatabase.CreateConnection();
            _objConnection.Open();
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>

        /// <Remarks>
        /// </Remarks>
        protected void CloseConnection()
        {
            if (_objConnection != null)
            {
                _objConnection.Close();
                _objConnection.Dispose();
            }
        }

        /// <summary>
        /// Starts the transaction.
        /// </summary>

        /// <Remarks>
        /// </Remarks>
        protected void StartTransaction()
        {
            if (_objConnection == null || _objConnection.State == ConnectionState.Closed)
            {
                StartConnection();
            }

            _SqlTransaction = _objConnection.BeginTransaction();
        }

        /// <summary>
        /// Rollbacks the transaction.
        /// </summary>

        /// <Remarks>
        /// </Remarks>
        protected void RollbackTransaction()
        {
            // Checking if transaction is not nothing and also connection instance of transaction not nothing
            // then it will rollback transaction.
            // if transaction has already been rollback than the connection instance of transaction is nothing
            // in that case transaction will not rollback.
            if (_SqlTransaction != null && _SqlTransaction.Connection != null)
            {
                _SqlTransaction.Rollback();
            }
        }

        /// <summary>
        /// Commits the transaction.
        /// </summary>

        /// <Remarks>
        /// </Remarks>
        protected void CommitTransaction()
        {
            if (_SqlTransaction != null)
            {
                _SqlTransaction.Commit();
            }
        }

        #endregion

        #region '---- Implementation of IDisposable ----'

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// Implement IDisposable. 
        /// Do not make this method virtual. 
        /// A derived class should not be able to override this method. 
        /// </summary>

        /// <Remarks>
        /// </Remarks>
        public void Dispose()
        {
            Dispose(true);

            // This object will be cleaned up by the Dispose method. 
            // Therefore, you should call GC.SupressFinalize to take this object off the finalization queue and
            // prevent finalization code for this object from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly or indirectly by a user's code.
        /// Managed and unmanaged resources can be disposed.
        /// If disposing equals false, the method has been called by the runtime from inside the finalizer and
        /// you should not reference other objects. Only unmanaged resources can be disposed.
        /// </summary>

        /// <Remarks>
        /// </Remarks>
        protected virtual void Dispose(bool blDisposing)
        {
            if (!blDisposing)
                return;

            if (_blDisposed)
                return;

            // Check to see if Dispose has already been called. 
            if (!this._blDisposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (blDisposing)
                {
                    // Dispose managed resources.
                    if (_ObjDatabase != null)
                        _ObjDatabase = null;

                    if (_SqlTransaction != null)
                        _SqlTransaction.Dispose();

                    if (_objConnection != null)
                        _objConnection.Dispose();
                }

                // Call the appropriate methods to clean up unmanaged resources here.
                // If disposing is false, only the following code is executed.

                // Note disposing has been done.
                _blDisposed = true;
            }
        }

        #endregion
    }
}
