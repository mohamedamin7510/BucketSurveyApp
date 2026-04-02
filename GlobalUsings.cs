//This for micrsoft libraries
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.Extensions.Options;
global using Microsoft.AspNetCore.Cors;
global using Microsoft.AspNetCore.Mvc;
global using System.Data;
global using System.Security.Cryptography;
global using Microsoft.AspNetCore.Identity.UI.Services;
global using Microsoft.AspNetCore.WebUtilities;
global using System.Text;

//This for out libraries
global using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
global using System.Reflection;
global using FluentValidation;
global using MapsterMapper;
global using Mapster;
global using Serilog;
global using OneOf;
global using Hangfire;


//This for local refrences 
global using BucketSurvey.Api.PollServices.Processing;
global using BucketSurvey.Api.Contract.Authentication;
global using System.ComponentModel.DataAnnotations;
global using BucketSurvey.Api.Authentication;
global using BucketSurvey.Api.Contract.Poll;
global using BucketSurvey.Api.Presistence;
global using BucketSurvey.Api.Abstractions;
global using BucketSurvey.Api.Entities;
global using BucketSurvey.Api.Services;
global using BucketSurvey.Api.Errors;
global using BucketSurvey.Api.Contract.Question;
global using BucketSurvey.Api.Extenstions;
global using BucketSurvey.Api.Abstractions.Const;
global using BucketSurvey.Api.Helpers;
global using BucketSurvey.Api.Authentication.Filters;
global using LoginRequest = BucketSurvey.Api.Contract.Authentication.LoginRequest;
global using RegisterRequest = BucketSurvey.Api.Contract.Authentication.RegisterRequest;
global using ResetPasswordRequest = BucketSurvey.Api.Contract.Authentication.ResetPasswordRequest;

