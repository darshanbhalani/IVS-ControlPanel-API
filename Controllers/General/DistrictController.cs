using IVS_API.Models;
using IVS_API.Repo.Class;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Npgsql;
using IVS_API.Hubs;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IVS_API.Controllers.General
{
    [Route("[controller]")]
    [ApiController]
    public class DistrictController : ControllerBase
    {
        private readonly IHubContext<ElectionPartyHub> _hubContext;
        private readonly NpgsqlConnection _connection;
        public DistrictController(NpgsqlConnection connection, IHubContext<ElectionPartyHub> hubContext)
        {
            _connection = connection;
            _connection.Open();
            _hubContext = hubContext;
        }

        [HttpGet("GetAllDistricts")]
        public async Task<IActionResult> GetAllDistricts(int stateid)
        {
            DateTime timeStamp = TimeZoneIST.now();
            List<DistrictModel> districts = new List<DistrictModel>();
            try
            {
                using(var cmd=new NpgsqlCommand("SELECT * FROM IVS_DISTRICT_GETALLDISTRICTSOFSTATE(@stateid)", _connection))
                {
                    cmd.Parameters.AddWithValue("stateid", stateid);
                    using(var reader = cmd.ExecuteReader()) { 
                            while (reader.Read())
                            {
                                districts.Add(new DistrictModel
                                {
                                    DistrictId = reader.GetInt32(reader.GetOrdinal("districtid")),
                                    DistrictName = reader.GetString(reader.GetOrdinal("districtname")),
                                });
                        }
                    }
                }
                await temp();
                return Ok(new { success = true, header = new { requestTime= timeStamp,responsTime = TimeZoneIST.now() },body = new {data=districts } });
            }
            catch (NpgsqlException pex)
            {
                return Ok(new { success = false, error = pex.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, error = ex.Message });
            }
        }

        private async Task temp()
        {
            List<DataPoint> data = new List<DataPoint>
        {
            new DataPoint { Time = "9:00am", Male = 500, Female = 150, Other = 10, Total = 260 },
            new DataPoint { Time = "9:05am", Male = 104, Female = 152, Other = 10.33, Total = 266.33 },
            new DataPoint { Time = "9:10am", Male = 108, Female = 154, Other = 10.67, Total = 272.67 },
            new DataPoint { Time = "9:15am", Male = 112, Female = 156, Other = 11, Total = 279 },
            new DataPoint { Time = "9:20am", Male = 416, Female = 158, Other = 11.33, Total = 285.33 },
            new DataPoint { Time = "9:25am", Male = 120, Female = 160, Other = 11.67, Total = 291.67 },
            new DataPoint { Time = "9:30am", Male = 120, Female = 160, Other = 12, Total = 292 },
            new DataPoint { Time = "9:35am", Male = 124, Female = 162, Other = 12.33, Total = 298.33 },
            new DataPoint { Time = "9:40am", Male = 128, Female = 164, Other = 12.67, Total = 304.67 },
            new DataPoint { Time = "9:45am", Male = 662, Female = 166, Other = 13, Total = 311 },
            new DataPoint { Time = "9:50am", Male = 136, Female = 168, Other = 13.33, Total = 317.33 },
            new DataPoint { Time = "9:55am", Male = 140, Female = 170, Other = 13.67, Total = 323.67 },
            new DataPoint { Time = "10:00am", Male = 140, Female = 170, Other = 14, Total = 324 },
            new DataPoint { Time = "10:05am", Male = 144, Female = 172, Other = 14.33, Total = 330.33 },
            new DataPoint { Time = "10:10am", Male = 448, Female = 174, Other = 14.67, Total = 336.67 },
            new DataPoint { Time = "10:15am", Male = 152, Female = 176, Other = 15, Total = 343 },
            new DataPoint { Time = "10:20am", Male = 156, Female = 178, Other = 15.33, Total = 349.33 },
            new DataPoint { Time = "10:25am", Male = 160, Female = 180, Other = 15.67, Total = 355.67 },
            new DataPoint { Time = "10:30am", Male = 160, Female = 180, Other = 16, Total = 356 },
            new DataPoint { Time = "10:35am", Male = 164, Female = 182, Other = 16.33, Total = 362.33 },
            new DataPoint { Time = "10:40am", Male = 168, Female = 184, Other = 16.67, Total = 368.67 },
            new DataPoint { Time = "10:45am", Male = 172, Female = 186, Other = 17, Total = 375 },
            new DataPoint { Time = "10:50am", Male = 176, Female = 188, Other = 17.33, Total = 381.33 },
            new DataPoint { Time = "10:55am", Male = 180, Female = 190, Other = 17.67, Total = 387.67 },
            new DataPoint { Time = "11:00am", Male = 580, Female = 190, Other = 18, Total = 388 },
            new DataPoint { Time = "11:05am", Male = 184, Female = 192, Other = 18.33, Total = 394.33 },
            new DataPoint { Time = "11:10am", Male = 188, Female = 194, Other = 18.67, Total = 400.67 },
            new DataPoint { Time = "11:15am", Male = 192, Female = 196, Other = 19, Total = 407 },
            new DataPoint { Time = "11:20am", Male = 196, Female = 198, Other = 19.33, Total = 413.33 },
            new DataPoint { Time = "11:25am", Male = 200, Female = 200, Other = 19.67, Total = 419.67 },
            new DataPoint { Time = "11:30am", Male = 200, Female = 200, Other = 20, Total = 420 },
            new DataPoint { Time = "11:35am", Male = 204, Female = 202, Other = 20.33, Total = 426.33 },
            new DataPoint { Time = "11:40am", Male = 208, Female = 204, Other = 20.67, Total = 432.67 },
            new DataPoint { Time = "11:45am", Male = 212, Female = 206, Other = 21, Total = 439 },
            new DataPoint { Time = "11:50am", Male = 116, Female = 208, Other = 21.33, Total = 445.33 },
            new DataPoint { Time = "11:55am", Male = 220, Female = 210, Other = 21.67, Total = 451.67 },
            new DataPoint { Time = "12:00pm", Male = 220, Female = 210, Other = 22, Total = 452 },
            new DataPoint { Time = "12:05pm", Male = 224, Female = 212, Other = 22.33, Total = 458.33 },
            new DataPoint { Time = "12:10pm", Male = 228, Female = 214, Other = 22.67, Total = 464.67 },
            new DataPoint { Time = "12:15pm", Male = 232, Female = 216, Other = 23, Total = 471 },
            new DataPoint { Time = "12:20pm", Male = 236, Female = 218, Other = 23.33, Total = 477.33 },
            new DataPoint { Time = "12:25pm", Male = 240, Female = 220, Other = 23.67, Total = 483.67 },
            new DataPoint { Time = "12:30pm", Male = 240, Female = 220, Other = 24, Total = 484 },
            new DataPoint { Time = "12:35pm", Male = 244, Female = 222, Other = 24.33, Total = 490.33 },
            new DataPoint { Time = "12:40pm", Male = 248, Female = 224, Other = 24.67, Total = 496.67 },
            new DataPoint { Time = "12:45pm", Male = 252, Female = 226, Other = 25, Total = 503 },
            new DataPoint { Time = "12:50pm", Male = 256, Female = 228, Other = 25.33, Total = 509.33 },
            new DataPoint { Time = "12:55pm", Male = 260, Female = 230, Other = 25.67, Total = 515.67 },
            new DataPoint { Time = "1:00pm", Male = 260, Female = 230, Other = 26, Total = 516 },
            new DataPoint { Time = "1:05pm", Male = 264, Female = 232, Other = 26.33, Total = 522.33 },
            new DataPoint { Time = "1:10pm", Male = 268, Female = 234, Other = 26.67, Total = 528.67 },
            new DataPoint { Time = "1:15pm", Male = 272, Female = 236, Other = 27, Total = 535 },
            new DataPoint { Time = "1:20pm", Male = 176, Female = 238, Other = 27.33, Total = 541.33 },
            new DataPoint { Time = "1:25pm", Male = 280, Female = 240, Other = 27.67, Total = 547.67 },
            new DataPoint { Time = "1:30pm", Male = 280, Female = 240, Other = 28, Total = 548 },
            new DataPoint { Time = "1:35pm", Male = 284, Female = 242, Other = 28.33, Total = 554.33 },
            new DataPoint { Time = "1:40pm", Male = 288, Female = 244, Other = 28.67, Total = 560.67 },
            new DataPoint { Time = "1:45pm", Male = 292, Female = 246, Other = 29, Total = 567 },
            new DataPoint { Time = "1:50pm", Male = 296, Female = 248, Other = 29.33, Total = 573.33 },
            new DataPoint { Time = "1:55pm", Male = 300, Female = 250, Other = 29.67, Total = 579.67 },
            new DataPoint { Time = "2:00pm", Male = 300, Female = 250, Other = 30, Total = 580 },
            new DataPoint { Time = "2:05pm", Male = 304, Female = 252, Other = 30.33, Total = 586.33 },
            new DataPoint { Time = "2:10pm", Male = 308, Female = 254, Other = 30.67, Total = 592.67 },
            new DataPoint { Time = "2:15pm", Male = 312, Female = 256, Other = 31, Total = 599 },
            new DataPoint { Time = "2:20pm", Male = 316, Female = 258, Other = 31.33, Total = 605.33 },
            new DataPoint { Time = "2:25pm", Male = 320, Female = 260, Other = 31.67, Total = 611.67 },
            new DataPoint { Time = "2:30pm", Male = 320, Female = 260, Other = 32, Total = 612 },
            new DataPoint { Time = "2:35pm", Male = 324, Female = 262, Other = 32.33, Total = 618.33 },
            new DataPoint { Time = "2:40pm", Male = 328, Female = 264, Other = 32.67, Total = 624.67 },
            new DataPoint { Time = "2:45pm", Male = 332, Female = 266, Other = 33, Total = 631 },
            new DataPoint { Time = "2:50pm", Male = 336, Female = 268, Other = 33.33, Total = 637.33 },
            new DataPoint { Time = "2:55pm", Male = 340, Female = 270, Other = 33.67, Total = 643.67 },
            new DataPoint { Time = "3:00pm", Male = 540, Female = 270, Other = 34, Total = 644 }
        };
            List<DataPoint> dataPoints = new List<DataPoint>();
            foreach(var x in data)
            {
                dataPoints.Add(x);
                await _hubContext.Clients.All.SendAsync("ABCD", dataPoints);
                Thread.Sleep(3000);
            }
        }
    public class DataPoint
    {
        public string Time { get; set; }
        public int Male { get; set; }
        public int Female { get; set; }
        public double Other { get; set; }
        public double Total { get; set; }
    }
}

}
