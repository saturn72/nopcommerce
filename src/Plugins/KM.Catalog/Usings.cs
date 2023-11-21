﻿global using Google.Apis.Auth.OAuth2;
global using Google.Cloud.Firestore;
global using Google.Cloud.Storage.V1;
global using Km.Catalog.Controllers;
global using Km.Catalog.Documents;
global using Km.Catalog.Domain;
global using Km.Catalog.ScheduledTasks;
global using Km.Catalog.Services;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Razor;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;
global using Nop.Core;
global using Nop.Core.Caching;
global using Nop.Core.Domain;
global using Nop.Core.Domain.Catalog;
global using Nop.Core.Domain.Media;
global using Nop.Core.Domain.Shipping;
global using Nop.Core.Domain.Stores;
global using Nop.Core.Domain.Vendors;
global using Nop.Core.Events;
global using Nop.Core.Infrastructure;
global using Nop.Data;
global using Nop.Services.Catalog;
global using Nop.Services.Configuration;
global using Nop.Services.Events;
global using Nop.Services.Logging;
global using Nop.Services.Media;
global using Nop.Services.ScheduleTasks;
global using Nop.Services.Stores;
global using Nop.Services.Vendors;
global using SixLabors.ImageSharp;
global using SixLabors.ImageSharp.Processing;
global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Text;
global using System.Text.Json;
global using System.Threading.Tasks;
global using Microsoft.AspNetCore.SignalR;