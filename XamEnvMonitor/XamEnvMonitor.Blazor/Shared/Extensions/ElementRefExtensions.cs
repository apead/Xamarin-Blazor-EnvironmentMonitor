﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor;
using Microsoft.JSInterop;

namespace XamEnvMonitor.Blazor.Shared.Extensions
{
    public static class ElementRefExtensions
    {
        public static Task<string> GetValue(this ElementRef elementRef)
        {
            return JSRuntime.Current.InvokeAsync<string>("SampleFunctions.GetElementValue", elementRef);
        }
    }
}