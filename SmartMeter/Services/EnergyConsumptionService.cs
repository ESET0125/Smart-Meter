//using Microsoft.EntityFrameworkCore;
//using SmartMeter.Data;
//using SmartMeter.Models;
//using SmartMeter.Models.DTOs;
//using SmartMeter.Services.Interface;

//namespace SmartMeter.Services.Implementation
//{
//    public class EnergyConsumptionService : IEnergyConsumptionService
//    {
//        private readonly SmartMeterDbContext _context;
//        private readonly ILogger<EnergyConsumptionService> _logger;


//        public EnergyConsumptionService(SmartMeterDbContext context, ILogger<EnergyConsumptionService> logger)
//        {
//            _context = context;
//            _logger = logger;
//        }

//        //public async Task<bool> RecordEnergyConsumptionAsync(EnergyConsumptionRecordDto record)
//        //{
//        //    try
//        //    {
//        //        // Validate meter exists and is active
//        //        var meter = await _context.Meters
//        //            .FirstOrDefaultAsync(m => m.MeterSerialNo == record.MeterSerialNo && m.Status == "Active");

//        //        if (meter == null)
//        //        {
//        //            _logger.LogWarning("Attempt to record consumption for invalid meter: {MeterSerialNo}", record.MeterSerialNo);
//        //            return false;
//        //        }

//        //        // Validate energy consumption is not negative
//        //        if (record.EnergyConsumed < 0)
//        //        {
//        //            _logger.LogWarning("Invalid energy consumption value: {EnergyConsumed}", record.EnergyConsumed);
//        //            return false;
//        //        }

//        //        // Create new meter reading
//        //        var meterReading = new Meterreading
//        //        {
//        //            MeterId = record.MeterSerialNo,
//        //            MeterReadingDate = record.ReadingDate,
//        //            EnergyConsumed = record.EnergyConsumed,
//        //            Voltage = 0, // Will be updated by billing parameters
//        //            Current = 0, // Will be updated by billing parameters
//        //            PowerFactor = 0.85m // Default value
//        //        };

//        //        await _context.Meterreadings.AddAsync(meterReading);
//        //        await _context.SaveChangesAsync();

//        //        _logger.LogInformation("Recorded energy consumption: {EnergyConsumed}kWh for meter {MeterSerialNo}",
//        //            record.EnergyConsumed, record.MeterSerialNo);

//        //        return true;
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        _logger.LogError(ex, "Error recording energy consumption for meter {MeterSerialNo}", record.MeterSerialNo);
//        //        return false;
//        //    }
//        //}
//        //public async Task<bool> RecordEnergyConsumptionAsync(EnergyConsumptionRecordDto record)
//        //{
//        //    try
//        //    {
//        //        _logger.LogInformation("Attempting to record consumption for meter: {MeterSerialNo}", record.MeterSerialNo);

//        //        // Validate meter exists and is active
//        //        var meter = await _context.Meters
//        //            .FirstOrDefaultAsync(m => m.MeterSerialNo == record.MeterSerialNo && m.Status == "Active");

//        //        if (meter == null)
//        //        {
//        //            _logger.LogWarning("Meter not found or inactive: {MeterSerialNo}", record.MeterSerialNo);
//        //            return false;
//        //        }

//        //        _logger.LogInformation("Meter found: {MeterSerialNo}, creating reading...", record.MeterSerialNo);

//        //        // Create new meter reading
//        //        var meterReading = new Meterreading
//        //        {
//        //            MeterId = record.MeterSerialNo,
//        //            MeterReadingDate = record.ReadingDate,
//        //            EnergyConsumed = record.EnergyConsumed,
//        //            Voltage = 0, // Default
//        //            Current = 0, // Default  
//        //            PowerFactor = 0.85m // Default
//        //        };

//        //        await _context.Meterreadings.AddAsync(meterReading);
//        //        await _context.SaveChangesAsync();

//        //        _logger.LogInformation("Successfully recorded energy consumption");
//        //        return true;
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        _logger.LogError(ex, "Error recording energy consumption for meter {MeterSerialNo}", record.MeterSerialNo);
//        //        return false;
//        //    }
//        //}


