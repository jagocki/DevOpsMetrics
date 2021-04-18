﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevOpsMetrics.Core.DataAccess.TableStorage;
using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.Common;
using DevOpsMetrics.Core.Models.GitHub;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DevOpsMetrics.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly IAzureTableStorageDA AzureTableStorageDA;

        public SettingsController(IConfiguration configuration, IAzureTableStorageDA azureTableStorageDA)
        {
            Configuration = configuration;
            AzureTableStorageDA = azureTableStorageDA;
        }

        [HttpGet("GetAzureDevOpsSettings")]
        public List<AzureDevOpsSettings> GetAzureDevOpsSettings(string rowKey = null)
        {
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            List<AzureDevOpsSettings> settings = AzureTableStorageDA.GetAzureDevOpsSettingsFromStorage(tableStorageConfig, tableStorageConfig.TableAzureDevOpsSettings, rowKey);
            return settings;
        }

        [HttpGet("GetGitHubSettings")]
        public List<GitHubSettings> GetGitHubSettings(string rowKey = null)
        {
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            List<GitHubSettings> settings = AzureTableStorageDA.GetGitHubSettingsFromStorage(tableStorageConfig, tableStorageConfig.TableGitHubSettings, rowKey);
            return settings;
        }

        [HttpGet("UpdateAzureDevOpsSetting")]
        public async Task<bool> UpdateAzureDevOpsSetting(string patToken,
                string organization, string project, string repository,
                string branch, string buildName, string buildId, string resourceGroup, int itemOrder)
        {
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            return await AzureTableStorageDA.UpdateAzureDevOpsSettingInStorage(patToken, tableStorageConfig, tableStorageConfig.TableAzureDevOpsSettings,
                     organization, project, repository, branch, buildName, buildId, resourceGroup, itemOrder);
        }

        [HttpGet("UpdateGitHubSetting")]
        public async Task<bool> UpdateGitHubSetting(string clientId, string clientSecret,
                string owner, string repo,
                string branch, string workflowName, string workflowId, string resourceGroup, int itemOrder)
        {
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            return await AzureTableStorageDA.UpdateGitHubSettingInStorage(clientId, clientSecret, tableStorageConfig, tableStorageConfig.TableGitHubSettings,
                    owner, repo, branch, workflowName, workflowId, resourceGroup, itemOrder);
        }

        [HttpPost("UpdateDevOpsMonitoringEvent")]
        public async Task<bool> UpdateDevOpsMonitoringEvent([FromBody] MonitoringEvent monitoringEvent)
        {
            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            return await AzureTableStorageDA.UpdateDevOpsMonitoringEventInStorage(tableStorageConfig, monitoringEvent);
        }

        [HttpGet("GetAzureDevOpsProjectLog")]
        public List<ProjectLog> GetAzureDevOpsProjectLog(string organization, string project, string repository)
        {
            string partitionKey = PartitionKeys.CreateAzureDevOpsSettingsPartitionKey(organization, project, repository);

            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            return AzureTableStorageDA.GetProjectLogsFromStorage(tableStorageConfig, partitionKey);
        }

        [HttpGet("UpdateAzureDevOpsProjectLog")]
        public async Task<bool> UpdateAzureDevOpsProjectLog(string organization, string project, string repository,
            int buildsUpdated, int prsUpdated, string exceptionMessage, string exceptionStackTrace)
        {
            ProjectLog log = new ProjectLog(
                PartitionKeys.CreateAzureDevOpsSettingsPartitionKey(organization, project, repository),
                buildsUpdated, prsUpdated, exceptionMessage, exceptionStackTrace);

            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            return await AzureTableStorageDA.UpdateProjectLogInStorage(tableStorageConfig, log);
        }

        [HttpGet("GetGitHubProjectLog")]
        public List<ProjectLog> GetGitHubProjectLog(string owner, string repo)
        {
            string partitionKey = PartitionKeys.CreateGitHubSettingsPartitionKey(owner, repo);

            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            return AzureTableStorageDA.GetProjectLogsFromStorage(tableStorageConfig, partitionKey);
        }

        [HttpGet("UpdateGitHubProjectLog")]
        public async Task<bool> UpdateGitHubProjectLog(string owner, string repo,
            int buildsUpdated, int prsUpdated, string exceptionMessage, string exceptionStackTrace)
        {
            ProjectLog log = new ProjectLog(
                PartitionKeys.CreateGitHubSettingsPartitionKey(owner, repo),
                buildsUpdated, prsUpdated, exceptionMessage, exceptionStackTrace);

            TableStorageConfiguration tableStorageConfig = Common.GenerateTableStorageConfiguration(Configuration);
            return await AzureTableStorageDA.UpdateProjectLogInStorage(tableStorageConfig, log);
        }

    }
}