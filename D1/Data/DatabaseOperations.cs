using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D1.Data;

namespace D1
{
    public static class DatabaseOperations
    {

        private static DatabaseEntities Entities;


        public static void InitializeEntities()
        {

            if(Entities is null)
            {
                Entities = new DatabaseEntities();
            }


        }


        public static List<MaterialWithSuppliers> GetAllMaterials()
        {
            

            return Entities.Material
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
            return Entities.MaterialType.ToList();
        }

        public static List<Unit> GetAllUnits()
        {

            return Entities.Unit.ToList();

        }

        public static List<Supplier> GetAllSuppliers()
        {

            return Entities.Supplier.ToList();

        }

        public static List<Material> GetMaterialsByID(List<int> idList)
        {
            return Entities.Material.Where(m => idList.Contains(m.ID)).ToList();

        }

        public static Material GetMaterialsByID(int id)
        {
            return Entities.Material.Where(m => m.ID == id).Single();

        }

        public static void ChangeMaterialsMinCount(List<Material> materialList, int newMinCount)
        {

            foreach (var material in materialList)
            {

                material.MinCount = newMinCount;

                Entities.SaveChanges();
            }


        }
        

        public static void SaveEditedMaterial(Material editedMaterial, List<string> supplierNames)
        {


            editedMaterial.Supplier.Clear();

            foreach (var supplierName in supplierNames)
            {
                editedMaterial.Supplier.Add(Entities.Supplier.Single(s => s.Title == supplierName));
                Entities.SaveChanges();
            }


            Entities.SaveChanges();

        }

        public static void AddNewMaterial(Material newMaterial, List<string> supplierNames)
        {

            Entities.Material.Add(newMaterial);


            foreach (var supplierName in supplierNames)
            {
                newMaterial.Supplier.Add(Entities.Supplier.Single(s => s.Title == supplierName));
                Entities.SaveChanges();
            }

            Entities.SaveChanges();

        }


    }
}