//        public async Task<bool> RecordEnergyConsumptionAsync(EnergyConsumptionRecordDto record)
//        {
//            try
//            {
//                _logger.LogInformation("Received record: Meter={Meter}, Date={Date}, Energy={Energy}",
//                    record.MeterSerialNo, record.ReadingDate, record.EnergyConsumed);

//                // Check if meter exists
//                var meter = await _context.Meters
//                    .FirstOrDefaultAsync(m => m.Meterserialno == record.MeterSerialNo);

//                _logger.LogInformation(" Meter lookup result: {Result}", meter != null ? "FOUND" : "NOT FOUND");

//                if (meter == null)
//                {
//                    _logger.LogError("Meter {MeterSerialNo} does not exist in database", record.MeterSerialNo);
//                    return false;
//                }

//                _logger.LogInformation(" Meter status: {Status}, Consumer: {ConsumerId}", meter.Status, meter.Consumerid);

//                if (meter.Status != "Active")
//                {
//                    _logger.LogError("Meter {MeterSerialNo} is not active. Status: {Status}", record.MeterSerialNo, meter.Status);
//                    return false;
//                }

//                // Try to create the reading
//                var meterReading = new Meterreading
//                {
//                    MeterId = record.MeterSerialNo,
//                    MeterReadingDate = record.ReadingDate,
//                    EnergyConsumed = record.EnergyConsumed,
//                    Voltage = 0,
//                    Current = 0,
//                    PowerFactor = 0.85m
//                };

//                _logger.LogInformation("Attempting to save to database...");

//                await _context.Meterreadings.AddAsync(meterReading);
//                await _context.SaveChangesAsync();

//                _logger.LogInformation("SUCCESS: Energy consumption recorded!");
//                return true;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "" +
//                    "CRITICAL ERROR: {ErrorMessage}", ex.Message);
//                return false;
//            }
//        }

//        public async Task<List<EnergyConsumptionResponseDto>> GetEnergyConsumptionAsync(EnergyConsumptionQueryDto query, int userId)
//        {
//            // Verify user has access to this meter
//            var canAccess = await _context.Meters
//                .Include(m => m.Consumer)
//                .AnyAsync(m => m.Meterserialno == query.MeterSerialNo
//                            && m.Status == "Active"
//                            && m.Consumer.Status == "Active");

//            if (!canAccess)
//                throw new UnauthorizedAccessException("Access to this meter's data is not authorized");

//            var readings = await _context.Meterreadings
//                .Where(mr => mr.MeterId == query.MeterSerialNo
//                          && mr.MeterReadingDate >= query.FromDate
//                          && mr.MeterReadingDate <= query.ToDate)
//                .OrderBy(mr => mr.MeterReadingDate)
//                .ToListAsync();

//            var response = new List<EnergyConsumptionResponseDto>();
//            decimal cumulative = 0;

//            foreach (var reading in readings)
//            {
//                cumulative += (decimal)reading.EnergyConsumed;
//                response.Add(new EnergyConsumptionResponseDto
//                {
//                    ReadingDate = reading.MeterReadingDate,
//                    EnergyConsumed = (decimal)reading.EnergyConsumed,


//                    CumulativeConsumption = cumulative
//                });
//            }

//            return response;
//        }

//        public async Task<decimal> GetTotalConsumptionAsync(string meterSerialNo, DateTime fromDate, DateTime toDate, int userId)
//        {
//            // Verify user has access to this meter
//            var canAccess = await _context.Meters
//                .Include(m => m.Consumerid)
//                .AnyAsync(m => m.Meterserialno == meterSerialNo
//                            && m.Status == "Active"
//                            && m.Consumer.Status == "Active");

//            if (!canAccess)
//                throw new UnauthorizedAccessException("Access to this meter's data is not authorized");

//            var total = await _context.Meterreadings
//                .Where(mr => mr.MeterId == meterSerialNo
//                          && mr.MeterReadingDate >= fromDate
//                          && mr.MeterReadingDate <= toDate)
//                .SumAsync(mr => mr.EnergyConsumed);

