using Data.INV_DB;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Models.DB_objects;
using Models.System;
using MudBlazor;

namespace Services.DB_Services
{
    public class DB_Services
    {

        public IDbContextFactory<InventoryDbContext> _dbcontext;
        public ILogger<DB_Services> _logger;


        public DB_Services(IDbContextFactory<InventoryDbContext> dbcontext,ILogger<DB_Services> logger)
        {
            _dbcontext = dbcontext;
            _logger = logger;

        }


        #region Division CRUD
        //Retrieve
        public async Task<List<AAP_Divisions>> GetAllDivisions()
        {
            var context = await _dbcontext.CreateDbContextAsync();

            return await context.AAP_Divisions.ToListAsync();
        }
        public async Task<AAP_Divisions> GetSingleDivision(int id)
        {
            var context = await _dbcontext.CreateDbContextAsync();
            return await context.AAP_Divisions.SingleOrDefaultAsync(x => x.ID == id);

        }
        //Update
        public async Task UpdateDivision(AAP_Divisions new_Division)
        {
            var context = await _dbcontext.CreateDbContextAsync();
            var old_division = await GetSingleDivision(new_Division.ID);

            old_division.Div_Code = new_Division.Div_Code;
            old_division.Desc = new_Division.Desc;

            context.AAP_Divisions.Update(old_division);
            context.SaveChanges();

        }
        //Create
        public async Task<bool> DivisionCreation(AAP_Divisions new_division)
        {          
            try
            {
                new_division.Div_Code = new_division.Div_Code.ToUpper();
                var context = await _dbcontext.CreateDbContextAsync();
                context.AAP_Divisions.Add(new_division);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }
        }
        //Delete
        public async Task<bool> DivisionDelete(AAP_Divisions current_division)
        {
            try
            {
                var context = await _dbcontext.CreateDbContextAsync();
                var old_division = await GetSingleDivision(current_division.ID);

                context.AAP_Divisions.Remove(old_division);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }

        }
        #endregion


        #region Zone CRUD
        //Retrieve
        public async Task<List<Div_Zones>> GetAllZones()
        {
            var context = await _dbcontext.CreateDbContextAsync();
            return await context.Div_Zones.ToListAsync();
        }
        public async Task<List<Div_Zones>> GetZonesByDivision(int id)
        {
            var list = await GetAllZones();
            return list.Where(x => x.ID == id).ToList();
        }
        public async Task<Div_Zones> GetSingleZone(int key)
        {
            var context = await _dbcontext.CreateDbContextAsync();
            var result = await context.Div_Zones.SingleOrDefaultAsync(x => x.ID == key);
            return result;

        }
        //Create
        public async Task<bool> CreateZone( Div_Zones new_zone)
        {
            
            try
            {
                var context = await _dbcontext.CreateDbContextAsync();
                new_zone.Zone_Code = new_zone.Zone_Code.ToUpper();
                context.Div_Zones.Add(new_zone);
                context.SaveChanges();
                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }

        }
        //Delete
        public async Task<bool> DeleteZone(Div_Zones zone)
        {
           
            try
            {
                var context = await _dbcontext.CreateDbContextAsync();
                context.Div_Zones.Remove(zone);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }
        }
        #endregion

        #region Inventories
        //Retrieve
        public async Task<List<Inventories>> GetAllInventories()
        {
            var context = await _dbcontext.CreateDbContextAsync();
            return await context.Inventories.ToListAsync();
        }
        public async Task<Inventories> GetSingleInventory(string id)
        {
            var context = await _dbcontext.CreateDbContextAsync();
            return await context.Inventories.SingleOrDefaultAsync(x => x.ID == id);

        }
        //Create
        public async Task<bool> AddInventory(Inventories new_inventory)
        {  
            try
            {
                var context = await _dbcontext.CreateDbContextAsync();
                context.Add(new_inventory);
                context.SaveChanges();
                foreach (var record in new_inventory.Records)
                {
                    await AddRecord(record);
                }

                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }
        }
        //Delete
        public async Task<bool> DeleteInventory(Inventories inventory)
        {
            
            try
            {
                var context = await _dbcontext.CreateDbContextAsync();
                context.Remove(inventory);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }

        }
        #endregion

        #region Inventory Records
        //Retrieve
        public async Task<List<Inventory_Records>> GetAllRecords()
        {
            var context = await _dbcontext.CreateDbContextAsync();
            return await context.inventory_Records.ToListAsync();
        }

            //#1 Gets the Master List for an inventory
            public async Task<List<Inventory_Records>> GetRecordsByInventory(string ID)
            {
                var context = await _dbcontext.CreateDbContextAsync();
                return await context.inventory_Records.Where(x => x.INVID == ID).ToListAsync();

            }
        //#2 Gets the records per division for given Inventory
        public async Task<List<Inventory_Records>> GetRecordsByInventoryPerDivision(List<Inventory_Records> record_master_list, int division_id)
        {
            List<Div_Zones> Zone_List = await GetAllZones();
            List<Record_Pair> Parings = new();


            foreach (var record in record_master_list)
            {
                Parings.Add(new() { Record = record, Zone = Zone_List.FirstOrDefault(x => x.ID == record.ZoneID) });
            }

            return Parings.Where(x=>x.Zone.DivID == division_id).Select(x=>x.Record).ToList();

        }

        public async Task<Inventory_Records> GetSingleRecord(string id)
            {
                var context = await _dbcontext.CreateDbContextAsync();
                return await context.inventory_Records.SingleOrDefaultAsync(x => x.ID == id);

            }
        //Create
        public async Task<bool> AddRecord(Inventory_Records record)
        {
            try
            {
                var context = await _dbcontext.CreateDbContextAsync();
                context.inventory_Records.Add(record);
                context.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }
        }
        //Update
        public async Task<bool> UpdateRecord(Inventory_Records new_record)
        {
            try
            {
                var context = await _dbcontext.CreateDbContextAsync();
                var old_record = await GetSingleRecord(new_record.ID);

                old_record.Status = new_record.Status;
                context.inventory_Records.Update(old_record);
                context.SaveChanges();
                return true;

            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false; 
            }
            

        }
        //Delete
        #endregion
        public string IDGenerator()
        {
            Random rnd = new Random();

            Int32 unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(2010, 1, 1)).TotalSeconds;
            return $"AAP_INV{rnd.NextDouble()}{rnd.Next(5000)}{unixTimestamp.ToString()}{DateTime.Now.Microsecond}{DateTime.Now.Day}{DateTime.Now.Month}{DateTime.Now.Year}";
        }





    }
}
