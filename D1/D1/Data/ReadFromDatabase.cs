using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D1.Data;

namespace D1
{
    public static class ReadFromDatabase
    {

        public static List<Material> GetAllMaterials()
        {
            var entities = new DatabaseEntities();

            return entities.Material.ToList();



        }
        



    }
}
