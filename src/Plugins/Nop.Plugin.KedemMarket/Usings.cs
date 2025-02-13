﻿global using static System.ArgumentNullException;
global using KedemMarket.Admin.Domain;
global using KedemMarket.Admin.Factories;
global using KedemMarket.Domain.Checkout;
global using KedemMarket.Domain.User;
global using KedemMarket.Factories.Catalog;
global using KedemMarket.Factories.Directory;
global using KedemMarket.Factories.Orders;
global using KedemMarket.Factories.ShoppingCart;
global using KedemMarket.Factories.Vendors;
global using KedemMarket.Infrastructure;
global using KedemMarket.Models.Checkout;
global using KedemMarket.Models.Orders;
global using KedemMarket.Models.Store;
global using KedemMarket.Models.Users;
global using KedemMarket.Services.Checkout;
global using KedemMarket.Services.Documents;
global using KedemMarket.Services.Media;
global using KedemMarket.Services.Navbar;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Rendering;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Routing;
global using Microsoft.AspNetCore.SignalR;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Configuration;
global using Newtonsoft.Json;
global using Newtonsoft.Json.Serialization;
global using Nop.Core;
global using Nop.Core.Caching;
global using Nop.Core.Domain.Catalog;
global using Nop.Core.Domain.Customers;
global using Nop.Core.Domain.Directory;
global using Nop.Core.Domain.Discounts;
global using Nop.Core.Domain.Localization;
global using Nop.Core.Domain.Media;
global using Nop.Core.Domain.Shipping;
global using Nop.Core.Domain.Stores;
global using Nop.Core.Domain.Vendors;
global using Nop.Core.Events;
global using Nop.Core.Infrastructure;
global using Nop.Data;
global using Nop.Services.Catalog;
global using Nop.Services.Directory;
global using Nop.Services.Events;
global using Nop.Services.Localization;
global using Nop.Services.Orders;
global using Nop.Services.Media;
global using Nop.Services.Messages;
global using Nop.Services.Plugins;
global using Nop.Services.Seo;
global using Nop.Services.Stores;
global using Nop.Services.Vendors;
global using Nop.Web.Areas.Admin.Controllers;
global using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
global using Nop.Web.Framework;
global using Nop.Web.Framework.Models.Extensions;
global using Nop.Web.Framework.Models;
global using Nop.Web.Framework.Mvc.Filters;
global using Nop.Web.Framework.Mvc.ModelBinding;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Text.Json;
global using System.Threading.Tasks;
global using Nop.Core.Domain.Orders;
global using KedemMarket.Models.Directory;
global using Nop.Core.Domain.Common;
global using Nop.Web.Models.Common;
global using KedemMarket.Models.Cart;
global using KedemMarket.Models.Catalog;
global using KedemMarket.Models.Media;
global using Microsoft.AspNetCore.Http;
global using Nop.Web.Factories;
global using Nop.Web.Models.Catalog;
global using KedemMarket.Models.Navbar;
global using Nop.Services.Attributes;
global using Nop.Services.Common;
global using Nop.Web.Models.Order;
global using KedemMarket.Services.User;
global using KedemMarket.Extensions;
global using Nop.Core.Domain.Security;
global using Nop.Core.Domain.Tax;
global using Nop.Services.Customers;
global using Nop.Services.Discounts;
global using Nop.Services.Helpers;
global using Nop.Services.Payments;
global using Nop.Services.Security;
global using Nop.Services.Shipping;
global using Nop.Services.Tax;
global using Nop.Web.Models.ShoppingCart;
global using Nop.Web.Framework.Events;
global using Nop.Web.Framework.Mvc;
global using Nop.Data.Mapping;
global using KedemMarket.Navbar.Widgets;
global using Nop.Core.Domain.Cms;
global using Nop.Services.Cms;
global using Nop.Services.Configuration;
global using Nop.Web.Framework.Infrastructure;
global using FluentValidation;
global using KedemMarket.Admin.Models;
global using KedemMarket.Admin.Validators;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.AspNetCore.Cors.Infrastructure;