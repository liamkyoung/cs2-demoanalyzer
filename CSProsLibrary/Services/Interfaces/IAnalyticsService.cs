using CSProsLibrary.Models.Dtos.Viewmodels;

namespace CSProsLibrary.Services.Interfaces;

public interface IAnalyticsService
{
    Task<AppStatsDto> GetAppStats();
}