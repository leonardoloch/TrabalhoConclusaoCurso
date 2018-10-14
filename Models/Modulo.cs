using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Modulo
    {
        public  List<No> nos { set; get; }
        public List<Ligacao> ligacoes { set; get; }


        public static IEnumerable<SelectListItem> GetProvincesList()
        {
            DBConnect db = new DBConnect();
            
            

            IList<SelectListItem> items = db.GetLocalizacao(); /*new List<SelectListItem>
            {
                new SelectListItem{Text = "California", Value = "B"},
                new SelectListItem{Text = "Alaska", Value = "B"},
                new SelectListItem{Text = "Illinois", Value = "B"},
                new SelectListItem{Text = "Texas", Value = "B"},
                new SelectListItem{Text = "Washington", Value = "B"}

            };*/
            return items;
        }

    }
}
