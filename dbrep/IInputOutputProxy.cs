using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace dbrep
{

    public interface IInputOutputProxy
    {

        string ReadLine();

        void WriteLine();

        void WriteLine(string TheLine);

        void WriteConnectionIsOpen();

        void WriteConnectionIsOpen(string TheConnectionString);

        void WriteConnectionIsClosed();

        void WriteException(Exception TheException);

        void WriteDataReader(DbDataReader TheDataReader);

    }

}
