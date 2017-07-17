namespace Client
{
    using System;
    using System.Collections.Generic;

    internal partial class Program
    {
        #region Constructor

        public Program()
        {

        }

        #endregion

        #region Public methods

        public void Run()
        {
            "Pick one".Choose(new Dictionary<string, Action>
            {
                { "Manage owned servers", ManageOwnedServers },
                { "Upload file", UploadFile }
            });
        }

        #endregion

        #region Private methods

        private void ManageOwnedServers() => throw new NotImplementedException();

        private void UploadFile() => throw new NotImplementedException();

        #endregion
    }
}
