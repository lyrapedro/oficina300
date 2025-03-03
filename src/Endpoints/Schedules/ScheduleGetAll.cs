﻿using MechShops.Infra.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace MechShops.Endpoints.Schedules;

public class ScheduleGetAll
{
    public static string Template => "/schedules";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "ShopPolicy")]
    public static IResult Action(HttpContext http, ApplicationDbContext context)
    {
        var shopId = http.User.Claims.First(c => c.Type == "ShopId").Value;

        var currentDate = DateTime.Now;
        int numberOfFutureDays = 5;
        var maxDate = currentDate.AddDays(numberOfFutureDays);

        var schedules = context.Schedules.Where(s => s.ShopId == shopId).ToList();
        schedules = schedules.Where(s => s.Date.Date >= currentDate.Date && s.Date.Date <= maxDate.Date).ToList();

        var schedulesIds = schedules.Select(s => s.Id).ToList();

        List<ScheduleResponse> response = new List<ScheduleResponse>();

        var demands = context.Demands.Include(d => d.Service).Where(d => schedulesIds.Contains(d.ScheduleId)).ToList();

        if(demands.Any())
            response = schedules.Select(s => new ScheduleResponse(s.Id, s.Date.Date.ToString("dd/MM/yy"), s.ModifiedAt, s.CreatedAt,
                demands.Where(d => d.ScheduleId == s.Id).Select(d => d.Service.Name).ToList())).ToList();

        response.OrderBy(r => r.date).ToList();

        return Results.Ok(response);
    }
}
