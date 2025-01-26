# JobManager - Central Scheduled Task Manager

**[For Turkish version, click here.](./README_TR.md)**

## 📜 Project Overview

JobManager is a **centralized scheduled task manager** designed to speed up development by automating job management. It provides **job grouping**, **dynamic job handling and execution** and **structured logging**.

### 🚀 Key Focus Areas:
- **Job Grouping & Segregation:**  
  - Services operate with their own set of jobs and cannot execute jobs from other groups.
  - Managed via `appsettings.json` and environment variables.
  - A single codebase supports all job groups.

- **Dynamic Job Execution:**  
  - Jobs can be executed via **Swagger**, independent of the Hangfire dashboard.
  - Supports **dynamic parameters**, retrieved based on naming conventions.

- **Advanced Job Scheduling & Control:**  
  - Update job schedules (all jobs or individual jobs).
  - Supports predefined **default times, disable and remove actions**.

- **Custom Logging & Monitoring:**  
  - Logs include job execution details, debug mode status, and environment information.
  - Uses **Elasticsearch** and **Serilog** for structured logging.

- **Automatic Dependency Injection:**  
  - All components are injected **except services**, which require manual injection due to cross-dependencies.
  - Jobs, handlers, and recurring tasks are automatically registered.

## 🛠️ Technologies Used

- **Hangfire** (Job Scheduling)
- **Elasticsearch** (Logging & Monitoring)
- **Serilog** (Structured Logging)
- **Swagger Integration** (Dynamic job execution & configuration)

## 🌐 API Endpoints

### 🔄 Job Execution & Management

```http
POST /StartJobByJobName
```
**Description:** Dynamically starts a job by its name.

```http
GET /GetJobParametersJobByName
```
**Description:** Retrieves job parameters dynamically.

```http
PATCH /UpdateJobTimeByName
```
**Description:** Updates the cron expression for a specific job.

```http
PATCH /UpdateAllJobsWithDefaultTimes
```
**Description:** Updates all jobs to their default schedule from `appsettings.json`.

```http
DELETE /DeactivateAllJobs
```
**Description:** Disables all scheduled jobs.

```http
DELETE /RemoveAllJobs
```
**Description:** Removes all recurring jobs from the Hangfire scheduler.

### 📜 Configuration & Constants

```http
GET /JobSettings
```
**Description:** Retrieves active job settings, default times, and debugging information.

```http
GET /AppSettings
```
**Description:** Retrieves application environment and project settings.

```http
GET /AppConstants
```
**Description:** Fetches all constants defined under `Constants` folder in `appsettings.json`.

## 🏗️ System Workflow

1. **Job Initialization & Grouping**
   - Jobs are dynamically loaded based on `appsettings.json` and **PROJECT** environment variable.
   - Each service is restricted to its own set of jobs.

2. **Job Execution & Scheduling**
   - Jobs can be executed via **Swagger** with dynamic parameters.
   - Supports **default cron schedules, disabling jobs (impossible time: 31st Feb), and removal**.

3. **Dependency Injection & Logging**
   - **Automatic injection** for jobs, handlers, and recurring jobs.
   - **Manual injection** required for services due to cross-dependencies.
   - **Structured logging** tracks job execution, mode, and parameters.
