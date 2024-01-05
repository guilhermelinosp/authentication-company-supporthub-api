﻿using Company.SupportHub.Application.Services.Cryptography;
using Company.SupportHub.Application.UseCases.Validators;
using Company.SupportHub.Domain.APIs;
using Company.SupportHub.Domain.DTOs.Requests;
using Company.SupportHub.Domain.DTOs.Responses;
using Company.SupportHub.Domain.Exceptions;
using Company.SupportHub.Domain.Messages;
using Company.SupportHub.Domain.Repositories;
using Company.SupportHub.Domain.Services;

namespace Company.SupportHub.Application.UseCases.Company.SignUp;

public class SignUpUseCase(
	ICompanyRepository repository,
	ICryptographyService cryptography,
	ISendGridService sendGridService,
	IBrazilApi brazilApi,
	IRedisService redis)
	: ISignUpUseCase
{
	public async Task<ResponseDefault> ExecuteAsync(RequestSignUp request)
	{
		var validatorRequest = await new ValidatorSignUp().ValidateAsync(request);
		if (!validatorRequest.IsValid)
			throw new DefaultException(validatorRequest.Errors.Select(er => er.ErrorMessage).ToList());

		var validateEmail = await repository.FindCompanyByEmailAsync(request.Email);
		if (validateEmail is not null)
			throw new DefaultException([MessageException.EMAIL_JA_REGISTRADO]);

		var validateCnpj = await repository.FindCompanyByCnpjAsync(request.Cnpj);
		if (validateCnpj is not null)
			throw new DefaultException([MessageException.CNPJ_JA_REGISTRADO]);
		
		var checkCnpj = await brazilApi.ConsultaCnpj(request.Cnpj);
		if (!checkCnpj)
			throw new DefaultException([MessageException.CNPJ_INVALIDO]);
		
		if (request.Password != request.PasswordConfirmation)
			throw new DefaultException([MessageException.SENHA_NAO_CONFERE]);

		var company = new Domain.Entities.Company
		{
			Cnpj = request.Cnpj,
			Phone = string.Empty,
			Email = request.Email,
			Password = cryptography.EncryptPassword(request.Password)
		};

		var code = redis.GenerateOneTimePassword(company.CompanyId.ToString());

		await repository.CreateCompanyAsync(company);

		await sendGridService.SendSignUpAsync(request.Email, code);

		return new ResponseDefault(company.CompanyId.ToString(), MessageResponse.CODIGO_ENVIADO_SIGN_UP);
	}
}