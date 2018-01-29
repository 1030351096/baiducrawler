using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
namespace ConsoleApplication1
{
    public class SqlHelper
    {

        public static bool Is_Id(string newsid)
        {
            DbcontextDB db = new DbcontextDB();
            var ID = db.FirstOrDefault<Crawler>("where NewsID=" + newsid + "");
            return ID == null ? true : false;
        }

        public static bool Insert(Crawler cl)
        {
            bool ok;
            DbcontextDB db = new DbcontextDB();
            int row= Convert.ToInt32(db.Insert(cl));
            return row == 1 ? ok = false : ok = true;
        }

        public static bool Is_Tid(string Tid)
        {
            DbcontextDB db = new DbcontextDB();
            var ID = db.FirstOrDefault<Tieba>("where Tid=" + Tid + "");
            return ID == null ? true : false;
        }

        public static bool InsertTieba(Tieba Tb)
        {
            bool ok;
            DbcontextDB db = new DbcontextDB();
            int row = Convert.ToInt32(db.Insert(Tb));
            return row == 1 ? ok = false : ok = true;
        }
    }
}
