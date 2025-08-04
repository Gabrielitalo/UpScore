using RecomeceAPI.Models.Common;

namespace RecomeceAPI.Services.Common
{
    public static class NotificationService
  {
    public static NotificationModel Custom(int type, string content, string? title)
    {
      return new NotificationModel
      {
        Type = type,
        Content = content,
        Title = title
      };
    }
    public static NotificationModel Success()
    {
      return new NotificationModel
      {
        Type = 1,
        Content = "Ação foi processada com sucesso",
      };
    }
    public static NotificationModel Success(string content)
    {
      return new NotificationModel
      {
        Type = 1,
        Content = content,
        Title = "Ação foi processada com sucesso",
        Item = ""
      };
    }
    public static NotificationModel NoContent()
    {
      return new NotificationModel
      {
        Type = 2,
        Content = "Nenhum resultado encontrado!",
      };
    }
    public static NotificationModel NoAction()
    {
      return new NotificationModel
      {
        Type = 2,
        Content = "Nenhuma ação foi processada",
      };
    }
    public static NotificationModel NoExists()
    {
      return new NotificationModel
      {
        Type = 2,
        Content = "O registro não foi encontrado",
      };
    }
    public static NotificationModel Validation(string content)
    {
      return new NotificationModel
      {
        Type = 3,
        Content = content,
        Title = "Validação",
      };
    }
    public static NotificationModel Validation(string content, object? item)
    {
      if (item == null)
        item = "";
      return new NotificationModel
      {
        Type = 3,
        Content = content,
        Title = "Validação",
        Item = item
      };
    }
    public static NotificationModel Error()
    {
      return new NotificationModel
      {
        Type = 3,
        Title = "Validação",
        Content = "Não foi possível atender a situação no momento, a equipe técnica já está verificando",
      };
    }
  }
}
