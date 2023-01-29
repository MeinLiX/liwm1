﻿using System;
using System.Reflection;
using FluentValidation.AspNetCore;
using MediatR;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServiceRegistration
{
	private static Assembly GetAssembly => typeof(ServiceRegistration).GetTypeInfo().Assembly;

    public static IServiceCollection AddAplicationLoyaut(this IServiceCollection services)
	=> services.AddMediatR(GetAssembly)
               .AddFluentValidation(new[] { GetAssembly });
}