//            return (decimal)total;
//        }
//        //public Task<List<EnergyConsumptionResponseDto>> GetEnergyConsumptionAsync(EnergyConsumptionQueryDto query, int userId)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        //public Task<decimal> GetTotalConsumptionAsync(string meterSerialNo, DateTime fromDate, DateTime toDate, int userId)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        //public Task<bool> RecordEnergyConsumptionAsync(EnergyConsumptionRecordDto record)
//        //{
//        //    throw new NotImplementedException();
//        //}
//    }
//}

using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;
using SmartMeter.Data;
using SmartMeter.Models;
using SmartMeter.Models.DTOs;
using SmartMeter.Models.DTOs.EnergyConsumptionDto;
using SmartMeter.Services.Interface;

namespace SmartMeter.Services.Implementation
{
    public class EnergyConsumptionService : IEnergyConsumptionService
    {
        private readonly Data.SmartMeterDbContext _context;
        private readonly ILogger<EnergyConsumptionService> _logger;

        public EnergyConsumptionService(Data.SmartMeterDbContext context, ILogger<EnergyConsumptionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> RecordEnergyConsumptionAsync(EnergyConsumptionRecordDto record)
        {
            try
            {
                _logger.LogInformation("Recording energy consumption for meter: {Meter}", record.MeterSerialNo);

                // Check if meter exists and is active
                var meter = await _context.Meters
                    .FirstOrDefaultAsync(m => m.Meterserialno == record.MeterSerialNo && m.Status == "Active");

                if (meter == null)
                {
                    _logger.LogWarning("Meter not found or inactive: {MeterSerialNo}", record.MeterSerialNo);
                    return false;
                }

                // Validate energy consumption is not negative
                if (record.EnergyConsumed < 0)
                {
                    _logger.LogWarning("Invalid energy consumption value: {EnergyConsumed}", record.EnergyConsumed);
                    return false;
                }

                // Create new meter reading
                var meterReading = new Meterreading
                {
                    Meterid = record.MeterSerialNo,
                    Meterreadingdate = DateTime.Parse(record.ReadingDate),
                    Energyconsumed = record.EnergyConsumed,
                    Voltage = 0,
                    Current = 0,
                    //Powerfactor = 0.85m
                };

                await _context.Meterreadings.AddAsync(meterReading);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully recorded {EnergyConsumed}kWh for meter {MeterSerialNo}",
                    record.EnergyConsumed, record.MeterSerialNo);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording energy consumption for meter {MeterSerialNo}", record.MeterSerialNo);
                return false;
            }
        }

        //public async Task<List<EnergyConsumptionResponseDto>> GetEnergyConsumptionAsync(EnergyConsumptionQueryDto query, int userId)
        //{
        //    try
        //    {
        //        _logger.LogInformation("Getting energy consumption for meter: {Meter}, User: {User}",
        //            query.MeterSerialNo, userId);

        //        // Verify user has access to this meter
        //        var canAccess = await CanAccessMeterAsync(query.MeterSerialNo, userId);
        //        if (!canAccess)
        //        {
        //            _logger.LogWarning("Access denied for user {UserId} to meter {Meter}", userId, query.MeterSerialNo);
        //            throw new UnauthorizedAccessException("Access to this meter's data is not authorized");
        //        }

        //        var readings = await _context.Meterreadings
        //            .Where(mr => mr.Meterid == query.MeterSerialNo
        //                      && mr.Meterreadingdate >= DateTime.SpecifyKind(DateOnly.Parse(query.FromDate).ToDateTime(TimeOnly.MinValue),DateTimeKind.Utc)
        //                      && mr.Meterreadingdate <= DateTime.SpecifyKind(DateOnly.Parse(query.ToDate).ToDateTime(TimeOnly.MinValue),DateTimeKind.Utc))
        //            .OrderBy(mr => mr.Meterreadingdate)
        //            .ToListAsync();

        //        _logger.LogInformation("Found {Count} readings for meter {Meter}", readings.Count, query.MeterSerialNo);

        //        var response = new List<EnergyConsumptionResponseDto>();
        //        decimal cumulative = 0;

