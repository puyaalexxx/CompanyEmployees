using AutoMapper;
using CompanyEmployees.Core.Domain.Entities;
using CompanyEmployees.Core.Domain.Exceptions;
using CompanyEmployees.Core.Domain.Repositories;
using CompanyEmployees.Core.Services.Abstractions;
using LoggingService;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Core.Services;

internal sealed class CompanyService : ICompanyService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public CompanyService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(bool trackChanges, CancellationToken ct = default)
    {
        var companies = await _repository.Company.GetAllCompaniesAsync(trackChanges, ct);

        var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);

        return companiesDto;
    }

    public async Task<CompanyDto> GetCompanyAsync(Guid companyId, bool trackChanges, CancellationToken ct = default)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges, ct);

        if (company is null)
        {
            throw new CompanyNotFoundException(companyId);
        }

        var companyDto = _mapper.Map<CompanyDto>(company);

        return companyDto;
    }

    public async Task<IEnumerable<CompanyDto>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges, CancellationToken ct = default)
    {
        if (ids is null) throw new IdParametersBadRequestException();

        var companyEntities = await _repository.Company.GetByIdsAsync(ids, trackChanges, ct);

        if (ids.Count() != companyEntities.Count()) throw new CollectionByIdsBadRequestException();

        var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);

        return companiesToReturn;
    }

    public async Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto company, CancellationToken ct = default)
    {
        var companyEntity = _mapper.Map<Company>(company);

        _repository.Company.CreateCompany(companyEntity);

        await _repository.SaveAsync(ct);

        var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);

        return companyToReturn;
    }

    public async Task<(IEnumerable<CompanyDto> companies, string ids)> CreateCompanyCollectionAsync(
        IEnumerable<CompanyForCreationDto> companyCollection, CancellationToken ct = default)
    {
        if (companyCollection is null) throw new CompanyCollectionBadRequest();

        var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);

        foreach (var company in companyEntities)
        {
            _repository.Company.CreateCompany(company);
        }

        await _repository.SaveAsync(ct);

        var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
        var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));

        return (companies: companyCollectionToReturn, ids: ids);
    }

    public async Task DeleteCompanyAsync(Guid companyId, bool trackChanges, CancellationToken ct = default)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges, ct);

        if (company is null)
        {
            throw new CompanyNotFoundException(companyId);
        }

        _repository.Company.DeleteCompany(company);

        await _repository.SaveAsync(ct);
    }

    public async Task UpdateCompanyAsync(Guid companyId, CompanyForCreationDto companyForUpdate,
        bool trackChanges, CancellationToken ct = default)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges, ct);
        if (company is null)
        {
            throw new CompanyNotFoundException(companyId);
        }

        _mapper.Map(companyForUpdate, company);

        await _repository.SaveAsync(ct);
    }

}