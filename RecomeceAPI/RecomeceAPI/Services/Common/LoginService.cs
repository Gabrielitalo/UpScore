using RecomeceAPI.DTOs;
using RecomeceAPI.Interfaces;
using RecomeceAPI.Models;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Server;

namespace RecomeceAPI.Services.Common
{
  public class LoginService
  {
    private readonly CadEmpresasService _cadEmpresasService;
    private readonly CadEquipeService _cadEquipeService;
    private readonly CadClientesService _cadClientesService;
    private readonly ICryptService _cyptService;

    public LoginService(CadEmpresasService cadEmpresasService,
      CadEquipeService cadEquipeService,
      CadClientesService cadClientesService,
      ICryptService cyptService)
    {
      _cadEmpresasService = cadEmpresasService;
      _cadEquipeService = cadEquipeService;
      _cadClientesService = cadClientesService;
      _cyptService = cyptService;
    }

    public async Task<SessionDTO> Login(int userType, string email, string password)
    {
      SessionModel sessionModel;
      if (userType == 0) // CadEquipe
      {
        CadEquipeModel model = await _cadEquipeService.Login(email, password);
        if (model.Id == 0)
          return new SessionDTO();
        model.CadEmpresas = await _cadEmpresasService.GetByIdAsync(model.CadEmpresas.Id);

        sessionModel = new SessionModel
        {
          Active = model.Ativo,
          IdCompany = model.CadEmpresas.Id,
          IdType = userType,
          IdRole = model.Tipo,
          IdUser = model.Id,
          DataBase = model.CadEmpresas.BancoDados,
          Server = model.CadEmpresas.Servidor,
          Username = model.Nome,
          IsAssociacao = model.CadEmpresas.Tipo == 3 ? true : false,
        };
        sessionModel.SetDateSession(DateTime.Now);
        sessionModel.SetExpiresSession();

        return new SessionDTO
        {
          Active = 1,
          IdType = sessionModel.IdType,
          IdRole= sessionModel.IdRole,
          IdCompany= sessionModel.IdCompany,
          ExpiresSession = sessionModel.ExpiresSession,
          Username = sessionModel.Username,
          IsAssociacao = sessionModel.IsAssociacao,
          Token = GenerateToken(sessionModel),
          WhiteLabelConfig = model.CadEmpresas.WhiteLabelConfig
        };
      }
      else if (userType == 2) // CadClientes
      {
        return new SessionDTO();
      }
      else
      {
        return new SessionDTO();
      }
    }
    private string GenerateToken(SessionModel session)
    {
      return _cyptService.Crypt(session.GetJson());
    }
  }
}
