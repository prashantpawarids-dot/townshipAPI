using System;
using Township_API.Data;
using Township_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using static Township_API.Models.commonTypes;
using System.Linq;
using System.Text.Json;
using System.Diagnostics.Eventing.Reader;
using System.Collections.Generic;
using System.Security.Cryptography;


namespace Township_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly AppDBContext _context;

        public ProfileController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Profile>>> GetProfiles()
        {
            return await _context.Profiles.ToListAsync();
        }

        [HttpGet("GetProfile/{id}")]
        public async Task<ActionResult<IEnumerable<Profile>>> GetProfileByID(int id)
        {
            return await _context.Profiles.Where(p => p.ID == id).ToListAsync();
        }

        [HttpPost("DeleteProfile/{ID}")]
        public async Task<IActionResult> DeleteProfile(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Profile ID mismatch.");
                }
                var existingOBJ = await _context.Profiles.FindAsync(id);
                if (existingOBJ == null)
                {
                    return BadRequest("Profile ID not found.");
                }
                existingOBJ.isActive = false;
                existingOBJ.isDeleted = true;

                _context.Entry(existingOBJ).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                // return Ok(new { message = $"device with ID {existingOBJ.ProfileName}  inactive & deleted successfully" });
                // Example records
                var filteredRecords = _context.ProfileDetails.Where(r => r.profileid == id).ToList();

                // Update the status of filtered records
                foreach (var record in filteredRecords)
                {
                    // Update status 
                    record.isactive = false;
                    record.isdeleted = true;
                }
                return Ok(new { Message = $"Profile with Dependent ProfileDetails {filteredRecords.Count} data Deleted Successfully", ProfileName = existingOBJ.ProfileName });
            }
            catch (Exception ex)
            {
                //     await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("AddUpdateProfile")]
        public async Task<ActionResult<Profile>> AddUpdateProfile(Profile profile)
        {
            try
            {
                if (profile == null)
                    return BadRequest("No Data provided");

                if (profile.ID == 0 )
                {
                    //profile can not edit from add provision
                    

                    var _existingprofile = await _context.Profiles.Where(p => p.ProfileName == profile.ProfileName && profile.Uid==0).ToListAsync();
                    if (_existingprofile != null)
                    {
                        if (_existingprofile.Count > 0)
                            return BadRequest("Profile already exists!");
                    }

                    profile.isDeleted = false; //by default profile should active and deleted false at first time
                    _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblProfile ON");
                    _context.Profiles.Add(profile);
                    await _context.SaveChangesAsync();
                    _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblProfile OFF");

                    return Ok(profile);
                }

                if (profile.ID > 0)
                {
                    var _exists = await _context.Profiles.Where(p => p.ID != profile.ID && (p.Uid==0 &&  p.ProfileName == profile.ProfileName )).ToListAsync();
                    if (_exists != null)
                    {
                        if (_exists.Count > 0)
                            return BadRequest("Profile already exists!");
                    }

                    var existingobj = await _context.Profiles.Where(p => p.ID == profile.ID).FirstOrDefaultAsync();

                    if (existingobj != null)
                    {
                        existingobj.ProfileName = profile.ProfileName;
                        existingobj.Companyid = profile.Companyid;
                        existingobj.UpdatedOn = profile.UpdatedOn;
                        existingobj.UpdatedBy = profile.UpdatedBy;
                        existingobj.isActive = profile.isActive;
                        existingobj.isDeleted = profile.isDeleted;

                        await _context.SaveChangesAsync();
                    }
                    return Ok(existingobj);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Error: " + ex.Message.ToString());
                return BadRequest("Error :" + ex.Message.ToString());

            }
            return Ok(null);
        }

        [HttpGet("GetProfileDetails/{id}")]
        public async Task<ActionResult<IEnumerable<ProfileDetails>>> GetProfileDetails(string id)
        {
            try
            {
                //in case no profile id for this user then need to set default user profile for this user


                // Retrieve the ProfileDetails associated with the given ID
                var result = await _context.ProfileDetails
                    .IgnoreAutoIncludes()
                    .Where(p => p.profileid.ToString() == id.ToString())
                    .ToListAsync();

                if (result != null && result.Count > 0)
                {
                    // Retrieve all Modules
                    var nr = await _context.Modules.ToListAsync();

                    // Loop through ProfileDetails and check for matching modules
                    foreach (var res in result)
                    {
                        // Check if there is any module that matches the ProfileDetails (e.g., by ModuleID)
                        var matchingModule = nr.FirstOrDefault(module => module.ModuleID == res.moduleId);

                        if (matchingModule != null)
                        {
                            // Found a matching module, you can populate the details here
                            Console.WriteLine($"Matching module found for Profile: {res.profileid} -> Module: {matchingModule.ModuleName}");
                            var r = (res.moduleId == matchingModule.ModuleID);
                            // You can also attach additional module info to ProfileDetails here if needed
                            //res.ModuleName = matchingModule.ModuleName; // Example of populating the profile
                        }
                        else
                        {
                            // No matching module found
                            Console.WriteLine($"No matching module found for Profile: {res.profileid}");
                        }
                    }

                    // Return the updated profile details (with matching modules)
                    return Ok(result);
                }

                // If no profiles found
                return NotFound(new { message = "Profile not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        //[HttpGet("GetProfileDetails/{id}")]
        //public async Task<ActionResult<IEnumerable<ProfileDetails>>> GetProfileDetails(string id)
        //{
        //    try
        //    {
        //        // Retrieve the ProfileDetails associated with the given ID
        //        var result = await _context.ProfileDetails
        //            .IgnoreAutoIncludes()
        //            .Where(p => p.profileid.ToString() == id.ToString())
        //            .ToListAsync();

        //        if (result != null && result.Count > 0)
        //        {
        //            // Retrieve all Modules
        //            var nr = await _context.Modules.ToListAsync();

        //            // Loop through ProfileDetails and check for matching modules
        //            foreach (var res in result)
        //            {
        //                // Check if there is any module that matches the ProfileDetails (e.g., by ModuleID)
        //                var matchingModule = nr.FirstOrDefault(module => module.ModuleID == res.moduleId);

        //                if (matchingModule != null)
        //                {
        //                    // Found a matching module, you can populate the details here
        //                    Console.WriteLine($"Matching module found for Profile: {res.profileid} -> Module: {matchingModule.ModuleName}");

        //                    // You can also attach additional module info to ProfileDetails here if needed
        //                    res.moduleId = matchingModule.ModuleID; // Example of populating the profile

        //                }
        //                else
        //                {
        //                    // No matching module found
        //                    Console.WriteLine($"No matching module found for Profile: {res.profileid}");
        //                }
        //            }

        //            // Return the updated profile details (with matching modules)
        //            return Ok(result);
        //        }

        //        // If no profiles found
        //        return NotFound(new { message = "Profile not found." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { error = ex.Message });
        //    }
        //}

        [HttpGet("GetProfileDetails_tmp/{id}")]
        public async Task<ActionResult<IEnumerable<ProfileDetails>>> GetProfileDetails_tmp(string id)
        {
            try
            {
                ProfileDetails p = new ProfileDetails();

                // Perform join in memory
                var result = await _context.ProfileDetails.IgnoreAutoIncludes().Where(p => p.profileid.ToString() == id.ToString()).ToListAsync();
                if (result != null)
                {
                    if (result.Count > 0)
                    {
                        foreach (var res in result)
                        {
                            var nr = await _context.Modules.Where(u => u.ModuleID.ToString() == res.moduleId.ToString()).FirstOrDefaultAsync();
                            if (nr != null)
                            {
                                res.module.ModuleID = nr.ModuleID;
                                res.module.ModuleName = nr.ModuleName;
                            }
                        }

                        return Ok(result);
                    }
                }
                return Ok(null);

            }
            catch (Exception ex)
            {
                //   await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }

        }

        [HttpPost("AddUpdateProfileDetails/{Uid}")]
        public async Task<IActionResult> AddUpdateProfileDetails(int Uid, [FromBody] List<ProfileDetails> Obj)
        {
            int _uid = 0;
            _uid = Uid;
            int _profileid = 0;
            _profileid = Convert.ToInt16(Obj[0].profileid);

            if (Obj == null || !Obj.Any())
                return BadRequest("No Data provided");

            Console.WriteLine("UID: " + Uid.ToString());
            //check where profileid for username already exists in the database and moduleid then update else insert new record
            //for same profile and change the profileid  as per user
            if (_uid > 0)
            {
                var _existingprofile = await _context.Profiles.Where(p => p.Uid == _uid).ToListAsync();

                if (_existingprofile.Count == 0)
                {

                    var profile = await _context.Profiles.Where(p => p.ID == _profileid).FirstOrDefaultAsync();
                    if (profile != null)
                    {

                        Profile prProfile;

                        prProfile = new Profile()
                        {
                            ID = 0,
                            ProfileName = profile.ProfileName,
                            Companyid = profile.Companyid,
                            Uid = _uid,
                            CreatedBy = profile.CreatedBy,
                            CreatedOn = profile.CreatedOn,
                            isActive = profile.isActive,
                            isDeleted = profile.isDeleted
                        };

                        try
                        {
                            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblProfile ON");
                            _context.Profiles.Add(prProfile);
                            await _context.SaveChangesAsync();
                            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblProfile OFF");

                        }
                        catch (Exception Ex)
                        {
                            return Ok(new { message = $"BreakPoint Error :" + Ex.Message.ToString() });
                        }
                        Console.WriteLine(prProfile.ID.ToString());
                        _profileid = prProfile.ID;

                    }
                    else
                    {
                        return BadRequest("Profile ID not found.");
                    }
                }
            }

            //using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var objID in Obj)
                {
                    
                    var existingobj = await _context.ProfileDetails
                        .FirstOrDefaultAsync(p => p.profileid == _profileid && p.moduleId == objID.moduleId);

                    if (existingobj != null)
                    {
                        
                        // var existingLandOwner = await _service.UpdateLandownerAsync(objID.ID, existingobj);
                        //var existingProfilDtls = await _context.ProfileDetails.FindAsync(objID.id);
                        if (existingobj != null)
                        {

                            existingobj.moduleId = objID.moduleId;
                            existingobj.profileid = objID.profileid;
                            existingobj.CanInsert = objID.CanInsert;
                            existingobj.CanView = objID.CanView;

                            existingobj.CanUpdate = objID.CanUpdate;
                            existingobj.CanDelete = objID.CanDelete;
                            existingobj.CanView = objID.CanView;
                            existingobj.isactive = objID.isactive;
                            existingobj.updatedby = objID.updatedby;
                            existingobj.updatedon = objID.updatedon;
                            existingobj.isdeleted = objID.isdeleted;
                        }
                        await _context.SaveChangesAsync();
                    }
                    else
                    {

                        objID.profileid = _profileid;
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblProfileDetails ON");
                        _context.ProfileDetails.Add(objID);
                        await _context.SaveChangesAsync();
                        _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT tblProfileDetails OFF");
                        

                    }

                   
                    }
                //   await transaction.CommitAsync();

                return Ok(new { message = $"Records of Profile Details processed successfully" });
            }
            catch (Exception ex)
            {
                //   await transaction.RollbackAsync();
                return StatusCode(500, new { error = ex.Message });
            }

        }
       
        [HttpGet("GetModuleNames/{isViewOnly}")]
        public async Task<ActionResult<IEnumerable<_module>>> GetModuleNames(bool isViewOnly = false)
        {
            if (isViewOnly) 
                return await _context.Modules.Where(p => p.Viewreadonly == isViewOnly).IgnoreAutoIncludes().ToListAsync();
             
                return await _context.Modules.IgnoreAutoIncludes().ToListAsync();
        }

    }
}


