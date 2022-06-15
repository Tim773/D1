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

        public static List<MaterialWithSuppliers> GetAllMaterials()
        {
            var entities = new DatabaseEntities();

            return entities.Material
                .AsEnumerable()
                .Select(m => new MaterialWithSuppliers
            {
                BaseMaterial = m,
                Suppliers = string.Join(",", m.Supplier.Select(s => s.Title)).Length == 0 
                ? "Нет поставщиков"
                : string.Join(",", m.Supplier.Select(s => s.Title))
                

            }).ToList();



        }

        public static List<MaterialType> GetAllMaterialTypes()
        {
            var entities = new DatabaseEntities();

            return entities.MaterialType.ToList();


        }

        public static List<Material> GetmaterialsByID(List<int> idList)
        {
            var entities = new DatabaseEntities();

            return entities.Material.Where(m => idList.Contains(m.ID)).ToList();


        }
        



    }
}
