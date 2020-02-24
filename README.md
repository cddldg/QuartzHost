# QuartzHost
Quart.Net 在dotnetcore3.1平台的实现  
![GitHub](https://img.shields.io/github/license/cddldg/QuartzHost)

## 主要实现
任务管理：启动、停止、新建、暂停、恢复等;   
ASP.NET Core Blazor展现（进行中）;  
任务支持参数;    
全API支持;   
## 特点
任务插件式运行;  
实现Job不需要引用Quart.Net;  
任务支持异步;   
集成NLog;   
可多节点部署;   

## 技术栈 
Asp.Net Core3.1、Dapper2.0、MSSQL、Quartz.Net、ASP.NET Core Blazor ...

## Nuget Packages

| Package Name |  Version | Downloads
|--------------|  ------- | ----  
| QuartzHost.Core | [![QuartzHost.Core](https://img.shields.io/nuget/v/QuartzHost.Core)](https://www.nuget.org/packages/QuartzHost.Core/) | [![QuartzHost.Core](https://img.shields.io/nuget/dt/QuartzHost.Core)](https://www.nuget.org/packages/QuartzHost.Core/)  
| QuartzHost.Base | [![QuartzHost.Base](https://img.shields.io/nuget/v/QuartzHost.Base)](https://www.nuget.org/packages/QuartzHost.Base/) | [![QuartzHost.Base](https://img.shields.io/nuget/dt/QuartzHost.Base)](https://www.nuget.org/packages/QuartzHost.Base/)  
| DG.Dapper | [![DG.Dapper](https://img.shields.io/nuget/v/DG.Dapper)](https://www.nuget.org/packages/DG.Dapper/) | [![DG.Dapper](https://img.shields.io/nuget/dt/DG.Dapper)](https://www.nuget.org/packages/DG.Dapper/)  
| DG.Logger | [![DG.Logger](https://img.shields.io/nuget/v/DG.Logger)](https://www.nuget.org/packages/DG.Logger/) | [![DG.Logger](https://img.shields.io/nuget/dt/DG.Logger)](https://www.nuget.org/packages/DG.Logger/)  

## 基本用法 

### 步骤 1 : 部署 QuartzHost.API  

### 步骤 2 : 部署 QuartzHost.UI  

### 步骤 3 : 新建任务项目  

### 步骤 4 : 任务项目 Install the package

Nuget 包安装

```
Install-Package QuartzHost.Base
```

### 步骤 5 : 打开QuartzHost.UI添加任务

