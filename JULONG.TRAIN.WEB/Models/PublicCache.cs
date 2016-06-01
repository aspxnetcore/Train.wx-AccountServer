using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.Mvc;
using System.Text;
using JULONG.TRAIN.Model;

namespace JULONG.TRAIN.WEB.Models
{
    using JULONG.TRAIN.LIB;
    public static class PublicCache
    {

        private static string TransfPartPath = "";
        private static Dictionary<string, Test> Tests = new Dictionary<string, Test>();
        public static void TestGC()
        {
            //using (DBContext db = new DBContext())
            //{
            //    foreach (var t in Tests)
            //    {
            //       if(t.Value. t.Value.Id
            //    }

            //}

        }


    }
}