        //        foreach (var reading in readings)
        //        {
        //            cumulative += reading.Energyconsumed ?? 0;
        //            response.Add(new EnergyConsumptionResponseDto
        //            {
        //                ReadingDate = reading.Meterreadingdate.ToString(),
        //                EnergyConsumed = reading.Energyconsumed ?? 0,
        //                CumulativeConsumption = cumulative
        //            });
        //        }

        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error getting energy consumption for meter {MeterSerialNo}", query.MeterSerialNo);
        //        throw;
        //    }
        //}




        public async Task<List<EnergyConsumptionResponseDto>> GetEnergyConsumptionAsync(EnergyConsumptionQueryDto query, int userId)
        {
            try
            {
                _logger.LogInformation("Getting energy consumption for meter: {Meter}, User: {User}",
                    query.MeterSerialNo, userId);

                // Verify user has access to this meter
                var canAccess = await CanAccessMeterAsync(query.MeterSerialNo, userId);
                if (!canAccess)
                {
                    _logger.LogWarning("Access denied for user {UserId} to meter {Meter}", userId, query.MeterSerialNo);
                    throw new UnauthorizedAccessException("Access to this meter's data is not authorized");
                }

                var readings = await _context.Meterreadings
                    .Where(mr => mr.Meterid == query.MeterSerialNo
                              && mr.Meterreadingdate >= DateTime.SpecifyKind(DateOnly.Parse(query.FromDate).ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc)
                              && mr.Meterreadingdate <= DateTime.SpecifyKind(DateOnly.Parse(query.ToDate).ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc))
                    .OrderBy(mr => mr.Meterreadingdate)
                    .ToListAsync();

                _logger.LogInformation("Found {Count} readings for meter {Meter}", readings.Count, query.MeterSerialNo);

                // Remove cumulative calculation - just return individual readings
                var response = readings.Select(reading => new EnergyConsumptionResponseDto
                {
                    ReadingDate = reading.Meterreadingdate.ToString("dd-MM-yyyy HH:mm:ss"),
                    EnergyConsumed = reading.Energyconsumed ?? 0
                    // Remove CumulativeConsumption line
                }).ToList();

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting energy consumption for meter {MeterSerialNo}", query.MeterSerialNo);
                throw;
            }
        }

        public async Task<decimal> GetTotalConsumptionAsync(string meterSerialNo, DateTime fromDate, DateTime toDate, int userId)
        {
            try
            {
                _logger.LogInformation("Getting total consumption for meter: {Meter}, User: {User}",
                    meterSerialNo, userId);

                // Verify user has access to this meter
                var canAccess = await CanAccessMeterAsync(meterSerialNo, userId);
                if (!canAccess)
                {
                    _logger.LogWarning("Access denied for user {UserId} to meter {Meter}", userId, meterSerialNo);
                    throw new UnauthorizedAccessException("Access to this meter's data is not authorized");
                }

                var total = await _context.Meterreadings
                    .Where(mr => mr.Meterid == meterSerialNo
                             && mr.Meterreadingdate >= DateTime.SpecifyKind(fromDate, DateTimeKind.Utc)
                             && mr.Meterreadingdate <= DateTime.SpecifyKind(toDate, DateTimeKind.Utc))
                    .SumAsync(mr => mr.Energyconsumed);

                _logger.LogInformation("Total consumption for meter {Meter}: {Total}kWh", meterSerialNo, total);

                return total ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total consumption for meter {MeterSerialNo}", meterSerialNo);
                throw;
            }
        }

        private async Task<bool> CanAccessMeterAsync(string meterSerialNo, int userId)
        {
            try
            {
                _logger.LogInformation("Checking access for UserId: {UserId}, Meter: {Meter}", userId, meterSerialNo);

                // Check if meter exists and is active
                var meterExists = await _context.Meters
                    .AnyAsync(m => m.Meterserialno == meterSerialNo && m.Status == "Active");

                _logger.LogInformation("Meter {Meter} access result: {Result}", meterSerialNo, meterExists);

                // TEMPORARY: Allow access to any active meter
                // TODO: Implement proper User-Consumer relationship
                return meterExists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking meter access for UserId: {UserId}, Meter: {Meter}", userId, meterSerialNo);
                return false;
            }
        }
    }
}