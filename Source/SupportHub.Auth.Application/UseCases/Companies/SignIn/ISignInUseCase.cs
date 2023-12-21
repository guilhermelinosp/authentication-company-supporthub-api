﻿using SupportHub.Auth.Application.Abstract;
using SupportHub.Auth.Domain.Dtos.Requests.Companies;

namespace SupportHub.Auth.Application.UseCases.Companies.SignIn;

public interface ISignInUseCase : IUseCaseBase
{
    Task ExecuteAsync(RequestSignInEmail request);
}