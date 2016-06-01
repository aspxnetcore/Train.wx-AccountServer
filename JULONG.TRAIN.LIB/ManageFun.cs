using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JULONG.TRAIN.LIB
{
    public static class ManageFun
    {
        public static string Frame_LeftMenuCode(dynamic viewBag,string code){

            if(viewBag!=null && viewBag.menu_code!=null){
                code = (string)viewBag.menu_code;
            }
            return "<script>activeLeftMenu('"+code+"')</script>";
        }
    }
}